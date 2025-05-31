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
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public TicketController(HelpdeskContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTicket(int ticketId, string assignedTo, string performedBy)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == assignedTo);
            if (user == null) return BadRequest("Assigned user not found");

            var performer = await _context.Users.FirstOrDefaultAsync(u => u.Username == performedBy);
            if (performer == null) return Unauthorized("Performing user not found");

            if (user.DepartmentId != ticket.DepartmentId)
            {
                return BadRequest("Cannot assign ticket to user in a different department");
            }

            if (user.Role == "JuniorOfficer" && ticket.Severity == "Critical" && performer.Role != "Supervisor")
            {
                return BadRequest("Junior Officers cannot be assigned Critical severity tickets unless by a Supervisor");
            }

            ticket.AssignedTo = assignedTo;
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(_context, "AssignTicket", performedBy, "Ticket", ticketId, $"Assigned to {assignedTo}");
            return Ok("Ticket assigned and audit logged.");
        }

        [Authorize(Roles = "Admin,Supervisor,Officer")]
        [HttpPost("status")]
        public async Task<IActionResult> UpdateStatus(int ticketId, string newStatus, string performedBy)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == performedBy);
            if (user == null) return Unauthorized("Performing user not found");

            if (ticket.DepartmentId != user.DepartmentId)
            {
                return Forbid("Cannot modify tickets from another department");
            }

            if (user.Role == "JuniorOfficer" && ticket.Severity == "Critical")
            {
                return BadRequest("Junior Officers cannot modify Critical severity tickets");
            }

            ticket.Status = newStatus;
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(_context, "UpdateStatus", performedBy, "Ticket", ticketId, $"Status changed to {newStatus}");
            return Ok("Ticket status updated and audit logged.");
        }

        [Authorize(Roles = "Officer,JuniorOfficer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(_context, "CreateTicket", ticket.CreatedBy, "Ticket", ticket.Id, $"Created ticket '{ticket.Title}'");
            return Ok("Ticket created and audit logged.");
        }

        [Authorize(Roles = "Admin,Supervisor,Officer,JuniorOfficer")]
        [HttpPost("remark")]
        public async Task<IActionResult> AddRemark([FromBody] Remark remark)
        {
            remark.Timestamp = DateTime.UtcNow;
            _context.Remarks.Add(remark);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(_context, "AddRemark", remark.MadeBy, "Remark", remark.Id, $"Added remark to ticket {remark.TicketId}");
            return Ok("Remark added and audit logged.");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Remarks)
                .ToListAsync();
            return Ok(tickets);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            var username = User.Identity?.Name ?? "System";
            await AuditHelper.LogAsync(_context, "DeleteTicket", username, "Ticket", id, $"Deleted ticket {id}");
            return Ok("Ticket deleted successfully");
        }
    }

}
