using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Authentication.Filters;
using SurveyBasket.API.Extensions;
using SurveyBasket.API.Health;
using SurveyBasket.API.Persistence;
using SurveyBasket.Authentication;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection Service, WebApplicationBuilder builder, IConfiguration configuration)
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

            Service.AddScoped<INotificationService, NotificationService>();

            Service.AddSingleton<IJwtProvider, JwtProvider>(); // Registers JwtProvider with singleton lifetime for JWT token creation

            Service.AddScoped<IQuestionService, QuestionService>();

            Service.AddScoped<IVoteService, VoteService>();

            Service.AddScoped<IUserService, UserService>();

            Service.AddScoped<IResultService, ResultService>();

            Service.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); // Registers ASP.NET Core Identity services with EF Core store


            Service.Configure<MailOptions>(builder.Configuration.GetSection("MailSettings"));

            Service.AddExceptionHandler<GlobalException>();

            Service.AddScoped<IEmailSender, EmailSender>();

            Service.AddHealthChecks().
                AddSqlServer(name: "MyDb", connectionString: configuration.GetConnectionString("DefaultConnection")!)
                .AddHangfire(options => { options.MinimumAvailableServers = 1; })
                .AddCheck<MailProviderHealthCheck>(name: "Mail Service");

            Service.AddScoped<IRoleService, RoleService>();

            Service.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();
            Service.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            Service.AddApiVersioning(option =>
            {
                option.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
            Service.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;


                rateLimiterOptions.AddPolicy<string>("ipLimit", httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    );
                });


                rateLimiterOptions.AddPolicy<string>("userLimit", httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.GetUserId() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    );
                });


                //rateLimiterOptions.AddConcurrencyLimiter("Concurrency", options =>
                //{
                //    options.PermitLimit = 2;
                //    options.QueueLimit =1;
                //    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;

                //});


                //rateLimiterOptions.AddTokenBucketLimiter("token", options =>
                //{
                //    options.TokenLimit = 2;
                //    options.QueueLimit = 1;
                //    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                //    options.AutoReplenishment = true;

                //});

                //rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
                //{
                //    options.PermitLimit = 2;
                //    options.Window = TimeSpan.FromSeconds(20);
                //    options.QueueLimit = 1;
                //    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;


                //});

                //rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
                //{
                //    options.PermitLimit = 2;
                //    options.Window = TimeSpan.FromSeconds(20);
                //    options.SegmentsPerWindow = 2;
                //    options.QueueLimit = 1;
                //    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;


                //});

            });


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


            Service.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.User.RequireUniqueEmail = true;
            });


            Service.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            Service.AddHangfireServer();
            return Service;



        }



    }
}
