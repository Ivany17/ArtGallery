var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var artworks = new List<ArtPiece>
{
    new ArtPiece(1, "The Starry Night", "Vincent van Gogh", 1889, "starry_night.jpg"),
    new ArtPiece(2, "Mona Lisa", "Leonardo da Vinci", 1503, "mona_lisa.jpg")
};

app.UseCors("AllowAll");

app.MapGet("/artworks", () =>
{
    return artworks;
})
.WithName("GetArtworks")
.WithOpenApi();

app.MapPost("/artworks", (ArtPiece newArt) =>
{
    if (string.IsNullOrWhiteSpace(newArt.Title) || newArt.Year <= 0)
    {
        Results.BadRequest("Invalid data");
    }
    artworks.Add(newArt);
    return Results.Created($"/artworks/{newArt.Id}", newArt);
});

app.MapDelete("/artworks/{id}", (int id) =>
{
    var art = artworks.FirstOrDefault(a => a.Id == id);
    if (art is null) return Results.NotFound();

    artworks.Remove(art);
    return Results.NoContent();
});

app.MapPut("/artworks/{id}", (int id, ArtPiece updatedArt) =>
{
    var index = artworks.FindIndex(a => a.Id == id);
    if (index == -1) return Results.NotFound();

    artworks[index] = updatedArt;
    return Results.Ok(updatedArt);
});

app.Run();

record ArtPiece(int Id, string Title, string Artist, int Year, string ImageUrl);