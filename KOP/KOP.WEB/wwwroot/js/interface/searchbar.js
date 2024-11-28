var usersList = document.querySelectorAll('.fullname');
var searchBoxElem = document.getElementById('searchbox');
var listItems = document.querySelectorAll('.list-items')
var homeSectionElem = document.querySelector(".home-section")

//console.log(usersList)

//if (searchBoxElem != null) {
    
//    searchBoxElem.addEventListener("keyup", function (e) {
//        let searchTargetElem = document.querySelector('.searchbox').value
//        //Если сепараторов больше 1, то открыть их
//        if (selectBtn.length > 1) {
//            selectBtn.forEach(item => {
//                    item.classList.add("open");
//                })
//        }

//        if (searchBoxElem.value == "") {
//            if (selectBtn.length > 1) {
//                selectBtn.forEach(item => {
//                    item.classList.remove("open");
//                })
//            }
                    
//        }

//        filterList(e.target.value);
        
//    });
//}

function searchBoxKeyUp(elem,type) {
    let searchTargetElem = document.querySelector('.searchbox').value
    
    //Если сепараторов больше 1, то открыть их
    if (selectBtn.length > 1) {
        selectBtn.forEach(item => {
            item.classList.add("open");
        })
    }

    if (searchBoxElem.value == "") {
        if (selectBtn.length > 1) {
            selectBtn.forEach(item => {
                item.classList.remove("open");
            })
        }

    }

    filterList(elem.value, type);
}

function filterList(searchTerm, type) {
    //console.log("ENTER FUNC")
    //console.log(searchTerm +"|||" +"enter value")
    searchTerm = searchTerm.toLowerCase();
    
    usersList.forEach(option => {
        
        let label = option.innerText.toLowerCase();
        //console.log('label' + " " + label);
        
        let rowElem = option.parentNode;
        //console.log('rowElem' + " " + rowElem)
        let rowElemFlag = rowElem.getAttribute('flagHide')
            if (label.indexOf(searchTerm) != -1) {
                rowElem.classList.remove('hide_table_tr');
                rowElem.classList.add('show_table_tr');
                rowElem.setAttribute('flagHide', 'false');
            } else {


                rowElem.classList.remove('show_table_tr');
                rowElem.classList.add('hide_table_tr');
                rowElem.setAttribute('flagHide', 'true');
            }
        
        
    });
    checkElemsToHideTrs(type)
}

function checkElemsToHideTrs(type) {
    let listDivisionsMainElem = document.querySelectorAll('.list_division_users_wrapper')

    listDivisionsMainElem.forEach((list) => {
        let tableUsers = list.querySelectorAll('.table_users')

        countToHideTrs(tableUsers, type)
    })
}


function countToHideTrs(table, type) {
    let flag = false
    let countHideTables = 0
    let tableLength = table.length
    table.forEach((elem, index) => {
        
            let countAllTrs = elem.getElementsByTagName('tr')
            let countAllHideTrs = elem.querySelectorAll('.hide_table_tr')


            if (countAllTrs.length - 1 == countAllHideTrs.length) {

                if (tableLength <= 1) {
                    elem.parentNode.parentNode.style.display = 'none'
                } else {

                    elem.parentNode.style.display = 'none'

                    countHideTables++
                    if (tableLength == countHideTables) {
                        elem.parentNode.parentNode.style.display = 'none'
                    } else {
                        elem.parentNode.parentNode.style.display = ''
                    }

                }

            } else {

                if (tableLength <= 1) {
                    elem.parentNode.parentNode.style.display = ''
                } else {
                    elem.parentNode.style.display = ''
                }

            }
        
            
        })
}



