const API_URL = 'http://localhost:5215/artworks';

// 1. Fetch and render the gallery
async function loadGallery() {
    const response = await fetch(API_URL);
    const data = await response.json();
    
    const gallery = document.getElementById('gallery');
    gallery.innerHTML = '';

    data.forEach(art => {
        const div = document.createElement('div');
        div.className = 'art-card'; // We will use this for CSS later
        div.innerHTML = `
            <h3>${art.title}</h3>
            <p>Artist: ${art.artist}</p>
            <p>Year: ${art.year}</p>
            <button onclick="deleteArt(${art.id})">Delete</button>
            <button onclick="editArt(${art.id})">Edit</button>
        `;
        gallery.appendChild(div);
    });
}

// 2. Handle form submission (Create)
document.getElementById('artForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const newArt = {
        id: parseInt(document.getElementById('id').value),
        title: document.getElementById('title').value,
        artist: document.getElementById('artist').value,
        year: parseInt(document.getElementById('year').value),
        imageUrl: document.getElementById('imageUrl').value
    };

    await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newArt)
    });
    
    location.reload();
});

// Load the gallery on startup
loadGallery();

// 3. Handle delete operation (Delete)
async function deleteArt(id) {
    await fetch(`${API_URL}/${id}`, {
        method: 'DELETE'
    });
    location.reload();
}

async function editArt(id) {
    const newYear = prompt("Enter new year:");
    if (!newYear) return;

    // Fetch the specific item first
    const response = await fetch(API_URL);
    const data = await response.json();
    const art = data.find(a => a.id === id); // Ensure this matches your C# record property casing

    if (art) {
        art.year = parseInt(newYear); // Update the local object

        await fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(art)
        });
        location.reload();
    }
}