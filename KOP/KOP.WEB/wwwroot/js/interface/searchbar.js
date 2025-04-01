function searchBoxKeyUp(elem, type) {
    let searchTerm = elem.value.toLowerCase(); // Получаем значение из поля поиска
    filterList(searchTerm, type); // Вызываем функцию фильтрации
}

function filterList(searchTerm, type) {
    // Приводим искомый термин к нижнему регистру и заменяем "ё" на "е"
    const normalizedSearchTerm = searchTerm.toLowerCase().replace(/ё/g, 'е');

    // Получаем все строки таблиц
    const userRows = document.querySelectorAll('.list_division_users_wrapper .fullname');

    userRows.forEach(option => {
        // Получаем текст из элемента fullname, приводим к нижнему регистру и заменяем "ё" на "е"
        let label = option.innerText.toLowerCase().replace(/ё/g, 'е');
        let rowElem = option.closest('tr'); // Находим родительский элемент tr

        // Проверяем, содержит ли текст искомую строку
        if (label.includes(normalizedSearchTerm)) {
            rowElem.classList.remove('hide_table_tr'); // Убираем класс hide_table_tr
            rowElem.classList.add('show_table_tr'); // Добавляем класс show_table_tr
        } else {
            rowElem.classList.remove('show_table_tr'); // Убираем класс show_table_tr
            rowElem.classList.add('hide_table_tr'); // Добавляем класс hide_table_tr
        }
    });

    // Проверяем, нужно ли скрыть list-items
    checkIfHideListItems();
}

function checkIfHideListItems() {
    // Получаем все элементы list-items
    const listItems = document.querySelectorAll('.list_division_users_wrapper ');

    listItems.forEach(item => {
        // Получаем все строки в текущей таблице
        const rows = item.querySelectorAll('.table_users tbody tr');
        let allHidden = true; // Флаг для проверки, все ли строки скрыты

        rows.forEach(row => {
            if (!row.classList.contains('hide_table_tr')) {
                allHidden = false; // Если хотя бы одна строка не скрыта, устанавливаем флаг в false
            }
        });

        // Если все строки скрыты, скрываем list-items
        if (allHidden) {
            item.style.display = 'none'; // Скрываем list-items
        } else {
            item.style.display = ''; // Показываем list-items
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
