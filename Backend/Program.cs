using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend_Example;
using Backend_Example.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Temporary CORS policy to allow all origins
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()
    )
);

builder.Services.AddDbContext<PotluckDb>();
builder.Services.AddAuthentication().AddCookie();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<PotluckDb>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 15;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PotluckDb>();
    db.Database.Migrate();
}

app.MapIdentityApi<User>();
app.UseAuthentication();
app.UseAuthorization();

var authed = app.MapGroup("").RequireAuthorization();

authed.SetupHomeRoutes();
authed.SetupCookingRoutes();
authed.SetupShoppingRoutes();
authed.SetupSettingsRoutes();

app.Run();
