using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Auth0 settings from appsettings.json or environment
var auth0Domain = builder.Configuration["Auth0:Domain"];    // e.g. dev-xxx.us.auth0.com
var auth0Audience = builder.Configuration["Auth0:Audience"]; // e.g. https://todo-api.local


// DB
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for localhost + *.vercel.app (+ optional ALLOWED_ORIGINS)
// CORS for localhost/127.0.0.1 + *.vercel.app (+ optional ALLOWED_ORIGINS)
builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.SetIsOriginAllowed(origin =>
    {
        try
        {
            var uri = new Uri(origin);
            var host = uri.Host;

            // Local dev
            if (host == "localhost" || host == "127.0.0.1") return uri.Port == 3000;

            // Vercel preview/prod
            if (host.EndsWith("vercel.app", StringComparison.OrdinalIgnoreCase)) return true;

            // Extra comma-separated origins in appsettings or env
            var extra = builder.Configuration["ALLOWED_ORIGINS"] ?? "";
            foreach (var x in extra.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                // compare full origin
                if (string.Equals(origin.TrimEnd('/'), x.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        catch { return false; }
    })
    .AllowAnyHeader()
    .AllowAnyMethod()
));


var app = builder.Build();

// Auto-migrate on boot
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();


