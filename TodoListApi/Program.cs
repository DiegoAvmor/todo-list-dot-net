using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TodoListApi.Config;
using TodoListApi.Endpoints;
using TodoListApi.Models.Data.DTO;
using TodoListApi.Models.Data.Validators;
using TodoListApi.Models.DB;
using TodoListApi.Utilities;

var builder = WebApplication.CreateBuilder(args);

//Setup Mapping from appsettings.json to the ApplicationConfig class
var appConfig = new ApplicationConfig();
builder.Configuration.GetSection(nameof(ApplicationConfig)).Bind(appConfig);
builder.Services.AddSingleton(appConfig);

//Setup Singleto for Utilities
builder.Services.AddSingleton<AesEncryption>();
//builder.Services.AddSingleton<TokenUtility>();

//Setup Logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
 
Log.Logger = logger;
 
builder.Host.UseSerilog();

//Setup Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.AddSecurityDefinition(appConfig.JwtConfig.Scheme, new OpenApiSecurityScheme{
        Description ="JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = appConfig.JwtConfig.Scheme
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = appConfig.JwtConfig.Scheme
                },
                Scheme = "oauth2",
                Name = appConfig.JwtConfig.Scheme,
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});
//Setup AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingConfig));

//Setup Serialization to ignore Circular Reference
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => 
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

//Setup Validators
builder.Services.AddValidatorsFromAssemblyContaining<TodoTaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddScoped<IValidator<TodoTaskRequestDTO>, TodoTaskValidator>();
builder.Services.AddScoped<IValidator<RegistrationRequestDTO>, UserValidator>();

//Setup Database Connection
builder.Services.AddDbContext<TodoTaskDB>(
    opt => opt.UseMySQL(appConfig.DbConfig.getConnectionString())
);

//Setup Authentication and Authorization
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>{
    x.RequireHttpsMetadata = false;
    x.SaveToken=true;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            appConfig.JwtConfig.Key
        )),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

//Setup Roles Policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("User", policy => policy.RequireRole("user"));
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Register Endpoints
app.RegisterAuthEndpoints();
app.RegisterTodoTaskEndpoints();
app.RegisterUserManagementEndpoints();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();