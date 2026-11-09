using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using AlmostNewOutpost.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;


namespace AlmostNewOutpost.Api.Controllers.Admin
{

    [ApiController]
    [Route("api/admin/roles")]



    public class AdminRolesController : ControllerBase
    {
        private readonly OutpostDbContext _db;

        public AdminRolesController(OutpostDbContext db)
        {
            _db = db;
        }

        [HttpGet]

        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            List<RoleDto> roles = await _db.Roles
                .OrderBy(role => role.RoleId)
                .Select(role => new RoleDto { RoleId = role.RoleId, RoleName = role.RoleName })
                .ToListAsync();

            return Ok(roles);
        }

    }
}


    
}
