// 1. Fetch and render the gallery
fetch('http://localhost:5215/artworks')
    .then(response => response.json())
    .then(data => {
        const gallery = document.getElementById('gallery');
        gallery.innerHTML = ''; // Clear gallery first

        data.forEach(art => {
            const div = document.createElement('div');
            div.innerHTML = `
                <h3>${art.title}</h3>
                <p>Artist: ${art.artist}</p>
                <p>Year: ${art.year}</p>
                <button onclick="deleteArt(${art.id})">Delete</button>
            `;
            // Update this section inside your fetch .then() block in app.js
            div.innerHTML = `
                <h3>${art.title}</h3>
                <p>Artist: ${art.artist}</p>
                <p>Year: ${art.year}</p>
                <button onclick="deleteArt(${art.id})">Delete</button>
                <button onclick="editArt(${art.id})">Edit</button> 
            `;
            gallery.appendChild(div);
        });
    });

// 2. Handle form submission (Create)
document.getElementById('artForm').addEventListener('submit', (e) => {
    e.preventDefault();

    const newArt = {
        id: parseInt(document.getElementById('id').value),
        title: document.getElementById('title').value,
        artist: document.getElementById('artist').value,
        year: parseInt(document.getElementById('year').value),
        imageUrl: document.getElementById('imageUrl').value
    };

    fetch('http://localhost:5215/artworks', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newArt)
    })
    .then(() => {
        alert("Artwork added!");
        location.reload();
    });
});

// 3. Handle delete operation (Delete)
function deleteArt(id) {
    fetch(`http://localhost:5215/artworks/${id}`, {
        method: 'DELETE'
    })
    .then(() => {
        location.reload();
    });
}

function editArt(id) {
    const newYear = prompt("Enter new year:");
    if (!newYear) return;

    // Find the current artwork to get its other details
    fetch(`http://localhost:5215/artworks`)
        .then(res => res.json())
        .then(data => {
            const art = data.find(a => a.Id === id);
            art.Year = parseInt(newYear);

            fetch(`http://localhost:5215/artworks/${id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(art)
            })
            .then(() => location.reload());
        });
}