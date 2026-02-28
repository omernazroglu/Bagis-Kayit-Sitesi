document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('tableSearch');
    const rows = document.querySelectorAll('#mainTable tbody tr');

    input.addEventListener('input', (e) => {
        const term = e.target.value.toLowerCase();
        rows.forEach(row => {
            row.style.display = row.innerText.toLowerCase().includes(term) ? '' : 'none';
        });
    });

  
});

