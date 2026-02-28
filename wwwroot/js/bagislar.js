document.addEventListener("DOMContentLoaded", function () {

    // Sayfa yüklenince tüm status-select değerlerini badge metniyle senkronize et
    document.querySelectorAll(".status-wrapper").forEach(wrapper => {
        const badge = wrapper.querySelector(".badge");
        const select = wrapper.querySelector(".status-select");
        if (badge && select) {
            const badgeText = badge.innerText.trim();
            for (let opt of select.options) {
                if (opt.value === badgeText) {
                    select.value = opt.value;
                    break;
                }
            }
        }
    });

    // Sayfa yüklenince istatistikleri doğru hesapla
    // (updateStats fonksiyonu tanımlandıktan sonra çağrılması için setTimeout kullanıyoruz)
    setTimeout(updateStats, 0);

    // --- YARDIMCI FONKSİYONLAR ---
    function closeAllRefDropdowns() {
        document.querySelectorAll(".ref-dropdown").forEach(d => d.remove());
    }

    function getNextBagisNo() {
        let max = 0;
        document.querySelectorAll("tbody tr.row").forEach(tr => {
            const cell = tr.children[1];
            if (cell) {
                const val = parseInt(cell.innerText.trim());
                if (!isNaN(val) && val > max) max = val;
            }
        });
        return max + 1;
    }

    function updateBadgeStyle(badge, text) {
        badge.innerHTML = '<span class="badge-dot"></span>' + text;
        badge.className = "badge";

        // Durum Metinlerine Göre Renk Ataması
        if (text === "Teslim Edildi") { badge.classList.add("badge-pickup"); }
        else if (text === "Gönderildi") { badge.classList.add("badge-fulfilled"); }
        else if (text === "Dernekte") { badge.classList.add("badge-in-progress"); }
        else if (text === "Referans'ta") { badge.classList.add("badge-pending"); }
        else if (text === "Alınmadı") { badge.classList.add("badge-cancelled"); }
        else if (text === "Gönderilmedi") { badge.classList.add("badge-failed"); }
    }

    // --- 1. MERKEZİ TIKLAMA VE KLAVYE YÖNETİMİ ---
    document.addEventListener("click", function (e) {
        const cell = e.target.closest(".editable-cell");

        // KRİTİK: Eğer tıklanan yer bir select kutusuysa, input açma mantığını durdur
        if (e.target.tagName === "SELECT" || e.target.closest(".status-select") || e.target.closest(".styled-select")) {
            return;
        }

        // A) Yeni Referans Satır İçi Giriş Alanı
        if (e.target.closest(".ref-add-btn")) {
            const btn = e.target.closest(".ref-add-btn");
            e.stopPropagation();
            const listContainer = btn.previousElementSibling;

            if (listContainer.querySelector(".inline-add-row")) return;

            const addRow = document.createElement("div");
            addRow.className = "inline-add-row";
            addRow.style.cssText = "display:flex; padding:8px; gap:5px; background:#f0f7ff; border-top:1px solid #d0e3ff;";

            addRow.innerHTML = `
                <input type="text" class="inlineRefInput" placeholder="Ad Soyad giriniz..." style="flex:1;">
                <button type="button" class="inlineRefSave">Ekle</button>`;

            listContainer.appendChild(addRow);
            const input = addRow.querySelector(".inlineRefInput");
            const saveBtn = addRow.querySelector(".inlineRefSave");

            input.focus();

            saveBtn.addEventListener("click", function (event) {
                event.preventDefault();
                event.stopPropagation();
                satirIciReferansKaydet(input, btn.closest(".ref-dropdown"));
            });

            input.addEventListener("keydown", function (event) {
                if (event.key === "Enter") { saveBtn.click(); }
            });
            return;
        }

        // B) Listeden Referans Seçimi
        if (e.target.closest(".ref-item")) {
            const item = e.target.closest(".ref-item");
            const dropdown = item.closest(".ref-dropdown");
            const parentCell = dropdown.targetCell;
            const span = parentCell.querySelector(".cell-value");
            const name = item.dataset.name;
            if (span) span.innerText = name;
            fetch('/Home/Deneme', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ BagislarId: parseInt(parentCell.dataset.recordId), SutunlarId: parseInt(parentCell.dataset.columnId), Aciklama: name })
            });
            closeAllRefDropdowns();
            updateStats();
            return;
        }

        if (!cell) {
            if (!e.target.closest(".ref-dropdown")) closeAllRefDropdowns();
            return;
        }

        const columnId = cell.dataset.columnId;
        const recordId = cell.dataset.recordId;

        // C) REFERANS LİSTESİNİ AÇMA (Sütun 7)
        if (columnId === "7") {
            e.stopPropagation();
            closeAllRefDropdowns();

            const dropdown = document.createElement("div");
            dropdown.className = "ref-dropdown";
            dropdown.style.cssText = "position:fixed; z-index:2147483647; background:#fff; box-shadow:0 10px 30px rgba(0,0,0,0.25); border-radius:8px; width:260px; overflow:hidden; border:1px solid #ddd;";

            dropdown.targetCell = cell;
            document.body.appendChild(dropdown);

            // İçeriği yükle, sonra konumlandır
            fetch('/Home/ReferanslariGetir').then(res => res.json()).then(data => {
                const renkler = ["#4facfe", "#43e97b", "#fa709a", "#30cfd0", "#f093fb"];
                const itemsHtml = data.map(ref => {
                    const displayName = ref.adSoyad || ref.ad || "İsimsiz";
                    const initials = displayName.split(" ").map(k => k.charAt(0)).join("").toUpperCase().substring(0, 2);
                    const color = renkler[ref.id % 5];
                    return `
                        <div class="ref-item" data-id="${ref.id}" data-name="${displayName}" style="display:flex; align-items:center; padding:10px; cursor:pointer; border-bottom:1px solid #f8f8f8;">
                            <div class="avatar" style="background:${color}; width:30px; height:30px; border-radius:50%; color:white; display:flex; align-items:center; justify-content:center; font-size:11px; margin-right:12px; font-weight:bold; flex-shrink:0;">
                                ${initials}
                            </div>
                            <span style="font-size:13px; font-weight:500; color:#333;">${displayName}</span>
                        </div>`;
                }).join('');

                dropdown.innerHTML = `
                    <div class="ref-list-container" style="max-height:220px; overflow-y:auto;">${itemsHtml}</div>
                    <button class="ref-add-btn" style="width:100%; padding:12px; border:none; background:#f8f9fa; cursor:pointer; color:#007bff; font-weight:bold; border-top:1px solid #eee;">
                        + Yeni Referans Ekle
                    </button>`;

                // Gerçek yüksekliği ölç ve her zaman aşağıya aç
                const rect = cell.getBoundingClientRect();
                const dropdownHeight = dropdown.offsetHeight;
                const dropdownWidth = dropdown.offsetWidth;
                const margin = 8;

                // Her zaman hücrenin altına aç
                let top = rect.bottom + margin;
                // Ekrandan taşarsa yukarıya aç
                if (top + dropdownHeight > window.innerHeight - margin) {
                    top = rect.top - dropdownHeight - margin;
                }

                // Yatay: hücreyle hizalı, sağdan taşarsa sola kaydır
                let left = rect.left;
                if (left + dropdownWidth > window.innerWidth - margin) {
                    left = window.innerWidth - dropdownWidth - margin;
                }
                if (left < margin) left = margin;

                dropdown.style.top = top + "px";
                dropdown.style.left = left + "px";
            });
            return;
        }

        // D) Normal Hücre Düzenleme (Sadece metin/sayı alanları için)
        // Select içeren sütunlarda (3, 5, 6) input açılmasını engelliyoruz
        if (["3", "5", "6"].includes(columnId)) return;

        if (cell.querySelector("select") || cell.querySelector("input")) return;
        const span = cell.querySelector(".cell-value");
        if (!span) return;

        const currentValue = span.innerText.trim();
        const input = document.createElement("input");
        input.className = "edit-input";
        input.value = (currentValue === "-" || currentValue === "") ? "" : currentValue;

        span.style.display = "none";
        cell.appendChild(input);
        cell.style.padding = "2px";
        input.focus();
        input.setSelectionRange(input.value.length, input.value.length);

        // KLAVYE KONTROLÜ
        input.onkeypress = function (e) {
            const charCode = (e.which) ? e.which : e.keyCode;
            if (columnId === "4") { // Tam Sayı
                if (charCode < 48 || charCode > 57) { e.preventDefault(); return false; }
            }
            if (columnId === "2") { // Telefon
                const allowedChars = /[0-9\s+()]/;
                if (!allowedChars.test(String.fromCharCode(charCode)) && charCode > 31) { e.preventDefault(); return false; }
            }
        };

        input.onblur = function () {
            const newValue = input.value.trim();
            const isOldEmpty = (currentValue === "-" || currentValue === "");
            const isNewEmpty = (newValue === "");

            if (columnId === "4" && !isNewEmpty && !/^\d+$/.test(newValue)) {
                Swal.fire({ icon: 'error', title: 'Hata', text: 'Lütfen sadece tam sayı giriniz.' });
                input.focus(); return;
            }

            if (newValue !== currentValue && !(isOldEmpty && isNewEmpty)) {
                span.innerText = newValue === "" ? "-" : newValue;
                fetch('/Home/Deneme', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ BagislarId: parseInt(recordId), SutunlarId: parseInt(columnId), Aciklama: newValue })
                });
            }
            cell.style.padding = "";
            span.style.display = "inline";
            input.remove();
        };

        input.onkeydown = function (e) {
            if (e.key === "Enter") input.blur();
            if (e.key === "Escape") { input.value = currentValue; input.blur(); }

            // TAB NAVİGASYONU
            if (e.key === "Tab") {
                e.preventDefault();
                input.blur();
                const nextCell = findNextEditableCell(cell, e.shiftKey ? -1 : 1);
                if (nextCell) {
                    setTimeout(() => nextCell.click(), 10);
                }
            }
        };
    });

    // --- 2. SELECT DEĞİŞİMLERİ VE TAB DESTEĞİ ---
    document.addEventListener("keydown", function (e) {
        if (e.key === "Tab" && (e.target.tagName === "SELECT")) {
            const cell = e.target.closest(".editable-cell");
            if (cell) {
                e.preventDefault();
                const nextCell = findNextEditableCell(cell, e.shiftKey ? -1 : 1);
                if (nextCell) {
                    setTimeout(() => nextCell.click(), 10);
                }
            }
        }
    });


    document.addEventListener("change", function (e) {
        if (!e.target.classList.contains("status-select") && !e.target.classList.contains("styled-select")) return;

        const select = e.target;
        const cell = select.closest(".editable-cell");
        const val = select.value;

        // "Alınmadı": "1" kuralına göre güncellenmiş Map
        const valueMap = {
            "Alınmadı": "1",
            "Referans'ta": "2",
            "Dernekte": "3",
            "Teslim Edildi": "4",
            "Gönderilmedi": "0",
            "Gönderildi": "1"
        };

        const numericValue = valueMap[val] || val;

        const wrapper = select.closest(".status-wrapper");
        if (wrapper) {
            const badge = wrapper.querySelector(".badge");
            if (badge) updateBadgeStyle(badge, val);
        }

        fetch('/Home/Deneme', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ BagislarId: parseInt(cell.dataset.recordId), SutunlarId: parseInt(cell.dataset.columnId), Aciklama: numericValue })
        });

        // İstatistikleri anlık güncelle
        updateStats();
    });

    // --- 3. İSTATİSTİKLERİ HESAPLA VE GÜNCELLE ---
    function updateStats() {
        const rows = document.querySelectorAll("tbody tr.row");
        const totalCount = rows.length;

        let d0 = 0, d1 = 0, d2 = 0, d3 = 0;
        let smsGonderildi = 0;

        rows.forEach(row => {
            // Bağış Durumu (column-id="5")
            const durumuCell = row.querySelector('.editable-cell[data-column-id="5"]');
            if (durumuCell) {
                const sel = durumuCell.querySelector(".status-select");
                const badgeText = sel ? sel.value : (durumuCell.querySelector(".badge")?.innerText.trim() || "");
                if (badgeText === "Alınmadı") d0++;
                else if (badgeText === "Referans'ta") d1++;
                else if (badgeText === "Dernekte") d2++;
                else if (badgeText === "Teslim Edildi") d3++;
            }

            // SMS Durumu (column-id="6")
            const smsCell = row.querySelector('.editable-cell[data-column-id="6"]');
            if (smsCell) {
                const sel = smsCell.querySelector(".status-select");
                if (sel && sel.value === "Gönderildi") smsGonderildi++;
            }
        });

        // Toplam bağış sayısı
        const totalEl = document.querySelector(".stat-card.primary .value");
        if (totalEl) totalEl.innerHTML = totalCount + ' <small>Bağış</small>';

        // SMS progress bar
        const smsOran = totalCount > 0 ? (smsGonderildi / totalCount * 100) : 0;
        const progressFill = document.querySelector(".progress-bar-fill");
        const progressInfo = document.querySelector(".progress-info");
        if (progressFill) progressFill.style.width = smsOran.toFixed(0) + "%";
        if (progressInfo) {
            const spans = progressInfo.querySelectorAll("span");
            if (spans[0]) spans[0].innerText = "Başarı: %" + smsOran.toFixed(0);
            if (spans[1]) spans[1].innerText = smsGonderildi + " / " + totalCount;
        }

        // Bağış durum kartları
        const durumToplam = d0 + d1 + d2 + d3;
        const miniCards = document.querySelectorAll(".status-mini-card");
        const durumValues = [d0, d1, d2, d3];
        miniCards.forEach((card, i) => {
            const strong = card.querySelector("strong");
            if (strong) strong.innerText = durumValues[i];
        });

        // Dağılım çubuğu
        const segments = document.querySelectorAll(".dist-segment");
        const segmentValues = [d0, d1, d2, d3];
        segments.forEach((seg, i) => {
            seg.style.width = durumToplam > 0 ? (segmentValues[i] / durumToplam * 100).toFixed(1) + "%" : "0%";
        });

        // Referans sıralaması
        const refMap = {};
        document.querySelectorAll('.editable-cell[data-column-id="7"] .cell-value').forEach(span => {
            const name = span.innerText.trim();
            if (name && name !== "-") refMap[name] = (refMap[name] || 0) + 1;
        });
        const sortedRefs = Object.entries(refMap).sort((a, b) => b[1] - a[1]).slice(0, 5);
        const rankList = document.querySelector(".rank-list");
        if (rankList) {
            if (sortedRefs.length === 0) {
                rankList.innerHTML = '<div style="font-size:12px; color:#94a3b8; padding:10px;">Henüz referans verisi bulunamadı.</div>';
            } else {
                rankList.innerHTML = sortedRefs.map(([name, count]) => `
                    <div class="rank-item">
                        <div class="rank-name"><span class="dot"></span>${name}</div>
                        <div class="rank-count">${count} Bağış</div>
                    </div>`).join('');
            }
        }
    }

    // --- 4. YENİ BAĞIŞ EKLEME ---
    const addBtn = document.getElementById("addRowBtn");
    if (addBtn) {
        addBtn.addEventListener("click", function () {
            fetch('/Home/EkleDeneme', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(res => res.json())
                .then(data => {
                    const serverId = data.newId;
                    const tbody = document.querySelector("table tbody");
                    const tr = document.createElement("tr");
                    tr.className = "row";

                    let rowHtml = `<td><input type="checkbox"></td><td>${getNextBagisNo()}</td>`;
                    const totalCols = document.querySelectorAll("thead th").length;

                    for (let i = 1; i <= totalCols - 3; i++) {
                        rowHtml += `<td class="editable-cell" data-record-id="${serverId}" data-column-id="${i}">`;
                        if (i === 3) {
                            rowHtml += `<select class="styled-select"><option value="">Seçiniz</option><option>AKİKA</option><option>ADAK</option><option>VACİP</option><option>MERHUM</option></select>`;
                        } else if (i === 5) {
                            rowHtml += `<div class="status-wrapper"><span class="badge badge-cancelled"><span class="badge-dot"></span>Alınmadı</span><select class="status-select"><option value="Alınmadı">Alınmadı</option><option value="Referans'ta">Referans'ta</option><option value="Dernekte">Dernekte</option><option value="Teslim Edildi">Teslim Edildi</option></select></div>`;
                        } else if (i === 6) {
                            rowHtml += `<div class="status-wrapper"><span class="badge badge-failed"><span class="badge-dot"></span>Gönderilmedi</span><select class="status-select"><option value="Gönderilmedi">Gönderilmedi</option><option value="Gönderildi">Gönderildi</option></select></div>`;
                        } else {
                            rowHtml += `<span class="cell-value">-</span>`;
                        }
                        rowHtml += `</td>`;
                    }
                    rowHtml += `<td style="text-align:center;" data-record-id="${serverId}">
                                    <button type="button" class="btn-delete" onclick="satirSil(this)"> <i>🗑</i> Sil</button>
                                </td>`;

                    tr.innerHTML = rowHtml;
                    tbody.insertBefore(tr, addBtn.closest("tr") || null);
                    updateStats();
                    Swal.fire({ icon: 'success', title: 'Yeni Satır Eklendi', toast: true, position: 'top-end', timer: 2000, showConfirmButton: false });
                });
        });
    }

    const searchInput = document.querySelector(".search-box input");
    const filterBtn = document.querySelector(".filter-btn");
    const filterPanel = document.getElementById("filterPanel");

    function applyCombinedFilters() {
        const generalSearchValue = (searchInput?.value || "").toLowerCase().trim();
        const rows = document.querySelectorAll("table tbody tr.row");
        const moreFilters = document.querySelectorAll(".filter-select, .filter-input");

        rows.forEach(row => {
            let isVisible = true;
            const col1 = row.querySelector('.editable-cell[data-column-id="1"]');
            const col1Content = col1 ? (col1.innerText || col1.querySelector("input")?.value || "").toLowerCase() : "";

            if (generalSearchValue && !col1Content.includes(generalSearchValue)) {
                isVisible = false;
            }

            if (isVisible) {
                moreFilters.forEach(filter => {
                    const colId = filter.dataset.col;
                    const filterValue = filter.value.toLowerCase().trim();
                    if (filterValue) {
                        const targetCell = row.querySelector(`.editable-cell[data-column-id="${colId}"]`);
                        if (targetCell) {
                            let cellContent = "";
                            const cellSelect = targetCell.querySelector("select");
                            const cellInput = targetCell.querySelector("input");

                            if (cellSelect) {
                                cellContent = cellSelect.options[cellSelect.selectedIndex].text.toLowerCase();
                            } else if (cellInput) {
                                cellContent = cellInput.value.toLowerCase();
                            } else {
                                cellContent = targetCell.innerText.toLowerCase();
                            }

                            if (!cellContent.includes(filterValue)) isVisible = false;
                        }
                    }
                });
            }
            row.style.display = isVisible ? "" : "none";
        });
    }

    function openFilterPanel() {
        filterPanel.style.visibility = "hidden";
        filterPanel.style.display = "block";

        const rect = filterBtn.getBoundingClientRect();
        const panelWidth = filterPanel.offsetWidth;
        const panelHeight = filterPanel.offsetHeight;
        const margin = 12;

        // Dikey: her zaman butonun üstüne aç
        let top = rect.top + window.scrollY - panelHeight - 8;
        if (top < window.scrollY + margin) top = window.scrollY + margin;

        // Yatay: butona sağ hizalı, taşarsa sola kaydır
        let left = rect.right - panelWidth;
        if (left < margin) left = margin;
        if (left + panelWidth > window.innerWidth - margin) left = window.innerWidth - panelWidth - margin;

        filterPanel.style.position = "absolute";
        filterPanel.style.top = top + "px";
        filterPanel.style.left = left + "px";
        filterPanel.style.visibility = "visible";
    }

    function closeFilterPanel() {
        filterPanel.style.display = "none";
    }

    if (filterBtn && filterPanel) {
        filterBtn.onclick = function (e) {
            e.stopPropagation();
            const isHidden = filterPanel.style.display === "none" || filterPanel.style.display === "";
            isHidden ? openFilterPanel() : closeFilterPanel();
        };

        // X butonuyla kapat
        const filterPanelClose = document.getElementById("filterPanelClose");
        if (filterPanelClose) filterPanelClose.addEventListener("click", closeFilterPanel);

        // ESC ile kapat
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape" && filterPanel.style.display !== "none") closeFilterPanel();
        });

        // Dışına tıklayınca kapat
        document.addEventListener("click", function (e) {
            if (filterPanel.style.display !== "none" &&
                !filterPanel.contains(e.target) &&
                !filterBtn.contains(e.target)) {
                closeFilterPanel();
            }
        });
    }

    document.querySelectorAll(".filter-select, .filter-input").forEach(el => el.addEventListener("input", applyCombinedFilters));
    if (searchInput) searchInput.addEventListener("input", applyCombinedFilters);

    // Filtreleri sıfırla butonu
    const clearFiltersBtn = document.getElementById("clearFilters");
    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener("click", function () {
            document.querySelectorAll(".filter-select").forEach(s => s.value = "");
            document.querySelectorAll(".filter-input").forEach(i => i.value = "");
            applyCombinedFilters();
        });
    }

    // --- İSTATİSTİK PANELİ (OFFCANVAS) ---
    const offcanvas = document.getElementById('statsOffcanvas');
    const overlay = document.getElementById('overlay');
    const openStatsBtn = document.getElementById('toggleStats');
    const closeStatsBtn = document.getElementById('closeStats');

    function closeOffcanvas() {
        offcanvas.classList.remove('open');
        overlay.classList.remove('show');
    }

    if (openStatsBtn) {
        openStatsBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            offcanvas.classList.add('open');
            overlay.classList.add('show');
        });
    }
    if (closeStatsBtn) closeStatsBtn.addEventListener('click', closeOffcanvas);

    // Overlay'e tıklayınca kapat
    if (overlay) overlay.addEventListener('click', closeOffcanvas);

    // Offcanvas dışına tıklayınca kapat
    document.addEventListener('click', function (e) {
        if (offcanvas && offcanvas.classList.contains('open')) {
            if (!offcanvas.contains(e.target) && e.target !== openStatsBtn && !openStatsBtn?.contains(e.target)) {
                closeOffcanvas();
            }
        }
    });

    // ESC tuşuyla kapat
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && offcanvas && offcanvas.classList.contains('open')) {
            closeOffcanvas();
        }
    });

});

// --- GLOBAL FONKSİYONLAR (Window scope) ---
window.findNextEditableCell = function (currentCell, direction) {
    const allCells = Array.from(document.querySelectorAll(".editable-cell"));
    const currentIndex = allCells.indexOf(currentCell);
    const nextIndex = currentIndex + direction;

    if (nextIndex >= 0 && nextIndex < allCells.length) {
        const nextCell = allCells[nextIndex];
        const select = nextCell.querySelector("select");
        if (select) {
            select.focus();
            return null;
        }
        return nextCell;
    }
    return null;
};

window.satirIciReferansKaydet = async function (inputElement, dropdown) {
    const ad = inputElement.value.trim();
    if (!ad) return;
    try {
        const response = await fetch('/Home/ReferansEkle', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ AdSoyad: ad }) });
        const result = await response.json();
        if (result.success) {
            const parentCell = dropdown.targetCell;
            const span = parentCell.querySelector(".cell-value");
            if (span) span.innerText = ad;
            fetch('/Home/Deneme', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ BagislarId: parseInt(parentCell.dataset.recordId), SutunlarId: parseInt(parentCell.dataset.columnId), Aciklama: ad }) });
            document.querySelectorAll(".ref-dropdown").forEach(d => d.remove());
            updateStats();
        }
    } catch (error) { console.error(error); }
};

window.satirSil = function (recordId) {
    if (!recordId || recordId === 0) return;

    Swal.fire({
        title: 'Emin misiniz?',
        text: "Kayıt tamamen silinecektir!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e74c3c',
        confirmButtonText: 'Evet, Sil',
        cancelButtonText: 'Vazgeç'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/Home/Sil', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: parseInt(recordId) })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        Swal.fire({
                            title: 'Başarılı!',
                            text: 'Kayıtlar başarıyla silindi.',
                            icon: 'success',
                            confirmButtonText: 'Tamam',
                            timer: 2000,
                            timerProgressBar: true,
                            toast: false,
                            position: 'center'
                        }).then(() => {
                            location.reload(); // 2 saniye sonra sayfa yenilenir
                        });
                    }
                });
        }
    });
};