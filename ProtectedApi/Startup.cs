using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProtectedApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Configure MySQL database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            // Add ASP.NET Core Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            Console.WriteLine("Identity added");
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = msg =>
                        {
                            var token = msg?.Request.Headers.Authorization.ToString();
                            string path = msg?.Request.Path ?? "";
                            if (!string.IsNullOrEmpty(token))

                            {
                                Console.WriteLine("Access token");
                                Console.WriteLine($"URL: {path}");
                                Console.WriteLine($"Token: {token}\r\n");
                            }
                            else
                            {
                                Console.WriteLine("Access token");
                                Console.WriteLine("URL: " + path);
                                Console.WriteLine("Token: No access token provided\r\n");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.Authority = Configuration["IdentityServer:Authority"];
                    options.Audience = "api1"; // Ensure this matches the audience claim in the token
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = "api1", // Ensure this matches the audience claim
                        ValidIssuer = Configuration["IdentityServer:Authority"]
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, "admin"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Example of how to use the logger
            logger.LogInformation("Application started");
        }
    }
}
