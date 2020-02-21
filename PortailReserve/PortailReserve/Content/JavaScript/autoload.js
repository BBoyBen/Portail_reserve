
function afficherCacherElement(id) {
    var elem = document.getElementById(id);
    if (elem.style.display === "block")
        elem.style.display = "none";
    else
        elem.style.display = "block";
}

function afficherElem(id) {
    var elem = document.getElementById(id);
    elem.style.display = "block";
}

function cacherElem(id) {
    var elem = document.getElementById(id);
    elem.style.display = "none";
}

function clickOn(id) {
    var lien = document.getElementById(id);
    lien.click();
}

function clickOnSupp(id) {
    var idSupp = "supp_" + id;
    var lien = document.getElementById(idSupp);
    lien.click();
}

function toutVert(classe) {
    var champs = document.getElementsByClassName(classe);
    champs.forEach(function (item) {
        item.style.boxShadow = "0px -1px 20px -5px green";
        item.style.outlineColor = "green";
        item.style.border = "1px solid green";
    });
}
