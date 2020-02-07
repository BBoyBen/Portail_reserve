var mdpValide = false;
var oldMdpValide = false;

function ableSubmit() {
    var bouton = document.getElementById("subModifMdp");

    if (mdpValide && oldMdpValide)
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
    var mdp = document.getElementById("New");
    var mdpBis = document.getElementById("NewBis");
    var erreurMdp = document.getElementById("erreurMdp");

    if ((mdp.value != "" || mdp.value.trim().length > 0) && !(mdp.value === mdpBis.value) && mdpBis.value.trim().length > 0) {
        erreurChamps("NewBis");
        erreurMdp.innerHTML = "Les mots de passe doivent être identique.";
        mdpValide = false;
        ableSubmit();
    }
    else if ((mdp.value === "" || mdp.value.trim().length === 0) && mdpBis.value.trim().length > 0) {
        erreurChamps("New");
        erreurChamps("NewBis");
        erreurMdp.innerHTML = "Le mot de passe ne peut pas être vide.";
        mdpValide = false;
        ableSubmit();
    }
    else if (mdp.value.trim().length === 0 || mdpBis.value.trim().length === 0) {
        champsOk("New");
        champsOk("NewBis");
        erreurMdp.innerHTML = "";
        mdpValide = false;
        ableSubmit();
    }
    else {
        champsOk("New");
        champsOk("NewBis");
        erreurMdp.innerHTML = "";
        mdpValide = true;
        ableSubmit();
    }
}

document.getElementById("New").addEventListener("input", function (target) {
    sameMdp();
});

document.getElementById("NewBis").addEventListener("input", function (target) {
    sameMdp();
});

document.getElementById("Old").addEventListener("input", function (target) {

});