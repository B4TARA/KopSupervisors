
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
        else if (data.statusCode == 210) {
            showProjectPopup();
        }
        else if (data.statusCode == 300) {
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = 'https://kop.mtb.minsk.by/Account/Login';

            for (const key in jsonToSend) {
                if (jsonToSend.hasOwnProperty(key)) {
                    const hiddenField = document.createElement('input');
                    hiddenField.type = 'hidden';
                    hiddenField.name = key;
                    hiddenField.value = jsonToSend[key];

                    form.appendChild(hiddenField);
                }
            }

            document.body.appendChild(form);
            form.submit();
        }
        else {
            //let validationTextElem = document.getElementById('validationText');
            //validationTextElem.innerText = data.description;

            alert(data.description);
            console.error(data.description);
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

function showProjectPopup() {
    document.getElementById('projectPopup').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function hideProjectPopup() {
    document.getElementById('projectPopup').style.display = 'none';
    document.getElementById('overlay').style.display = 'none';
}

document.getElementById('heavenSection').addEventListener('click', function () {
    const jsonToSend = {};
    jsonToSend.Login = document.getElementById('login').value;
    jsonToSend.Password = document.getElementById('password').value;
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = 'https://localhost:44305/Account/LoginNow';

    for (const key in jsonToSend) {
        if (jsonToSend.hasOwnProperty(key)) {
            const hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = key;
            hiddenField.value = jsonToSend[key];

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();

    hideProjectPopup();
});

document.getElementById('hellSection').addEventListener('click', function () {
    const jsonToSend = {};
    jsonToSend.Login = document.getElementById('login').value;
    jsonToSend.Password = document.getElementById('password').value;
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = 'https://kop.mtb.minsk.by/Account/Login';

    for (const key in jsonToSend) {
        if (jsonToSend.hasOwnProperty(key)) {
            const hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = key;
            hiddenField.value = jsonToSend[key];

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();

    hideProjectPopup();
});

document.getElementById('overlay').addEventListener('click', function () {
    hideProjectPopup();
});