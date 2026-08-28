using System.Text;
using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Lost_Found
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads", "oglasi"));

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "Jwt:Key is not configured. Set it via dotnet user-secrets (Development) or the Jwt__Key environment variable.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            const string AngularDevCors = "AngularDev";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AngularDevCors, policy =>
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IKorisnikService, KorisnikService>();
            builder.Services.AddScoped<IOglasService, OglasService>();
            builder.Services.AddScoped<IPotrazivanjeService, PotrazivanjeService>();
            builder.Services.AddScoped<IRazgovorService, RazgovorService>();
            builder.Services.AddScoped<IPorukaService, PorukaService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors(AngularDevCors);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
