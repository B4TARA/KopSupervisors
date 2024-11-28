
var dateList = document.querySelectorAll('.date');

let sortByDateBtn = {
    content: 'Показать',
    className: 'custom-button-classname',


}

let showReportBtnHistory = {
    content: 'Скачать',
    className: 'custom-button-classname',
    onClick: (dp) => {
        let viewDate = document.querySelector('.input_datepicker').value
        postRangeDate(viewDate)
    }

}

new AirDatepicker('#input-datepicker', {
    range: true,
    multipleDatesSeparator: ' - ',
    buttons: [showReportBtnHistory, 'clear'],
});


async function postRangeDate(viewDate) {
    const response = await fetch('/History/ChooseDateInterval', {
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        method: 'POST',
        body: JSON.stringify(viewDate)

    })
    const result = await response.json();
    window.open(`../attachments/${result}`, "_blank");
}

