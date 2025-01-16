

let showReportBtn = {
    content: 'Показать',
    className: 'custom-button-classname',
    onClick: (dp) => {
        GetReportFromDatepicker()     
    }
}


new AirDatepicker('#datepickerReportPeriod', {
    range: true,
    multipleDatesSeparator: ' - ',
    buttons: [showReportBtn, 'clear'],
});


new AirDatepicker('#datepickerReportPeriodNoButtons', {
    range: true,
    multipleDatesSeparator: ' - ',
    buttons: ['clear'],
});


new AirDatepicker('#datepickerReportDate', {
    multipleDatesSeparator: ' - ',
    buttons: [showReportBtn, 'clear'],
});


new AirDatepicker('#datepickerReportDateSelect', {
    multipleDatesSeparator: ' - ',
    buttons: ['clear'],
});


new AirDatepicker('#datepickerReportOnlyMonthSelect', {
    view: 'months',
    minView: 'months',
    dateFormat: 'MMMM yyyy',
    multipleDatesSeparator: ' - ',
    buttons: ['clear'],
});


new AirDatepicker('#datepickerReportOnlyMonthPeriod', {
    range: true,
    view: 'months',
    minView: 'months',
    dateFormat: 'MMMM yyyy',
    multipleDatesSeparator: ' - ',
    buttons: [showReportBtn, 'clear'],
});


function GetReportFromDatepicker(param) {

    let viewDate = document.querySelector('.input_datepicker').value


    const elemReport = document.getElementById('reportType')
    const idElemReport = elemReport.getAttribute('reportid')


    if (idElemReport == "1") {
        $.post('GetReport1ViewComponent', { viewDate: viewDate },
            function (result) {
                $("#main_container_content").html(result);
                addFilterIcons()
            });
    }


    else if (idElemReport == "2") {
        $.post('GetReport2ViewComponent', { viewDate: viewDate },
            function (result) {
                $("#main_container_content").html(result);
            });
    }


    else if (idElemReport == "3part1") {
        console.log(param);
        $.post('GetReport3part1ViewComponent', { viewDate: viewDate, mtAssessmentStatus: param },
            function (result) {
                $("#main_container_content").html(result);
            });
    }


    else if (idElemReport == "3part2") {
        $.post('GetReport3part2ViewComponent', { viewDate: viewDate },
            function (result) {
                $("#main_container_content").html(result);
            });
    }


    else if (idElemReport == "4") {
        $.post('GetReport4ViewComponent', { viewDate: viewDate },
            function (result) {
                $("#main_container_content").html(result);
            });
    }


    else if (idElemReport == "5") {
        $.post('GetReport5ViewComponent', { viewDate: viewDate, employeeServiceNumber: param },
            function (result) {
                $("#main_container_content").html(result);
            });
    }
}


function SaveReportFromDatepicker(reportNumber, viewDate, param) {


    if (reportNumber == 1) {
        $.post("SaveReport1", { viewDate: viewDate }, function (result) {
            window.open(`../attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false)
            })
            .fail(function (result) {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 2) {
        $.post("SaveReport2", { viewDate: viewDate }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 31) {
        $.post("SaveReport3part1", { viewDate: viewDate, mtAssessmentStatus: param }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 32) {
        $.post("SaveReport3part2", { viewDate: viewDate }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 4) {
        $.post("SaveReport4", { viewDate: viewDate }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 5) {
        $.post("SaveReport5", { viewDate: viewDate, employeeServiceNumber: param }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }


    else if (reportNumber == 6) {
        $.post("SaveReport6", { viewDate: viewDate }, function (result) {
            window.open(`/attachments/${result}`, "_blank");
        })
            .done(function () {
                popupAlert("Отчет успешно скачан!", false);
            })
            .fail(function () {
                popupAlert(result.responseJSON, false);
            })
    }
}