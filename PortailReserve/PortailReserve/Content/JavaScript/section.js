

function changerGrade(id) {

    var lien = document.getElementById("ajax_" + id);

    var select = document.getElementById("utilGrade_" + id);

    lien.href = lien.href + "&grade=" + select.options[select.selectedIndex].value;

    lien.click();
}

function modifierSection() {
    var boutonModif = document.getElementById("modifSection");
    var boutonValider = document.getElementById("valideModif");

    boutonModif.style.display = "none";
    boutonValider.style.display = "block";

    var listeGrade = document.getElementsByClassName("grade");
    var listeSelectGrade = document.getElementsByClassName("modifGrade");

    for (var i = 0; i < listeGrade.length; i++) {
        listeGrade[i].style.display = "none";
        listeSelectGrade[i].style.display = "inline";
    }
}

function validerModifSection() {
    var boutonModif = document.getElementById("modifSection");
    var boutonValider = document.getElementById("valideModif");

    boutonModif.style.display = "block";
    boutonValider.style.display = "none";

    var listeGrade = document.getElementsByClassName("grade");
    var listeSelectGrade = document.getElementsByClassName("modifGrade");

    for (var i = 0; i < listeGrade.length; i++) {
        listeGrade[i].style.display = "inline";
        listeSelectGrade[i].style.display = "none";
    }
}