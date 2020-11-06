
$("#fichierCours").change(function () {

    var file = document.getElementById("fichierCours").files[0];
    var fileName = file.name;
    var size = file.size;

    if (size < 4096000) {
        document.getElementById("fichierChoisi").innerText = fileName;
        document.getElementById("erreurFichierCours").innerText = "";
    }
    else {
        document.getElementById("fichierChoisi").innerText = "";
        document.getElementById("erreurFichierCours").innerText = "La taille du fichier ne doit pas depasser 4096ko.";
        document.getElementById("fichierCoursLabel").style.marginBottom = "10px";
    }
});

$("#nveauFichier").change(function () {

    var file = document.getElementById("nveauFichier").files[0];
    var fileName = file.name;
    var size = file.size;

    if (size < 4096000) {
        document.getElementById("nveauFichierChoisi").innerText = fileName;
        document.getElementById("erreurNveauFichierCours").innerText = "";
    }
    else {
        document.getElementById("nveauFichierChoisi").innerText = "";
        document.getElementById("erreurNveauFichierCours").innerText = "La taille du fichier ne doit pas depasser 4096ko.";
        document.getElementById("nveauFichierCoursLabel").style.marginBottom = "10px";
    }
});