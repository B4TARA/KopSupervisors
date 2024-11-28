var searchBox = document.getElementById("input_searchbox_compare");
if (searchBox != null) {
    searchBox.addEventListener("keyup", function (e) {
        filterList(e.target.value);
    });
}
    


function filterList(searchTerm) {
    let optionsList = document.querySelectorAll(".option");

    searchTerm = searchTerm.toLowerCase();
    optionsList.forEach(option => {
        let label = option.firstElementChild.nextElementSibling.innerText.toLowerCase();

        if (label.indexOf(searchTerm) != -1) {
            option.style.display = "block";
        } else {
            option.style.display = "none";
        }
    });
}