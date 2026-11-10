using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Entities;
using AlmostNewOutpost.Api.Models.Requests;
using AlmostNewOutpost.Api.Models.Responses;
using AlmostNewOutpost.Api.Models.Rows;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlmostNewOutpost.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        // Store instance of EF helper OutpostDbContext as private readonly
        private readonly OutpostDbContext _db;
        private readonly IPasswordHasher<UserRow> _passwordHasher;


        // Constructor and assignment
        public AdminUsersController(OutpostDbContext db, IPasswordHasher<UserRow> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminUserDto>>> GetAll([FromQuery] bool includeArchived = false)
        {
            List<AdminUserDto> users = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where includeArchived || user.ArchivedAtUtc == null
                orderby user.Username
                    select new AdminUserDto
                    {
                        Id = user.UserId,
                        Username = user.Username,
                        RoleId = role.RoleId,
                        Role = role.RoleName,
                        ArchivedAtUtc = user.ArchivedAtUtc,
                    }).ToListAsync(); // Make sure to have "using Microsoft.EntityFrameworkCore;"

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> GetById([FromRoute] int id)
        {
            AdminUserDto? adminUser = await FindUserAsync(id);

            if(adminUser == null)
            {
                return Problem(title: "User not found.", statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(adminUser);
        }


        [HttpPost]
        public async Task<ActionResult<AdminUserDto>> Create(CreateUserRequest request)
        {
            string username = request.Username.Trim();
            bool usernameTaken = await _db.Users.AnyAsync(user => user.Username == username);

            if (usernameTaken)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            // students write their own LINQ
            bool roleExists = await _db.Roles.AnyAsync(role => role.RoleId == request.RoleId);
            if (!roleExists)
            {
                ModelState.AddModelError(nameof(CreateUserRequest.RoleId), "Choose a role that exists.");
                return ValidationProblem(ModelState);
            }

            // Hash password for saving NOTE: Students need to reference user Entity
            string passwordHash = _passwordHasher.HashPassword(new UserRow(), request.Password);
            User user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                RoleId = request.RoleId!.Value
            };

            // queue change
            _db.Users.Add(user);

            try
            {
                // apply changes
                await _db.SaveChangesAsync();
            } catch (DbUpdateException exception) when (DatabaseErrors.GetErrorNumber(exception) == DatabaseErrors.DuplicateKey)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            AdminUserDto? created = await FindUserAsync(user.UserId);
            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> Update([FromRoute] int id, UpdateUserRequest request)
        {
            User? user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return Problem(title: "User not found.", statusCode: StatusCodes.Status404NotFound);
            }

            string username = request.Username.Trim();

            // if taken and the user isn't the same user
            bool usernameTaken = await _db.Users.AnyAsync(other => other.Username == username && other.UserId != id);

            if (usernameTaken)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            // double check role
            bool roleExists = await _db.Roles.AnyAsync(role => role.RoleId == request.RoleId);
            if (!roleExists)
            {
                ModelState.AddModelError(nameof(UpdateUserRequest.RoleId), "Choose a role that exists.");
                return ValidationProblem(ModelState);
            }

            // Add new data to a user entity
            user.Username = username;
            user.RoleId = request.RoleId!.Value;

            try
            {
                // apply changes
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (DatabaseErrors.GetErrorNumber(exception) == DatabaseErrors.DuplicateKey)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            return Ok(await FindUserAsync(id));
        }

        private async Task<AdminUserDto?> FindUserAsync(int id)
        {
            AdminUserDto? adminUser = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where user.UserId == id
                orderby user.Username
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                }).FirstOrDefaultAsync();

            return adminUser;

        }
    }
}
