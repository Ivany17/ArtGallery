// --- SETUP: Create the builder for the web application ---
using Microsoft.EntityFrameworkCore; // Required for .ToListAsync()

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES: Configure dependencies the app needs ---
builder.Services.AddEndpointsApiExplorer(); // Enables API documentation discovery
builder.Services.AddSwaggerGen();           // Generates the Swagger UI documentation

// Define a CORS policy to allow the frontend to talk to this backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Register the Database Context
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keeps PascalCase
});

// --- BUILD: Create the actual app instance ---
var app = builder.Build();

// --- DATABASE INITIALIZATION ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // This creates the file and tables if they are missing
}

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

// Apply the CORS policy created above
app.UseCors("AllowAll");

// --- ENDPOINTS: API Routing ---

// GET: Retrieve the list of all artworks
// Use "async" and wrap the return in "Task"
app.MapGet("/artworks", async (AppDbContext db, int page = 1, int pageSize = 12) =>
{
    // Ensure we don't have negative pages
    if (page < 1) page = 1;

    return await db.Artworks
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
})
.WithName("GetArtworks")
.WithOpenApi();

app.MapGet("/artworks/{id}", async (int id, AppDbContext db) =>
{
    var art = await db.Artworks.FindAsync(id);
    return art is not null ? Results.Ok(art) : Results.NotFound();
});

// POST: Add a new art piece to the list
app.MapPost("/artworks", async (ArtPiece newArt, AppDbContext db) =>
{
    db.Artworks.Add(newArt);
    await db.SaveChangesAsync();
    return Results.Created($"/artworks/{newArt.Id}", newArt);
});

// --- DELETE: Remove an art piece by Id from Database ---
app.MapDelete("/artworks/{id}", async (int id, AppDbContext db) =>
{
    var art = await db.Artworks.FindAsync(id);
    if (art is null) return Results.NotFound();

    db.Artworks.Remove(art);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// --- PUT: Update an existing art piece by Id in Database ---
app.MapPut("/artworks/{id}", async (int id, ArtPiece updatedArt, AppDbContext db) =>
{
    var art = await db.Artworks.FindAsync(id);
    if (art is null) return Results.NotFound();

    // Update properties
    // Note: If using a record, you might need to map properties individually
    // or create a new object depending on your design.
    db.Entry(art).CurrentValues.SetValues(updatedArt);

    await db.SaveChangesAsync();
    return Results.Ok(updatedArt);
});

// --- EXECUTION: Start the server ---
app.Run();