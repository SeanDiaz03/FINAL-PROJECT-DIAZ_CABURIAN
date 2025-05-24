namespace HelpDesk.Data
{
    using HelpDesk.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;

    public class HelpdeskContext : DbContext
    {
        public HelpdeskContext(DbContextOptions<HelpdeskContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine(">>> OnModelCreating is executing"); //connection test

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Remarks)
                .WithOne(r => r.Ticket)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" }
            );

            // ✅ Use pre-hashed passwords (SHA256 in base64)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = "n5dLFAhGAP1H7qHjzSk2+Xdrxb0Hq+0CDpIovJYKDvA=", Role = "Admin", DepartmentId = 1 },
                new User { Id = 2, Username = "supervisor1", PasswordHash = "Xhv42rUalMZkOZ+YPbZDo7B+u5W4Gk6OLq+yMGxu0/I=", Role = "Supervisor", DepartmentId = 1 },
                new User { Id = 3, Username = "officer1", PasswordHash = "fYNgDaRAA+zE81WJPCECbj2tQBgTxEXQKFLK+krAHOw=", Role = "Officer", DepartmentId = 2 },
                new User { Id = 4, Username = "junior1", PasswordHash = "RZL1E1bRH5/30hFTjRIgl1gzVj/fMdq9d7x2WoMQP9o=", Role = "JuniorOfficer", DepartmentId = 3 }
            );

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 1,
                    Title = "Fix Printer",
                    Description = "Printer not working in HR",
                    Severity = "Low",
                    Status = "Open",
                    CreatedBy = "officer1",
                    AssignedTo = "junior1",
                    DepartmentId = 2
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Email Issue",
                    Description = "Unable to send emails",
                    Severity = "High",
                    Status = "InProgress",
                    CreatedBy = "admin",
                    AssignedTo = "supervisor1",
                    DepartmentId = 1
                }
            );

            var staticTime = new DateTime(2024, 01, 01, 12, 0, 0);

            modelBuilder.Entity<Remark>().HasData(
                new Remark
                {
                    Id = 1,
                    Text = "Started looking into the issue.",
                    Timestamp = staticTime,
                    MadeBy = "junior1",
                    TicketId = 1
                },
                new Remark
                {
                    Id = 2,
                    Text = "Issue is being resolved.",
                    Timestamp = staticTime,
                    MadeBy = "supervisor1",
                    TicketId = 2
                }
            );
        }

    }
}
