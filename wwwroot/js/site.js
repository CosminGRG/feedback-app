// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const feedbackForm = document.getElementById('createFeedbackForm');
feedbackForm.addEventListener('sumbit', function () {
    const button = document.getElementById('submitFeedback');
    button.disabled = true;

    setTimeout(() => {
        button.disabled = false;
    }, 3000)
});

/*const searchInput = document.getElementById('searchBox');
searchInput.addEventListener('input', function () {
    const searchString = searchInput.value;
    
    fetch(`/Home/SearchFeedback?searchString=${encodeURIComponent(searchString)}`)
        .then(response => {
            if (!response.ok) {
                console.error('Error fetching search results:', error);
            }
            return response.text(); // Parse the response as text
        })
        .then(html => {
            // Update the search results container with the HTML content
            searchResultsContainer.innerHTML = html;
        })
        .catch(error => {
            console.error('Error fetching search results:', error);
    });
});*/