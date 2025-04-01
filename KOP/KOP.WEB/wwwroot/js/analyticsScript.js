
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
function drawAssessmentAnalytics(item, typeRender) {
    const colleaguesAvgValueContainer = document.getElementById('colleaguesAvgValueContainer');
    // Удаляем предыдущий график, если он существует
    if (window.radarChart) {
        window.radarChart.destroy();
    }

    if (window.gaugeChart) {
        window.gaugeChart.destroy();
    }

    if (item.typeName == 'Управленческие') {
        colleaguesAvgValueContainer.style.display = 'none'
    } else {
        colleaguesAvgValueContainer.style.display = 'flex'
    }

    console.log(item)
    if (typeRender) {
        // Проверяем наличие данных
        if (!hasData(item)) {
            displayNoDataMessage(); // Вызываем сообщение об отсутствии данных
            return; // Завершаем выполнение функции
        }

        clearNoDataMessage(); // Очищаем сообщение об отсутствии данных

    }
    

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


    var maxValue = 13; // Максимальное значение

    var options = {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [item.generalAvgValue, maxValue - item.generalAvgValue], // Используем значение
                backgroundColor: ["#1b74fd", "#ededfb"]
            }]
        },
        options: {
            rotation: 270, // начальный угол в градусах
            circumference: 180, // угол охвата в градусах
            plugins: {
                legend: {
                    display: false // скрыть легенду
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            // Отображаем только значение в подсказке
                            return tooltipItem.raw;
                        }
                    }
                }
            }
        }
    }

    //// Создание графика
    var gaugeCtx = document.getElementById('chartJSContainer').getContext('2d');

    window.gaugeChart = new Chart(gaugeCtx, options);

}

// Проверка наличия данных
function hasData(item) {
    return item.selfDataArray.length > 0 ||
        item.supervisorDataArray.length > 0 ||
        item.colleaguesDataArray.length > 0;
}

// Очищаем сообщение об отсутствии данных
function clearNoDataMessage() {
    const dashboardItemContentLeft = document.getElementById('dashboardItemContentLeft');
    dashboardItemContentLeft.style.display = 'flex';
    const dashboardItemContentRight = document.getElementById('dashboardItemContentRight');
    dashboardItemContentRight.style.display = 'flex'

    const emptyImageElement = document.getElementById('emptyImage');
    emptyImageElement.innerHTML = ''; // Очищаем содержимое элемента

    const emptyImageElementRight = document.getElementById('emptyImageRight');
    emptyImageElementRight.innerHTML = ''; // Очищаем содержимое элемента
}

function displayNoDataMessage() {
    const dashboardItemContentLeft = document.getElementById('dashboardItemContentLeft');
    dashboardItemContentLeft.style.display = 'none'
    const dashboardItemContentRight = document.getElementById('dashboardItemContentRight');
    dashboardItemContentRight.style.display = 'none'

    const emptyImageElement = document.getElementById('emptyImage');
    emptyImageElement.innerHTML = `
        <div class="undefined_page_wrapper">

			<div class="container_description">
				<div class="title">
					Нет данных
				</div>
				<div class="mid_description">
					Данные для аналитики еще заполнены
				</div>
			</div>
			<div class="empty_state_image_wrapper_middle">
				<img src="/image/EmptyState.png" alt="default_page">
			</div>
		</div>
    `;



    const emptyImageElementRight = document.getElementById('emptyImageRight');
    emptyImageElementRight.innerHTML = `
        <div class="undefined_page_wrapper">

			<div class="container_description">
				<div class="title">
					Нет данных
				</div>
				<div class="mid_description">
					Данные для аналитики еще заполнены
				</div>
			</div>
			<div class="empty_state_image_wrapper_middle">
				<img src="/image/EmptyState.png" alt="default_page">
			</div>
		</div>
    `;

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
        tab.innerText = item.typeName;;
        tab.onclick = () => {

            drawAssessmentAnalytics(item,false);

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
            drawAssessmentAnalytics(item, true);

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
        //const topDescriptions = document.getElementById('topDescriptions');
        //const antiTopDescriptions = document.getElementById('antiTopDescriptions');

        //topCompetencies.innerHTML = '';
        //topDescriptions.innerHTML = '';
       // data.topCompetencies.forEach((item, index) => {
       //     var newTopCompetenceDiv = document.createElement('div');
       //     newTopCompetenceDiv.classList.add('dashboard-competences-group-item');
       //     const itemPercentage = (item.avgValue / 13) * 100;
       //     newTopCompetenceDiv.innerHTML = ` 
       //     <div class="dashboard-competences-group-item-text">${item.name}</div>
							//<div class="dashboard-competences-group-item-scale-wrapper">
							//	<div class="dashboard-competences-group-item-scale" style="width:${itemPercentage}%;"></div>
							//	<div class="description">${item.avgValue}</div>
							//</div>
       //     `;
       //     topCompetencies.appendChild(newTopCompetenceDiv);

       //     var newTopDescription = document.createElement('li');
       //     newTopDescription.textContent = item.competenceDescription;
       //     topDescriptions.appendChild(newTopDescription);
       // });

       // antiTopCompetencies.innerHTML = '';
       // antiTopDescriptions.innerHTML = '';
       // data.antiTopCompetencies.forEach((item, index) => {
       //     const itemPercentage = (item.avgValue / 13) * 100;
       //     var newAntiTopCompetenceDiv = document.createElement('div');
       //     newAntiTopCompetenceDiv.classList.add('dashboard-competences-group-item');
       //     newAntiTopCompetenceDiv.innerHTML = ` 
       //     <div class="dashboard-competences-group-item-text">${item.name}</div>
							//<div class="dashboard-competences-group-item-scale-wrapper">
							//	<div class="dashboard-competences-group-item-scale red" style="width:${itemPercentage}%;"></div>
							//	<div class="description">${item.avgValue}</div>
							//</div>
       //     `;
       //     antiTopCompetencies.appendChild(newAntiTopCompetenceDiv);

       //     var newAntiTopDescription = document.createElement('li');
       //     newAntiTopDescription.textContent = item.competenceDescription;
       //     antiTopDescriptions.appendChild(newAntiTopDescription);
       // });


        document.querySelectorAll('.dashboard-description-header-btn').forEach(function (headerBtn) {

            headerBtn.classList.remove('active')

            headerBtn.addEventListener('click', function (e) {
                e.stopPropagation();
                // Находим соответствующий контент
                const content = this.nextElementSibling;

                this.classList.toggle('active')

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
