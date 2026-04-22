using Stock.Application;
using Stock.Infrastructure;
using Stock.Infrastructure.Persistence;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddApplicationLayer(typeof(Stock.Application.AssemblyReference).Assembly);
builder.Services.AddInfrastructureLayer(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    dbContext.Database.Migrate();
}

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
