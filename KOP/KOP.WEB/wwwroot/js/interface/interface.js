var arrowShowMenu = document.querySelectorAll(".arrow"),
    arrowShowSubMenu = document.querySelectorAll(".arrow-sub"),
    selectBtn = document.querySelectorAll(".select-btn"),
    sidebar = document.querySelector(".sidebar"),
    sidebarBtn = document.querySelector(".sidebar_close_btn_wrapper");

const bodyTag = document.getElementsByTagName('body')[0];

//arrowShowMenu.forEach(item => {
//    item.addEventListener("click", (e) => {
//        let arrowParent = e.target.closest('.iocn-link').parentElement; // Находим родительский элемент li
//        arrowParent.classList.toggle("showMenu"); // Переключаем класс для отображения подменю
//    });
//});

if (sidebarBtn != null) {
    sidebarBtn.addEventListener("click", () => {
        sidebar.classList.toggle("close_sidebar");
    });
}

const navItems = document.querySelectorAll(".nav-links > li");

navItems.forEach(item => {
    item.addEventListener("click", (e) => {
            item.classList.toggle("showMenu"); // Переключаем класс для отображения подменю
        
    });
});

arrowShowSubMenu.forEach(item => {
    item.addEventListener("click", (e) => {
        let targetElem = e.target.parentElement;
        let arrowParent = e.target.parentElement.nextElementSibling;//selecting main parent of arrow
        console.log(targetElem)
        arrowParent.classList.toggle("active");
        targetElem.classList.toggle("active");
    });
})

function toggleSelectButton(button) {
    // Находим родительский элемент, который содержит все элементы
    const listItems = button.closest('.list-items');

    // Находим все элементы с классом 'list_division_users_wrapper users' внутри родительского элемента
    const userWrappers = listItems.querySelectorAll('.list_division_users_wrapper.users');

    // Проверяем, есть ли класс 'open' у кнопки
    const isOpen = button.classList.contains("open");

    // Переключаем класс 'open' для кнопки
    button.classList.toggle("open");

    // Устанавливаем класс 'open' для каждого элемента
    userWrappers.forEach(wrapper => {
        if (isOpen) {
            wrapper.classList.remove("open"); // Скрываем
        } else {
            wrapper.classList.add("open"); // Показываем
        }
    });
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
                                <input type="date" name="Kpis[${newIndex}].PeriodStartDateTime" required />
                            </td>
                            <td>
                                <input type="date" name="Kpis[${newIndex}].PeriodEndDateTime" required />
                            </td>
                            <td>
                                <input type="text" name="Kpis[${newIndex}].Name" placeholder="КПЭ" required />
                            </td>
                            <td>
                                <input type="number" name="Kpis[${newIndex}].CompletionPercentage" placeholder="% выполнения" required />
                            </td>
                            <td>
                                <input type="text" name="Kpis[${newIndex}].CalculationMethod" placeholder="Методика расчета" required />
                            </td>
                <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `
        tablePopup.appendChild(row)
    }
    else if (type === 'strategy') {

        const row = document.createElement('tr');
        row.innerHTML = `
              <td>
                            <input type="text" name="StrategicTasks[${newIndex}].Name" placeholder="Название" required />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].Purpose" placeholder="Цель" required />
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].PlanDateTime" required />
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].FactDateTime" required />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].PlanResult" placeholder="План" required />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].FactResult" placeholder="Факт" required />
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
                <input type="text" name="Projects[${newIndex}].SupervisorSspName" placeholder="ФИО руководителя ССП" required /> является заказчиком стратегического проекта 
                <input type="text" name="Projects[${newIndex}].Name" placeholder="Наименование проекта" required />
            </div>
            <div>
                Проект находится на этапе
                <input type="text" name="Projects[${newIndex}].Stage" placeholder="Этап проекта" required />
            </div>
            <div>
                Дата открытия проекта:
                <input type="date" name="Projects[${newIndex}].StartDateTime" required />
            </div>
            <div>
                Срок реализации проекта: 
                <input type="date" name="Projects[${newIndex}].EndDateTime" required />
            </div>
            <div>
                На число 
                <input type="date" name="Projects[${newIndex}].CurrentStatusDateTime" required /> по проекту выполнены 
                <input type="number" name="Projects[${newIndex}].FactStages" required /> из 
                <input type="number" name="Projects[${newIndex}].PlanStages" required /> этапов
            </div>
            <div>
                Коэффициент реализации проекта: 
                <input type="number"  name="Projects[${newIndex}].SPn" required /> %
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
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].StartDateTime" required />
                    по
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].EndDateTime" required />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].OrganizationName" required />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].PositionName" required />
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
                        <td><input type="text" name="MarkTypes[${id}].Marks[${tableBodyRowsLength}].Period" placeholder="Период" required /></td>
                        <td><input type="number" name="MarkTypes[${id}].Marks[${tableBodyRowsLength}].PercentageValue" required/></td>
                    </tr>
                    <i class="fa-solid fa-trash" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this)"></i>
                `
        tableBody.appendChild(row)
    }
}
function deleteRow(elem) {
    elem.parentElement.remove()
}

// StrategicTasks
async function getStrategicTasksPopup(gradeId) {
    try {
        let response = await fetch(`/StrategicTask/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteStrategicTask(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/StrategicTask/Delete/${id}`, {
        method: 'DELETE'
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
            popupResult(successMessage, false);
            getStrategicTasksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveStrategicTasksAsDraft(gradeId) {

    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    fetch(`/StrategicTask/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getStrategicTasksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveStrategicTasksAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/StrategicTask/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getStrategicTasksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// Projects
async function getProjectsPopup(gradeId) {
    try {
        let response = await fetch(`/Project/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteProject(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/Project/Delete/${id}`, {
        method: 'DELETE'
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
            popupResult(successMessage, false);
            getProjectsPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveProjectsAsDraft(gradeId) {

    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    fetch(`/Project/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getProjectsPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveProjectsAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/Project/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getProjectsPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// Kpis
async function getKpisPopup(gradeId) {
    try {
        let response = await fetch(`/Kpi/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteKpi(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/Kpi/Delete/${id}`, {
        method: 'DELETE'
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
            popupResult(successMessage, false);
            getKpisPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveKpisAsDraft(gradeId) {
    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    fetch(`/Kpi/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getKpisPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveKpisAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/Kpi/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getKpisPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// Marks
async function getMarksPopup(gradeId) {
    try {
        let response = await fetch(`/Mark/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteMark(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/Mark/Delete/${id}`, {
        method: 'DELETE'
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
            popupResult(successMessage, false);
            getMarksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveMarksAsDraft(gradeId) {
    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    fetch(`/Mark/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getMarksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveMarksAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/Mark/EditAll`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getMarksPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// Qualification
async function getQualificationPopup(gradeId) {
    try {
        let response = await fetch(`/Qualification/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deletePreviousJob(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/Qualification/DeletePreviousJob/${id}`, {
        method: 'DELETE'
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
            popupResult(successMessage, false);
            getQualificationPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveQualificationAsDraft(gradeId) {
    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    formData.forEach((value, key) => {
        console.log(`${key}: ${value}`);
    });

    fetch(`/Qualification/Edit`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getQualificationPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveQualificationAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/Qualification/Edit`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getQualificationPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// ValueJudgment
async function getValueJudgmentPopup(gradeId) {
    try {
        let response = await fetch(`/ValueJudgment/getPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function saveValueJudgmentAsDraft(gradeId) {
    const form = document.getElementById('popupForm');
    form.setAttribute('novalidate', 'true')

    const formData = new FormData(form);
    formData.append('IsFinalized', false);

    fetch(`/ValueJudgment/Edit`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getValueJudgmentPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}
function saveValueJudgmentAsFinal(gradeId) {

    const form = document.getElementById('popupForm');
    const formData = new FormData(form);
    formData.append('IsFinalized', true);

    fetch(`/ValueJudgment/Edit`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            let popupSection = document.getElementById("section_popup");
            popupSection.remove();

            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.error || 'Неизвестная ошибка');
                });
            }
            return response.text();
        })
        .then(successMessage => {
            popupResult(successMessage, false);
            getValueJudgmentPopup(gradeId);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            popupResult('Ошибка: ' + error.message, false);
        });
}

// CorporateCompetencies
async function getManagmentCompetenciesPopup(employeeId, gradeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Grade/GetManagmentCompetenciesPopup?employeeId=${encodeURIComponent(employeeId)}&gradeId=${encodeURIComponent(gradeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        //document.getElementById('popup').innerHTML = htmlContent;
        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// CorporateCompetencies
async function getCorporateCompetenciesPopup(employeeId, gradeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Grade/GetCorporateCompetenciesPopup?employeeId=${encodeURIComponent(employeeId)}&gradeId=${encodeURIComponent(gradeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        //document.getElementById('popup').innerHTML = htmlContent;
        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// TrainingEvents
async function getTrainingEventsPopup(gradeId) {
    try {
        let response = await fetch(`/Grade/GetTrainingEventsPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}