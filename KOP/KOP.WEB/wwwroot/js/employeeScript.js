﻿
async function getGradeLayout() {
    try {
        let response = await fetch(`/Employee/GetGradeLayout`);
        let htmlContent = await response.text();
        document.getElementById('gradeLayout').innerHTML = htmlContent;
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getEmployeeLayout() {
    try {
        let response = await fetch(`/Employee/GetGradeLayout`);
        let htmlContent = await response.text();
        document.getElementById('gradeLayout').innerHTML = htmlContent;
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getAssessmentLayout() {
    try {
        let response = await fetch(`/Employee/GetAssessmentLayout`);
        let htmlContent = await response.text();
        document.getElementById('assessmentLayout').innerHTML = htmlContent;

        await getColleaguesAssessment();
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getColleaguesAssessment() {
    try {
        let colleaguesAssessmentLinkItem = document.getElementById('colleagues_assessment_link_item');
        let selfAssessmentLinkItem = document.getElementById('self_assessment_link_item');

        selfAssessmentLinkItem.classList.remove("active");
        colleaguesAssessmentLinkItem.classList.add("active");

        let response = await fetch(`/Employee/GetColleaguesAssessment`);
        let htmlContent = await response.text();
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getSelfAssessment(assessmentId, userId) {   
    try {
        let colleaguesAssessmentLinkItem = document.getElementById('colleagues_assessment_link_item');
        let selfAssessmentLinkItem = document.getElementById('self_assessment_link_item');

        selfAssessmentLinkItem.classList.add("active");
        colleaguesAssessmentLinkItem.classList.remove("active");

        let response = await fetch(`/Employee/GetSelfAssessmentLayout`);
        let htmlContent = await response.text();
        document.getElementById('infoblock_main_container').innerHTML = htmlContent;

        let response2 = await fetch(`/Assessment/GetLastAssessments?userId=${encodeURIComponent(userId)}`);
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
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}

async function getAssessment(assessmentId) {
    try {
        const buttons = document.querySelectorAll('.self_assesment .link_menu');
        buttons.forEach(btn => {
            btn.addEventListener('click', () => {
                buttons.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
            });
        });

        let response = await fetch(`/Employee/GetSelfAssessment?assessmentId=${encodeURIComponent(assessmentId)}`);
        let htmlContent = await response.text();
        document.getElementById('lastAssessment').innerHTML = htmlContent;       
    } catch (error) {
        console.error('Произошла ошибка:', error);
        alert('Не удалось выполнить действие. Попробуйте снова.');
    }
}



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

async function approveGrade(gradeId) {
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
            getGradeLayout();
        } else {
            console.error("Ошибка при согласовании оценки:", response.statusText);
            alert("Ошибка при согласовании оценки. Пожалуйста, посмотрите в консоль для деталей.");
        }
    } catch (error) {
        console.error("Ошибка:", error);
        alert("Произошла ошибка. Пожалуйста, посмотрите в консоль для деталей.");
    }
}