using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services;
using NewsApp.Core.Services.UsersServices;
using NewsApp.Core.Services.UsersServices.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<NewsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddSingleton<AuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(g =>
{
    g.SwaggerDoc("v1", new OpenApiInfo { Title = "News app", Version = "v1" });
    g.CustomSchemaIds(id => id.FullName!.Replace('*', '-'));

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authentication",
        Description = "Enter the token you recieved after successful login",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    };
    g.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    };
    g.AddSecurityRequirement(securityRequirement);
});

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
    options.AddPolicy(PoliciesEnum.God.ToString(), policy =>
        policy.RequireClaim(
            builder.Configuration["Jwt:AuthorizationClaim"] ?? "role", RolesEnum.Admin.ToString()
        )
    );
    options.AddPolicy(PoliciesEnum.MustBeRegisteredAuthor.ToString(), policy => 
        policy.RequireClaim(
            builder.Configuration["Jwt:AuthorizationClaim"] ?? "role", RolesEnum.Author.ToString(), RolesEnum.Admin.ToString()
        )
    );
    options.AddPolicy(PoliciesEnum.MustBeRegistered.ToString(), policy => 
        policy.RequireRole(
            builder.Configuration["Jwt:AuthorizationClaim"] ?? "role", RolesEnum.User.ToString(), RolesEnum.Author.ToString(), RolesEnum.Admin.ToString()
        )
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await InitializeData.SeedRoles(services);
    await InitializeData.SeedUsers(services);
    await InitializeData.SeedCategories(services);
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors("AllowDomains");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
