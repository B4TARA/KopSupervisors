
async function getSubordinates(supervisorId) {
    try {
        let response = await fetch(`/supervisors/Supervisor/GetSubordinates?supervisorId=${encodeURIComponent(supervisorId)}`);

        if (!response.ok) {
            throw new Error(`Ошибка при загрузке данных: ${response.status} ${response.statusText}`);
        }

        let htmlContent = await response.text();
        document.getElementById('subordinates').innerHTML = htmlContent;

        filterTable('assessmentTrue');

    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

function findEmployeeRow(employeeId) {

    const rows = document.querySelectorAll('.table_users tbody tr');

    for (let row of rows) {
        if (row.id === employeeId.toString()) {
            return row;
        }
    }

    return null;
}

async function getEmployeeLayout(employeeId, elem) {
    try {

        const userRows = document.querySelectorAll('.user_row');
        userRows.forEach(row => {
            row.classList.remove('active')
        })

        const foundRow = findEmployeeRow(employeeId);
        if (foundRow) {
            foundRow.classList.add('active')
        } else {
            elem.classList.add('active')
        }

        // Выполняем fetch запрос
        let response = await fetch(`/supervisors/Supervisor/GetEmployeeLayout?employeeId=${encodeURIComponent(employeeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('subordinateEmployeeLayout').innerHTML = htmlContent;

        await getEmployeeGradeLayout(employeeId);

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getEmployeeAssessmentLayout(employeeId) {
    try {
        let linkMenuItemGrade = document.getElementById('grade_link_item');
        let linkMenuItemAssessment = document.getElementById('assessment_link_item');

        linkMenuItemGrade.classList.remove("active");
        linkMenuItemAssessment.classList.add("active");

        let response = await fetch(`/supervisors/Supervisor/GetEmployeeAssessmentLayout?employeeId=${encodeURIComponent(employeeId)}`);

        if (!response.ok) {
            throw new Error(`Ошибка при загрузке оценки сотрудника: ${response.status} ${response.statusText}`);
        }

        let htmlContent = await response.text();
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        let firstAssessmentId = document.getElementById('firstAssessmentId').value;

        if (firstAssessmentId && firstAssessmentId > 0) {
            await getEmployeeAssessment(firstAssessmentId);
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

async function getEmployeeAssessment(assessmentId) {

    const buttons = document.querySelectorAll('.self_assesment .link_menu');
    buttons.forEach(btn => {
        btn.addEventListener('click', () => {
            buttons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
        });
    });
    try {
        let response = await fetch(`/supervisors/Supervisor/GetEmployeeAssessment?assessmentId=${encodeURIComponent(assessmentId)}`);

        let htmlContent = await response.text();

        document.getElementById('lastAssessment').innerHTML = htmlContent;

    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

function validationFormAssessment(item, type) {
    item.style.color = "#f00";
    let errorElem = item.parentNode.parentNode.nextElementSibling.querySelector('.grade_error_description');

    if (type == 'errorInclude') {
        textError = 'Неверное значение';
        errorElem.innerHTML = textError;
        item.parentNode.parentNode.nextElementSibling.style.display = 'flex'
    } else if (type == 'errorValidation') {
        textError = 'Значение должно быть в пределах ' + item.min + ' и ' + item.max;
        errorElem.innerHTML = textError;
        item.parentNode.parentNode.nextElementSibling.style.display = 'flex'
    } else if (type == 'success') {
        item.style.color = "#000";
        item.parentNode.parentNode.nextElementSibling.style.display = 'none'
    }
}

async function getEmployeeGradeLayout(employeeId) {
    try {
        let linkMenuItemGrade = document.getElementById('grade_link_item');
        let linkMenuItemAssessment = document.getElementById('assessment_link_item');

        linkMenuItemAssessment.classList.remove("active");
        linkMenuItemGrade.classList.add("active");

        let response = await fetch(`/supervisors/Supervisor/GetEmployeeGradeLayout?employeeId=${encodeURIComponent(employeeId)}`);

        let htmlContent = await response.text();

        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

async function approveEmployeeGrade(gradeId, employeeId) {
    try {
        let response = await fetch('/supervisors/Supervisor/ApproveEmployeeGrade', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: gradeId
        });

        if (response.ok) {
            popupAlert('Оценка успешно завершена', false);
            getEmployeeGradeLayout(employeeId);
        } else {
            console.error("Ошибка при создании Word документа:", response.statusText);
        }
    } catch (error) {
        console.error("Ошибка:", error);
    }
}