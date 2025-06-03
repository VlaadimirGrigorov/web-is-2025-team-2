function checkAuth() {
    const loggedInUser = localStorage.getItem('loggedInUser');
    if (!loggedInUser) {
        window.location.href = "../login-page/index.html";
    } else {
        document.getElementById('username').textContent = loggedInUser;
    }
}

function logout() {
    localStorage.removeItem('loggedInUser');
    window.location.href = "../login-page/index.html";
}

window.onload = checkAuth;