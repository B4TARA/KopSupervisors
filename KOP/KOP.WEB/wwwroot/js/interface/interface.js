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
            <div id="popupText">
                ${text}
            </div>
        </div>
        
        `;
    homeSection.appendChild(alertSection)
}

function addRow(elem, type, id) {

    const rowContainer = document.getElementById("rowContainer");
    let newIndex;

    if (rowContainer) {
        newIndex = rowContainer.children.length;
    }

    const tablePopup = document.querySelector(' table tbody');
    const projectsInfoList = document.querySelector('.project_info_list');

    if (type === 'kpi') {

        const row = document.createElement('tr');
        row.innerHTML = `
                <td>
                                <input type="date" name="Kpis[${newIndex}].PeriodStartDateTime" />
                            </td>
                            <td>
                                <input type="date" name="Kpis[${newIndex}].PeriodEndDateTime" />
                            </td>
                            <td>
                                <input type="text" name="Kpis[${newIndex}].Name" placeholder="КПЭ" />
                            </td>
                            <td>
                                <input type="number" name="Kpis[${newIndex}].CompletionPercentage" placeholder="% выполнения" />
                            </td>
                            <td>
                                <input type="text" name="Kpis[${newIndex}].CalculationMethod" placeholder="Методика расчета" />
                            </td>
                <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `
        tablePopup.appendChild(row)
    }
    else if (type === 'strategy') {

        const row = document.createElement('tr');
        row.innerHTML = `
              <td>
                            <input type="text" name="StrategicTasks[${newIndex}].Name" placeholder="Название" />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].Purpose" placeholder="Цель" />
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].PlanDateTime"/>
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].FactDateTime" />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].PlanResult" placeholder="План" />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].FactResult" placeholder="Факт" />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].Remark" placeholder="Примечание" />
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
                Проект ${newIndex + 1}
            </div>
            <div>
                <input type="text" name="Projects[${newIndex}].SupervisorSspName" placeholder="ФИО руководителя ССП" /> является заказчиком стратегического проекта 
                <input type="text" name="Projects[${newIndex}].Name" placeholder="Наименование проекта" />
            </div>
            <div>
                Проект находится на этапе
                <input type="text" name="Projects[${newIndex}].Stage" placeholder="Этап проекта" />
            </div>
            <div>
                Дата открытия проекта:
                <input type="date" name="Projects[${newIndex}].StartDateTime"/>
            </div>
            <div>
                Срок реализации проекта: 
                <input type="date" name="Projects[${newIndex}].EndDateTime" />
            </div>
            <div>
                На число 
                <input type="date" name="Projects[${newIndex}].CurrentStatusDateTime" /> по проекту выполнены 
                <input type="number" name="Projects[${newIndex}].FactStages" /> из 
                <input type="number" name="Projects[${newIndex}].PlanStages" /> этапов
            </div>
            <div>
                Коэффициент реализации проекта: 
                <input type="number"  name="Projects[${newIndex}].SPn" /> %
            </div>
            <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>         
        </div>
                `
        projectsInfoList.appendChild(row)
    }
    else if (type === 'experience') {
        const experienceList = document.querySelector('.experience_list');
        const row = document.createElement('div');
        row.classList.add('experience_item')
        row.innerHTML = `
           <div>
                    c
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].StartDateTime" />
                    по
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].EndDateTime" />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].OrganizationName" />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].PositionName" />
                    ;
                    <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>         
                </div>
                `

        rowContainer.appendChild(row)
    }
    else if (type === 'mark') {
        const row = document.createElement('tr');


        const tableBody = elem.parentElement.querySelector('.table_popup tbody');

        tableBodyRowsLength = tableBody.querySelectorAll('tr').length;
        row.innerHTML = `
            
        <tr>
                        <td><input type="text" name="MarkTypes[${id}].Marks[${tableBodyRowsLength}].Period" /></td>
                        <td><input type="number" name="MarkTypes[${id}].Marks[${tableBodyRowsLength}].PercentageValue" /></td>
                    </tr>
                    <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `
        tableBody.appendChild(row)
    }

}

function deleteRow(elem) {
    elem.parentElement.remove()
}

function popupFormOnsubmit(e, controllerMethod) {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);

    fetch(`/Grade/${controllerMethod}`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.text())
        .then(html => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();
            popupResult(html, false);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            alert('Не удалось выполнить действие. Попробуйте снова.');
        });
}