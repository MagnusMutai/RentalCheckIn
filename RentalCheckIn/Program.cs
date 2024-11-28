using Fido2NetLib;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using RentalCheckIn.Configuration.WhatsApp;
using Serilog;
using Serilog.Formatting.Compact;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
// Add in-memory distributed cache **Check later
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    // Ensure the cookie is essential
    options.Cookie.IsEssential = true; 
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "V1" });
});

// Configure EF Core with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new System.Version(8, 0, 40))));

//Register HttpClient with BaseAddress
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7110") });
builder.Services.AddHttpClient();

// Configure JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"];
var key = Encoding.ASCII.GetBytes(secretKey);
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Set to false only during development
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: @"C:\Logs\log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        formatter: new CompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add Fido2 configuration
// Investigate why Fido2 is faded
builder.Services.AddSingleton<Fido2>(sp =>
{
    var config = new Fido2Configuration
    {
        ServerDomain = "localhost", 
        ServerName = "RentalCheckIn",
        Origins = new HashSet<string>() { "https://localhost:7110" }
    };

    return new Fido2(config);
});

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILHostRepository, LHostRepository>();
builder.Services.AddScoped<ILHostService, LHostService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationBusinessService, ReservationBusinessService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAppartmentRepository, AppartmentRepository>();
builder.Services.AddScoped<IAppartmentBusinessService, AppartmentBusinessService>();
builder.Services.AddScoped<IAppartmentService, AppartmentService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<ITOTPService, TOTPService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPDFService, PDFService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IApartmentTranslationRepository, ApartmentTranslationRepository>();
builder.Services.AddScoped<IStatusTranslationRepository, StatusTranslationRepository>();
builder.Services.AddScoped<IReservationTranslationRepository, ReservationTranslationRepository>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<ILocalizationUIService, LocalizationUIService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Global exception handling
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception != null)
            {
                Log.Error(exception, "Unhandled exception occurred");
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected error occurred.");
        });
    });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseRequestLocalization(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-EN"),
        new CultureInfo("fr-FR"),
        new CultureInfo("nl-NL"),
    };
    // Default to English as fallback language
    options.DefaultRequestCulture = new RequestCulture(new CultureInfo("en-EN"));
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Clear();
    // Add a provider to check for the culture in a cookie
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
    // Add a custom provider to handle fallbacks and browser defaults
    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
    {
        // Check if a culture cookie is already set
        var cultureCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];

        if (!string.IsNullOrEmpty(cultureCookie))
        {
            var cookieCulture = CookieRequestCultureProvider.ParseCookieValue(cultureCookie);
            return await Task.FromResult(new ProviderCultureResult(cookieCulture?.Cultures.First().Value));
        }

        // If no culture cookie, fallback to the browser's settings
        var userLanguages = context.Request.Headers["Accept-Language"].ToString();
        var primaryLanguage = userLanguages.Split(',').FirstOrDefault();
        if (primaryLanguage == "en") primaryLanguage = "en-EN";
        else if (primaryLanguage == "fr") primaryLanguage = "fr-FR";
        else if (primaryLanguage == "nl") primaryLanguage = "nl-NL";

        var userCulture = new CultureInfo(primaryLanguage);
        return await Task.FromResult(new ProviderCultureResult(userCulture?.Name ?? supportedCultures[0].Name));
    }));
});


app.UseRouting();
// Enable session middleware
app.UseSession(); 
app.UseHttpsRedirection();

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth V1");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
