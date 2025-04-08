
const wrapper = document.querySelector(".login_main_wrapper");
const signupHeader = document.querySelector(".signup header");
const forgotPassBtn = document.getElementById("forgot_pass");

forgotPassBtn.addEventListener("click", () => {
    wrapper.classList.add("active");
});

signupHeader.addEventListener("click", () => {
    wrapper.classList.remove("active");
});

document.getElementById('loginForm').onsubmit = function (event) {
    event.preventDefault();
    sendLoginRequest();
}

document.getElementById('remindPasswordForm').onsubmit = function (event) {
    event.preventDefault();
    sendRemindPasswordRequest();
}

function sendRemindPasswordRequest() {

    const jsonToSend = {
        Login: document.getElementById('loginRemindPassword').value
    };

    fetch('/supervisors/Account/RemindPassword', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(jsonToSend)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.json();
        })
        .then(data => {
            // Скрываем текст ошибки, если он был показан
            document.getElementById('remindPasswordErrorMessage').textContent = '';
            document.getElementById('remindPasswordErrorValidationText').style.display = 'none';

            // Отображаем успешное сообщение
            document.getElementById('remindPasswordSuccessMessage').textContent = data.message;
            document.getElementById('remindPasswordSuccessValidationText').style.display = 'inline-flex';
        })
        .catch(error => {
            // Скрываем текст успешного сообщения, если он был показан
            document.getElementById('remindPasswordSuccessMessage').textContent = '';
            document.getElementById('remindPasswordSuccessValidationText').style.display = 'none';

            // Отображаем сообщение об ошибке
            console.error('Произошла ошибка:', error);
            document.getElementById('remindPasswordErrorMessage').textContent = 'Ошибка: ' + error.message;
            document.getElementById('remindPasswordErrorValidationText').style.display = 'inline-flex';
        });
}

function sendLoginRequest() {

    const jsonToSend = {
        Login: document.getElementById('login').value,
        Password: document.getElementById('password').value
    };

    fetch('/supervisors/Account/Login', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(jsonToSend)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }

            location.href = '/supervisors/Home/Index';
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            const validationText = document.getElementById('loginValidationText');
            const errorMessage = document.getElementById('loginErrorMessage');

            errorMessage.textContent = 'Ошибка: ' + error.message;
            validationText.style.display = 'inline-flex';
        });
}