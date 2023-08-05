var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    //This can be added to add timespan
    //options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout as needed
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
