
document.getElementById("titreAVenir").onclick = function () {
    document.getElementById("titreAVenir").style.opacity = 1;
    document.getElementById("titrePasse").style.opacity = 0.5;

    document.getElementById("aVenir").style.display = "block";
    document.getElementById("passe").style.display = "none";
};

document.getElementById("titrePasse").onclick = function () {
    document.getElementById("titreAVenir").style.opacity = 0.5;
    document.getElementById("titrePasse").style.opacity = 1;

    document.getElementById("aVenir").style.display = "none";
    document.getElementById("passe").style.display = "block";
};