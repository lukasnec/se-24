using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using se_24.backend.src.Data;
using se_24.backend.src.FileManipulation;
using se_24.backend.src.Interfaces;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<IReadingLevelRepository, ReadingLevelRepository>();
builder.Services.AddScoped<IFinderLevelRepository, FinderLevelRepository>();
builder.Services.AddScoped<ILevelFilesRepository, LevelFilesRepository>();
builder.Services.AddScoped<ILevelLoader<Level>, LevelLoader<Level>>();
builder.Services.AddScoped<ILevelLoader<ReadingLevel>, LevelLoader<ReadingLevel>>();

// Add Authentication using JWT Bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7175") // Replace with your Blazor frontend's URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS Middleware
app.UseCors("AllowFrontend");

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
