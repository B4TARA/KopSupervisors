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
    console.log(item)
    // Проверяем, является ли переданный элемент тем, который нужно обработать
    if (item && item.id === 'disagreeBtnConfirm') {
        // Находим родительский элемент с классом 'section_popup'
        const popupSection = item.closest('.section_popup');
        if (popupSection) {
            popupSection.remove(); // Удаляем родительский элемент
        }
    } else if (item.id == 'overlayWrapper') {
        const defaultPopupSection = document.getElementById('section_popup');
        if (defaultPopupSection) {
            defaultPopupSection.remove(); // Удаляем родительский элемент
        }
    }

    const overlay = document.querySelector('.overlay');
    overlay.classList.remove('active');

    bodyTag.style.overflow = 'auto';

    if (isReload === true) {
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
    
    const rowContainerEducation = document.getElementById("rowContainerEducation");
    let newIndex;

    if (rowContainer) {
        newIndex = rowContainer.children.length;
    }

    let newIndexEducation;

    if (rowContainerEducation) {
        newIndexEducation = rowContainerEducation.children.length;
    }

    const tablePopup = document.querySelector(' table tbody');
    const projectsInfoList = document.querySelector('.project_info_list');

    if (type === 'kpi') {
        const row = document.createElement('tr');

        // Если это не первая строка, копируем значения из предыдущей строки
        let previousRowValues = {};
        if (newIndex > 0) {
            const previousRow = tablePopup.children[newIndex - 1];
            previousRowValues.startDate = previousRow.querySelector('input[name^="Kpis["][name$="PeriodStartDateTime"]').value;
            previousRowValues.endDate = previousRow.querySelector('input[name^="Kpis["][name$="PeriodEndDateTime"]').value;
            previousRowValues.name = previousRow.querySelector('input[name^="Kpis["][name$="Name"]').value;
            previousRowValues.completionPercentage = previousRow.querySelector('input[name^="Kpis["][name$="CompletionPercentage"]').value;
            previousRowValues.calculationMethod = previousRow.querySelector('input[name^="Kpis["][name$="CalculationMethod"]').value;
        }

        row.innerHTML = `
            <td>
                <input type="date" name="Kpis[${newIndex}].PeriodStartDateTime" required min="1994-01-01" value="${previousRowValues.startDate || ''}"/>
            </td>
            <td>
                <input type="date" name="Kpis[${newIndex}].PeriodEndDateTime" required min="1994-01-01" value="${previousRowValues.endDate || ''}"/>
            </td>
            <td>
                <input type="text" name="Kpis[${newIndex}].Name" placeholder="КПЭ" required value="${previousRowValues.name || ''}" />
            </td>
            <td>
                <input type="text" name="Kpis[${newIndex}].CompletionPercentage" placeholder="% выполнения" required value="${previousRowValues.completionPercentage || ''}" />
            </td>
            <td>
                <input type="text" name="Kpis[${newIndex}].CalculationMethod" placeholder="Методика расчета" required value="${previousRowValues.calculationMethod || ''}" />
            </td>
            <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
        `;
        tablePopup.appendChild(row);
    }
    else if (type === 'strategy') {

        const row = document.createElement('tr');
        row.innerHTML = `
              <td>
                            <textarea type="text" name="StrategicTaskDtoList[${newIndex}].Name" placeholder="Название"></textarea>
                        </td>
                        <td>
                            <textarea type="text" name="StrategicTaskDtoList[${newIndex}].Purpose" placeholder="Цель"></textarea>
                        </td>
                        <td>
                            <input type="date" name="StrategicTaskDtoList[${newIndex}].PlanDateTime" required min="1994-01-01"/>
                        </td>
                        <td>
                            <input type="date" name="StrategicTaskDtoList[${newIndex}].FactDateTime" required min="1994-01-01"/>
                        </td>
                        <td>
                            <input type="text" name="StrategicTaskDtoList[${newIndex}].PlanResult" placeholder="План" required />
                        </td>
                        <td>
                            <input type="text" name="StrategicTaskDtoList[${newIndex}].FactResult" placeholder="Факт" required />
                        </td>
                        <td>
                            <textarea type="text" name="StrategicTaskDtoList[${newIndex}].Remark" placeholder="Примечание"></textarea>
                        </td>
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
                `
        tablePopup.appendChild(row)
    }
    else if (type === 'project') {
        var selectedUserFullName = document.getElementById("SelectedUserFullName").value;
        const row = document.createElement('div');
        row.innerHTML = `
            
        <div class="project_info_content">
            <div class="mid_title">
                Проект ${newIndex + 1}
            </div>
            <div>
                ${selectedUserFullName} является 
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
                Оценка реализации проекта SP 
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
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].OrganizationName" required placeholder="Организация" />
                    -
                    <input type="text" name="Qualification.PreviousJobs[${newIndex}].PositionName" required placeholder="Должность"/>
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
                        <td><input type="number" step="0.01" name="MarkTypes[${id}].Marks[${tableBodyRowsLength}].PercentageValue" required/></td>
                    </tr>
                    <i class="fa-solid fa-trash delete_item" style="color: #db1a1a; margin-left:15px; margin-top:15px;" class="delete_row" onclick="deleteRow(this,'${type}')"></i>
                `
        tableBody.appendChild(row)
    }
    else if (type === 'education') {
        const row = document.createElement('div');
        row.classList.add('experience_item')
        row.innerHTML = `
           <div>
                    <input type="text" name="Qualification.HigherEducations[${newIndexEducation}].Education" required />
                    , специальность
                    <input type="text" name="Qualification.HigherEducations[${newIndexEducation}].Speciality" required />
                    , квалификация
                    <input type="text" name="Qualification.HigherEducations[${newIndexEducation}].QualificationName" required />
                    , период обучения c
                    <input type="date" name="Qualification.HigherEducations[${newIndexEducation}].StartDateTime" required min="1970-01-01"/>
                    по
                    <input type="date" name="Qualification.HigherEducations[${newIndexEducation}].EndDateTime" required min="1970-01-01"/>
                    ;
                       
                </div>
                <i class="fa-solid fa-trash delete_item" style="color: #db1a1a;" class="delete_row" onclick="deleteRow(this,'education')"></i>      
                `

        rowContainerEducation.appendChild(row)
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
    const rowContainerEducation = document.getElementById("rowContainerEducation");
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


    // Обновляем индексы для образования
     if (type === 'education') {
        const educationRows = rowContainerEducation.querySelectorAll('.experience_item');
        educationRows.forEach((row, index) => {
            row.querySelector('input[name^="Qualification.HigherEducations["][name$=".Education"]').name = `Qualification.HigherEducations[${index}].Education`;
            row.querySelector('input[name^="Qualification.HigherEducations["][name$=".Speciality"]').name = `Qualification.HigherEducations[${index}].Speciality`;
            row.querySelector('input[name^="Qualification.HigherEducations["][name$=".QualificationName"]').name = `Qualification.HigherEducations[${index}].QualificationName`;
            row.querySelector('input[name^="Qualification.HigherEducations["][name$=".StartDateTime"]').name = `Qualification.HigherEducations[${index}].StartDateTime`;
            row.querySelector('input[name^="Qualification.HigherEducations["][name$=".EndDateTime"]').name = `Qualification.HigherEducations[${index}].EndDateTime`;
        });
    }

}

// StrategicTasks
async function getStrategicTasksPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/StrategicTask/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}
function deleteStrategicTask(id, gradeId) {

    fetch(`/supervisors/StrategicTask/Delete/${id}`, {
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
        });
}
function saveStrategicTasksAsDraft(gradeId, employeeId) {
    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {
            const form = document.getElementById('popupForm');
            form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            fetch(`/supervisors/StrategicTask/EditAll`, {
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
                    getEmployeeLayout(employeeId)
                    getStrategicTasksPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                    popupResult('Ошибка: ' + error.message, false);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
        
    

}
function saveStrategicTasksAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить как черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);



            fetch(`/supervisors/StrategicTask/EditAll`, {
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
                    getEmployeeLayout(employeeId)
                    getStrategicTasksPopup(gradeId);

                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                    popupResult('Ошибка: ' + error.message, false);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}

// Projects
async function getProjectsPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/Project/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteProject(id, gradeId) {
    //if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
    //    return;
    //}

    fetch(`/supervisors/Project/Delete/${id}`, {
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
function saveProjectsAsDraft(gradeId, employeeId) {

    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            fetch(`/supervisors/Project/EditAll`, {
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
                    getEmployeeLayout(employeeId);
                    getProjectsPopup(gradeId);
                })
                .catch(error => {
                });

        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })

}
function saveProjectsAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить ка черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);

            fetch(`/supervisors/Project/EditAll`, {
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
                    getEmployeeLayout(employeeId);
                    getProjectsPopup(gradeId);
                })
                .catch(error => {
                });
        }
        else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
                closeBtnPopup(event.target, false)
            }

        })
}

// Kpis
async function getKpisPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/Kpi/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteKpi(id, gradeId) {
    if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
        return;
    }

    fetch(`/supervisors/Kpi/Delete/${id}`, {
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
function saveKpisAsDraft(gradeId, employeeId) {

    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {


            const form = document.getElementById('popupForm');
            form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            fetch(`/supervisors/Kpi/EditAll`, {
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
                    getEmployeeLayout(employeeId)
                    getKpisPopup(gradeId);
                })
                .catch(error => {
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}
function saveKpisAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить ка черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);

            fetch(`/supervisors/Kpi/EditAll`, {
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
                    getEmployeeLayout(employeeId)
                    getKpisPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                    popupResult('Ошибка: ' + error.message, false);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}

// Marks
async function getMarksPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/Mark/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function deleteMark(id, gradeId) {

    fetch(`/supervisors/Mark/Delete/${id}`, {
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
function saveMarksAsDraft(gradeId, employeeId) {

    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

        const form = document.getElementById('popupForm');
        form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            fetch(`/supervisors/Mark/EditAll`, {
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
                    getEmployeeLayout(employeeId);
                    getMarksPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                });

        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
    
}
function saveMarksAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить ка черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);

            fetch(`/supervisors/Mark/EditAll`, {
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
                    getEmployeeLayout(employeeId);
                    getMarksPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}

// Qualification
async function getQualificationPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/Qualification/GetPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
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

    fetch(`/supervisors/Qualification/DeletePreviousJob/${id}`, {
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
    //if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
    //    return;
    //}

    fetch(`/supervisors/Qualification/DeleteHigherEducation/${id}`, {
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
function saveQualificationAsDraft(gradeId, employeeId) {
    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            formData.forEach((value, key) => {
                console.log(`${key}: ${value}`);
            });

            fetch(`/supervisors/Qualification/Edit`, {
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
                    getEmployeeLayout(employeeId);
                    getQualificationPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                });

        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}
function saveQualificationAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить ка черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);

            fetch(`/supervisors/Qualification/Edit`, {
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
                    getEmployeeLayout(employeeId);
                    getQualificationPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                });

        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}

// ValueJudgment
async function getValueJudgmentPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/ValueJudgment/getPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}
function saveValueJudgmentAsDraft(gradeId, employeeId) {

    let confirmationText = 'Вы действительно хотите сохранить как черновик?'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            form.setAttribute('novalidate', 'true')

            const formData = new FormData(form);
            formData.append('IsFinalized', false);

            fetch(`/supervisors/ValueJudgment/Edit`, {
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
                    getEmployeeLayout(employeeId)
                    getValueJudgmentPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                    popupResult('Ошибка: ' + error.message, false);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}
function saveValueJudgmentAsFinal(gradeId, employeeId) {

    let confirmationText = 'Вы уверены, что хотите сохранить изменения? После сохранения вы не сможете редактировать это поле. Если вы планируете вносить изменения, нажмите \"Сохранить ка черновик\"'

    popupConfirmation(confirmationText, false)
    const confirmationBtnsWrapper = document.querySelector('.confirmation_btns_wrapper')
    confirmationBtnsWrapper.addEventListener('click', async (event) => {

        if (event.target.getAttribute('id') == 'agreeBtnConfirm') {

            const form = document.getElementById('popupForm');
            const formData = new FormData(form);
            formData.append('IsFinalized', true);

            fetch(`/supervisors/ValueJudgment/Edit`, {
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
                    getEmployeeLayout(employeeId)
                    getValueJudgmentPopup(gradeId);
                })
                .catch(error => {
                    console.error('Произошла ошибка:', error);
                    popupResult('Ошибка: ' + error.message, false);
                });
        } else if (event.target.getAttribute('id') == 'disagreeBtnConfirm') {
            closeBtnPopup(event.target, false)
        }

    })
}

// ManagmentCompetencies
async function getManagmentCompetenciesPopup(employeeId, gradeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/supervisors/Assessment/GetManagmentCompetenciesPopup?employeeId=${encodeURIComponent(employeeId)}&gradeId=${encodeURIComponent(gradeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        //document.getElementById('popup').innerHTML = htmlContent;
        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// CorporateCompetencies
async function getCorporateCompetenciesPopup(employeeId, gradeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/supervisors/Assessment/GetCorporateCompetenciesPopup?employeeId=${encodeURIComponent(employeeId)}&gradeId=${encodeURIComponent(gradeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        //document.getElementById('popup').innerHTML = htmlContent;
        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// TrainingEvents
async function getTrainingEventsPopup(gradeId) {
    try {
        let response = await fetch(`/supervisors/TrainingEvent/GetTrainingEventsPopup?gradeId=${encodeURIComponent(gradeId)}`);
        let htmlContent = await response.text();

        popupResult(htmlContent, false)

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
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
    const noResultsMessage = document.getElementById('no-results-message'); // Предполагаем, что у вас есть элемент с этим ID для сообщения

    let anyVisibleRows = false; // Флаг для отслеживания наличия видимых строк

    userWrappers.forEach(wrapper => {
        const rows = wrapper.querySelectorAll('.user_row');
        let hasVisibleRows = false; // Флаг для отслеживания наличия видимых строк в текущем блоке

        rows.forEach(row => {
            const assessmentFlag = row.getAttribute('data-assessment-flag');

            if (filterValue === 'all') {
                row.style.display = '';
            } else if (filterValue === 'assessmentTrue') {
                row.style.display = (assessmentFlag === 'True') ? '' : 'none';
            } else if (filterValue === 'assessmentFalse') {
                row.style.display = (assessmentFlag === 'False') ? '' : 'none';
            }

            // Проверяем, есть ли видимые строки
            if (row.style.display !== 'none') {
                hasVisibleRows = true;
                anyVisibleRows = true; // Устанавливаем флаг, если хотя бы одна строка видима
            }
        });

        // Скрываем или показываем блок в зависимости от наличия видимых строк
        if (hasVisibleRows) {
            wrapper.style.display = ''; // Показываем блок, если есть видимые строки
        } else {
            wrapper.style.display = 'none'; // Скрываем блок, если нет видимых строк
        }
    });

    // Обновляем сообщение о результатах
    if (anyVisibleRows) {
        noResultsMessage.style.display = 'none'; // Скрываем сообщение, если есть видимые строки
    } else {
        noResultsMessage.style.display = ''; // Показываем сообщение, если нет видимых строк
    }
}

function popupConfirmation(text, isReload) {
    let alertSection = document.createElement('section');
    let homeSection = document.querySelector('.home-content')
    alertSection.className = "section_popup alert_popup active_popup";
    alertSection.setAttribute('id', 'section_popup')

    alertSection.innerHTML = `<div class="modal-box">
        
        <div class="mid_title" style="text-align:center; margin-bottom:40px;">${text}</div>
        <div class="action_buttons_wrapper confirmation_btns_wrapper">

                <div class="action_btn green_btn" id = "agreeBtnConfirm">
                    Да
                </div>
                <div class="action_btn red_btn" id="disagreeBtnConfirm">
                    Нет
                </div>

            </div>
        </div>
        `;
    homeSection.appendChild(alertSection)
}