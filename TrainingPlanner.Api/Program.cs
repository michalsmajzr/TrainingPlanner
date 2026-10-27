using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;
using TrainingPlanner.Application.Auth.Commands;
using TrainingPlanner.Application.Workouts;
using TrainingPlanner.Application.Workouts.Commands;
using TrainingPlanner.Infrastructure.Auth;
using TrainingPlanner.Infrastructure.Identity;
using TrainingPlanner.Infrastructure.Persistence;
using TrainingPlanner.Infrastructure.Persistence.Workouts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Retrieve connection string and register PostgreSQL DbContext.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-TrainingPlanner.Auth";
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__Host-TrainingPlanner.Antiforgery";
    options.Cookie.Path = "/";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICommandHandler<RegisterUserCommand, AuthUser>, RegisterUserCommandHandler>();
builder.Services.AddScoped<ICommandHandler<LoginUserCommand, AuthUser>, LoginUserCommandHandler>();
// Register the workout repository and command handler for creating workouts.
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<ICommandHandler<CreateWorkoutCommand, Guid>, CreateWorkoutCommandHandler>();

// Configure CORS policy to allow requests from the Angular frontend.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    if (!context.Request.Path.StartsWithSegments("/openapi"))
    {
        context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
    }
    await next();
});

app.UseCors("AllowAngular");

app.UseRateLimiter();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    var isApiRequest = context.Request.Path.StartsWithSegments("/api");
    var isSafeMethod = HttpMethods.IsGet(context.Request.Method) ||
                       HttpMethods.IsHead(context.Request.Method) ||
                       HttpMethods.IsOptions(context.Request.Method) ||
                       HttpMethods.IsTrace(context.Request.Method);

    if (!isApiRequest || isSafeMethod)
    {
        await next();
        return;
    }

    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException ex)
    {
        app.Logger.LogWarning(ex, "CSRF validation failed for {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
        {
            code = "csrf_validation_failed",
            description = "Missing or invalid CSRF token/cookie pair."
        });
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
