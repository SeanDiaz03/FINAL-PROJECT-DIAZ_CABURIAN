namespace HelpDesk.Controllers
{
    using HelpDesk.Data;
    using HelpDesk.Helpers;
    using HelpDesk.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Supervisor,Officer,JuniorOfficer")]
    public class RemarksController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public RemarksController(HelpdeskContext context)
        {
            _context = context;
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetRemarksByTicket(int ticketId)
        {
            var remarks = await _context.Remarks
                .Where(r => r.TicketId == ticketId)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();

            return Ok(remarks);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRemark([FromBody] Remark remark)
        {
            remark.Timestamp = DateTime.UtcNow;
            _context.Remarks.Add(remark);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(_context, "AddRemark", remark.MadeBy, "Remark", remark.Id, $"Added remark to ticket {remark.TicketId}");
            return Ok("Remark added and audit logged.");
        }
    }
}
