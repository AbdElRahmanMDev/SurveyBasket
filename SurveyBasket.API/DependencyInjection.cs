using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Persistence;
using SurveyBasket.Authentication;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SurveyBasket.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection Service, WebApplicationBuilder builder,IConfiguration configuration)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            Service.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(connectionString));
            // Add services to the container.

            Service.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Service.AddEndpointsApiExplorer();
            Service.AddSwaggerGen();

            //Add Mapseter
            var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
            mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
            Service.AddSingleton<IMapper>(new Mapper(mappingConfiguration));


            //Fluent Validations
            Service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            Service.AddFluentValidationAutoValidation();
            //builder.Services.AddMapster();

            //IServices
            Service.AddScoped<IPollService, PollService>();
            Service.AddScoped<IAuthService, AuthService>();


            //Identity Services

            Service.AddSingleton<IJwtProvider, JwtProvider>();
            Service.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            Service.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            Service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer=true,
                    ValidateLifetime=true,
                    ValidateAudience=true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                };
            });

            var test = new
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!)),
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
            };
            return Service;

        }


        
    }
}
