const API_URL = 'http://localhost:5215/artworks';

// 1. Initialize everything once the page is fully loaded
document.addEventListener("DOMContentLoaded", () => {
    loadGallery();

    const form = document.getElementById('artForm');
    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            // Use lowercase keys to match the API requirements
            const newArt = {
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
            
            form.reset();
            loadGallery();
        });
    }
});

// 2. Fetch and render the gallery
async function loadGallery() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error("Could not fetch data");
        const data = await response.json();
        
        const gallery = document.getElementById('gallery');
        if (!gallery) return;
        
        gallery.innerHTML = ''; 

        data.forEach(art => {
            const div = document.createElement('div');
            div.className = 'art-card';
            
            div.innerHTML = `
                <div style="width: 100%; height: 200px; background-color: #444; overflow: hidden; margin-bottom: 10px;">
                    <img src="${art.imageUrl}" alt="${art.title}" style="width: 100%; height: 100%; object-fit: cover;" 
                        onerror="this.onerror=null; this.src='https://upload.wikimedia.org/wikipedia/commons/a/ac/No_image_available.svg';">
                </div>
                <h3>${art.title}</h3>
                <p>Artist: ${art.artist}</p>
                <p>Year: ${art.year}</p>
                <button onclick="deleteArt(${art.id})">Delete</button>
                <button onclick="editArt(${art.id})">Edit</button>
            `;
            gallery.appendChild(div);
        });
    } catch (error) {
        console.error("Gallery load error:", error);
    }
}

// 3. Handle delete operation
async function deleteArt(id) {
    await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    loadGallery();
}

// 4. Handle edit operation
async function editArt(id) {
    const newYear = prompt("Enter new year:");
    if (!newYear) return;

    // Fetch the latest data to find the correct object
    const response = await fetch(API_URL);
    const data = await response.json();
    
    // Use lowercase 'id' to find the art piece
    const art = data.find(a => a.id === id);

    if (art) {
        // Update the lowercase 'year' property
        art.year = parseInt(newYear);
        
        // Send the PUT request with the updated object
        await fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(art)
        });
        
        loadGallery();
    }
}