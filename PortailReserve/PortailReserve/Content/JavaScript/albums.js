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

$("#photoFiles").change(function () {
    updatePrevisu(this);
});