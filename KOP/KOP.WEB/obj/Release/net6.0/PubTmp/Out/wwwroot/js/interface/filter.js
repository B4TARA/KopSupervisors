/*FILTER BY TRS*/
var tables = document.querySelectorAll(".table_users"),
    table,
    thead,
    headers,
    i,
    j;
var dir = "desc";
//Добавление заголовков
function addFilterIcons() {
    for (i = 0; i < tables.length; i++) {
        table = tables[i];


        if (thead = table.querySelector(".table_header")) {
            headers = thead.querySelectorAll(".table_header_elem");

            for (j = 0; j < headers.length; j++) {
                headers[j].innerHTML = `<a href='#' class = "link_decoration filter_wrapper"> ${headers[j].innerText}<i class="fa-solid fa-sort filter_icon" style="color: #697388;"></i> </a>`;
            }
            let sort_asc = true;

            let type = thead.tagName;
            thead.addEventListener("click", sortTableFunction(table, sort_asc, type));

        }
    }
}



function sortTableFunction(table, sort_asc, type) {

    return function (ev) {

        if (ev.target.tagName.toLowerCase() == 'i') {
            //Для изменения состояния иконки
            ev.target.parentNode.classList.add('active');

            //Добавляем как мы будет сортировать реверс или нет
            ev.target.parentNode.classList.toggle('desc', sort_asc);
            sort_asc = ev.target.parentNode.classList.contains('desc') ? false : true;

            //console.log(ev.target)
            //Вызов функции сортировки
            if (type == 'DIV') {
                sortRowsByDIV(table, siblingIndex(ev.target.parentNode.parentNode), sort_asc, type);
            } else {
                sortRows(table, siblingIndex(ev.target.parentNode.parentNode), sort_asc, type);
            }
            
            ev.preventDefault();
        }
    };

}

//Ищем  индекс/номер столбца по которой искать
function siblingIndex(node) {
    var count = 0;
    //console.log(node)

    while (node = node.previousElementSibling) {
        count++;
    }

    return count;
}

//Сортируем столбцы
function sortRows(table, columnIndex, sort_asc, type) {
    var rows = table.querySelectorAll("tbody tr"),
            sel = "thead th:nth-child(" + (columnIndex + 1) + ")",
            sel2 = "td:nth-child(" + (columnIndex + 1) + ")",
            classList = table.querySelector(sel).classList,
            values = [],
            cls = "",
            allNum = true,
            val,
            index,
            node;
    
    if (classList) {
        if (classList.contains("date")) {
            cls = "date";
        } else if (classList.contains("number")) {
            cls = "number";
        }
    }

    for (index = 0; index < rows.length; index++) {
        node = rows[index].querySelector(sel2);
        if (cls == "date") {

            val = node.getAttribute('date');
            //console.log(val)

            if (val == '') {

                val = `00${index}-01-01`

            } else {
                yearVal = val.split('.')[2];
                monthVal = val.split('.')[1];
                dayVal = val.split('.')[0];
                val = `${yearVal}-${monthVal}-${dayVal}`
            }


        } else {
            val = ""
            //console.log(sel)
            //console.log(sel2)
            //console.log(node)
            val = node.innerText;
        }
        if (isNaN(val)) {
            allNum = false;
        } else {
            val = parseFloat(val);
        }


        values.push({ value: val, row: rows[index] });

    }



    if (cls == "" && allNum) {
        cls = "number";
    }

    if (cls == "number") {
        values.sort(sortNumberVal);
        values = values.reverse();
        if (sort_asc == true) {
            values.sort(sortNumberVal);
            //dir = "desc"
        }
        else if (sort_asc == false) {
            values.sort(sortNumberVal).reverse();
            //dir = "asc"
        }
    }

    else if (cls == "date") {
        if (sort_asc == true) {
            values.sort(sortDateVal);
            //dir = "desc"
        }
        else if (sort_asc == false) {
            values.sort(sortDateVal).reverse();
            //dir = "asc"
        }

    }

    else {
        if (sort_asc == true) {
            values.sort(sortTextVal);
        }
        else if (sort_asc == false) {
            values.sort(sortTextVal).reverse();
        }
    }
    //console.log(values)

    for (var idx = 0; idx < values.length; idx++) {
        table.querySelector("tbody").appendChild(values[idx].row);
    }
}


function sortRowsByDIV(table, columnIndex, sort_asc, type) {
    
        var rows = table.querySelectorAll(".d-tbody .d-tr"),
            sel = ".table_header .d-th:nth-child(" + (columnIndex+1) + ")",
            sel2 = ".d-td:nth-child(" + (columnIndex + 1) + ")",
            classList = table.querySelector(sel).classList,
            values = [],
            cls = "",
            allNum = true,
            val,
            index,
            node;
    

    if (classList) {
        if (classList.contains("date")) {
            cls = "date";
        } else if (classList.contains("number")) {
            cls = "number";
        }
    }

    for (index = 0; index < rows.length; index++) {
        node = rows[index].querySelector(sel2);
        if (cls == "date") {

            val = node.getAttribute('date');

            if (val == '') {

                val = `00${index}-01-01`

            } else {
                yearVal = val.split('.')[2];
                monthVal = val.split('.')[1];
                dayVal = val.split('.')[0];
                val = `${yearVal}-${monthVal}-${dayVal}`
            }
        } else {
            val = ""
            val = node.innerText;
        }

        if (isNaN(val)) {
            allNum = false;
        } else {
            val = parseFloat(val);
        }


        values.push({ value: val, row: rows[index] });
        
    }



    if (cls == "" && allNum) {
        cls = "number";
    }

    if (cls == "number") {
        values.sort(sortNumberVal);
        values = values.reverse();
        if (sort_asc == true) {
            values.sort(sortNumberVal);
            //dir = "desc"
        }
        else if (sort_asc == false) {
            values.sort(sortNumberVal).reverse();
            //dir = "asc"
        }
    }

    else if (cls == "date") {
        if (sort_asc == true) {
            values.sort(sortDateVal);
            
            //dir = "desc"
        }
        else if (sort_asc == false) {
            
            values.sort(sortDateVal).reverse();
            //dir = "asc"
        }
        console.log(values)
    }

    else {
        if (sort_asc == true) {
            values.sort(sortTextVal).reverse();
            //dir = "desc"
        }
        else if (sort_asc == false) {
            
            values.sort(sortTextVal);
            //dir = "asc"
        }
    }
    for (var idx = 0; idx < values.length; idx++) {
        table.querySelector(".d-tbody").appendChild(values[idx].row);
    }
}
function sortNumberVal(a, b) {
    return sortNumber(a.value, b.value);
}

function sortNumber(a, b) {
    return a - b;
}

function sortDateVal(a, b) {
    var dateA = new Date(a.value),
        dateB = new Date(b.value);
        //console.log(new Date(dateA))

    return sortNumber(dateA, dateB);
}

function sortTextVal(a, b) {

    var textA = (a.value + "").toUpperCase();
    var textB = (b.value + "").toUpperCase();

    if (textA < textB) {
        return -1;
    }

    if (textA > textB) {
        return 1;
    }
    //dir = "desc"

    //console.log(dir)
    return 0;


}

/*FILTER BY DATE*/

addFilterIcons()