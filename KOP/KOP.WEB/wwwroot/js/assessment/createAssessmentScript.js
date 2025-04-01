
async function validateAssessmentValues(assessmentValues) {
    const jsonToSend = {
        resultValues: [],
        assessmentResultId: null // Это будет установлено позже
    };
    let isValid = true;

    // Проверка значений оценок
    assessmentValues.forEach((item) => {
        const itemValue = item.value.trim();
        const itemMax = +item.max;
        const itemMin = +item.min;

        // Проверка на недопустимые символы
        const invalidCharacters = /[^\d]/;
        if (invalidCharacters.test(itemValue)) {
            isValid = false;
            validationFormAssessment(item, 'errorInclude');
            return;
        }

        const numericValue = +itemValue;

        // Проверка на допустимый диапазон
        if (numericValue < itemMin || numericValue > itemMax) {
            isValid = false;
            validationFormAssessment(item, 'errorValidation');
        } else {
            validationFormAssessment(item, 'success');
            jsonToSend.resultValues.push(numericValue.toString());
        }
    });

    return { isValid, jsonToSend };
}

async function sendAssessmentData(jsonToSend) {
    const response = await fetch('/Employee/AssessUser', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(jsonToSend)
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(`Ошибка ${response.status}: ${errorData.message}`);
    }
}

async function assessEmployee(elem, assessmentId, assessmentResultId, judgedId) {
    try {
        const assessmentValues = elem.parentNode.querySelectorAll(".input_assessment_value");
        const { isValid, jsonToSend } = await validateAssessmentValues(assessmentValues);
        jsonToSend.assessmentResultId = assessmentResultId;

        // Если есть ошибки валидации
        if (!isValid) {
            popupAlert('Пожалуйста, убедитесь, что все значения находятся в допустимом диапазоне.', false);
            return;
        }

        // Отправка данных на сервер
        await sendAssessmentData(jsonToSend);

        // Успешное завершение
        popupAlert('Оценка успешно принята!', false);

        // Обновление данных
        await getEmployeeLayout(judgedId);
        await getEmployeeAssessmentLayout(judgedId);
        await getEmployeeAssessment(assessmentId);

    } catch (error) {
        console.error('Произошла ошибка:', error);
        popupAlert('Не удалось выполнить действие. Попробуйте снова.', false);
    }
}

async function createAssessment(elem, assessmentResultId, judgedId, isSelfAssessment = false, assessmentId = null) {
    try {
        const assessmentValues = elem.parentNode.querySelectorAll(".input_assessment_value");
        const { isValid, jsonToSend } = await validateAssessmentValues(assessmentValues);
        jsonToSend.assessmentResultId = assessmentResultId;

        // Если есть ошибки валидации
        if (!isValid) {
            popupAlert('Пожалуйста, убедитесь, что все значения находятся в допустимом диапазоне.', false);
            return;
        }

        // Отправка данных на сервер
        await sendAssessmentData(jsonToSend);

        // Успешное завершение
        popupAlert('Оценка успешно принята!', false);

        // Обновление данных
        await getEmployeeLayout(judgedId);
        await getEmployeeAssessmentLayout(judgedId);
        if (isSelfAssessment) {
            await getSelfAssessment(assessmentId, judgedId);
        }

    } catch (error) {
        console.error('Произошла ошибка:', error);
        popupAlert('Не удалось выполнить действие. Попробуйте снова.', false);
    }
}

async function assessYourself(elem, assessmentId, assessmentResultId, judgedId) {
    await createAssessment(elem, assessmentResultId, judgedId, true, assessmentId);
}

async function assessAnotherUser(elem, assessmentResultId, judgedId) {
    await createAssessment(elem, assessmentResultId, judgedId);
}