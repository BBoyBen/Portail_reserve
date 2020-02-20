document.getElementById("typeSelect").addEventListener("change", function (target) {
    var select = document.getElementById("typeSelect").options[document.getElementById("typeSelect").selectedIndex].text;

    var elem = document.getElementById("missionOuStage");

    if (select === "Instruction" || select === "Exercice") {
        elem.style.display = "none";
    }
    else {
        elem.style.display = "block";
    }
});

document.getElementById("patracdrFile").addEventListener("change", function (target) {
    var fileName = document.getElementById("patracdrFile").files[0].name;

    document.getElementById("chosenFile").innerText = fileName;
});

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

function checkAllFields() {
    var allIsOk = true;

    var titre = document.getElementById("Event_Nom");
    var erreurTitre = document.getElementById("erreurTitre");
    erreurTitre.innerText = "";
    if (titre.value === null || titre.value.trim().length < 1) {
        titre.style.marginBottom = "10px";
        erreurTitre.innerText = "Le titre est obligatoire.";
        erreurChamps("Event_Nom");
        allIsOk = false;
    }
    else {
        champsOk("Event_Nom");
    }

    var description = document.getElementById("Event_Description");
    var erreurDes = document.getElementById("erreurDes");
    erreurDes.innerText = "";
    if (description.value === null || description.value.trim().length < 1) {
        description.style.marginBottom = "10px";
        erreurDes.innerText = "Les informations complémentaires sont obligatoires.";
        erreurChamps("Event_Description");
        allIsOk = false;
    }
    else {
        champsOk("Event_Description");
    }

    var debut = document.getElementById("Event_Debut");
    var fin = document.getElementById("Event_Fin");
    var erreurDate = document.getElementById("erreurDate");
    var dateValid = true;
    erreurDate.innerText = "";
    if (debut.value === null || debut.value.trim().length < 10) {
        debut.style.marginBottom = "10px";
        fin.style.marginBottom = "10px";
        erreurDate.innerText = "La date de début est obligatoire."
        erreurChamps("Event_Debut");
        allIsOk = false;
        dateValid = false;
    }
    else {
        champsOk("Event_Debut");
    }
    if (fin.value === null || fin.value.trim().length < 10) {
        debut.style.marginBottom = "10px";
        fin.style.marginBottom = "10px";
        erreurDate.innerText = "La date de fin est obligatoire."
        erreurChamps("Event_Fin");
        allIsOk = false;
        dateValid = false;
    }
    else {
        champsOk("Event_Fin");
    }

    if (dateValid) {
        var dateDebut = new Date(debut.value);
        var dateFin = new Date(fin.value);
        if (dateDebut - dateFin > 0) {
            debut.style.marginBottom = "10px";
            fin.style.marginBottom = "10px";
            erreurDate.innerText = "La date de début doit être antérieure à la date de fin.";
            erreurChamps("Event_Debut");
            erreurChamps("Event_Fin");
            allIsOk = false;
        }
        else {
            champsOk("Event_Debut");
            champsOk("Event_Fin");
        }
    }

    var lieu = document.getElementById("Event_Lieu");
    var erreurLieu = document.getElementById("erreurLieu");
    erreurLieu.innerText = "";
    if (lieu.value === null || lieu.value.trim().length < 1) {
        lieu.style.marginBottom = "10px";
        erreurLieu.innerText = "Le lieu est obligatoire.";
        erreurChamps("Event_Lieu");
        allIsOk = false;
    }
    else {
        champsOk("Event_Lieu");
    }

    var patracdr = document.getElementById("patracdrFile");
    var erreurPatrac = document.getElementById("erreurPatrac");
    var labelPatrac = document.getElementById("patracdrLabel");
    erreurPatrac.innerText = "";
    console.log(patracdr.files[0]);
    if (patracdr.files[0] === null || patracdr.files[0] === undefined) {
        labelPatrac.style.marginBottom = "10px";
        erreurPatrac.innerText = "Veuillez ajouter un PATRACDR";
        erreurChamps("patracdrLabel");
        allIsOk = false;
    }
    else {
        labelPatrac.style.border = "3px solid black";
        labelPatrac.style.boxShadow = "none";
    }

    if (allIsOk)
        document.getElementById("subAjoutEvent").click();
}