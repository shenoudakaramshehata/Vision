using Vision;
using Vision.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using NToastNotify;
using Microsoft.AspNetCore.Identity.UI.Services;
using Email;
using Vision.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

//testc changes

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#region Identity Configuration

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(45);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

#endregion


#region "Data Context"

builder.Services.AddDbContext<CRMDBContext>(options => options.UseSqlServer(connectionString));

#endregion

#region "Localization"

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null).AddDataAnnotationsLocalization(
               options =>
               {
                   var type = typeof(Vision.SharedResource);
                   var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                   var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                   var localizer = factory.Create("SharedResource", assembblyName.Name);
                   options.DataAnnotationLocalizerProvider = (t, f) => localizer;
               }
               );
builder.Services.AddControllers().AddDataAnnotationsLocalization(
          options =>
          {
              var type = typeof(Vision.SharedResource);
              var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
              var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
              var localizer = factory.Create("SharedResource", assembblyName.Name);
              options.DataAnnotationLocalizerProvider = (t, f) => localizer;
          });
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization(
          options =>
          {
              var type = typeof(Vision.SharedResource);
              var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
              var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
              var localizer = factory.Create("SharedResource", assembblyName.Name);
              options.DataAnnotationLocalizerProvider = (t, f) => localizer;
          }
          );

#endregion

builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
builder.Services.Configure<RazorViewEngineOptions>(o =>
{
    o.ViewLocationFormats.Add("/Shared/Components/InputForm/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("/Pages/Shared/Components/InputForm/{0}" + RazorViewEngine.ViewExtension);
});

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<Email.IEmailSender, EmailSender>();
//builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddRazorPages().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PreventDuplicates = true,
    CloseButton = true
});
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerGeneratorOptions.IgnoreObsoleteActions = true;
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();
System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseNToastNotify();
app.MapRazorPages();

var supportedCultures = new[]
           {
                new CultureInfo("en-US"),

                new CultureInfo("ar-EG")

            };


app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});


app.UseEndpoints(endpoints =>
{

    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapSwagger();
});





app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

});

app.Run();
