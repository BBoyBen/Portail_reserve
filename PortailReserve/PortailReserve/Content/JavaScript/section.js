
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

    var listIconeModif = document.getElementsByClassName("iconeCol");

    for (var j = 0; j < listIconeModif.length; j++) {
        listIconeModif[j].style.display = "inline";
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

    var listIconeModif = document.getElementsByClassName("iconeCol");

    for (var j = 0; j < listIconeModif.length; j++) {
        listIconeModif[j].style.display = "none";
    }
}

function messageSuccess(texte) {
    var popUp = document.getElementById("suppressionOk");
    var message = document.getElementById("texteSuccess");

    message.innerHTML = texte;

    popUp.style.display = "block";
    popUp.classList.add("messageOkKo");

    setTimeout(function () {
        var popUp = document.getElementById("suppressionOk");

        popUp.style.display = "none";
        popUp.classList.remove("messageOkKo");
    }, 5000);
}

function messageError(texte) {
    var popUp = document.getElementById("suppressionKo");
    var message = document.getElementById("texteError");

    message.innerHTML = texte;

    popUp.style.display = "block";
    popUp.classList.add("messageOkKo");

    setTimeout(function () {
        var popUp = document.getElementById("suppressionKo");

        popUp.style.display = "none";
        popUp.classList.remove("messageOkKo");
    }, 5000);
}

function changerGroupe() {

    var select = document.getElementById("selectGroupe");

    var lien = document.getElementById("lienChgmnt");

    lien.href = lien.href.split('?')[0] + "?grp=" + select.options[select.selectedIndex].value;
}