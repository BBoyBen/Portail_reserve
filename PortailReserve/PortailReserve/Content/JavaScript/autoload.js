
function afficherCacherElement(id) {
    var elem = document.getElementById(id);
    if (elem.style.display === "block")
        elem.style.display = "none";
    else
        elem.style.display = "block";
}
