using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;
using RoyalVilla_API.Services;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.")));

// Configure JWT Authentication
builder.Services.AddAuthentication(options => {
    // Set the default authentication scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
   options.RequireHttpsMetadata = false;
   options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero

    };
});

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(options =>
{
    options.CreateMap<Villa, VillaCreateDTO>().ReverseMap();
    options.CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
    options.CreateMap<Villa, VillaDTO>().ReverseMap();
    options.CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
    options.CreateMap<User, UserDTO>().ReverseMap();
});

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

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

app.UseAuthentication();

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