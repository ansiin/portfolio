const cafesContainer = document.getElementById("cafes");
const statusElement = document.getElementById("status");
const filterForm = document.getElementById("filter-form");
const createForm = document.getElementById("create-form");
const updateForm = document.getElementById("update-form");
const deleteForm = document.getElementById("delete-form");
const showAllButton = document.getElementById("show-all");

function formToObject(form, keepEmpty = false) {
    // Convert form values into a plain object for API requests.
    const data = new FormData(form);
    const result = {};

    for (const [key, value] of data.entries()) {
        if (keepEmpty || value.trim() !== "") {
            result[key] = value.trim();
        }
    }

    return result;
}

function showStatus(message, isError = false) {
    // Display the latest API result near the cafe list.
    statusElement.textContent = message;
    statusElement.className = isError ? "status error" : "status";
}

function renderEmptyState() {
    // Show a friendly message when no cafes match the request.
    cafesContainer.innerHTML = '<p class="empty">Sobivaid kohvikuid ei leitud.</p>';
}

function renderCafes(cafes) {
    // Render the current cafe list into card elements.
    if (!cafes.length) {
        renderEmptyState();
        return;
    }

    cafesContainer.innerHTML = cafes.map((cafe) => `
        <article class="card">
            <div class="card-top">
                <strong>${cafe.name}</strong>
                <span>#${cafe.id}</span>
            </div>
            <p>${cafe.location}</p>
            <p>Avatud: ${cafe.time_open} - ${cafe.time_closed}</p>
            <p>Teenusepakkuja: ${cafe.provider || "-"}</p>
        </article>
    `).join("");
}

async function parseApiResponse(response, fallbackMessage) {
    // Parse JSON and raise a readable error when the API fails.
    const data = await response.json();
    if (!response.ok) {
        throw new Error(data.error || fallbackMessage);
    }
    return data;
}

async function fetchJson(url, options = {}, fallbackMessage = "Päring ebaõnnestus.") {
    // Send an HTTP request and return validated JSON data.
    const response = await fetch(url, options);
    return parseApiResponse(response, fallbackMessage);
}

async function loadCafes(query = "") {
    // Load cafes from the API and refresh the visible list.
    try {
        const cafes = await fetchJson(`/api/cafes${query}`);
        renderCafes(cafes);
        showStatus(`Leitud: ${cafes.length}`);
    } catch (error) {
        renderEmptyState();
        showStatus(error.message, true);
    }
}

async function handleFilterSubmit(event) {
    // Filter cafes by the requested time range.
    event.preventDefault();
    const values = formToObject(filterForm, true);
    const query = `?start=${encodeURIComponent(values.start)}&end=${encodeURIComponent(values.end)}`;
    await loadCafes(query);
}

async function handleCreateSubmit(event) {
    // Create a new cafe using form input.
    event.preventDefault();
    const payload = formToObject(createForm, true);

    try {
        const cafe = await fetchJson(
            "/api/cafes",
            {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(payload),
            },
            "Lisamine ebaõnnestus.",
        );

        createForm.reset();
        showStatus(`Lisatud kohvik ID-ga ${cafe.id}.`);
        await loadCafes();
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function handleUpdateSubmit(event) {
    // Update the selected cafe and reload the list.
    event.preventDefault();
    const values = formToObject(updateForm);
    const {id} = values;
    delete values.id;

    try {
        const cafe = await fetchJson(
            `/api/cafes/${id}`,
            {
                method: "PUT",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(values),
            },
            "Muutmine ebaõnnestus.",
        );

        updateForm.reset();
        showStatus(`Muudetud kohvik ID-ga ${cafe.id}.`);
        await loadCafes();
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function handleDeleteSubmit(event) {
    // Delete a cafe by ID and refresh the current list.
    event.preventDefault();
    const values = formToObject(deleteForm, true);

    try {
        const result = await fetchJson(
            `/api/cafes/${values.id}`,
            {method: "DELETE"},
            "Kustutamine ebaõnnestus.",
        );

        deleteForm.reset();
        showStatus(result.message);
        await loadCafes();
    } catch (error) {
        showStatus(error.message, true);
    }
}

function initializePage() {
    // Bind form handlers and load data on first page view.
    filterForm.addEventListener("submit", handleFilterSubmit);
    createForm.addEventListener("submit", handleCreateSubmit);
    updateForm.addEventListener("submit", handleUpdateSubmit);
    deleteForm.addEventListener("submit", handleDeleteSubmit);
    showAllButton.addEventListener("click", async () => {
        filterForm.reset();
        await loadCafes();
    });

    loadCafes();
}

initializePage();
