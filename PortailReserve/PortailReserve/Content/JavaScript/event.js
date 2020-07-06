
function clickToutePeriodeAjout () {
    var date = document.getElementById("partieDate");
    var box = document.getElementById("dispoTouteLaPeriodeAjout");

    if (box.checked)
        date.style.display = "none";
    else
        date.style.display = "block";
}


function clickToutePeriodeModif () {
    var date = document.getElementById("partieDateModif");
    var box = document.getElementById("touteLaPeriodeModif");

    if (box.checked)
        date.style.display = "none";
    else
        date.style.display = "block";
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

function checkDateAjout() {
    var dateDebut = document.getElementById("Dispo_Debut");
    var dateFin = document.getElementById("Dispo_Fin");
    var check = document.getElementById("dispoTouteLaPeriodeAjout");

    var erreurDate = document.getElementById("erreurDateDispo");

    var submitButon = document.getElementById("popUpSubmit");

    if (!check.checked) {
        if (dateDebut.value == "") {
            erreurDate.innerText = "Veuillez renseigner une date de début";
            erreurChamps("Dispo_Debut");
            return;
        }
        else {
            erreurDate.innerText = "";
            champsOk("Dispo_Debut");
        }

        if (dateFin.value == "") {
            erreurDate.innerText = "Veuillez renseigner une date de fin";
            erreurChamps("Dispo_Fin");
            return;
        }
        else {
            erreurDate.innerText = "";
            champsOk("Dispo_Fin");
        }
    }

    var debut = new Date(dateDebut.value);
    var fin = new Date(dateFin.value);

    if (!check.checked) {
        if (debut - fin > 0) {
            erreurDate.innerText = "La date de début doit être anterieure à la date de fin.";
            erreurChamps("Dispo_Debut");
            erreurChamps("Dispo_Fin");
        }
        else {
            erreurDate.innerText = "";
            champsOk("Dispo_Debut");
            champsOk("Dispo_Fin");
            submitButon.click();
        }
    }
    else {
        erreurDate.innerText = "";
        champsOk("Dispo_Debut");
        champsOk("Dispo_Fin");
        submitButon.click();
    }
}

function checkDateModif() {
    var dateDebut = document.getElementById("debutModif");
    var dateFin = document.getElementById("finModif");
    var check = document.getElementById("touteLaPeriodeModif");

    var erreurDate = document.getElementById("erreurDateDispoModif");

    var submitButon = document.getElementById("popUpSubmitModif");

    if (!check.checked) {
        if (dateDebut.value == "") {
            erreurDate.innerText = "Veuillez renseigner une date de début";
            erreurChamps("debutModif");
            return;
        }
        else {
            erreurDate.innerText = "";
            champsOk("debutModif");
        }

        if (dateFin.value == "") {
            erreurDate.innerText = "Veuillez renseigner une date de fin";
            erreurChamps("finModif");
            return;
        }
        else {
            erreurDate.innerText = "";
            champsOk("finModif");
        }
    }

    var debut = new Date(dateDebut.value);
    var fin = new Date(dateFin.value);

    if (!check.checked) {
        if (debut - fin > 0) {
            erreurDate.innerText = "La date de début doit être anterieure à la date de fin.";
            erreurChamps("debutModif");
            erreurChamps("finModif");
        }
        else {
            erreurDate.innerText = "";
            champsOk("debutModif");
            champsOk("finModif");
            submitButon.click();
        }
    }
    else {
        erreurDate.innerText = "";
        champsOk("debutModif");
        champsOk("finModif");
        submitButon.click();
    }
}


function afficherCacherPartPres(idChevron, idPart) {
    var part = document.getElementById(idPart);
    var chevron = document.getElementById(idChevron);

    if (part.style.display === "block") {
        part.style.display = "none";
        chevron.style.transform = "rotate(-90deg)"
    }
    else {
        part.style.display = "block";
        chevron.style.transform = "rotate(0deg)"
    }
}

function showMessagerie() {
    var boutonAffichage = document.getElementById("boutonAfficherMessagerie");
    var messagerie = document.getElementById("messagerie");

    messagerie.classList.remove("animAFermerMessage");

    boutonAffichage.style.display = "none";
    messagerie.style.display = "block";
    messagerie.classList.add("animAfficheMessage");
}

function closeMessagerie() {
    var messagerie = document.getElementById("messagerie");

    messagerie.classList.remove("animAfficheMessage");

    messagerie.classList.add("animAFermerMessage");

    setTimeout(function () {
        var messagerie = document.getElementById("messagerie");
        var boutonAffichage = document.getElementById("boutonAfficherMessagerie");

        messagerie.style.display = "none";
        boutonAffichage.style.display = "block";
    }, 1500);
}