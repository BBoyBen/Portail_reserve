var mdpValide = false;
var mailValide = true;
var telValide = true;
var cpValide = true;

function ableSubmit() {
    var bouton = document.getElementById("subPremierCo");

    if (mdpValide && mailValide && telValide && cpValide)
        bouton.disabled = "";
    else
        bouton.disabled = "disabled";
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

function sameMdp() {
    var mdp = document.getElementById("Mdp");
    var mdpBis = document.getElementById("MdpBis");
    var erreurMdp = document.getElementById("erreurMdp");

    if ((mdp.value != "" || mdp.value.trim().length > 0) && !(mdp.value === mdpBis.value) && mdpBis.value.trim().length > 0) {
        erreurChamps("MdpBis");
        erreurMdp.innerHTML = "Les mots de passe doivent être identique.";
        mdpValide = false;
        ableSubmit();
    }
    else if ((mdp.value === "" || mdp.value.trim().length === 0) && mdpBis.value.trim().length > 0) {
        erreurChamps("Mdp");
        erreurChamps("MdpBis");
        erreurMdp.innerHTML = "Le mot de passe ne peut pas être vide.";
        mdpValide = false;
        ableSubmit();
    }
    else if (mdp.value.trim().length === 0 || mdpBis.value.trim().length === 0) {
        champsOk("Mdp");
        champsOk("MdpBis");
        erreurMdp.innerHTML = "";
        mdpValide = false;
        ableSubmit();
    }
    else {
        champsOk("Mdp");
        champsOk("MdpBis");
        erreurMdp.innerHTML = "";
        mdpValide = true;
        ableSubmit();
    }
}

document.getElementById("Mdp").addEventListener("input", function (target) {
    sameMdp();
});

document.getElementById("MdpBis").addEventListener("input", function (target) {
    sameMdp();
});

function validateEmail(email) {
    var re = /.+@.+\..+/;
    return re.test(String(email).toLowerCase());
}


document.getElementById("Util_Email").addEventListener("input", function (target) {
    var email = document.getElementById("Util_Email");
    var erreurMail = document.getElementById("erreurMail");

    if (validateEmail(email.value)) {
        champsOk("Util_Email");
        erreurMail.innerHTML = "";
        mailValide = true;
        ableSubmit();
    }
    else {
        erreurChamps("Util_Email");
        erreurMail.innerHTML = "Mail invalide.";
        mailValide = false;
        ableSubmit();
    }
});

function validateTel(tel) {
    var re = /^(\+\d+(\s|-))?0\d(\s|-)?(\d{2}(\s|-)?){4}$/;
    return re.test(String(tel).toLowerCase());
}

document.getElementById("Util_Telephone").addEventListener("input", function (target) {
    var tel = document.getElementById("Util_Telephone");
    var erreurTel = document.getElementById("erreurTel");

    if (validateTel(tel.value)) {
        champsOk("Util_Telephone");
        erreurTel.innerHTML = "";
        telValide = true;
        ableSubmit();
    }
    else {
        erreurChamps("Util_Telephone");
        erreurTel.innerHTML = "Téléphone invalide.";
        telValide = false;
        ableSubmit();
    }
});

function validateCp(cp) {
    var re = /^(([0-8][0-9])|(9[0-5]))[0-9]{3}$/;
    return re.test(String(cp).toLowerCase());
}

document.getElementById("Adr_CodePostal").addEventListener("input", function (target) {
    var cp = document.getElementById("Adr_CodePostal");
    var erreurCp = document.getElementById("erreurCp");

    if (validateCp(cp.value)) {
        champsOk("Adr_CodePostal");
        erreurCp.innerHTML = "";
        cpValide = true;
        ableSubmit();
    }
    else {
        erreurChamps("Adr_CodePostal");
        erreurCp.innerHTML = "Code postal invalide.";
        cpValide = false;
        ableSubmit();
    }
})