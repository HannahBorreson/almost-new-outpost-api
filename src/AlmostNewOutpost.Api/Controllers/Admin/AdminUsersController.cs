using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlmostNewOutpost.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users/")]
    public class AdminUsersController(OutpostDbContext db) : ControllerBase
    {
        private readonly OutpostDbContext _db = db;

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
                }).ToListAsync();

            return Ok(users);
        }

    }
}
