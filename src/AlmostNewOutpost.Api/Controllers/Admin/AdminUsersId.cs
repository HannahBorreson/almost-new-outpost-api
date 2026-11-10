using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlmostNewOutpost.Api.Controllers.Admin
{
    [ApiController]

    public class AdminUsersId : ControllerBase
    {
        private readonly OutpostDbContext _db;

        public AdminUsersId(OutpostDbContext db) => _db = db;


        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> Get([FromRoute] int id)
        {
            AdminUserDto? adminUser = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where user.UserId == id
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                })
                .FirstOrDefaultAsync();

            if (adminUser == null)
            {
                return NotFound();
            }
            return Ok(adminUser);
        }

    }
}
