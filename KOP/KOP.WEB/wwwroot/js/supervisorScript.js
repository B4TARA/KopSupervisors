
async function getSubordinates(supervisorId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Supervisor/GetSubordinates?supervisorId=${encodeURIComponent(supervisorId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('subordinates').innerHTML = htmlContent;
        filterTable('assessmentTrue')
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

function findEmployeeRow(employeeId) {
    // Находим все строки таблиц с классом 'table_users'
    const rows = document.querySelectorAll('.table_users tbody tr');

    // Перебираем все строки
    for (let row of rows) {
        // Проверяем, совпадает ли id строки с переданным employeeId
        if (row.id === employeeId.toString()) {
            // Если совпадает, возвращаем найденную строку
            return row;
        }
    }

    // Если строка не найдена, возвращаем null
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
        let response = await fetch(`/Supervisor/GetEmployeeLayout?employeeId=${encodeURIComponent(employeeId)}`);

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

        let response = await fetch(`/Supervisor/GetEmployeeAssessmentLayout?employeeId=${encodeURIComponent(employeeId)}`, {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error('Ошибка при загрузке оценки сотрудника');
        }

        let htmlContent = await response.text();
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        let response2 = await fetch(`/Assessment/GetLastAssessments?userId=${encodeURIComponent(employeeId)}`);

        if (!response2.ok) {
            throw new Error('Ошибка при загрузке последних оценок');
        }

        let jsonResponse = await response2.json();

        if (!jsonResponse.success) {
            console.warn('Не удалось получить последние оценки:', jsonResponse.message);
            return;
        }

        const lastAssessments = jsonResponse.data;

        if (lastAssessments && lastAssessments.length > 0) {
            await getEmployeeAssessment(lastAssessments[0].id);
        }

        arrUsersForAssessment.splice(0, arrUsersForAssessment.length); // Удаляем все элементы
    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

async function getEmployeeAssessment(assessmentId) {

    const buttons = document.querySelectorAll('.self_assesment .link_menu');
    buttons.forEach(btn => {
        btn.addEventListener('click', () => {
            // Удаляем класс active у всех кнопок
            buttons.forEach(b => b.classList.remove('active'));
            // Добавляем класс active к текущей кнопке
            btn.classList.add('active');
        });
    });


    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Supervisor/GetEmployeeAssessment?assessmentId=${encodeURIComponent(assessmentId)}`, {
            method: 'GET'
        });

        // Получаем HTML-контент
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('lastAssessment').innerHTML = htmlContent;

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
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
        // Устанавливаем активный класс на ссылку Grade и убираем с Assessment
        let linkMenuItemGrade = document.getElementById('grade_link_item');
        let linkMenuItemAssessment = document.getElementById('assessment_link_item');

        linkMenuItemAssessment.classList.remove("active");
        linkMenuItemGrade.classList.add("active");

        // Выполняем fetch запрос
        let response = await fetch(`/Supervisor/GetEmployeeGradeLayout?employeeId=${encodeURIComponent(employeeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function approveEmployeeGrade(gradeId, employeeId) {
    try {
        let response = await fetch('/Supervisor/ApproveEmployeeGrade', {
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
            //alert("Ошибка при создании Word документа. Пожалуйста, посмотрите в консоль для деталей.");
        }
    } catch (error) {
        console.error("Ошибка:", error);
        //alert("Произошла ошибка. Пожалуйста, посмотрите в консоль для деталей.");
    }
}