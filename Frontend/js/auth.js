export function getCurrentUser() {
    const token = localStorage.getItem('token');
    if (!token) return null;

    try {
        // Extract the payload part of the JWT
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const payload = JSON.parse(atob(base64));

        return {
            username: payload.unique_name || payload.sub, // Different JWT libraries use different claims
            role: payload.role,
            departmentId: payload.DepartmentId ? parseInt(payload.DepartmentId) : null
        };
    } catch (error) {
        console.error('Failed to parse JWT:', error);
        return null;
    }
}

export function isAuthenticated() {
    return !!localStorage.getItem('token');
}

export function isAdmin() {
    const user = getCurrentUser();
    return user?.role === 'Admin';
}