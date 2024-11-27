using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.FileManipulation;
using se_24.backend.src.Interfaces;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<IReadingLevelRepository, ReadingLevelRepository>();
builder.Services.AddScoped<IFinderLevelRepository, FinderLevelRepository>();
builder.Services.AddScoped<ILevelFilesRepository, LevelFilesRepository>();
builder.Services.AddScoped<ILevelLoader<Level>, LevelLoader<Level>>();
builder.Services.AddScoped<ILevelLoader<ReadingLevel>, LevelLoader<ReadingLevel>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
