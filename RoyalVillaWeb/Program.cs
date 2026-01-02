using RoyalVilla.DTO;
using RoyalVillaWeb.Services;
using RoyalVillaWeb.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(options =>
{
    options.CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
    options.CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap(); 
});

builder.Services.AddHttpClient("RoyalVillaAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceUrls:VillaAPI") ?? throw new InvalidOperationException("Villa API URL is not configured."));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IVillaService, VillaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
