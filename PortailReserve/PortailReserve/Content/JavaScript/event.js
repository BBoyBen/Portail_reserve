
document.getElementById("Dispo_TouteLaPeriode").addEventListener("change", function (target) {
    var date = document.getElementById("partieDate");
    var box = document.getElementById("Dispo_TouteLaPeriode");

    if (box.checked)
        date.style.display = "none";
    else
        date.style.display = "block";
})