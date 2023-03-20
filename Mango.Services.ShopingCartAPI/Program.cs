﻿using AutoMapper;
using Mango.AzureBus;
using Mango.Services.ShopingCartAPI.DbContexts;
using Mango.Services.ShopingCartAPI.Helpers;
using Mango.Services.ShopingCartAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

Mango.Services.ShopingCartAPI.Helpers.SD.BaseTopic = config["ServiceBusTopics:DefaultTopic"];
Mango.Services.ShopingCartAPI.Helpers.SD.CouponAPIBase = config["ServiceUrls:CouponApi"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Description = @"Enter 'Bearer' [space] and your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
            Reference = new OpenApiReference
            {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseSqlServer(config.GetConnectionString("Windows")));

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>{
    options.Authority = "https://localhost:7295/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization( options =>{
    options.AddPolicy("ApiScope", policy =>{
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope","mango");
    });
});

builder.Services.AddHttpClient<ICouponRepository, CouponRepository>(x=>x.BaseAddress = new Uri(config["ServiceUrls:CouponApi"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

