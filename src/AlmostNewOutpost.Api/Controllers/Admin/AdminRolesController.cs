using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models;
using AlmostNewOutpost.Api.Models.Responses;
using AlmostNewOutpost.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace AlmostNewOutpost.Api.Controllers.Admin
{
    // Attributes 
    [ApiController]
    [Route("api/admin/roles")]

    // Class and inheritance
    public class AdminRolesController : ControllerBase
    {
        // Store instance of EF helper OutpostDbContext as private readonly
        private readonly OutpostDbContext _db;

        // Contructor and assignment
        public AdminRolesController (OutpostDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            List<RoleDto> roles = await _db.Roles
                // AC: Sorted by id
                .OrderBy(role => role.RoleId)
                // Note: we are just selecting and specifying the mapping to our new object. No filter yet.
                .Select(role => new RoleDto { RoleId = role.RoleId, RoleName = role.RoleName })
                // We want to ask the thread  to drop off real - not dummy results now
                .ToListAsync();

            /*
            SQL Equivalent:

                Select 
                    role.RoleId,
                    role.RoleName
                From Role AS r
                Order By role.RoleId

             */

            return Ok(roles);
        }
    }
}

