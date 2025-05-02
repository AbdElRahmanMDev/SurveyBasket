using FluentValidation;
using MapsterMapper;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Mapseter
var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));


//Fluent Validations
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddMapster();

builder.Services.AddScoped<IPollService,PollService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
