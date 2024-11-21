using Fido2NetLib;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache(); // Add in-memory distributed cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the cookie HttpOnly
    options.Cookie.IsEssential = true; // Ensure the cookie is essential
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
        // Explicitly set the NameClaimType
        NameClaimType = ClaimTypes.Name 
    };
});

// Add Fido2 configuration
builder.Services.AddSingleton<Fido2>(sp =>
{
    var config = new Fido2Configuration
    {
        ServerDomain = "localhost", // Your server domain
        ServerName = "RentalCheckIn", // Your application name
        Origins = new HashSet<string>() { "https://localhost:7110" }// Your origin (e.g., https://yourdomain.com)
    };

    return new Fido2(config);
});


// Register application services
//builder.Services.AddScoped<ILHostRepository, HostRepository>();
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
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<RefreshTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseRequestLocalization(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("fr-FR"),
        new CultureInfo("nl-NL"),
    };
    // Default to English as fallback language
    options.DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US")); 
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
        if (primaryLanguage == "en") primaryLanguage = "en-US";
        else if (primaryLanguage == "fr") primaryLanguage = "fr-FR";
        else if (primaryLanguage == "nl") primaryLanguage = "nl-NL";

        var userCulture = new CultureInfo(primaryLanguage);
        return await Task.FromResult(new ProviderCultureResult(userCulture?.Name ?? supportedCultures[0].Name));
    }));
});


app.UseRouting();

app.UseSession(); // Enable session middleware
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
