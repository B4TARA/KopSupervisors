
// // // // // // SUPERVISOR SCRIPT // // // // //
async function getSubordinates(supervisorId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Supervisor/GetSubordinates?supervisorId=${encodeURIComponent(supervisorId)}}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('subordinates').innerHTML = htmlContent;
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getEmployeeLayout(employeeId, elem) {
    try {

        const userRows = document.querySelectorAll('.user_row');
        userRows.forEach(row => {
            row.classList.remove('active')
        })

        elem.classList.add('active')

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

// // // // // ASSESSMENTs SCRIPT // // // // //
async function getEmployeeAssessmentLayout(employeeId) {
    try {
        // Устанавливаем активный элемент меню
        let linkMenuItemGrade = document.getElementById('grade_link_item');
        let linkMenuItemAssessment = document.getElementById('assessment_link_item');
        linkMenuItemGrade.classList.remove("active");
        linkMenuItemAssessment.classList.add("active");

        // Выполняем fetch запрос
        let response = await fetch(`/Supervisor/GetEmployeeAssessmentLayout?employeeId=${encodeURIComponent(employeeId)}`, {
            method: 'GET'
        });

        // Получаем HTML-контент
        let htmlContent = await response.text();

        // Вставляем HTML-контент в нужный элемент
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        /// Получаем типы количественных оценок
        let response2 = await fetch(`/Assessment/GetLastAssessments?employeeId=${encodeURIComponent(employeeId)}`);

        // Получаем JSON-ответ
        let jsonResponse = await response2.json();

        // Проверка на результат ответа
        if (!jsonResponse.success) {
            //alert(jsonResponse.message);
            return;
        }

        // Извлекаем данные из ответа
        const lastAssessments = jsonResponse.data;

        // Если есть типы оценок, то подгружаем самый первый
        if (lastAssessments && lastAssessments.length > 0) {
            await getEmployeeAssessment(lastAssessments[0].id);
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
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

async function assessEmployee(elem, assessmentId, assessmentResultId, employeeId) {
    try {
        let assessmentValues = elem.parentNode.querySelectorAll(".input_assessment_value");
        let assessmentContainer = document.getElementById('assessment_container');

        const jsonToSend = {};
        jsonToSend.resultValues = [];
        jsonToSend.assessmentResultId = assessmentResultId;

        let isValid = true; // Флаг для проверки валидности

        assessmentValues.forEach((item) => {
            let itemValue = item.value.trim(); // Убираем пробелы по краям
            let itemMax = +item.max;
            let itemMin = +item.min;

            // Проверка на наличие недопустимых символов
            const invalidCharacters = /[^\d]/; // Регулярное выражение для проверки на недопустимые символы (все, кроме цифр)

            if (invalidCharacters.test(itemValue)) {
                isValid = false; // Устанавливаем флаг в false
                validationFormAssessment(item, 'errorInclude'); // Вызов функции валидации
                return; // Прерываем выполнение текущей итерации
            }

            // Преобразуем значение в число
            let numericValue = +itemValue;

            // Проверка на валидность
            if (numericValue < itemMin || numericValue > itemMax) {
                isValid = false; // Устанавливаем флаг в false
                validationFormAssessment(item, 'errorValidation'); // Вызов функции валидации
            } else {
                validationFormAssessment(item, 'success'); // Успешная валидация
                jsonToSend.resultValues.push(numericValue + ""); // Добавляем значение в массив
            }
        });

        // Если есть ошибки валидации, не отправляем данные
        if (!isValid) {
            //alert('Пожалуйста, убедитесь, что все значения находятся в допустимом диапазоне.');
            return;
        }

        console.log(JSON.stringify(jsonToSend))
        
        let url = '/Employee/AssessEmployee';

        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            method: 'POST',
            body: JSON.stringify(jsonToSend)
        });

        if (!response.ok) {
            let errorData = await response.json();
            alert(`Ошибка ${response.status}: ${errorData.message}`);
            return;
        }

        popupAlert('Оценка успешно принята!', false)

        await getEmployeeLayout(employeeId);

        await getEmployeeAssessmentLayout(employeeId);

        await getEmployeeAssessment(assessmentId);

    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// Пример функции валидации

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

// // // // // GRADEs SCRIPT // // // // //
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

async function getGradeInfo(id, isClickable) {
    try {
        let url;

        if (isClickable) {
            url = `/Supervisor/GetEmployeeGrade?gradeId=${encodeURIComponent(id)}`;
        } else {
            url = '/image/EmptyState.png';
        }

        // Выполняем fetch запрос
        let response = await fetch(url);

        // Если `isClickable` равно false, обрабатываем ответ как изображение
        if (!isClickable && url.endsWith('.svg') || url.endsWith('.png')) {
            let blob = await response.blob();
            let objectURL = URL.createObjectURL(blob);
            document.getElementById('gradeInfo').innerHTML = `
            <div class= "empty-grade-wrapper">
            <img src="${objectURL}" alt="undefined page" class="empty-grade">
            </div>
            `;
        } else {
            // Получаем текстовый HTML-контент из ответа
            let htmlContent = await response.text();
            // Вставляем HTML-контент в нужный элемент
            document.getElementById('gradeInfo').innerHTML = htmlContent;
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
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