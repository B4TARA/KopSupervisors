
//var indicatorStatusWrapper = document.getElementById('indicators_wrapper')

var listDivsMainWrapper = document.querySelectorAll('.list_division_users_wrapper');

listDivsMainWrapper.forEach((item) => {

    let insertDescriptionWrapper = item.querySelector('.indicators_wrapper')
    let resultObj = countStatuses(item)
    //console.log(resultObj)

    if (resultObj.resultProcess != 0) {
        let div = document.createElement('div');
        div.classList.add('indicator_wrapper')
        div.classList.add('process_status')
        div.innerHTML = `${resultObj.resultProcess}`
        insertDescriptionWrapper.appendChild(div)
    }
    if (resultObj.resultPurple != 0) {
        let div = document.createElement('div');
        div.classList.add('indicator_wrapper')
        div.classList.add('purple_status')
        div.innerHTML = `${resultObj.resultPurple}`
        insertDescriptionWrapper.appendChild(div)
    }
    if (resultObj.resultAgreement != 0) {
        let div = document.createElement('div');
        div.classList.add('indicator_wrapper')
        div.classList.add('agreement_status')
        div.innerHTML = `${resultObj.resultAgreement}`
        insertDescriptionWrapper.appendChild(div)
    }






})


function countStatuses(elem) {
    const tableUsers = elem.querySelectorAll('.table_users');

    //let processStatusCount = 0;
    let statusesObj = {}
    let arrProcessStatuses = [];
    let arrPurpleStatuses = [];
    let arrAgreementStatuses = [];

    tableUsers.forEach((item) => {
        let processStatusCount = item.querySelectorAll('.process_status').length;
        let purpleStatusCount = item.querySelectorAll('.purple_status').length;
        let agreementStatusCount = item.querySelectorAll('.agreement_status').length;

        arrProcessStatuses.push(processStatusCount)
        arrPurpleStatuses.push(purpleStatusCount)
        arrAgreementStatuses.push(agreementStatusCount)

        statusesObj.resultProcess = arrProcessStatuses.reduce((sum, current) => {
            return sum + current;
        }, 0)

        statusesObj.resultPurple = arrPurpleStatuses.reduce((sum, current) => {
            return sum + current;
        }, 0)

        statusesObj.resultAgreement = arrAgreementStatuses.reduce((sum, current) => {
            return sum + current;
        }, 0)


    })
    return statusesObj

}