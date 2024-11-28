
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
    

    if (!arrUsersForAssessment.includes(idCol)) {
        if (arrUsersForAssessment.length <= 1) {
            selected.innerHTML += `<div class="selected_item_main_wrapper" idcol = "${idCol}">
                                                            <div class="description">
                                                                ${elem.querySelector("label").innerText}
                                                            </div>
                                                            <i class="fa-solid fa-trash delete_item" onclick = "deleteUserAssessmentList(this)"></i>
                                                          </div>`
            console.log(idCol)
            arrUsersForAssessment.push(idCol);
        }
    } else {

        let alertText = "Этот сотрудник уже добавлен";
        popupAlert(alertText, false)
    }
    if (arrUsersForAssessment.length == 2 && divBtnSubmit == null) {
        let divBtnSubmit = document.createElement('div');
        optionsContainer.classList.remove("active");
        divBtnSubmit.classList.add('action_btn');
        divBtnSubmit.classList.add('primary_btn');
        divBtnSubmit.classList.add('assessment');
        divBtnSubmit.setAttribute('id', 'users_assessment_submit');
        divBtnSubmit.innerHTML = "Добавить";
        divBtnSubmit.setAttribute('onclick', "submitAssessment(this)")
        choose_user_container.appendChild(divBtnSubmit)
    }

    // Здесь добавить код для записи в БД выбраннх сотрудников  arrUsersForAssessment


    /*optionsContainer.classList.remove("active");*/
}

function optionClickReport(elem) {
    let divBtnSubmit = document.getElementById("users_assessment_submit");
    let selected = document.getElementById("selected_main_wrapper");
    let optionsContainer = document.getElementById("options-container");
    let idCol = elem.querySelector("label").getAttribute('idcol');

    if (!arrUsersForAssessment.includes(idCol)) {
        if (arrUsersForAssessment.length <= 0) {
            selected.innerHTML += `<div class="selected_item_main_wrapper" idcol = "${idCol}">
                                                            <div class="description">
                                                                ${elem.querySelector("label").innerText}
                                                            </div>
                                                            <i class="fa-solid fa-trash delete_item" onclick = "deleteUserAssessmentList(this)"></i>
                                                          </div>`
            console.log(idCol)
            arrUsersForAssessment.push(idCol);
        }
    } else {

        let alertText = "Этот сотрудник уже добавлен";
        popupAlert(alertText, false)
    }
    if (arrUsersForAssessment.length == 1 && divBtnSubmit == null) {
        let divBtnSubmit = document.createElement('div');
        optionsContainer.classList.remove("active");
        divBtnSubmit.classList.add('action_btn');
        divBtnSubmit.classList.add('primary_btn');
        divBtnSubmit.setAttribute('id', 'users_assessment_submit');
        divBtnSubmit.innerHTML = "Показать";
        divBtnSubmit.setAttribute('onclick', `GetReportFromDatepicker(${idCol})`)
        choose_user_container.appendChild(divBtnSubmit)
    }
}

if (searchBoxItem != null) {
    searchBoxItem.addEventListener("keyup", function (e) {

        filterListSearch(e.target.value);
    });
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
    divBtnSubmit.remove()
}