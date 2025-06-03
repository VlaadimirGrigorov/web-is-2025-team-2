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
        window.location.href = "../welcome-page/index.html";
    }
}

checkLoggedIn();

function handleSubmit(event) {
    event.preventDefault();
    
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const messageDiv = document.getElementById('message');
    
    const user = userDatabase.find(u => u.username === username && u.password === password);
    
    if (user) {

        messageDiv.className = 'success';
        messageDiv.textContent = 'Успешен вход!';

        localStorage.setItem('loggedInUser', username);
        
        setTimeout(() => {
            window.location.href = "../welcome-page/index.html";
        }, 1500);
    } else {

        messageDiv.className = 'error';
        messageDiv.textContent = 'Невалидно потребителско име или парола!';
    }
    
    return false;
}