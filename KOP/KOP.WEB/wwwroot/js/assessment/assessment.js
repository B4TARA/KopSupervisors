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
    item.addEventListener("click", (e) => {
        e.stopPropagation()
        sections.forEach(item => {
            item.classList.remove("active_popup")
        })
        const overlay = document.querySelector('.overlay');
        overlay.classList.add('active') 
        item.nextElementSibling.classList.toggle("active_popup");
        
    });
})

document.addEventListener('click', (event) => {

    if (event.target.closest('.show-modal-btn')) {
        event.stopPropagation()
        const sectionPopup = event.target.closest('td').querySelector('.section_popup');
        sectionPopup.classList.toggle("active_popup");
        bodyTag.style.overflow = 'hidden'
        //const overlay = document.querySelector('.overlay');
        //overlay.classList.add('active');
        
    }

    if (event.target.closest('.close_btn')) {
        const sectionPopup = event.target.closest('.section_popup'); 
        if (sectionPopup) {
            sectionPopup.classList.remove('active_popup');
        }

        bodyTag.style.overflow = 'auto'
    }

    
});

closeBtn.forEach(item => {
    item.addEventListener("click", () => {
        let parentNodeModalBox = item.parentNode;
        parentNodeModalBox.parentNode.classList.remove("active_popup");
    });
})

var selectAssessmentBtn = document.querySelectorAll(".dropdown_assessment_wrapper")

function openDropdownList(elem) {
    elem.classList.toggle("open");
}

