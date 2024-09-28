using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsApp.Api.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<NewsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDomains", builder =>
    {
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty /* Empty string just to resolve possible null warning */))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole(
            RolesEnum.Admin.ToString()
        )
    );
    options.AddPolicy("AuthorPolicy", policy => 
        policy.RequireRole(
            RolesEnum.Author.ToString(),
            RolesEnum.Admin.ToString()
        )
    );
    options.AddPolicy("UserPolicy", policy => 
        policy.RequireRole(
            RolesEnum.User.ToString(), 
            RolesEnum.Author.ToString(), 
            RolesEnum.Admin.ToString()
        )
    );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeData.SeedRoles(services);
    await InitializeData.SeedUsers(services);
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors("AllowDomains");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
