async function openUserPopup(userId) {
    let response = await fetch(`/Admin/GetUserPopup?employeeId=${encodeURIComponent(employeeId)}&gradeId=${encodeURIComponent(gradeId)}`);
    let htmlContent = await response.text();
    popupResult(htmlContent, false)
}

document.addEventListener('DOMContentLoaded', function () {
    const cells = document.querySelectorAll('#matrixTable td');
    let draggedCell = null;
    let targetCell = null;

    cells.forEach(cell => {
        cell.addEventListener('dragstart', dragStart);
        cell.addEventListener('dragover', dragOver);
        cell.addEventListener('drop', drop);
        cell.addEventListener('dragleave', dragLeave);
    });

    function dragStart(e) {
        draggedCell = e.target; // Сохраняем ссылку на перетаскиваемую ячейку
        e.dataTransfer.effectAllowed = 'move';
    }

    function dragOver(e) {
        e.preventDefault(); // Позволяет элементу быть сброшенным
        e.dataTransfer.dropEffect = 'move';

        if (e.target.tagName === 'TD' && e.target !== draggedCell) {
            targetCell = e.target;
            targetCell.classList.add('highlight'); // Подсвечиваем целевую ячейку
        }
    }

    function drop(e) {
        e.preventDefault();
        if (targetCell && targetCell !== draggedCell) {
            // Меняем местами текст ячеек
            swapCells(draggedCell, targetCell);
        }
        clearHighlight(); // Убираем подсветку
    }

    function dragLeave(e) {
        if (e.target === targetCell) {
            clearHighlight(); // Убираем подсветку, если покинули целевую ячейку
        }
    }

    function swapCells(cell1, cell2) {
        const tempText = cell2.querySelector('input[type="text"]').value;
        cell2.querySelector('input[type="text"]').value = cell1.querySelector('input[type="text"]').value;
        cell1.querySelector('input[type="text"]').value = tempText;

        // Обновляем скрытые поля
        const draggedRow = cell1.querySelector('input[type="hidden"][name*="Row"]');
        const draggedColumn = cell1.querySelector('input[type="hidden"][name*="Column"]');
        const targetRow = cell2.querySelector('input[type="hidden"][name*="Row"]');
        const targetColumn = cell2.querySelector('input[type="hidden"][name*="Column"]');

        const tempRow = draggedRow.value;
        const tempColumn = draggedColumn.value;

        draggedRow.value = targetRow.value;
        targetRow.value = tempRow;

        draggedColumn.value = targetColumn.value;
        targetColumn.value = tempColumn;
    }

    function clearHighlight() {
        if (targetCell) {
            targetCell.classList.remove('highlight'); // Убираем подсветку
            targetCell = null; // Сбрасываем целевую ячейку
        }
    }

    // Пример данных для дерева
    const subdivisions = [
        { id: 1, name: "Главное подразделение", parentId: null },
        { id: 2, name: "Подразделение 1", parentId: 1 },
        { id: 3, name: "Подразделение 2", parentId: 1 },
        { id: 4, name: "Подразделение 1.1", parentId: 2 },
        { id: 5, name: "Подразделение 1.2", parentId: 2 }
    ];

    // Функция для создания дерева
    function createTree(data, parentId = null) {
        const ul = document.createElement('ul');
        data.filter(item => item.parentId === parentId).forEach(item => {
            const li = document.createElement('li');
            li.textContent = item.name;
            li.dataset.id = item.id;

            // Рекурсивно добавляем дочерние элементы
            const children = createTree(data, item.id);
            if (children.childElementCount > 0) {
                li.appendChild(children);
            }

            ul.appendChild(li);
        });
        return ul;
    }

    // Создаем дерево и добавляем его в DOM
    const treeContainer = document.getElementById('subdivisionTree');
    treeContainer.appendChild(createTree(subdivisions));

    // Инициализация Sortable для drag-and-drop
    const sortable = new Sortable(treeContainer, {
        group: 'subdivisions',
        animation: 150,
        onEnd: function (evt) {
            // Логика обработки перемещения
            console.log('Перемещено:', evt.item.dataset.id);
        }
    });

    // Сохранение изменений
    document.getElementById('saveChanges').addEventListener('click', function () {
        const updatedSubdivisions = [];

        // Функция для обхода дерева и сбора данных
        function collectData(ul, parentId) {
            Array.from(ul.children).forEach(li => {
                const id = li.dataset.id;
                updatedSubdivisions.push({ id: parseInt(id), parentId: parentId });
                const childrenUl = li.querySelector('ul');
                if (childrenUl) {
                    collectData(childrenUl, id);
                }
            });
        }

        collectData(treeContainer, null);

        // Отправка данных на сервер
        fetch('/api/subdivisions/update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedSubdivisions)
        })
            .then(response => {
                if (response.ok) {
                    alert('Изменения сохранены!');
                } else {
                    alert('Ошибка при сохранении изменений.');
                }
            })
            .catch(error => {
                console.error('Ошибка:', error);
            });
    });

    document.getElementById('save-button').addEventListener('click', function () {
        const form = document.getElementById('matrixForm');
        const formData = new FormData(form);

        // Выводим содержимое FormData в консоль
        for (const [key, value] of formData.entries()) {
            console.log(`${key}: ${value}`);
        }

        fetch(`/Admin/UpdateMatrix`, {
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
    });
});