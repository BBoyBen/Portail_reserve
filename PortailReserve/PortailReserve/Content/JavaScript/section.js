
function modifierSection() {
    var boutonModif = document.getElementById("modifSection");
    var boutonValider = document.getElementById("valideModif");
    var boutonAjouter = document.getElementById("boutonAjouter");

    boutonModif.style.display = "none";
    boutonValider.style.display = "block";
    boutonAjouter.style.display = "block";

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
    var boutonAjouter = document.getElementById("boutonAjouter");

    boutonModif.style.display = "block";
    boutonValider.style.display = "none";
    boutonAjouter.style.display = "none";

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

function clickSurCreer() {

    var check = document.getElementById("creerPersonnel");
    var partieCreation = document.getElementById("creation");

    if (check.checked)
        partieCreation.style.display = "block";
    else
        partieCreation.style.display = "none";
}

function clickSurCreerCdg() {

    var check = document.getElementById("creerCdg");
    var partieCreation = document.getElementById("creationCdg");

    if (check.checked)
        partieCreation.style.display = "block";
    else
        partieCreation.style.display = "none";
}

function erreurChamps(id) {
    var champs = document.getElementById(id);

    champs.style.boxShadow = "0px -1px 20px -5px red";
    champs.style.outlineColor = "red";
    champs.style.border = "1px solid red";
}

function champsOk(id) {
    var champs = document.getElementById(id);

    champs.style.boxShadow = "none";
    champs.style.outlineColor = "darkgrey";
    champs.style.border = "1px solid lightgrey";
}

function validerAjoutPersonnel(confirmation) {

    var allOk = true;
    var erreurAjout = document.getElementById("erreurAjout");

    var checkBox = document.getElementById("creerPersonnel");
    if (!checkBox.checked) {
        var selectGroupe = document.getElementById("personnelExistant");
        if (selectGroupe.options[selectGroupe.selectedIndex].value === "00000000-0000-0000-0000-000000000000") {
            allOk = false;
            erreurChamps("personnelExistant");
        }
        else {
            champsOk("personnelExistant");
        }
    }
    else {
        champsOk("personnelExistant");

        var nom = document.getElementById("nomPersonne");
        if (nom.value.trim().length === 0) {
            allOk = false;
            erreurChamps("nomPersonne");
        }
        else {
            champsOk("nomPersonne");
        }

        var prenom = document.getElementById("prenomPersonne");
        if (prenom.value.trim().length === 0) {
            allOk = false;
            erreurChamps("prenomPersonne");
        }
        else {
            champsOk("prenomPersonne");
        }

        var matricule = document.getElementById("matriculePersonne");
        var validMatricule = /[0-9]{10}/;
        if (validMatricule.test(matricule.value) && matricule.value.trim().length === 10) {
            champsOk("matriculePersonne");
        }
        else {
            allOk = false;
            erreurChamps("matriculePersonne");
        }

        var dateNaissance = document.getElementById("naissancePersonne");
        if (dateNaissance.value.trim().length != 10) {
            allOk = false;
            erreurChamps("naissancePersonne");
        }
        else {
            champsOk("naissancePersonne");
        }

        var mail = document.getElementById("mailPersonne");
        var validMail = /.+@.+\..+/;
        if (validMail.test(mail.value)) {
            champsOk("mailPersonne")
        }
        else {
            allOk = false;
            erreurChamps("mailPersonne")
        }
    }
    if (allOk) {
        erreurAjout.style.display = "none";
        if (checkBox.checked) {
            if (confirmation) {
                document.getElementById("lienAjoutPersonnel").click();
            }
            else {
                document.getElementById("partiePreConfirme").style.display = "none";
                document.getElementById("confirmationAjout").style.display = "block";
            }
        }
        else {
            document.getElementById("lienAjoutPersonnel").click();
        }
    }
    else {
        erreurAjout.style.display = "block";
    }
}

function validerAjoutCdg(confirmation) {

    var allOk = true;
    var erreurAjout = document.getElementById("erreurAjoutCdg");

    var checkBox = document.getElementById("creerCdg");
    if (!checkBox.checked) {
        var selectGroupe = document.getElementById("cdgExistant");
        if (selectGroupe.options[selectGroupe.selectedIndex].value === "00000000-0000-0000-0000-000000000000") {
            allOk = false;
            erreurChamps("cdgExistant");
        }
        else {
            champsOk("cdgExistant");
        }
    }
    else {
        champsOk("cdgExistant");

        var nom = document.getElementById("nomCdg");
        if (nom.value.trim().length === 0) {
            allOk = false;
            erreurChamps("nomCdg");
        }
        else {
            champsOk("nomCdg");
        }

        var prenom = document.getElementById("prenomCdg");
        if (prenom.value.trim().length === 0) {
            allOk = false;
            erreurChamps("prenomCdg");
        }
        else {
            champsOk("prenomCdg");
        }

        var matricule = document.getElementById("matriculeCdg");
        var validMatricule = /[0-9]{10}/;
        if (validMatricule.test(matricule.value) && matricule.value.trim().length === 10) {
            champsOk("matriculeCdg");
        }
        else {
            allOk = false;
            erreurChamps("matriculeCdg");
        }

        var dateNaissance = document.getElementById("naissanceCdg");
        if (dateNaissance.value.trim().length != 10) {
            allOk = false;
            erreurChamps("naissanceCdg");
        }
        else {
            champsOk("naissanceCdg");
        }

        var mail = document.getElementById("mailCdg");
        var validMail = /.+@.+\..+/;
        if (validMail.test(mail.value)) {
            champsOk("mailCdg")
        }
        else {
            allOk = false;
            erreurChamps("mailCdg")
        }
    }
    if (allOk) {
        erreurAjout.style.display = "none";
        if (checkBox.checked) {
            if (confirmation) {
                document.getElementById("lienAjoutCdg").click();
            }
            else {
                document.getElementById("partiePreConfirmeCdg").style.display = "none";
                document.getElementById("confirmationAjoutCdg").style.display = "block";
            }
        }
        else {
            document.getElementById("lienAjoutCdg").click();
        }
    }
    else {
        erreurAjout.style.display = "block";
    }
}