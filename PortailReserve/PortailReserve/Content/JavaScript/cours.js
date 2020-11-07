
$("#fichierCours").change(function () {

    var file = document.getElementById("fichierCours").files[0];
    var fileName = file.name;
    var size = file.size;

    var boutonAjouter = document.getElementById("valideAjoutCours");

    if (size / 1024 < 2097152) {
        document.getElementById("fichierChoisi").innerText = fileName;
        document.getElementById("erreurFichierCours").innerText = "";
        boutonAjouter.disabled = "";
    }
    else {
        document.getElementById("fichierChoisi").innerText = "";
        document.getElementById("erreurFichierCours").innerText = "La taille du fichier ne doit pas depasser 2Go.";
        document.getElementById("fichierCoursLabel").style.marginBottom = "10px";
        boutonAjouter.disabled = "disabled";
    }
});

$("#nveauFichier").change(function () {

    var file = document.getElementById("nveauFichier").files[0];
    var fileName = file.name;
    var size = file.size;

    var boutonModifier = document.getElementById("valideModifCours");

    if (size / 1024 < 2097152) {
        document.getElementById("nveauFichierChoisi").innerText = fileName;
        document.getElementById("erreurNveauFichierCours").innerText = "";
        boutonModifier.disabled = "";
    }
    else {
        document.getElementById("nveauFichierChoisi").innerText = "";
        document.getElementById("erreurNveauFichierCours").innerText = "La taille du fichier ne doit pas depasser 2Go.";
        document.getElementById("nveauFichierCoursLabel").style.marginBottom = "10px";
        boutonModifier.disabled = "disabled";
    }
});

function afficherCacherListeCours(id) {
    var liste = document.getElementById(id);

    var nbCours = liste.getElementsByClassName("ligneCours").length;
    var taillePartCours = 5 + (5 * nbCours);

    //partie liste de cours non visible
    if (liste.style.maxHeight == "0rem") {
        liste.style.transition = "2s max-height 0.1s";
        liste.style.maxHeight = taillePartCours + "rem";
    }
    else {
        liste.style.transition = "2s max-height 0.1s";
        liste.style.maxHeight = "0rem";
    }
}