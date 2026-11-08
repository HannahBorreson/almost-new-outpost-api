using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Responses;
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

        // Contructor and assignment
        public AdminUsersController(OutpostDbContext db)
        {
            _db = db; 
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
        public async Task<ActionResult<AdminUserDto>> GetAll([FromRoute] int id)
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

            if(adminUser == null)
            {
                return Problem(title: "User not found.", statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(adminUser);
        }
    }
}
