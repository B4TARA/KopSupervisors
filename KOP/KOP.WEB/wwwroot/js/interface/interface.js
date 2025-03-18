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

    const overlay = document.querySelector('.overlay');
    overlay.classList.remove('active')
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
    const overlay = document.querySelector('.overlay');
    overlay.classList.add('active');

    alertSection.innerHTML = `<div class="modal-box">
        <div class="close_btn close-btn" onclick = "closeBtnPopup(this,${isReload})">
                                    <i class="fa-solid fa-xmark"></i>
                                </div>
        <div class="mid_title">${text}</div>
        </div>
        `;
    homeSection.appendChild(alertSection)

    //setTimeout(closeBtnPopup, 1500);
}
function popupResult(text, isReload) {

    bodyTag.style.overflow = 'hidden';
    const overlay = document.querySelector('.overlay');
    overlay.classList.add('active')

    const sectionPopup = document.querySelector('.section_popup');
    if (sectionPopup) {
        sectionPopup.remove()
    }
    console.log(overlay)
    let alertSection = document.createElement('section');
    let homeSection = document.querySelector('.home-content')
    alertSection.className = "section_popup result_popup active_popup";
    alertSection.setAttribute('id', 'section_popup')

    alertSection.innerHTML = `<div class="modal-box">
        <div class="close_btn close-btn" onclick = "closeBtnPopup(this,${isReload})">
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
                                <input type="date" name="Kpis[${newIndex}].PeriodStartDateTime" required min="1994-01-01"/>
                            </td>
                            <td>
                                <input type="date" name="Kpis[${newIndex}].PeriodEndDateTime" required min="1994-01-01"/>
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
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
                `
        tablePopup.appendChild(row)
    }
    else if (type === 'strategy') {

        const row = document.createElement('tr');
        row.innerHTML = `
              <td>
                            <textarea type="text" name="StrategicTasks[${newIndex}].Name" placeholder="Название"></textarea>
                        </td>
                        <td>
                            <textarea type="text" name="StrategicTasks[${newIndex}].Purpose" placeholder="Цель"></textarea>
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].PlanDateTime" required min="1994-01-01"/>
                        </td>
                        <td>
                            <input type="date" name="StrategicTasks[${newIndex}].FactDateTime" required min="1994-01-01"/>
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].PlanResult" placeholder="План" required />
                        </td>
                        <td>
                            <input type="text" name="StrategicTasks[${newIndex}].FactResult" placeholder="Факт" required />
                        </td>
                        <td>
                            <textarea type="text" name="StrategicTasks[${newIndex}].Remark" placeholder="Примечание"></textarea>
                        </td>
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
                `
        tablePopup.appendChild(row)
    }
    else if (type === 'project') {
        const row = document.createElement('div');
        row.innerHTML = `
            
        <div class="project_info_content">
            <div class="mid_title">
                Проект ${newIndex + 1}
            </div>
            <div>
                ФИО является 
                <input type="text" name="Projects[${newIndex}].UserRole" placeholder="руководителем/заказчиком/со-заказчиком" required />
                стратегического проекта 
                <input type="text" name="Projects[${newIndex}].Name" placeholder="Наименование проекта" required />
            </div>
            <div>
                Проект
                <input type="text" name="Projects[${newIndex}].Stage" placeholder="в реализации/завершен." required />
            </div>
            <div>
                Дата открытия проекта
                <input type="date" name="Projects[${newIndex}].StartDateTime" required min="1994-01-01"/>
            </div>
            <div>
                Дата окончания проекта (план)
                <input type="date" name="Projects[${newIndex}].EndDateTime" required min="1994-01-01"/>
            </div>      
            <div>
                Коэффициент успешности проекта
                <input type="number"  name="Projects[${newIndex}].SuccessRate" required /> %
            </div>
            <div>
                Cредний KPI проекта
                <input type="number"  name="Projects[${newIndex}].AverageKpi" required /> %
            </div>
            <div>
                Коэффициент реализации проекта
                <input type="number"  name="Projects[${newIndex}].SP" required /> %
            </div>
            <i class="fa-solid fa-trash delete_item" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>         
        </div>
                `
        projectsInfoList.appendChild(row)
    }
    else if (type === 'experience') {
        const row = document.createElement('div');
        row.classList.add('experience_item')
        row.innerHTML = `
           <div>
                    c
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].StartDateTime" required min="1970-01-01"/>
                    по
                    <input type="date" name="Qualification.PreviousJobs[${newIndex}].EndDateTime" required min="1970-01-01"/>
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].OrganizationName" required />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].PositionName" required />
                    ;
                       
                </div>
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>      
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
                    <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
                `
        tableBody.appendChild(row)
    }
}

function deleteRow(elem, type) {
    // Удаляем строку
    elem.parentElement.remove();

    // Обновляем индексы оставшихся строк
    updateIndices(type);
}

function updateIndices(type) {
    const tablePopup = document.querySelector('table tbody');
    const rowContainer = document.getElementById("rowContainer");
    const markTableBody = document.querySelector('.table_popup tbody');

    // Обновляем индексы для KPI
    if (type === 'kpi') {
        const kpiRows = tablePopup.querySelectorAll('tr');
        kpiRows.forEach((row, index) => {
            row.querySelector('input[name^="Kpis["][name$=".PeriodStartDateTime"]').name = `Kpis[${index}].PeriodStartDateTime`;
            row.querySelector('input[name^="Kpis["][name$=".PeriodEndDateTime"]').name = `Kpis[${index}].PeriodEndDateTime`;
            row.querySelector('input[name^="Kpis["][name$=".Name"]').name = `Kpis[${index}].Name`;
            row.querySelector('input[name^="Kpis["][name$=".CompletionPercentage"]').name = `Kpis[${index}].CompletionPercentage`;
            row.querySelector('input[name^="Kpis["][name$=".CalculationMethod"]').name = `Kpis[${index}].CalculationMethod`;
        });
    }

    // Обновляем индексы для стратегий
    if (type === 'strategy' && rowContainer) {
        const strategyRows = rowContainer.querySelectorAll('tr');
        strategyRows.forEach((row, index) => {
            const nameInput = row.querySelector('input[name^="StrategicTasks"][name$=".Name"]');
            if (nameInput) {
                nameInput.name = `StrategicTasks[${index}].Name`;
            }

            const purposeInput = row.querySelector('input[name^="StrategicTasks"][name$=".Purpose"]');
            if (purposeInput) {
                purposeInput.name = `StrategicTasks[${index}].Purpose`;
            }

            const planDateTimeInput = row.querySelector('input[name^="StrategicTasks"][name$=".PlanDateTime"]');
            if (planDateTimeInput) {
                planDateTimeInput.name = `StrategicTasks[${index}].PlanDateTime`;
            }

            const factDateTimeInput = row.querySelector('input[name^="StrategicTasks"][name$=".FactDateTime"]');
            if (factDateTimeInput) {
                factDateTimeInput.name = `StrategicTasks[${index}].FactDateTime`;
            }

            const planResultInput = row.querySelector('input[name^="StrategicTasks"][name$=".PlanResult"]');
            if (planResultInput) {
                planResultInput.name = `StrategicTasks[${index}].PlanResult`;
            }

            const factResultInput = row.querySelector('input[name^="StrategicTasks"][name$=".FactResult"]');
            if (factResultInput) {
                factResultInput.name = `StrategicTasks[${index}].FactResult`;
            }

            const remarkInput = row.querySelector('input[name^="StrategicTasks"][name$=".Remark"]');
            if (remarkInput) {
                remarkInput.name = `StrategicTasks[${index}].Remark`;
            }
        });
    }

    // Обновляем индексы для проектов
    if (type === 'project' && rowContainer) {
        const projectRows = rowContainer.querySelectorAll('div.project_info_content');
        projectRows.forEach((row, index) => {
            const supervisorInput = row.querySelector('input[name^="Projects"][name$=".SupervisorSspName"]');
            if (supervisorInput) {
                supervisorInput.name = `Projects[${index}].SupervisorSspName`;
            }

            const nameInput = row.querySelector('input[name^="Projects"][name$=".Name"]');
            if (nameInput) {
                nameInput.name = `Projects[${index}].Name`;
            }

            const stageInput = row.querySelector('input[name^="Projects"][name$=".Stage"]');
            if (stageInput) {
                stageInput.name = `Projects[${index}].Stage`;
            }

            const startDateInput = row.querySelector('input[name^="Projects"][name$=".StartDateTime"]');
            if (startDateInput) {
                startDateInput.name = `Projects[${index}].StartDateTime`;
            }

            const endDateInput = row.querySelector('input[name^="Projects"][name$=".EndDateTime"]');
            if (endDateInput) {
                endDateInput.name = `Projects[${index}].EndDateTime`;
            }

            const currentStatusDateInput = row.querySelector('input[name^="Projects"][name$=".CurrentStatusDateTime"]');
            if (currentStatusDateInput) {
                currentStatusDateInput.name = `Projects[${index}].CurrentStatusDateTime`;
            }

            const factStagesInput = row.querySelector('input[name^="Projects"][name$=".FactStages"]');
            if (factStagesInput) {
                factStagesInput.name = `Projects[${index}].FactStages`;
            }

            const planStagesInput = row.querySelector('input[name^="Projects"][name$=".PlanStages"]');
            if (planStagesInput) {
                planStagesInput.name = `Projects[${index}].PlanStages`;
            }

            const spnInput = row.querySelector('input[name^="Projects"][name$=".SP"]');
            if (spnInput) {
                spnInput.name = `Projects[${index}].SP`;
            }
        });
    }

    // Обновляем индексы для квалификации
    if (type === 'experience' && rowContainer) {
        const experienceItems = rowContainer.querySelectorAll('.experience_item');
        experienceItems.forEach((item, index) => {
            const startDateInput = item.querySelector('input[name^="Qualification.PreviousJobs"][name$=".StartDateTime"]');
            if (startDateInput) {
                startDateInput.name = `Qualification.PreviousJobs[${index}].StartDateTime`;
            }

            const endDateInput = item.querySelector('input[name^="Qualification.PreviousJobs"][name$=".EndDateTime"]');
            if (endDateInput) {
                endDateInput.name = `Qualification.PreviousJobs[${index}].EndDateTime`;
            }

            const organizationInput = item.querySelector('input[name^="Qualification.PreviousJobs"][name$=".OrganizationName"]');
            if (organizationInput) {
                organizationInput.name = `Qualification.PreviousJobs[${index}].OrganizationName`;
            }

            const positionInput = item.querySelector('input[name^="Qualification.PreviousJobs"][name$=".PositionName"]');
            if (positionInput) {
                positionInput.name = `Qualification.PreviousJobs[${index}].PositionName`;
            }
        });
    }

    // Обновляем индексы для марок
    if (type === 'mark' && markTableBody) {
        const markRows = markTableBody.querySelectorAll('tr');
        markRows.forEach((row, index) => {
            const periodInput = row.querySelector('input[name^="MarkTypes"][name$=".Period"]');
            if (periodInput) {
                periodInput.name = `MarkTypes[0].Marks[${index}].Period`;
            }

            const percentageValueInput = row.querySelector('input[name^="MarkTypes"][name$=".PercentageValue"]');
            if (percentageValueInput) {
                percentageValueInput.name = `MarkTypes[0].Marks[${index}].PercentageValue`;
            }
        });
    }


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
function addEducationRow() {

    const rowContainerEducation = document.getElementById("rowContainerEducation");
    let newIndex;

    if (rowContainerEducation) {
        newIndex = rowContainerEducation.children.length;
    }

    const row = document.createElement('div');
    row.classList.add('experience_item')
    row.innerHTML = `
           <div>
                    <input type="text" name="Qualification.HigherEducations[${newIndex}].Education" required />
                    , специальность
                    <input type="text" name="Qualification.HigherEducations[${newIndex}].Speciality" required />
                    , квалификация
                    <input type="text" name="Qualification.HigherEducations[${newIndex}].QualificationName" required />
                    , период обучения c
                    <input type="date" name="Qualification.HigherEducations[${newIndex}].StartDateTime" required min="1970-01-01"/>
                    по
                    <input type="date" name="Qualification.HigherEducations[${newIndex}].EndDateTime" required min="1970-01-01"/>
                    ;
                       
                </div>
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this,'education')"></i>      
                `

    rowContainer.appendChild(row)
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
function deleteHigherEducation(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/Qualification/DeleteHigherEducation/${id}`, {
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

// ManagmentCompetencies
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

// Открытие/закрытие дропдауна
function openUserDropdown() {
    const dropdownContent = document.querySelector('.dropdown-content');
    dropdownContent.style.display = dropdownContent.style.display === 'block' ? 'none' : 'block';

    // Обработчик клика для выбора элемента
    const dropdownItems = document.querySelectorAll('.dropdown-item');
    dropdownItems.forEach(item => {
        item.onclick = function () {
            const dropdownButtonText = document.querySelector('.dropdown-button-text');
            dropdownButtonText.textContent = this.textContent; // Устанавливаем текст кнопки
            dropdownContent.style.display = 'none'; // Закрываем дропдаун

            // Фильтрация таблицы
            filterTable(this.dataset.value);
        };
    });
}

// Функция для фильтрации таблицы
function filterTable(filterValue) {
    const userWrappers = document.querySelectorAll('.list_division_users_wrapper.users.open');

    userWrappers.forEach(wrapper => {
        const rows = wrapper.querySelectorAll('.user_row');
        let hasVisibleRows = false; // Флаг для отслеживания наличия видимых строк

        rows.forEach(row => {
            const assessmentFlag = row.getAttribute('data-assessment-flag');

            if (filterValue === 'all') {
                // Если выбрано "Все", показываем все строки
                row.style.display = '';
            } else if (filterValue === 'assessmentTrue') {
                // Если выбрано "Идет оценка", показываем только строки с TRUE
                row.style.display = (assessmentFlag === 'True') ? '' : 'none';
            } else if (filterValue === 'assessmentFalse') {
                // Если выбрано "Не идет оценка", показываем только строки с FALSE
                row.style.display = (assessmentFlag === 'False') ? '' : 'none';
            }

            // Проверяем, есть ли видимые строки
            if (row.style.display !== 'none') {
                hasVisibleRows = true;
            }
        });

        // Скрываем или показываем блок в зависимости от наличия видимых строк
        if (hasVisibleRows) {
            wrapper.style.display = ''; // Показываем блок, если есть видимые строки
        } else {
            wrapper.style.display = 'none'; // Скрываем блок, если нет видимых строк
        }
    });
}

// Закрытие дропдауна при клике вне его
window.onclick = function (event) {
    const dropdownContent = document.querySelector('.dropdown-content');
    if (!event.target.matches('.dropdown-button') && !event.target.closest('.dropdown')) {
        dropdownContent.style.display = 'none';
    }
};