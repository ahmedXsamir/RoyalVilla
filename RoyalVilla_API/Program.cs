using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

await SeedDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Scalar UI configuration 
    app.MapScalarApiReference(options =>
    {
        options
            .WithTheme(ScalarTheme.BluePlanet)
            .WithLayout(ScalarLayout.Modern)
            .WithTitle("v1");

        options.ShowSidebar = true;
        options.HideDarkModeToggle = false;
        options.ShowOperationId = false;
        options.DefaultOpenAllTags = false;
        options.ExpandAllResponses = false;
        options.ExpandAllModelSections = false;
    });

}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Apply pending migrations and seed data
static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}