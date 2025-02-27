
var searchBoxItem = document.getElementById("input_searchbox_assessment");
var arrUsersForAssessment = [];


function selectedContainerOpen(elem) {
    let optionsContainer = document.getElementById("options-container");

    optionsContainer.classList.toggle("active");
    searchBoxItem.value = "";
    filterListSearch("");

    if (optionsContainer.classList.contains("active")) {
        searchBoxItem.focus();
    }

}

function optionClick(elem) {
    let divBtnSubmit = document.getElementById("users_assessment_submit");
    let selected = document.getElementById("selected_main_wrapper");
    let optionsContainer = document.getElementById("options-container");
    let idCol = elem.querySelector("label").getAttribute('idcol');

    // Проверка на максимальное количество пользователей
    if (arrUsersForAssessment.length >= 3) {
        let alertText = "Нельзя добавить больше трех сотрудников";
        popupAlert(alertText, false);
        return; // Выходим из функции, если достигнут лимит
    }

    // Проверка на уникальность добавляемого пользователя
    if (!arrUsersForAssessment.includes(idCol)) {
        selected.innerHTML += `
            <div class="selected_item_main_wrapper" idcol="${idCol}">
                <div class="description">
                    ${elem.querySelector("label").innerText}
                </div>
                <i class="fa-solid fa-trash delete_item"></i>
            </div>`;

        arrUsersForAssessment.push(idCol);
        console.log(idCol);
    } else {
        let alertText = "Этот сотрудник уже добавлен";
        popupAlert(alertText, false);
    }

    // Создание кнопки "Добавить"
    if ( divBtnSubmit == null) {
        divBtnSubmit = document.createElement('div');
        divBtnSubmit.classList.add('action_btn', 'primary_btn', 'assessment');
        divBtnSubmit.setAttribute('id', 'users_assessment_submit');
        divBtnSubmit.innerHTML = "Добавить";
        divBtnSubmit.setAttribute('onclick', "addJudges()");
        choose_user_container.appendChild(divBtnSubmit);
    }
    //Если добавлено 3 сотрудника, то закрываем
    if (arrUsersForAssessment.length === 3) {
        optionsContainer.classList.remove("active");
    }

    const deleteItemBtn = document.querySelectorAll('.delete_item');
    deleteItemBtn.forEach(elem => {
        elem.addEventListener('click', (e) => {
            e.stopPropagation()
            console.log(arrUsersForAssessment)
            let divBtnSubmit = document.getElementById("users_assessment_submit")
            let deleteUserSelect = e.target.parentElement;
            deleteUserSelect.remove()

            let idCol = deleteUserSelect.getAttribute('idcol');

            let userIndex = arrUsersForAssessment.indexOf(idCol);
            if (userIndex !== -1) {
                arrUsersForAssessment.splice(userIndex, 1);
            }

            if (arrUsersForAssessment.length == 0) {
                divBtnSubmit.remove()
            }

        })
    })

}

function filterListSearch(searchTerm) {
    let optionsList = document.querySelectorAll(".option");

    searchTerm = searchTerm.toLowerCase();
    optionsList.forEach(option => {
        let label = option.firstElementChild.nextElementSibling.innerText.toLowerCase();

        if (label.indexOf(searchTerm) != -1) {
            option.style.display = "block";
        } else {
            option.style.display = "none";
        }
    });
}

function deleteUserAssessmentList(idcol) {
    let divBtnSubmit = document.getElementById("users_assessment_submit")
    let deleteUserSelect = idcol.parentElement;
    deleteUserSelect.remove()

    let idCol = deleteUserSelect.getAttribute('idcol');

    let userIndex = arrUsersForAssessment.indexOf(idCol);
    if (userIndex !== -1) {
        arrUsersForAssessment.splice(userIndex, 1);
    }

    if (arrUsersForAssessment.length == 0) {
        divBtnSubmit.remove()
    }
    
}

function addJudges() {

    let chooseAssessmentUserContainer = document.getElementById('choose_assessment_user_container')
    let assessmentId = chooseAssessmentUserContainer.getAttribute('assessmentId');
    let employeeId = chooseAssessmentUserContainer.getAttribute('employeeId');

    const formData = new FormData();
    formData.append('assessmentId', assessmentId);
    formData.append('judgesIds', JSON.stringify(arrUsersForAssessment));
    let htmlContentMessage;

    fetch(`/Supervisor/AddJudges`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            htmlContentMessage = `<div class="title">${successMessage}</div>`
            popupResult(htmlContentMessage, false);
            getEmployeeLayout(employeeId);
            getEmployeeAssessmentLayout(employeeId);
            getEmployeeAssessment(assessmentId);
        })
        .catch(error => {
            //console.error('Произошла ошибка:', error);
            htmlContentMessage = `<div class="title">Ошибка:${error.message}</div>`
            popupResult(htmlContentMessage, false);
        });
}

function deleteJudge(assessmentId, employeeId, assessmentResultId) {

    let htmlContentMessage;

    fetch(`/Supervisor/DeleteJudge`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: assessmentResultId
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            htmlContentMessage = `<div class="title">${successMessage}</div>`
            popupResult(htmlContentMessage, false);
            getEmployeeLayout(employeeId);
            getEmployeeAssessmentLayout(employeeId);
            getEmployeeAssessment(assessmentId);
        })
        .catch(error => {
            //console.error('Произошла ошибка:', error);
            htmlContentMessage = `<div class="title">Ошибка:${error.message}</div>`
            popupResult(htmlContentMessage, false);
        });

    let divBtnSubmit = document.getElementById("users_assessment_submit")
    if (arrUsersForAssessment.length == 0) {
        divBtnSubmit.remove()
    }
}