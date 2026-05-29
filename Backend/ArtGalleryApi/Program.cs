var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapGet("/artworks", () =>
{
    return artworks;
})
.WithName("GetArtworks")
.WithOpenApi();

app.Run();

record ArtPiece(int Id, string Title, string Artist, int Year, string ImageUrl);