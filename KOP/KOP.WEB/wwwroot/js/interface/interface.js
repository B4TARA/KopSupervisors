var arrowShowMenu = document.querySelectorAll(".arrow"),
    arrowShowSubMenu = document.querySelectorAll(".arrow-sub"),
    selectBtn = document.querySelectorAll(".select-btn"),
    sidebar = document.querySelector(".sidebar"),
    sidebarBtn = document.querySelector(".sidebar_close_btn_wrapper");

const bodyTag = document.getElementsByTagName('body')[0];


arrowShowMenu.forEach(item => {
    item.addEventListener("click", (e) => {
        let arrowParent = e.target.parentElement.parentElement;//selecting main parent of arrow
        arrowParent.classList.toggle("showMenu");
    });
})
if (sidebarBtn != null) {
    sidebarBtn.addEventListener("click", () => {
        sidebar.classList.toggle("close_sidebar");
    });
}

arrowShowSubMenu.forEach(item => {
    item.addEventListener("click", (e) => {
        let targetElem = e.target.parentElement;
        let arrowParent = e.target.parentElement.nextElementSibling;//selecting main parent of arrow
        console.log(targetElem)
        arrowParent.classList.toggle("active");
        targetElem.classList.toggle("active");
    });
})



selectBtn.forEach(item => {
    item.addEventListener("click", () => {
        item.classList.toggle("open");
    });

})

if (selectBtn.length != 0) {
    if (selectBtn.length <= 1) {
        selectBtn[0].classList.toggle("open");
    }
}


/*COMPARE BUTTON*/





/******POPUP ALERT*******/
function closeBtnPopup(item, isReload) {
    let popupSection = document.getElementById("section_popup");
    let isCompareBox = document.getElementById('compare_box')
    let isSelectedWrapper = document.getElementById('selected_main_wrapper')
    popupSection.remove();
    //if (isCompareBox != undefined || isSelectedWrapper != undefined) {
    //    console.log(isCompareBox)
    //} else {
    //    location.reload();
    //}
    bodyTag.style.overflow = 'auto'


    if (isReload == true) {
        location.reload();
    }

    

}

//document.addEventListener('click', (event) => {
//    let popupSection = document.getElementById("section_popup");
//    if (!event.target.classList.contains('modal-box') && popupSection && !event.target.classList.contains('add_table_row')) {
//        popupSection.remove()
//    }
//})


function popupAlert(text, isReload) {
    let alertSection = document.createElement('section');
    let homeSection = document.querySelector('.home-content')
    alertSection.className = "section_popup alert_popup active_popup";
    alertSection.setAttribute('id', 'section_popup')

    alertSection.innerHTML = `<div class="modal-box">
        <div class="close_btn close-btn margin_container_bottom_middle" onclick = "closeBtnPopup(this,${isReload})">
                                    <i class="fa-solid fa-xmark"></i>
                                </div>
        <div class="mid_title">${text}</div>
        </div>
        `;
    homeSection.appendChild(alertSection)

    //setTimeout(closeBtnPopup, 1500);
}


function popupResult(text, isReload) {

    
    bodyTag.style.overflow = 'hidden'

    const sectionPopup = document.querySelector('.section_popup');
    if (sectionPopup) {
        sectionPopup.remove()
    }

    let alertSection = document.createElement('section');
    let homeSection = document.querySelector('.home-content')
    alertSection.className = "section_popup result_popup active_popup";
    alertSection.setAttribute('id', 'section_popup')

    

    alertSection.innerHTML = `<div class="modal-box">
        <div class="close_btn close-btn margin_container_bottom_middle" onclick = "closeBtnPopup(this,${isReload})">
                                    <i class="fa-solid fa-xmark"></i>
        </div>
            ${text}
        </div>
        
        `;
    homeSection.appendChild(alertSection)

    
}

function addRow(elem, type) {
    const tablePopup = document.querySelector(' table tbody');
    const projectsInfoList = document.querySelector('.project_info_list');

    if (type === 'kpi') {

        const row = document.createElement('tr');
        row.innerHTML = `
                <td>
                                <input type="date"></input>
                            </td>
                            <td>
                                <input type="date"></input>
                            </td>
                            <td>
                                <input type="text"></input>
                            </td>
                            <td>
                                <input type="number"></input>
                            </td>
                            <td>
                                <input type="text"></input>
                            </td>
                <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `

        tablePopup.appendChild(row)


    }
    else if (type === 'strategy') {

        const row = document.createElement('tr');
        row.innerHTML = `
                <td>
                    <textarea></textarea>
                </td>
                <td class="fullname">
                    <textarea></textarea>
                </td>
                <td>
                    <textarea></textarea>
                </td>
                <td>
                    <textarea></textarea>
                </td>
                <td>
                    <textarea></textarea>
                </td>
                <td>
                    <textarea></textarea>
                </td>
                <td>
                    <textarea></textarea>
                </td>
                <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `

        tablePopup.appendChild(row)


    }
    else if (type === 'project') {
        const row = document.createElement('div');
        row.innerHTML = `
            
            <div class="project_info">
                <div class="mid_title">
                Проект
            </div>
            <div>
                <input type="text" placeholder="Введите имя заказчика"></input> является заказчиком стратегического проекта <input type="text" placeholder="Введите название проекта"></input>
            </div>
            <div>
                Проект находится на этапе <input type="text" placeholder="Введите этап"></input>
            </div>
            <div>
                Дата открытия проекта: <input type="date"></input>
            </div>
            <div>
                Срок реализации проекта: <input type="date"></input>
            </div>
            <div>
                На число <input type="date"></input> по проекту выполнены <textarea></textarea> из <textarea></textarea> этапов
            </div>
            <div>
                Коэффициент реализации проекта: <input type="number"></input>
            </div>
                <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                
            </div>
                `

        projectsInfoList.appendChild(row)
    } else if (type === 'experience') {
        const experienceList = document.querySelector('.experience_list');
        const row = document.createElement('div');
        row.classList.add('experience_item')
        row.innerHTML = `
            с <input type="date"></input> по <input type="date"></input> - <input type="text"></input> - <input type="text"></input>
            <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `

        experienceList.insertBefore(row,elem)
    }

}

function deleteRow(elem) {
    elem.parentElement.remove()
}

function popupCalculator(isReload, planVal, markId, employeeId, isPercentage) {
    let alertSection = document.createElement('section');
    let homeSection = document.querySelector('.home-content')
    alertSection.className = "section_popup alert_popup active_popup";
    alertSection.setAttribute('id', 'section_popup')
    
    alertSection.innerHTML = `<div class="modal-box result">
        <div class="close_btn close-btn margin_container_bottom_middle" onclick = "closeBtnPopup(this,${isReload})">
                                    <i class="fa-solid fa-xmark"></i>
                                </div>

                                <div class="modal-box-content">
                                    <div class="container_description margin_container_bottom">
                                         <div class="mid_title">
                                            Рассчет показателя
                                         </div>
                                         <div class="mid_description">
                                            Введите данные для рассчета
                                         </div>
                                    </div>
                                    <div class="form_group_item margin_container_bottom">
                                        <label for="position_name" class="mid_description">Фактическое значение</label>
                                        <input type="number" value="" class="calculate-fact-popup-value" id="calculateFactValue" name="position_name">
                                    </div>

                                    <div class="container_description margin_container_bottom">
                                         <div class="mid_title">
                                            Результат
                                         </div>
                                         <div class="title" id="resultParametr">
                                            0
                                         </div>
                                    </div>

                                     <div onclick="saveResultParameter(${markId}, ${employeeId})" class="action_btn primary_btn">
                                        Сохранить
                                     </div>
                                </div>
        
        </div>
        `;
    homeSection.appendChild(alertSection)
    //setTimeout(closeBtnPopup, 1500);

    const factInput = document.getElementById('calculateFactValue')
    if (factInput) {

        const calculateResult = document.getElementById('resultParametr')
        //Сюда формулу рассчета
        if (isPercentage) {
            factInput.addEventListener('input', function (event) {
                const calculateVal = (factInput.value / Number(planVal) * 100).toFixed(0);
                if (calculateVal >= 100) {
                    calculateResult.classList.remove('red-text')
                    calculateResult.classList.add('green-text')
                    
                } else {
                    calculateResult.classList.remove('green-text')
                    calculateResult.classList.add('red-text')
                }
                calculateResult.textContent = calculateVal + '%';

            })
        } else {
            factInput.addEventListener('input', function (event) {
                

                if (factInput.value >= planVal) {
                    calculateResult.classList.remove('red-text')
                    calculateResult.classList.add('green-text')

                } else {
                    calculateResult.classList.remove('green-text')
                    calculateResult.classList.add('red-text')
                }

                calculateResult.textContent = factInput.value;

            })
        }
        
    }
}