function searchBoxKeyUp(elem, type) {
    let searchTerm = elem.value.toLowerCase(); // Получаем значение из поля поиска
    filterList(searchTerm, type); // Вызываем функцию фильтрации
}

function filterList(searchTerm, type) {
    // Получаем все строки таблиц
    const userRows = document.querySelectorAll('.list_division_users_wrapper .fullname');

    userRows.forEach(option => {
        let label = option.innerText.toLowerCase(); // Получаем текст из элемента fullname
        let rowElem = option.closest('tr'); // Находим родительский элемент tr

        // Проверяем, содержит ли текст искомую строку
        if (label.includes(searchTerm)) {
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
    const listItems = document.querySelectorAll('.list_division_users_wrapper .list-items');

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