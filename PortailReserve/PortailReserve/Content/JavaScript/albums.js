function updatePrevisu(input) {

    if (input.files) {
        var divPrevisu = document.getElementById("partiePrevisu");

        // on enleve les anviens previsus
        while (divPrevisu.firstChild) {
            divPrevisu.removeChild(divPrevisu.firstChild);
        }

        var nbFiles = input.files.length;
        document.getElementById("nombrePhotos").innerText = nbFiles + " photos séléctionnées.";

        $.each(input.files, function (i, file) {
            // div de l'image
            var divImage = document.createElement("div");
            divImage.id = "divPrevisu_" + i;
            divImage.className = "col-12 col-sm-6 col-md-4 col-lg-3 selfCenter divImage";

            // image
            var image = document.createElement("img");
            image.alt = "Previsu image " + i;
            image.id = "previsu_" + i;
            image.className = "imagePrevisu";

            // ajout de la previsu
            divImage.appendChild(image);
            divPrevisu.appendChild(divImage);

            // setup de l'image
            var reader = new FileReader();
            reader.onload = function (e) {
                image.src = e.target.result;
            }
            reader.readAsDataURL(file);

        });
    }
}

$("#photos").change(function () {
    updatePrevisu(this);
});

function afficherZoom(i) {
    var partieZoom = document.getElementById("partieZoom");
    var background = document.getElementById("background");
    var photo = document.getElementsByClassName(i);

    partieZoom.style.display = "block";
    background.style.display = "block";

    photo[0].style.display = "block";
    photo[1].style.display = "block";
    photo[2].style.display = "block";
}

function fermerZoom() {
    var partieZoom = document.getElementById("partieZoom");
    var background = document.getElementById("background");
    var photos = document.getElementsByClassName("divPhotoZoom");

    partieZoom.style.display = "none";
    background.style.display = "none";
    for (var i = 0; i < photos.length; i++) {
        photos[i].style.display = "none";
    }
}

function suivant(i) {
    var photo = document.getElementsByClassName(i);

    if (i + 1 == document.getElementsByClassName('lignePhoto').length)
        var suivante = document.getElementsByClassName(0);
    else
        var suivante = document.getElementsByClassName(i+1);

    photo[0].style.display = "none";
    photo[1].style.display = "none";
    photo[2].style.display = "none";

    suivante[0].style.display = "block";
    suivante[1].style.display = "block";
    suivante[2].style.display = "block";
}

function precendant(i) {
    var photo = document.getElementsByClassName(i);

    if (i - 1 == -1)
        var precedante = document.getElementsByClassName(document.getElementsByClassName('lignePhoto').length - 1);
    else
        var precedante = document.getElementsByClassName(i - 1);

    photo[0].style.display = "none";
    photo[1].style.display = "none";
    photo[2].style.display = "none";

    precedante[0].style.display = "block";
    precedante[1].style.display = "block";
    precedante[2].style.display = "block";
}

function modifierAlbum() {
    var boutonModifier = document.getElementById("boutonModifier");
    var boutonTerminer = document.getElementById("boutonTerminer");
    var listeClassique = document.getElementById("listePhotos");
    var listeSupp = document.getElementById("listePhotosSupp");

    boutonModifier.style.display = "none";
    listeClassique.style.display = "none";

    boutonTerminer.style.display = "block";
    listeSupp.style.display = "block";
}

function terminerAlbum() {
    var boutonModifier = document.getElementById("boutonModifier");
    var boutonTerminer = document.getElementById("boutonTerminer");
    var listeClassique = document.getElementById("listePhotos");
    var listeSupp = document.getElementById("listePhotosSupp");

    boutonModifier.style.display = "block";
    listeClassique.style.display = "block";

    boutonTerminer.style.display = "none";
    listeSupp.style.display = "none";
}

function selecPhoto(index) {

    var checkBox = document.getElementById("check_" + index);
    var image = document.getElementById("image_" + index);
    var nbPhotoSelec = document.getElementById("inputNbPhotoSelec");
    var messagePhotoSelec = document.getElementById("nbPhotosSelec");
    var nbPopUp = document.getElementById("nbPhotosPopUp");

    var valueNbSelec = parseInt(messagePhotoSelec.innerHTML);

    if (checkBox.checked == true) {
        image.classList.remove("selecPourSupp");
        checkBox.checked = false;

        valueNbSelec--;
    }
    else {
        image.classList.add("selecPourSupp");
        checkBox.checked = true;

        valueNbSelec++;
    }

    nbPhotoSelec.value = valueNbSelec;
    messagePhotoSelec.innerHTML = valueNbSelec;
    nbPopUp.innerHTML = valueNbSelec;
}