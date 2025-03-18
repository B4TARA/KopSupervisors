
// Grade script
async function getGradeLayout(employeeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Employee/GetGradeLayout?employeeId=${encodeURIComponent(employeeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в элемент с id 'gradeLayout'
        document.getElementById('gradeLayout').innerHTML = htmlContent;
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getGradeInfo(id, isClickable) {
    try {
        let url;

        // Определяем, какой URL использовать в зависимости от значения isClickable
        if (isClickable) {
            url = `/Employee/GetGrade?id=${encodeURIComponent(id)}`;
        } else {
            url = '/image/EmptyState.png'; // путь к изображению
        }

        // Выполняем fetch запрос
        let response = await fetch(url);

        // Если это SVG или изображение, обрабатываем его как Blob
        if (!isClickable && url.endsWith('.svg') || url.endsWith('.png')) {
            let blob = await response.blob();
            let objectURL = URL.createObjectURL(blob);
            document.getElementById('gradeInfo').innerHTML = `
            <div class= "empty-grade-wrapper">
            <img src="${objectURL}" alt="undefined page" class="empty-grade">
            </div>
            `;
        } else {
            // Если это HTML-контент
            let htmlContent = await response.text();
            document.getElementById('gradeInfo').innerHTML = htmlContent;
        }
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

// Assessment script
async function getAssessmentLayout(employeeId) {
    try {
        // Выполняем fetch запрос
        let response = await fetch(`/Employee/GetAssessmentLayout?userId=${encodeURIComponent(employeeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в элемент с id 'assessmentLayout'
        document.getElementById('assessmentLayout').innerHTML = htmlContent;

        await getColleaguesAssessment(employeeId);
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getColleaguesAssessment(employeeId) {
    try {
        // Устанавливаем активные классы
        let colleaguesAssessmentLinkItem = document.getElementById('colleagues_assessment_link_item');
        let selfAssessmentLinkItem = document.getElementById('self_assessment_link_item');

        selfAssessmentLinkItem.classList.remove("active");
        colleaguesAssessmentLinkItem.classList.add("active");

        // Выполняем fetch запрос
        let response = await fetch(`/Employee/GetColleaguesAssessment?employeeId=${encodeURIComponent(employeeId)}`);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в элемент с id 'infoblock_main_container'
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        const firstLinkMenu = document.querySelector('.self_assesment .link_menu');
        console.log(firstLinkMenu)
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getSelfAssessment(employeeId, assessmentId) {
    let colleaguesAssessmentLinkItem = document.getElementById('colleagues_assessment_link_item');
    let selfAssessmentLinkItem = document.getElementById('self_assessment_link_item');

    selfAssessmentLinkItem.classList.add("active");
    colleaguesAssessmentLinkItem.classList.remove("active");
    try {
        let response = await fetch(`/Employee/GetSelfAssessmentLayout`);
        let htmlContent = await response.text();

        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        let response2 = await fetch(`/Assessment/GetLastAssessments?employeeId=${encodeURIComponent(employeeId)}`);
        let jsonResponse = await response2.json();

        if (!jsonResponse.success) {
            console.error('Произошла ошибка:', jsonResponse.message);
            return;
        }

        const lastAssessments = jsonResponse.data;

        if (lastAssessments && lastAssessments.length > 0) {

            if (assessmentId !== undefined) {
                await getAssessment(assessmentId);
            }
            else {
                await getAssessment(lastAssessments[0].id);
            }
        }        
    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

async function getAssessment(assessmentId) {
    try {

        const buttons = document.querySelectorAll('.self_assesment .link_menu');

        buttons.forEach(btn => {
            btn.addEventListener('click', () => {
                // Удаляем класс active у всех кнопок
                buttons.forEach(b => b.classList.remove('active'));
                // Добавляем класс active к текущей кнопке
                btn.classList.add('active');
            });
        });

        // Формируем URL для запроса
        const url = `/Employee/GetSelfAssessment?assessmentId=${encodeURIComponent(assessmentId)}`;

        // Выполняем fetch запрос
        let response = await fetch(url);

        // Получаем текстовый HTML-контент из ответа
        let htmlContent = await response.text();

        // Вставляем HTML-контент в элемент с id 'lastAssessment'
        document.getElementById('lastAssessment').innerHTML = htmlContent;

        
    } catch (error) {
        console.error('Произошла ошибка:', error);
        //alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function assessEmployee(elem, assessmentId, assessmentResultId, employeeId, isSelfAssessment) {
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
            return;
        }

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
            return;
        }

        popupAlert('Оценка успешно принята!', false);

        // Обновление интерфейса после успешной оценки
        await getAssessmentLayout(employeeId);

        if (isSelfAssessment) {
            await getSelfAssessment(employeeId, assessmentId);
        }

    } catch (error) {
        console.error('Произошла ошибка:', error);
    }
}

// Пример функции валидации
function validationFormAssessment(item, type) {
    item.style.color = "#f00";
    let errorElem = item.parentNode.parentNode.nextElementSibling.querySelector('.grade_error_description');

    if (type == 'errorInclude') {
        textError = 'Неверное значение';
        errorElem.innerHTML = textError;
        item.parentNode.parentNode.nextElementSibling.style.display = 'flex';
    } else if (type == 'errorValidation') {
        textError = 'Значение должно быть в пределах ' + item.min + ' и ' + item.max;
        errorElem.innerHTML = textError;
        item.parentNode.parentNode.nextElementSibling.style.display = 'flex';
    } else if (type == 'success') {
        item.style.color = "#000";
        item.parentNode.parentNode.nextElementSibling.style.display = 'none';
    }
}

async function approveGrade(gradeId, employeeId) {
    try {
        let response = await fetch('/Employee/ApproveGrade', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: gradeId
        });

        if (response.ok) {
            popupAlert('Оценка успешно завершена', false);
            getGradeLayout(employeeId);
        } else {
            console.error("Ошибка при создании Word документа:", response.statusText);
            //alert("Ошибка при создании Word документа. Пожалуйста, посмотрите в консоль для деталей.");
        }
    } catch (error) {
        console.error("Ошибка:", error);
        //alert("Произошла ошибка. Пожалуйста, посмотрите в консоль для деталей.");
    }
}