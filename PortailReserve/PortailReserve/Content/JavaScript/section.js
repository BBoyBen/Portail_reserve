

function changerGrade(id) {
    var lien = document.getElementById("ajax_" + id);
    var select = document.getElementById("utilGrade_" + id);

    lien.href = lien.href + "&grade=" + select.options[select.selectedIndex].value; 

    lien.click();
    
}