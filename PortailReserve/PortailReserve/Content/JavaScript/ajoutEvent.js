document.getElementById("typeSelect").addEventListener("change", function (target) {
    var select = document.getElementById("typeSelect").options[document.getElementById("typeSelect").selectedIndex].text;

    var elem = document.getElementById("missionOuStage");

    if (select === "Instruction" || select === "Exercice") {
        elem.style.display = "none";
    }
    else {
        elem.style.display = "block";
    }
});