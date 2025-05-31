using HelpDesk.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public DepartmentController(HelpdeskContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var depts = await _context.Departments.ToListAsync();
            return Ok(depts);
        }
    }
}
