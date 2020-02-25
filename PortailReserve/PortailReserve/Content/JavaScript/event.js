
document.getElementById("Dispo_TouteLaPeriode").addEventListener("change", function (target) {
    var date = document.getElementById("partieDate");
    var box = document.getElementById("Dispo_TouteLaPeriode");

    if (box.checked)
        date.style.display = "none";
    else
        date.style.display = "block";
})


document.getElementById("touteLaPeriodeModif").addEventListener("change", function (target) {
    var date = document.getElementById("partieDateModif");
    var box = document.getElementById("touteLaPeriodeModif");

    if (box.checked)
        date.style.display = "none";
    else
        date.style.display = "block";
})