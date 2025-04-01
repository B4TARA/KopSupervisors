
async function openUserLayout(userId) {
    let response = await fetch(`/supervisors/Admin/GetUserLayout?userId=${encodeURIComponent(userId)}`);
    let htmlContent = await response.text();
    popupResult(htmlContent, false)
    openUserSystemRoles(userId);
}

async function openUserSubordinates(userId) {
    let response = await fetch(`/supervisors/Admin/GetUserSubordinates?userId=${encodeURIComponent(userId)}`);
    let htmlContent = await response.text();
    document.getElementById('popupContent').innerHTML = htmlContent;
    showSubordinatesTree();
}

async function openUserSystemRoles(userId) {
    let response = await fetch(`/supervisors/Admin/GetUserSystemRoles?userId=${encodeURIComponent(userId)}`);
    let htmlContent = await response.text();
    document.getElementById('popupContent').innerHTML = htmlContent;
}

function updateUserSystemRoles() {
    const form = document.getElementById('userSystemRolesForm');
    const formData = new FormData(form);

    fetch(`/supervisors/Admin/UpdateUserSystemRoles`, {
        method: 'POST',
        body: formData
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
            alert(successMessage);

        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            alert('Ошибка: ' + error.message);
        });
}

function updateUserSubordinates() {
    // Получаем выбранные идентификаторы
    var userId = document.getElementById("subordinatesTree").getAttribute("data-user-id");
    var subordinatesTree = $('#subordinatesTree');
    var selectedIds = Array.from(subordinatesTree.jstree(true).get_selected());

    // Создаем объект данных для отправки
    var data = {
        UserId: userId,
        SubordinateSubdivisionsIds: selectedIds
    };

    // Отправка данных на сервер с помощью fetch
    fetch('/supervisors/Admin/UpdateUserSubordinates', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
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
            alert(successMessage);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            alert('Ошибка: ' + error.message);
        });
}

function selectedContainerOpen(elem) {
    let optionsContainer = elem.previousElementSibling;
    optionsContainer.classList.toggle("active");
    elem.classList.toggle("active_border");
}

function optionClick(elem) {

    const resultAssessmentWrapper = elem.parentElement.nextElementSibling.querySelector('.input_assessment_value');
    const descriptionAssessmentValElem = elem.parentElement.nextElementSibling.querySelector('.value_asessessment');
    //Открытие закрытие дива с селектом
    const optionsContainer = elem.parentElement;
    optionsContainer.nextElementSibling.classList.toggle("active_border");
    optionsContainer.classList.toggle("active");

    const resultAssessmentText = elem.querySelector(".select_user_assessment").innerText
    //console.log(resultAssessmentText)
    descriptionAssessmentValElem.innerHTML = resultAssessmentText;

    const resultAssessmentVal = elem.querySelector(".select_user_assessment").getAttribute('itemid')
    resultAssessmentWrapper.value = resultAssessmentVal;

    //Отображение фактической даты
    const resultAssessmentId = elem.querySelector(".select_user_assessment").getAttribute('itemid');

    const inputResultAdminRole = elem.parentElement.nextElementSibling.querySelector('.input_assessment_value');

    inputResultAdminRole.setAttribute('value', resultAssessmentId);
}

function closeCard(elem) {
    const elemTodelete = elem.parentElement.parentElement.parentElement;
    elemTodelete.remove()
}

function toggleBtnsHedaerMenu(elem, type) {
    if (type == 'header') {
        let arrBtnsHeader = document.querySelectorAll('.tab_btn');
        toggleBtnsHedaerMenuAction(arrBtnsHeader, elem)
    }
    if (type == 'leftnavbar') {
        let arrBtnsHeader = document.querySelectorAll('.leftside_info_popup_block_item');
        toggleBtnsHedaerMenuAction(arrBtnsHeader, elem)
    }
}

function toggleBtnsHedaerMenuAction(arrElem, elem) {
    arrElem.forEach(elem => {
        elem.classList.remove('active')
    })
    elem.classList.add('active')
}

function updateMatrix() {
    const form = document.getElementById('matrixForm');
    const formData = new FormData(form);

    fetch(`/supervisors/Admin/UpdateMatrix`, {
        method: 'POST',
        body: formData
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
            alert(successMessage);
        })
        .catch(error => {
            console.error('Произошла ошибка:', error);
            alert('Ошибка: ' + error.message);
        });
}

function showSubordinatesTree() {
    var treeData = [];
    var dataFromRazor = JSON.parse(document.getElementById("subordinatesTree").getAttribute("data-subordinates"));

    dataFromRazor.forEach(function (sub) {
        treeData.push({
            'id': sub.Id,
            'parent': (sub.ParentId == null ? "#" : sub.ParentId.toString()),
            'text': sub.Name,
            'state': {
                'selected': sub.IsSelected
            }
        });
    });

    $('#subordinatesTree').jstree({
        'plugins': ['search', 'checkbox', 'wholerow'],
        'core': {
            'data': treeData,
            'animation': false,
            'themes': {
                'icons': false,
            }
        },
        'search': {
            'show_only_matches': true,
            'show_only_matches_children': true
        }
    });

    $('#search').on("keyup change", function () {
        $('#subordinatesTree').jstree(true).search($(this).val())
    })

    $('#clear').click(function (e) {
        $('#search').val('').change().focus()
    })

    $('#subordinatesTree').on('changed.subordinatesTree', function (e, data) {
        var objects = data.instance.get_selected(true)
        var leaves = $.grep(objects, function (o) { return data.instance.is_leaf(o) })
        var list = $('#output')
        list.empty()
        $.each(leaves, function (i, o) {
            $('<li/>').text(o.text).appendTo(list)
        })
    })
}