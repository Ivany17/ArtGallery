// --- SETUP: Create the builder for the web application ---
var builder = WebApplication.CreateBuilder(args);

// --- SERVICES: Configure dependencies the app needs ---
builder.Services.AddEndpointsApiExplorer(); // Enables API documentation discovery
builder.Services.AddSwaggerGen();           // Generates the Swagger UI documentation

// Define a CORS policy to allow the frontend to talk to this backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// --- BUILD: Create the actual app instance ---
var app = builder.Build();

// --- MIDDLEWARE: Global error safety net ---
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

// --- MIDDLEWARE: Development-only tools ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Serve the Swagger JSON
    app.UseSwaggerUI();  // Serve the interactive Swagger UI
}

// Redirect all HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// --- DATA: In-memory storage (The "Database") ---
var artworks = new List<ArtPiece>
{
    new ArtPiece(1, "The Starry Night", "Vincent van Gogh", 1889, "starry_night.jpg"),
    new ArtPiece(2, "Mona Lisa", "Leonardo da Vinci", 1503, "mona_lisa.jpg")
};

// Apply the CORS policy created above
app.UseCors("AllowAll");

// --- ENDPOINTS: API Routing ---

// GET: Retrieve the list of all artworks
// Use "async" and wrap the return in "Task"
app.MapGet("/artworks", async () =>
{
    // Await ensures we don't block the thread
    return await Task.FromResult(artworks);
})
.WithName("GetArtworks")
.WithOpenApi();

// POST: Add a new art piece to the list
app.MapPost("/artworks", async (ArtPiece newArt) =>
{
    if (string.IsNullOrWhiteSpace(newArt.Title) || newArt.Year <= 0)
    {
        return Results.BadRequest("Invalid data");
    }
    // Using Task.Run simulates a non-blocking operation
    await Task.Run(() => artworks.Add(newArt));
    return Results.Created($"/artworks/{newArt.Id}", newArt);
});

// DELETE: Remove an art piece by Id
app.MapDelete("/artworks/{id}", async (int id) =>
{
    var art = artworks.FirstOrDefault(a => a.Id == id);
    if (art is null) return Results.NotFound(); // Return 404 if item doesn't exist

    await Task.Run(() => artworks.Remove(art));
    return Results.NoContent(); // Return 204 success (nothing to show)
});

// PUT: Update an existing art piece by Id
app.MapPut("/artworks/{id}", async (int id, ArtPiece updatedArt) =>
{
    var index = artworks.FindIndex(a => a.Id == id);
    if (index == -1) return Results.NotFound(); // Return 404 if item not found

    await Task.Run(() => artworks[index] = updatedArt);
    return Results.Ok(updatedArt); // Return the updated object
});

// --- EXECUTION: Start the server ---
app.Run();

// --- DATA MODEL: The structure of our objects ---
record ArtPiece(int Id, string Title, string Artist, int Year, string ImageUrl);