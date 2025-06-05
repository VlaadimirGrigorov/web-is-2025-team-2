const API_BASE_URL = "https://localhost:7192/api";

async function handleSubmit(event) {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const messageDiv = document.getElementById('message');

    messageDiv.textContent = ''; // Clear previous messages
    messageDiv.className = '';

    if (password !== confirmPassword) {
        messageDiv.className = 'error';
        messageDiv.textContent = 'Паролите не съвпадат!';
        return;
    }
        
    const userData = {
        Username: username,
        Email: email,
        Password: password
    };

    try {
        const response = await fetch(`${API_BASE_URL}/users/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });

        if (response.status === 201) {
            messageDiv.className = 'success';
            messageDiv.textContent = 'Успешна регистрация! Пренасочване към страницата за вход...';
            setTimeout(() => {
                window.location.href = "../login-page/index.html";
            }, 2000);
        } else {
            const errorData = await response.json().catch(() => null); // Try to parse JSON, otherwise null
            messageDiv.className = 'error';
            if (errorData && errorData.message) {
                messageDiv.textContent = errorData.message;
            } else if (response.status === 409) {
                messageDiv.textContent = 'Потребител с този имейл вече съществува.';
            } else if (response.status === 400) {
                messageDiv.textContent = 'Невалидни данни. Моля, проверете въведената информация.';
            }
            else {
                messageDiv.textContent = `Грешка при регистрация: ${response.status} ${response.statusText}`;
            }
        }
    } catch (error) {
        messageDiv.className = 'error';
        messageDiv.textContent = 'Възникна грешка при свързване със сървъра. Моля, опитайте отново по-късно.';
        console.error('Registration error:', error);
    }
}