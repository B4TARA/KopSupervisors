/*var selected = document.getElementById("selected_main_wrapper");*/
/*var optionsContainer = document.getElementById("options-container");*/

/*var assessmentBtnSubmit = document.getElementById('users_assessment_submit');*/
/*var chooseUserContainer = document.getElementById('choose_user_container')*/

/*var optionsList = document.querySelectorAll(".option");*/



// В ASSESSMENT JS

var sections = document.querySelectorAll(".section_popup"),
    closeBtn = document.querySelectorAll(".close-btn");

var infoBtn = document.querySelectorAll(".info_btn")

infoBtn.forEach(item => {
    item.addEventListener("click", () => {
        sections.forEach(item => {
            item.classList.remove("active_popup")
        })
        item.nextElementSibling.classList.toggle("active_popup");
    });
})

closeBtn.forEach(item => {
    item.addEventListener("click", () => {
        let parentNodeModalBox = item.parentNode;
        parentNodeModalBox.parentNode.classList.remove("active_popup");
    });
})

var selectAssessmentBtn = document.querySelectorAll(".dropdown_assessment_wrapper")


//selectAssessmentBtn.forEach(item => {
//    item.addEventListener("click", () => {
        
//    });
//})

function openDropdownList(elem) {
    elem.classList.toggle("open");
}


async function submitAssessment(elem) {
    let chooseAssessmentUserContainer = document.getElementById('choose_assessment_user_container')
    let idUser = chooseAssessmentUserContainer.getAttribute('iduser');

    let dataToSend = {};
    dataToSend.judgesServiceNumbers = arrUsersForAssessment;
    dataToSend.judgedSeviceNumber = idUser;

    chooseAssessmentUserContainer.remove();

    const response = await fetch('/SupervisorUdpo/ChooseJudgesForKkAssessment', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(dataToSend)

    })

    if (response.status == 200) {
        let alertText = "Оценщики успешно добавлены!";
        popupAlert(alertText,false)
    } else {
        let alertText = "Упс..Что-то пошло не так";
        popupAlert(alertText,false)
    }
}

