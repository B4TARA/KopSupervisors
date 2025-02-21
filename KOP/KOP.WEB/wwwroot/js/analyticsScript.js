
document.querySelectorAll('.custom-select-politics').forEach(select => {
    const selectBox = select.querySelector('.select-box-politics');
    const options = select.querySelector('.select-options-politics');
    const label = select.querySelector('.select-label-politics');

    selectBox.addEventListener('click', function (event) {
        event.stopPropagation();

        const isOptionsVisible = options.style.display === 'block';

        // Скрыть все открытые селекты
        document.querySelectorAll('.select-options-politics').forEach(opt => {
            opt.style.display = 'none';
        });

        // Показать или скрыть текущий селект
        if (!isOptionsVisible) {
            options.style.display = 'block';
        }
    });

    options.querySelectorAll('.option-politics').forEach(option => {
        option.addEventListener('click', function () {
            label.textContent = this.textContent; // Обновить текст метки
            options.style.display = 'none'; // Скрыть опции

            const userId = this.getAttribute('data-id');
            loadCompetenciesAnalytics(userId) 
            loadAssessmentAnalytics(userId);
        });
    });
});

const modal = document.getElementById('chartModal');
const modalContent = modal.querySelector('.modal-content');
const closeBtn = document.querySelector('.modal .close');

// Переменные для хранения исходных местоположений графиков
const originalParents = {};

// Обработчик клика на кнопки "Расширить"
document.querySelectorAll('.expand-btn').forEach(button => {
    button.addEventListener('click', function () {
        const targetChartId = this.getAttribute('data-target');
        const targetChart = document.getElementById(targetChartId);

        // Сохраняем исходное родительское место для возвращения графика
        originalParents[targetChartId] = targetChart.parentNode;

        // Перемещаем выбранный график в модальное окно
        modalContent.innerHTML = ''; // Очищаем содержимое
        modalContent.appendChild(targetChart); // Добавляем график в модальное окно

        // Открываем модальное окно
        modal.style.display = 'flex';
    });
});

// Обработчик клика на кнопку закрытия модального окна
closeBtn.addEventListener('click', function () {
    // Возвращаем график обратно в его изначальное место
    Object.keys(originalParents).forEach(chartId => {
        const chart = document.getElementById(chartId);
        const originalParent = originalParents[chartId];
        if (chart && originalParent) {
            originalParent.appendChild(chart); // Возвращаем график обратно
        }
    });

    modal.style.display = 'none';
});

// Закрытие модального окна при клике вне его
window.addEventListener('click', function (event) {
    if (event.target === modal) {
        closeBtn.click(); // Закрытие при клике вне модального окна
    }
});

// Отрисовать график
function drawAssessmentAnalytics(item) {
    // Удаляем предыдущий график, если он существует
    if (window.radarChart) {
        window.radarChart.destroy();
    }

    // Проверяем наличие данных
    if (!hasData(item)) {
        displayNoDataMessage(); // Вызываем сообщение об отсутствии данных
        return; // Завершаем выполнение функции
    }

    clearNoDataMessage(); // Очищаем сообщение об отсутствии данных

    const ctx = document.getElementById('chart4').getContext('2d');

    // Создаем новый график
    window.radarChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: item.labelsArray,
            datasets: createDatasets(item)
        },
        options: getChartOptions()
    });

}

// Проверка наличия данных
function hasData(item) {
    return item.selfDataArray.length > 0 ||
        item.supervisorDataArray.length > 0 ||
        item.colleaguesDataArray.length > 0;
}

// Очищаем сообщение об отсутствии данных
function clearNoDataMessage() {
    const emptyImageElement = document.getElementById('emptyImage');
    emptyImageElement.innerHTML = ''; // Очищаем содержимое элемента
}

// Создаем наборы данных для графика
function createDatasets(item) {
    return [
        {
            label: 'Самооценка',
            data: item.selfDataArray,
            borderColor: 'rgba(255, 0, 0, 1)', // Красный цвет
            backgroundColor: 'transparent', // Полупрозрачный красный
        },
        {
            label: 'Руководитель',
            data: item.supervisorDataArray,
            borderColor: 'rgba(0, 0, 255, 1)', // Синий цвет
            backgroundColor: 'transparent', // Полупрозрачный синий
        },
        {
            label: 'Коллега',
            data: item.colleaguesDataArray,
            borderColor: 'rgba(255, 255, 0, 1)', // Желтый цвет
            backgroundColor: 'transparent', // Полупрозрачный желтый
        }
    ];
}

// Получаем параметры графика
function getChartOptions() {
    return {
        responsive: true,
        plugins: {
            legend: {
                display: true,
                position: 'top',
            },
            title: {
                display: false,
                text: 'Chart.js Polar Area Chart'
            },
            tooltip: {
                callbacks: {
                    label: function (tooltipItem) {
                        const label = tooltipItem.chart.data.labels[tooltipItem.dataIndex];
                        const value = tooltipItem.raw;
                        return `${label}: ${value}`;
                    }
                }
            }
        }
    };
}

// Отобразить сообщение об отсутствии данных
function displayNoDataMessage() {
    const emptyImageElement = document.getElementById('emptyImage');
    emptyImageElement.innerHTML = `
        <div class="undefined_page_wrapper">
            <div class="title">Никто не оценил</div>
            <div class="empty_state_image_wrapper_middle">
                <img src="/image/EmptyState.png" alt="default_page">
            </div>
        </div>
    `;

}

// Получить данные для отрисовки графика (json)
async function loadAssessmentAnalytics(userId) {
    const url = `/Analytics/GetAssessmentAnalytics?userId=${userId}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();

        if (!Array.isArray(data)) {
            console.error('Полученные данные не являются массивом:', data);
            return;
        }

        createTabs(data);
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

// Создаем вкладки
function createTabs(data) {
    const tabContainer = document.getElementById('tabContainer');
    tabContainer.innerHTML = ''; // Очищаем содержимое контейнера

    data.forEach((item, index) => {
        const tab = document.createElement('button');
        tab.className = 'tab';
        tab.innerText = item.typeName;            ;
        tab.onclick = () => {

            drawAssessmentAnalytics(item);

            const generalAvgValue = document.getElementById('generalAvgValue');
            generalAvgValue.innerHTML = item.generalAvgValue;
            const selfAvgValue = document.getElementById('selfAvgValue');
            selfAvgValue.innerHTML = item.selfAvgValue;
            const supervisorAvgValue = document.getElementById('supervisorAvgValue');
            supervisorAvgValue.innerHTML = item.supervisorAvgValue;
            const colleaguesAvgValue = document.getElementById('colleaguesAvgValue');
            colleaguesAvgValue.innerHTML = item.colleaguesAvgValue;

            setActiveTab(tab);
        };

        tabContainer.appendChild(tab);

        // Отрисовываем график для первой вкладки
        if (index === 0) {
            drawAssessmentAnalytics(item);

            const generalAvgValue = document.getElementById('generalAvgValue');
            generalAvgValue.innerHTML = item.generalAvgValue;
            const selfAvgValue = document.getElementById('selfAvgValue');
            selfAvgValue.innerHTML = item.selfAvgValue;
            const supervisorAvgValue = document.getElementById('supervisorAvgValue');
            supervisorAvgValue.innerHTML = item.supervisorAvgValue;
            const colleaguesAvgValue = document.getElementById('colleaguesAvgValue');
            colleaguesAvgValue.innerHTML = item.colleaguesAvgValue;

            setActiveTab(tab); // Делаем первую вкладку активной
        }
    });
}

// Устанавливаем активную вкладку
function setActiveTab(activeTab) {
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    activeTab.classList.add('active');
}

// Получить данные для аналитики компетенций
async function loadCompetenciesAnalytics(userId) {
    const url = `/Analytics/GetCompetenciesAnalytics?userId=${userId}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        const topCompetencies = document.getElementById('topCompetencies');
        const antiTopCompetencies = document.getElementById('antiTopCompetencies');
        const topDescriptions = document.getElementById('topDescriptions');
        const antiTopDescriptions = document.getElementById('antiTopDescriptions');

        topCompetencies.innerHTML = '';
        topDescriptions.innerHTML = '';
        data.topCompetencies.forEach((item, index) => {
            var newTopCompetenceDiv = document.createElement('div');
            newTopCompetenceDiv.classList.add('dashboard-competences-group-item');
            const itemPercentage = ( item.avgValue/13) * 100;
            newTopCompetenceDiv.innerHTML = ` 
            <div class="dashboard-competences-group-item-text mid_description">${item.name}</div>
							<div class="dashboard-competences-group-item-scale-wrapper">
								<div class="dashboard-competences-group-item-scale" style="width:${itemPercentage}%;"></div>
								<div class="description">${item.avgValue}</div>
							</div>
            `;
            topCompetencies.appendChild(newTopCompetenceDiv);

            var newTopDescription = document.createElement('div');
            newTopDescription.textContent = item.competenceDescription;
            newTopDescription.classList.add('mid_description')
            topDescriptions.appendChild(newTopDescription);
        });

        antiTopCompetencies.innerHTML = '';
        antiTopDescriptions.innerHTML = '';
        data.antiTopCompetencies.forEach((item, index) => {
            const itemPercentage = (item.avgValue/13) * 100;
            var newAntiTopCompetenceDiv = document.createElement('div');
            newAntiTopCompetenceDiv.classList.add('dashboard-competences-group-item');
            newAntiTopCompetenceDiv.innerHTML = ` 
            <div class="dashboard-competences-group-item-text mid_description">${item.name}</div>
							<div class="dashboard-competences-group-item-scale-wrapper">
								<div class="dashboard-competences-group-item-scale red" style="width:${itemPercentage}%;"></div>
								<div class="description">${item.avgValue}</div>
							</div>
            `;
            antiTopCompetencies.appendChild(newAntiTopCompetenceDiv);

            var newAntiTopDescription = document.createElement('div');
            newAntiTopDescription.textContent = item.competenceDescription;
            newAntiTopDescription.classList.add('mid_description')
            antiTopDescriptions.appendChild(newAntiTopDescription);
        });

        
        document.querySelectorAll('.dashboard-description-header-btn').forEach(function (headerBtn) {
            headerBtn.addEventListener('click', function (e) {
                e.stopPropagation();
                // Находим соответствующий контент
                const content = this.nextElementSibling;

                // Проверяем, есть ли у контента класс active
                if (content.classList.contains('active')) {
                    // Если есть, удаляем класс active
                    content.classList.remove('active');
                } else {
                    // Если нет, сначала убираем класс active у всех элементов контента
                    document.querySelectorAll('.dashboard-description-content').forEach(function (item) {
                        item.classList.remove('active');
                    });
                    // Затем добавляем класс active к текущему контенту
                    content.classList.add('active');
                }
            });
        });
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

