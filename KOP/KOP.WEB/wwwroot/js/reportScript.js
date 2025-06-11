
async function downloadGradesReport(gradeId, employeeFullName) {
    const loader = document.getElementById('loader');
    const downloadText = document.getElementById('downloadText');
    const downloadButton = document.getElementById('downloadButton');

    try {
        // Показываем индикатор загрузки
        downloadText.style.display = 'none';
        loader.style.display = 'block';
        downloadButton.disabled = true;  // Кнопка теперь неактивна

        const response = await fetch(`/supervisors/Report/GetGradesReport?gradeId=${gradeId}`, {
            method: 'GET',
        });

        if (response.ok) {
            // Получаем Blob из ответа
            const blob = await response.blob();

            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${employeeFullName}.docx`; // Имя файла          

            // Добавляем ссылку на страницу, кликаем на нее и удаляем
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        } else {
            console.error("Ошибка при создании Word документа:", response.statusText);
            alert("Ошибка при создании Word документа. Пожалуйста, посмотрите в консоль для деталей.");
        }
    } catch (error) {
        console.error("Ошибка:", error);
        alert("Произошла ошибка. Пожалуйста, посмотрите в консоль для деталей.");
    } finally {
        // Скрываем индикатор загрузки
        loader.style.display = 'none';
        downloadText.style.display = 'block';
        downloadButton.disabled = false;  // Кнопка теперь активна
    }
}

async function downloadUpcomingGradesReport() {
    const loader = document.getElementById('loader');
    const downloadText = document.getElementById('downloadText');
    const downloadButton = document.getElementById('downloadButton');

    try {
        // Показываем индикатор загрузки
        downloadText.style.display = 'none';
        loader.style.display = 'block';
        downloadButton.disabled = true;  // Кнопка теперь неактивна

        const response = await fetch(`/supervisors/Report/GetUpcomingGradesReport`, {
            method: 'GET',
        });

        if (response.ok) {
            // Получаем Blob из ответа
            const blob = await response.blob();

            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Предстоящие оценки.xlsx`; // Имя файла          

            // Добавляем ссылку на страницу, кликаем на нее и удаляем
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        } else {
            console.error("Ошибка при создании Word документа:", response.statusText);
            alert("Ошибка при создании Word документа. Пожалуйста, посмотрите в консоль для деталей.");
        }
    } catch (error) {
        console.error("Ошибка:", error);
        alert("Произошла ошибка. Пожалуйста, посмотрите в консоль для деталей.");
    } finally {
        // Скрываем индикатор загрузки
        loader.style.display = 'none';
        downloadText.style.display = 'block';
        downloadButton.disabled = false;  // Кнопка теперь активна
    }
}