const uri = 'api/TagsApi';
let tags = [];

function getTags() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayTags(data))
        .catch(error => console.error('Unable to get tags.', error));
}

function generateSlug(name) {
    return name.trim().toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9\-]/g, '');
}

function addTag() {
    const nameInput = document.getElementById('add-name');
    const name = nameInput.value.trim();
    const slug = generateSlug(name);

    const tag = { name, slug };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(tag)
    })
        .then(response => response.json())
        .then(() => {
            getTags();
            nameInput.value = '';
        })
        .catch(error => console.error('Unable to add tag.', error));
}

function deleteTag(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getTags())
        .catch(error => console.error('Unable to delete tag.', error));
}

function displayEditForm(id) {
    const tag = tags.find(t => t.id === id);

    document.getElementById('edit-id').value = tag.id;
    document.getElementById('edit-name').value = tag.name;
    document.getElementById('editTag').style.display = 'block';
}

function updateTag() {
    const id = parseInt(document.getElementById('edit-id').value, 10);
    const name = document.getElementById('edit-name').value.trim();
    const slug = generateSlug(name);

    const tag = { id, name, slug };

    fetch(`${uri}/${id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(tag)
    })
        .then(() => getTags())
        .catch(error => console.error('Unable to update tag.', error));

    closeInput();
    return false;
}

function closeInput() {
    document.getElementById('editTag').style.display = 'none';
}

function _displayTags(data) {
    const tBody = document.getElementById('tags');
    tBody.innerHTML = '';

    const button = document.createElement('button');

    data.forEach(tag => {
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(document.createTextNode(tag.name));

        let td2 = tr.insertCell(1);
        td2.appendChild(document.createTextNode(tag.slug));

        let td3 = tr.insertCell(2);
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${tag.id})`);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteTag(${tag.id})`);
        td4.appendChild(deleteButton);
    });

    tags = data;
}


/////////////////////
const pdfUri = '/api/PDFs';
let pdfItems = [];


function showAddForm() {
    hideForms();
    document.getElementById('addPdf').style.display = 'block';
}

function hideForms() {
    document.getElementById('addPdf').style.display = 'none';
    document.getElementById('editPdf').style.display = 'none';
}

function getPdfs() {
    fetch(pdfUri)
        .then(r => r.json())
        .then(data => displayPdfs(data))
        .catch(e => console.error('Unable to get PDFs.', e));
}

function displayPdfs(data) {
    const container = document.getElementById('pdfsContainer');
    container.innerHTML = '';
    pdfItems = data;

    data.forEach(pdf => {
        const col = document.createElement('div');
        col.className = 'col-3 col-6-medium col-12-small';

        const card = document.createElement('section');

        const canvas = document.createElement('canvas');
        canvas.style.cursor = 'pointer';
        canvas.onclick = () => window.open(pdf.filePath, '_blank');
        card.appendChild(canvas);
        renderPdfPreview(pdf.filePath, canvas);

        const info = document.createElement('div');
        info.innerHTML = `<h2>${pdf.title}</h2><p>by ${pdf.author}</p><p><small>Uploaded: ${new Date(pdf.uploadDate).toLocaleDateString()}</small></p>`;
        card.appendChild(info);


        const editBtn = document.createElement('button');
        editBtn.innerText = 'Edit';
        editBtn.onclick = () => displayEditForm(pdf.id);
        const delBtn = document.createElement('button');
        delBtn.innerText = 'Delete';
        delBtn.onclick = () => pdfDelete(pdf.id);
        card.appendChild(editBtn);
        card.appendChild(delBtn);


        col.appendChild(card);
        container.appendChild(col);
    });
}

function renderPdfPreview(url, canvas) {
    pdfjsLib.getDocument(url).promise
        .then(pdf => pdf.getPage(1))
        .then(page => {
            const desiredHeight = 300;
            const originalViewport = page.getViewport({ scale: 1 });
            const scale = desiredHeight / originalViewport.height;
            const viewport = page.getViewport({ scale });

            canvas.width = viewport.width;
            canvas.height = viewport.height;

            return page.render({ canvasContext: canvas.getContext('2d'), viewport }).promise;
        })
        .catch(e => console.error('Preview error:', e));
}



function pdfAdd() {
    const title = document.getElementById('add-title').value.trim();
    const abstract = document.getElementById('add-abstract').value.trim();
    const file = document.getElementById('add-file').files[0];
    const author = document.getElementById('add-author').value.trim();
    if (!file) return alert('Select a PDF');

    const formData = new FormData();
    formData.append('Title', title);
    formData.append('Abstract', abstract);
    formData.append('Author', author);
    formData.append('File', file);

    fetch(pdfUri, { method: 'POST', body: formData })
        .then(r => r.ok ? r.json() : Promise.reject(r.statusText))
        .then(() => { hideForms(); getPdfs(); })
        .catch(err => alert('Error adding PDF: ' + err));

    return false;
}

function displayEditForm(id) {
    const pdf = pdfItems.find(p => p.id === id);
    if (!pdf) return console.error('PDF not found:', id);

    document.getElementById('edit-id').value = pdf.id;
    document.getElementById('edit-title').value = pdf.title;
    document.getElementById('edit-abstract').value = pdf.abstract || '';
    hideForms();
    document.getElementById('editPdf').style.display = 'block';
}

function updatePdf() {
    const id = document.getElementById('edit-id').value;
    const title = document.getElementById('edit-title').value.trim();
    const author = document.getElementById('edit-author').value.trim();
    const abstract = document.getElementById('edit-abstract').value.trim();

    const pdfData = { id, title, abstract, author };
    fetch(`${pdfUri}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(pdfData)
    })
        .then(r => r.ok ? getPdfs() : Promise.reject(r.statusText))
        .then(() => hideForms())
        .catch(err => alert('Error updating PDF: ' + err));

    return false;
}

function pdfDelete(id) {
    if (!confirm('Are you sure you want to delete this PDF?')) return;
    fetch(`${pdfUri}/${id}`, { method: 'DELETE' })
        .then(r => r.ok ? getPdfs() : Promise.reject(r.statusText))
        .catch(err => alert('Error deleting PDF: ' + err));
}

function showDetails(id) {
    const pdf = pdfItems.find(p => p.id === id);
    if (!pdf) return;
    document.getElementById('detail-title').textContent = pdf.title;
    document.getElementById('detail-author').textContent = pdf.author || 'Unknown';
    document.getElementById('detail-date').textContent = new Date(pdf.uploadDate).toLocaleString();
    document.getElementById('detail-abstract').textContent = pdf.abstract || '';
    const toast = document.getElementById('detailsToast');
    toast.style.display = 'block';
}

function hideDetails() {
    const toast = document.getElementById('detailsToast');
    toast.style.display = 'none';
}


getPdfs();
