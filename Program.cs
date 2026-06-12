var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        if (origins.Length == 0 || origins.Contains("*"))
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddScoped<FestivalCine.Database.IDbConnectionFactory, FestivalCine.Database.SqlConnectionFactory>();
builder.Services.AddScoped<FestivalCine.Services.ITaquillaService, FestivalCine.Services.TaquillaService>();
builder.Services.AddScoped<FestivalCine.Services.IAgendaService, FestivalCine.Services.AgendaService>();
builder.Services.AddScoped<FestivalCine.Services.IReportesService, FestivalCine.Services.ReportesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<FestivalCine.Common.SqlExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("FrontendCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
