using Blazored.LocalStorage;
using FellesForumAPI.Data;
using FellesForumAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Twilio;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the JWT key from the environment variable
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT key not found in environment variables.");
var key = Encoding.ASCII.GetBytes(jwtKey);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddRazorPages();          // Add Razor Pages support
builder.Services.AddServerSideBlazor();    // Add Blazor Server support
builder.Services.AddControllers();         // API Controllers
builder.Services.AddScoped<UserService>(); // Custom user service

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<SmsService>();

builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("IpRateLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 1;
        limiterOptions.Window = TimeSpan.FromSeconds(30);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

TwilioClient.Init(accountSid, authToken);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseStaticFiles(); // For serving Blazor static files

app.UseRouting();

app.UseAuthentication(); // Enable JWT Authentication
app.UseAuthorization();

// Redirect root URL to /login
app.MapGet("/", context =>
{
    context.Response.Redirect("/login");
    return Task.CompletedTask;
});

// Map Blazor and API Endpoints
app.MapControllers();       // Map API controllers
app.MapBlazorHub();          // Map Blazor Server SignalR Hub
app.MapFallbackToPage("/_Host"); // Fallback route for Blazor Server

app.Run();
