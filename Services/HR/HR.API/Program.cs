using HR.Application;
using HR.Infrastructure;
using HR.Infrastructure.Persistence;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddApplicationLayer(typeof(HR.Application.AssemblyReference).Assembly);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOperationalFoundation();
builder.Services.AddInfrastructureLayer(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HRDbContext>();
    dbContext.Database.Migrate();
}

app.UseGlobalExceptionHandler();
app.UseRequestCorrelation();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

