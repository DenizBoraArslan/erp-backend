using Finance.API;
using Finance.Application.Consumers;
using HR.API;
using Identity.API;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using Inventory.API;
using Inventory.Application.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sales.API;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddFinanceModule(builder.Configuration);
builder.Services.AddHRModule(builder.Configuration);

// Controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Identity.API.Controllers.AuthController).Assembly)
    .AddApplicationPart(typeof(Inventory.API.Controllers.ProductController).Assembly)
    .AddApplicationPart(typeof(Sales.API.Controllers.OrderController).Assembly)
    .AddApplicationPart(typeof(Finance.API.Controllers.InvoiceController).Assembly)
    .AddApplicationPart(typeof(HR.API.Controllers.EmployeeController).Assembly);

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderConfirmedConsumer>();
    x.AddConsumer<PurchaseInvoiceConfirmedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed a single Admin account on first run. Public self-registration has
// been removed, so this is the only way in until the Admin creates more
// users from the "Kullanıcı Yönetimi" screen.
using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    const string adminEmail = "admin@dbsoft.com";

    var existingAdmin = await userRepository.GetByEmailAsync(adminEmail);
    if (existingAdmin is null)
    {
        const string adminPassword = "DbSoft!Admin2026";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
        var admin = User.Create("Sistem", "Yöneticisi", adminEmail, passwordHash, UserRole.Admin);
        await userRepository.AddAsync(admin);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();