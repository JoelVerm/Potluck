using Data;
using Logic;
using Logic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Potluck.API;
using Saunter;
using Saunter.AsyncApiSchema.v2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAsyncApiSchemaGeneration(o =>
{
    o.AsyncApi = new AsyncApiDocument
    {
        Info = new Info("Potluck API", "0.0.1"),
        Servers = { ["ws"] = new Server("0.0.0.0", "ws") }
    };
});

// Temporary CORS policy to allow all origins
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    )
);

builder.Services.AddDbContext<IPotluckDb, PotluckDb>();
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

builder.Services.AddScoped(typeof(UserLogic), p => new UserLogic(p));
builder.Services.AddScoped(typeof(HouseLogic), p => new HouseLogic(p));

var app = builder.Build();

app.UseCors();
app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapAsyncApiDocuments();
    app.MapAsyncApiUi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PotluckDb>();
    db.Database.Migrate();
}

app.MapIdentityApi<User>().WithTags("Identity");
app.UseAuthentication();
app.UseAuthorization();

var authed = app.MapGroup("").RequireAuthorization();

authed.MapGroup("").SetupUsersRoutes().WithTags("Users");
authed.MapGroup("").SetupHousesRoutes().WithTags("Houses");

app.Run();