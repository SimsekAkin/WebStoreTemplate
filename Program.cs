using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Repositories;
using WebStore.Repositories.Interfaces;

// Force invariant culture so decimal fields always use dot (.) as separator.
// Without this, Turkish/German locale renders "10,00" in inputs which fails model binding.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// ============ SQLite Database Context ============
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllersWithViews();

// ============ Repository Configuration ============
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// ============ Session Configuration ============
builder.Services.AddDistributedMemoryCache();// Use in-memory cache for session state storage.
builder.Services.AddSession(options =>// Configure session options such as timeout and cookie settings.
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);// Set session timeout to 30 minutes of inactivity.
    options.Cookie.HttpOnly = true;// Make session cookie accessible only via HTTP, not JavaScript.
    options.Cookie.IsEssential = true;// Mark session cookie as essential for GDPR compliance.
});

// ============ HttpContext Accessor (for session) ============
builder.Services.AddHttpContextAccessor();// Register IHttpContextAccessor to allow access to HttpContext in services and repositories, enabling session usage.

var app = builder.Build();

// Create database and tables automatically if they do not exist yet.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();// Enable HTTP Strict Transport Security (HSTS) for production environments
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();// Enable session middleware, it make possible to use session in controllers and views
app.UseAuthorization();

app.MapStaticAssets();//this makes possible to serve static files from wwwroot folder, and also allows
//to use static assets in views with the help of the WithStaticAssets() method in MapControllerRoute

// Configure the default route for controllers, and also enable static assets in views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
