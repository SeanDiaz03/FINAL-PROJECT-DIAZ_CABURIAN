using HelpDesk.Data;
using HelpDesk.Models;

namespace HelpDesk.Helpers
{
    public static class AuditHelper
    {
        public static async Task LogAsync(HelpdeskContext context, string action, string user, string entityType, int entityId, string details)
        {
            var log = new AuditLog
            {
                Action = action,
                PerformedBy = user,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
                Details = details
            };
            context.AuditLogs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}
