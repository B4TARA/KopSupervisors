function searchBoxKeyUp(elem, type) {
    let searchTerm = elem.value.toLowerCase(); // Получаем значение из поля поиска
    const currentFilterValue = getCurrentFilterValue(); // Получаем текущее значение фильтра
    filterList(searchTerm, currentFilterValue); // Вызываем функцию фильтрации
}

function getCurrentFilterValue() {
    // Получаем текущее значение фильтра из выпадающего списка
    const dropdownItems = document.querySelectorAll('.dropdown-item');
    let selectedValue = 'all'; // Значение по умолчанию

    dropdownItems.forEach(item => {
        if (item.classList.contains('selected')) {
            selectedValue = item.getAttribute('data-value');
        }
    });

    return selectedValue;
}

function filterList(searchTerm, filterValue) {
    // Приводим искомый термин к нижнему регистру и заменяем "ё" на "е"
    const normalizedSearchTerm = searchTerm.toLowerCase().replace(/ё/g, 'е');

    // Получаем все строки таблиц
    const userRows = document.querySelectorAll('.list_division_users_wrapper .user_row');

    let anyVisibleRows = false; // Флаг для отслеживания наличия видимых строк

    userRows.forEach(rowElem => {
        const fullnameElem = rowElem.querySelector('.fullname');
        const label = fullnameElem.innerText.toLowerCase().replace(/ё/g, 'е');
        const assessmentFlag = rowElem.getAttribute('data-assessment-flag');

        // Проверяем, видима ли строка в зависимости от текущего фильтра
        let isVisible = true;

        if (filterValue === 'assessmentTrue') {
            isVisible = (assessmentFlag === 'True');
        } else if (filterValue === 'assessmentFalse') {
            isVisible = (assessmentFlag === 'False');
        }

        // Проверяем, содержит ли текст искомую строку и видима ли строка
        if (isVisible && label.includes(normalizedSearchTerm)) {
            rowElem.classList.remove('hide_table_tr'); // Убираем класс hide_table_tr
            rowElem.classList.add('show_table_tr'); // Добавляем класс show_table_tr
            anyVisibleRows = true; // Устанавливаем флаг, если строка видима
        } else {
            rowElem.classList.remove('show_table_tr'); // Убираем класс show_table_tr
            rowElem.classList.add('hide_table_tr'); // Добавляем класс hide_table_tr
        }
    });

    // Проверяем, нужно ли скрыть list-items
    checkIfHideListItems();

    // Обновляем сообщение о результатах
    const noResultsMessage = document.getElementById('no-results-message');
    if (anyVisibleRows) {
        noResultsMessage.style.display = 'none'; // Скрываем сообщение, если есть видимые строки
    } else {
        noResultsMessage.style.display = ''; // Показываем сообщение, если нет видимых строк
    }
}

function checkIfHideListItems() {
    // Получаем все элементы list-items
    const listItems = document.querySelectorAll('.list_division_users_wrapper');

    listItems.forEach(item => {
        // Получаем все строки в текущей таблице
        const rows = item.querySelectorAll('.table_users tbody tr');
        let allHidden = true; // Флаг для проверки, все ли строки скрыты

        rows.forEach(row => {
            if (!row.classList.contains('hide_table_tr')) {
                allHidden = false; // Если хотя бы одна строка не скрыта, устанавливаем флаг в false
            }
        });

        // Получаем заголовок
        const description = item.querySelector('.description');

        // Если все строки скрыты, скрываем list-items и заголовок
        if (allHidden) {
            item.style.display = 'none'; // Скрываем list-items
            description.style.display = 'none'; // Скрываем заголовок
        } else {
            item.style.display = ''; // Показываем list-items
            description.style.display = ''; // Показываем заголовок
        }
    });
}
function searchBoxKeyUpReport(input, tableClass) {
    
    const filter = input.value.toLowerCase(); // Получаем текст из поля поиска и приводим к нижнему регистру
    const tableRows = document.querySelectorAll(`.d-tbody .d-tr`); // Получаем все строки таблицы

    tableRows.forEach(row => {
        const fullname = row.querySelector('.fullname').textContent.toLowerCase(); // Получаем ФИО
        const position = row.querySelector('.d-td:nth-child(4)').textContent.toLowerCase(); // Получаем должность

        // Проверяем, содержит ли ФИО или должность текст из поля поиска
        if (fullname.includes(filter) || position.includes(filter)) {
            row.style.display = ''; // Показываем строку
        } else {
            row.style.display = 'none'; // Скрываем строку
        }
    });
}
