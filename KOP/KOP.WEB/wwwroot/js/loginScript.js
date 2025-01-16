
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

async function sendRemindPasswordRequest() {
    try {
        const url = '/Account/RemindPassword';
        const jsonToSend = {};
        jsonToSend.Login = document.getElementById('loginRemindPassword').value;

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(jsonToSend)
        });

        let data = await response.json();      

        alert(data.description);

        if (data.statusCode == 200) {
            location.reload();
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert(error);
    }
}

async function sendLoginRequest() {
    try {
        const url = '/Account/Login';
        const jsonToSend = {};
        jsonToSend.Login = document.getElementById('login').value;
        jsonToSend.Password = document.getElementById('password').value;

        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            method: 'POST',
            body: JSON.stringify(jsonToSend)
        });

        let data = await response.json();


        if (data.statusCode == 200) {
            location.href = '/Home/Index';
        }
        else {
            alert(data.description);
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}