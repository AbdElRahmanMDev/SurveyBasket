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
                 options.UseSqlServer(connectionString)); // Registers Entity Framework DbContext using SQL Server provider

            Service.AddControllers(); // Registers MVC controllers for API support

            Service.AddEndpointsApiExplorer(); // Registers API endpoint metadata for minimal APIs or Swagger

            Service.AddSwaggerGen(); // Registers Swagger generator for API documentation

            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()!; //Bind AllowedOrigins 

            Service.AddCors(option =>
            {
                option.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(allowedOrigins);

                });

               
            });  //Register Cors


            var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
            mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
            Service.AddSingleton<IMapper>(new Mapper(mappingConfiguration)); // Registers Mapster's IMapper as a singleton for object mapping


            Service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // Registers FluentValidation validators from current assembly


            Service.AddFluentValidationAutoValidation(); // Enables automatic FluentValidation model validation



            Service.AddScoped<IPollService, PollService>(); // Registers PollService with scoped lifetime for dependency injection


            Service.AddScoped<IAuthService, AuthService>(); // Registers AuthService with scoped lifetime for dependency injection


            Service.AddSingleton<IJwtProvider, JwtProvider>(); // Registers JwtProvider with singleton lifetime for JWT token creation

            Service.AddScoped<IQuestionService, QuestionService>();


            Service.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>(); // Registers ASP.NET Core Identity services with EF Core store



            Service.AddExceptionHandler<GlobalException>();

            Service.AddProblemDetails();

            //services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt")));

            Service.AddOptions<JwtOptions>() // Registers JwtOptions for IOptions<T> support
                .BindConfiguration("Jwt") // Binds JwtOptions to configuration section "Jwt"
                .ValidateDataAnnotations() // Adds validation using data annotations
                .ValidateOnStart(); // Validates options at application startup

            Service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                };
            }); // Registers JWT Bearer authentication scheme and token validation parameters

            return Service;

        }


        
    }
}
