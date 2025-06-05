const userDatabase = [
    {
        username: "admin",
        email: "admin",
        phone: "123456",
        password: "admin1"
    },
    {
        username: "antonio",
        email: "antonio",
        phone: "654321",
        password: "antonio"
    }
];

function checkLoggedIn() {
    const loggedInUser = localStorage.getItem('loggedInUser');
    if (loggedInUser) {
        window.location.href = "../index.html";
    }
}

checkLoggedIn();

function handleSubmit(event) {
    event.preventDefault();
    
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const phone = document.getElementById('phone').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const messageDiv = document.getElementById('message');
    
    //if (userDatabase.some(u => u.email === email)) {
    //    registerError.textContent = "Този имейл вече е регистриран.";
    //    return false;
    //}
    
    //if (userDatabase.some(u => u.phone === phone)) {
    //    registerError.textContent = "Този телефонен номер вече е регистриран.";
    //    return false;
    //}
    
    if (password.length < 6) {
        messageDiv.className = 'error';
        messageDiv.textContent = 'Паролата трябва да е поне 6 символа!';
        return false;
    }
    
    if (password !== confirmPassword) {
        messageDiv.className = 'error';
        messageDiv.textContent = 'Паролите не съвпадат!';
        return false;
    }

    messageDiv.className = 'success';
    messageDiv.textContent = 'Успешна регистрация! Пренасочване към входа...';
    
    userDatabase.push( { username, email, phone, password });
    
    setTimeout(() => {
        window.location.href = "../login-page/index.html";
    }, 1500);
    
    return false;
}