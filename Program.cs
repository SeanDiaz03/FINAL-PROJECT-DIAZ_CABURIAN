using HelpDesk.Data;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<HelpdeskContext>(options =>
    options.UseInMemoryDatabase("HelpdeskDb"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_SUPER_SECRET_KEY_1234567890"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Manual seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HelpdeskContext>();

    if (!db.Users.Any())
    {
        db.Departments.AddRange(
            new Department { Id = 1, Name = "IT" },
            new Department { Id = 2, Name = "HR" },
            new Department { Id = 3, Name = "Finance" }
        );

        db.Users.AddRange(
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin", DepartmentId = 1 },
            new User { Id = 2, Username = "supervisor1", Password = "super123", Role = "Supervisor", DepartmentId = 1 },
            new User { Id = 3, Username = "officer1", Password = "officer123", Role = "Officer", DepartmentId = 2 },
            new User { Id = 4, Username = "junior1", Password = "junior123", Role = "JuniorOfficer", DepartmentId = 3 }
        );

        db.Tickets.AddRange(
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

        db.Remarks.AddRange(
            new Remark
            {
                Id = 1,
                Text = "Started looking into the issue.",
                Timestamp = new DateTime(2024, 01, 01, 12, 0, 0),
                MadeBy = "junior1",
                TicketId = 1
            },
            new Remark
            {
                Id = 2,
                Text = "Issue is being resolved.",
                Timestamp = new DateTime(2024, 01, 01, 12, 0, 0),
                MadeBy = "supervisor1",
                TicketId = 2
            }
        );

        db.SaveChanges();
        Console.WriteLine("Data manually seeded.");
    }
    else
    {
        Console.WriteLine("Seed already exists. Skipping.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
