using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.SetIsOriginAllowed(origin =>
    {
        try
        {
            var host = new Uri(origin).Host;
            if (host == "localhost") return true;
            if (host.EndsWith("vercel.app")) return true;
            var extra = builder.Configuration["ALLOWED_ORIGINS"] ?? "";
            return extra.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Any(x => string.Equals(new Uri(x).Host, host, StringComparison.OrdinalIgnoreCase));
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


