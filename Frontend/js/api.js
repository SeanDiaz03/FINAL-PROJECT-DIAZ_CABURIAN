const API_BASE = 'https://localhost:5001/api';

export async function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem('token');
    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
        ...options.headers
    };

    const response = await fetch(`${API_BASE}${url}`, { ...options, headers });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }

    return await response.json();
}

export async function login(credentials) {
    const response = await fetch(`${API_BASE}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
    });

    if (!response.ok) throw new Error('Login failed');
    return await response.json();
}

import { getCurrentUser } from './auth.js';

export async function fetchWithAuth(url, options = {}) {
    if (!isAuthenticated()) {
        window.location.href = 'login.html';
        return;
    }

    const token = localStorage.getItem('token');
    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
        ...options.headers
    };

    // ... rest of the fetchWithAuth implementation
}

export function getCurrentUser() {
    return auth.getCurrentUser();
}