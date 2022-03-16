using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaxiCentral.API.Infrastructure;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Helpers;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(x =>
{
    x.ReturnHttpNotAcceptable = true;
    x.OutputFormatters.RemoveType<StringOutputFormatter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });

    x.AddSecurityRequirement(new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:Secret"]))
        };
    });

//builder.Services.AddAuthorization(x =>
//{
//    x.AddPolicy("Driver", y =>
//    {
//        y.RequireAuthenticatedUser();
//        y.RequireClaim("type", "driver");
//    });

//    x.AddPolicy("Dispatcher", y =>
//    {
//        y.RequireAuthenticatedUser();
//        y.RequireClaim("type", "dispatcher");
//    });

//    x.AddPolicy("Admin", y =>
//    {
//        y.RequireAuthenticatedUser();
//        y.RequireClaim("type", "admin");
//    });
//});

builder.Services.AddDbContext<TaxiCentralContext>(x =>
{
    var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
    x.UseSqlServer(builder.Configuration["ConnectionString"], 
        y => y.MigrationsAssembly(migrationsAssembly));
});

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IRideRepository, RideRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidation();

var app = builder.Build();

//if (app.Environment.IsProduction())
//{
    app.UseExceptionHandler(x =>
    {
        x.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                if (exceptionHandlerFeature.Error is not AppException applicationException)
                {
                    const string serverErrorMessage = "An unexpected error occurred.  Please try again later.";
                    context.Response.AddApplicationError(serverErrorMessage);

                    // log the error
                    app.Logger.LogError(context.Response.StatusCode, exceptionHandlerFeature.Error,
                        exceptionHandlerFeature.Error.Message);

                    await context.Response.WriteAsync(serverErrorMessage);
                    return;
                }

                context.Response.AddApplicationError(exceptionHandlerFeature.Error.Message, applicationException.IsJson);
                await context.Response.WriteAsync(exceptionHandlerFeature.Error.Message);
            }
        });
    });
//}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

//todo: test this!!
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TaxiCentralContext>();
    context.Database.Migrate();
}

app.Run();
