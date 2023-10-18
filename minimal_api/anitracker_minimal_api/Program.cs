using anitracker_minimal_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContext<TitleDb>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DevelopConnection")));
else
    builder.Services.AddDbContext<TitleDb>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var app = builder.Build();

#region GETs
app.MapGet("/", () => "Service is alive");
#endregion

#region POSTs
app.MapPost("titlelist/add", async (string title, string? poster, TitleDb db) =>
{
    try
    {
        var response = await TitleLibrary.Add(db, title, poster);
        return response.ID == -1 ? Results.BadRequest("Title is a duplicate") : Results.Ok(response);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: $"An error occurred: {ex.Message}", statusCode: 500);
    }
});
app.MapPost("titlelist/bulk", async ([FromBody] List<TitleItem> titles, TitleDb db) =>
{
    try
    {
        return Results.Ok(await TitleLibrary.AddRange(db, titles));
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: $"An error occurred: {ex.Message}", statusCode: 500);
    }
});
app.MapPost("titlelist/find", (string title, TitleDb db) =>
{
    try
    {
        return TitleLibrary.Find(db, title) is TitleItem db_title
            ? Results.Ok(new { is_found = true, message = $"{db_title.Title} found with ID {db_title.ID}" })
            : Results.NotFound(new { is_found = false, message = $"{title} was not found" });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: $"An error occurred: {ex.Message}", statusCode: 500);
    }
});
#endregion

app.UseCors();

app.Run();
