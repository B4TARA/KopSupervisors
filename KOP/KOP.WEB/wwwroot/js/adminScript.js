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

    document.getElementById('save-button').addEventListener('click', function () {
        const form = document.getElementById('matrixForm');
        const formData = new FormData(form);

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