using MazeScraper.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<ITvShowRepository, TvShowRepository>();
builder.Services.AddScoped<ICastRepository, CastRepository>();
builder.Services.AddScoped<ITvShowService, TvShowService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();