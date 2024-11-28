

function selectedContainerOpen(elem) {
    let optionsContainer = elem.previousElementSibling;
    console.log(elem.parentElement.parentElement.nextElementSibling)

    //Отображение секции с опшинами в селекте
    optionsContainer.classList.toggle("active");

    elem.classList.toggle("active_border");
}


function optionClick(elem) {

    const resultAssessmentWrapper = elem.parentElement.nextElementSibling.querySelector('.input_assessment_value');
    const descriptionAssessmentValElem = elem.parentElement.nextElementSibling.querySelector('.value_asessessment');
    console.log(elem)
    //Открытие закрытие дива с селектом
    const optionsContainer = elem.parentElement;
    optionsContainer.nextElementSibling.classList.toggle("active_border");
    optionsContainer.classList.toggle("active");

    const resultAssessmentText = elem.querySelector(".select_user_assessment").innerText
    //console.log(resultAssessmentText)
    descriptionAssessmentValElem.innerHTML = resultAssessmentText;

    const resultAssessmentVal = elem.querySelector(".select_user_assessment").getAttribute('itemid')
    resultAssessmentWrapper.value = resultAssessmentVal;

    //Отображение фактической даты
    const resultAssessmentId = elem.querySelector(".select_user_assessment").getAttribute('itemid');

    const inputResultAdminRole = elem.parentElement.nextElementSibling.querySelector('.input_assessment_value');
    
    //console.log(resultAssessmentId)
    inputResultAdminRole.setAttribute('value', resultAssessmentId);
}


async function openPopUp(serviceNumber) {

    const mainContentContainer = document.getElementById('main_container_content');
    let div = document.createElement('div')
    div.setAttribute("id", "userInfo");

    await fetch('GetEmployeePopUpViewComponent?' + new URLSearchParams({
        employeeServiceNumber: serviceNumber
    }),
        {
            method: 'GET',

        })
        .then((response) => response.text())
        .then((data) => {

            const userInfoWrapper = document.getElementById('userInfo');
            if (userInfoWrapper != null) {
                userInfoWrapper.remove()
            }

            div.innerHTML = data;
            mainContentContainer.appendChild(div)

            const elem = document.getElementById('prava');
            openResults(serviceNumber, elem)
        })
}


function closeCard(elem) {
    const elemTodelete = elem.parentElement.parentElement.parentElement;
    elemTodelete.remove()
}


async function showMatrixContent(structureType, elem) {

    const response = await fetch('/Admin/ShowMatrix1ContentComponent', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(structureType)

    })
    const mainMatrixContent = document.getElementById("main_matrix_content");
    mainMatrixContent.innerHTML = await response.text();
    toggleBtnsHedaerMenu(elem, "header")
}


async function showKkMatrixContent(structureType, elem) {

    const response = await fetch('/Admin/ShowMatrix2ContentComponent', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(structureType)

    })
    const mainMatrixContent = document.getElementById("main_matrix_content");
    mainMatrixContent.innerHTML = await response.text();
    toggleBtnsHedaerMenu(elem, "header")
}


function toggleBtnsHedaerMenu(elem, type) {
    if (type == 'header') {
        let arrBtnsHeader = document.querySelectorAll('.tab_btn');
        toggleBtnsHedaerMenuAction(arrBtnsHeader, elem)
    }
    if (type == 'leftnavbar') {
        let arrBtnsHeader = document.querySelectorAll('.leftside_info_popup_block_item');
        toggleBtnsHedaerMenuAction(arrBtnsHeader, elem)
    }


}


function toggleBtnsHedaerMenuAction(arrElem, elem) {
    arrElem.forEach(elem => {
        elem.classList.remove('active')
    })
    elem.classList.add('active')
}


function openInfo(serviceNumber, elem) {
    $('#content').load("/Admin/GetInfoViewComponent?" + $.param({ employeeServiceNumber: serviceNumber }));
    toggleBtnsHedaerMenu(elem, "leftnavbar")
}


function openAssessment(serviceNumber, elem) {
    $('#content').load("/Admin/GetAssessmentViewComponent?" + $.param({ employeeServiceNumber: serviceNumber }));
    toggleBtnsHedaerMenu(elem, "leftnavbar")
}


function openPosition(serviceNumber, elem) {
    $('#content').load("/Admin/GetPositionViewComponent?" + $.param({ employeeServiceNumber: serviceNumber }));
    toggleBtnsHedaerMenu(elem, "leftnavbar")
}

function openResults(serviceNumber, elem) {
    $('#content').load("/Admin/GetResultsViewComponent?" + $.param({ employeeServiceNumber: serviceNumber }));
    toggleBtnsHedaerMenu(elem, "leftnavbar")
}