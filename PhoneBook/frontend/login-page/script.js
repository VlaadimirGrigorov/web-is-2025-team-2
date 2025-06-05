document.addEventListener('DOMContentLoaded', () => {
    localStorage.removeItem('authToken'); // Clear token on login page load

    // const loginForm = document.getElementById('loginForm'); // Assuming a form with ID 'loginForm'
    // If no ID, you might use document.querySelector('form');
    // For this script, we'll assume the form's submit event is handled by an inline `onsubmit="return handleSubmit(event)"`
    // or the handleSubmit function is globally accessible and called appropriately.
    // If the form needs to be selected here, uncomment the line above.

    // If the form ISN'T using onsubmit="return handleSubmit(event)" in the HTML,
    // and you want to attach the listener programmatically, you could do:
    const loginForm = document.getElementById('loginForm'); // Or document.querySelector('form');
    if (loginForm) {
        loginForm.addEventListener('submit', handleSubmit);
    } else {
        // Fallback for forms that might not have an ID but are the only form on the page
        const pageForm = document.querySelector('form');
        if (pageForm) {
            pageForm.addEventListener('submit', handleSubmit);
        } else {
            console.warn("Login form not found to attach submit listener.");
        }
    }
});

const API_BASE_URL = "https://localhost:7192/api";

async function handleSubmit(event) {
    if (event) {
        event.preventDefault();
    }

    const usernameInput = document.getElementById('username'); // Assuming input with ID 'username'
    const passwordInput = document.getElementById('password'); // Assuming input with ID 'password'
    const messageDiv = document.getElementById('message');   // Assuming div with ID 'message' for feedback

    const username = usernameInput ? usernameInput.value : '';
    const password = passwordInput ? passwordInput.value : '';

    if (!messageDiv) {
        console.error("Error message display element not found (expected ID 'message').");
    } else {
        messageDiv.textContent = '';
        messageDiv.className = '';
    }

    if (!username || !password) {
        const msg = 'Моля, въведете потребителско име и парола.';
        if (messageDiv) {
            messageDiv.className = 'error';
            messageDiv.textContent = msg;
        } else {
            alert(msg);
        }
        return false;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                Username: username,
                Password: password,
            }),
        });

        if (response.ok) {
            const data = await response.json();
            if (data.Token || data.token) { // Checking for both casings of Token
                localStorage.setItem('authToken', data.Token || data.token);
                if (messageDiv) {
                    messageDiv.className = 'success';
                    messageDiv.textContent = 'Успешен вход! Пренасочване...';
                }
                // Redirect to the main application page (explicitly to index.html)
                window.location.href = '/index.html';
            } else {
                const msg = 'Грешка: Токен не е получен от сървъра.';
                if (messageDiv) {
                    messageDiv.className = 'error';
                    messageDiv.textContent = msg;
                } else {
                    alert(msg);
                }
            }
        } else {
            let errorMessageText = 'Грешка при вход.';
            if (response.status === 400 || response.status === 401) { // Bad Request or Unauthorized
                try {
                    const errorData = await response.json();
                    // Check for common error message structures
                    if (errorData.message) errorMessageText = errorData.message;
                    else if (errorData.Message) errorMessageText = errorData.Message;
                    else if (errorData.error) errorMessageText = errorData.error;
                    else if (typeof errorData === 'string') errorMessageText = errorData;
                    else if (response.status === 401) errorMessageText = 'Невалидно потребителско име или парола.';
                } catch (e) {
                    if (response.status === 401) errorMessageText = 'Невалидно потребителско име или парола.';
                    // Could not parse error JSON, stick to default or status-based message
                }
            }
            // Ensure a fallback message if none derived from response body
            if (errorMessageText === 'Грешка при вход.' && response.status !== 401) {
                errorMessageText = `Грешка: ${response.status} ${response.statusText}`;
            }

            if (messageDiv) {
                messageDiv.className = 'error';
                messageDiv.textContent = errorMessageText;
            } else {
                alert(errorMessageText);
            }
        }
    } catch (error) {
        console.error('Login API call failed:', error);
        const msg = 'Грешка при свързване със сървъра.';
        if (messageDiv) {
            messageDiv.className = 'error';
            messageDiv.textContent = msg;
        } else {
            alert(msg);
        }
    }

    return false;
}

// Ensure handleSubmit is globally accessible if the form calls it via onsubmit="return handleSubmit(event)"
// This is generally true for functions declared at the top level of a script.
// For robustness, especially if this script were part of a module system later:
// window.handleSubmit = handleSubmit;
