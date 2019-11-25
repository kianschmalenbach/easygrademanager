function initPage() {
    const studentResultTableRow = document.getElementById("StudentResultTableRow");
    if (studentResultTableRow !== null && authorizedUser.Roles.includes("Teacher"))
        studentResultTableRow.remove();
    initAddAssignment();
}

function initAddAssignment() {
    const deriveCheckbox = document.getElementById("derive");
    if (deriveCheckbox !== null)
        deriveCheckbox.addEventListener("change", () => listener());
    listener();

    function listener() {
        if (deriveCheckbox.checked) {
            let elements = document.getElementById("deriveInputs").querySelectorAll("*");
            for (let i = 0; i < elements.length; ++i)
                elements[i].removeAttribute("disabled");
            elements = document.getElementById("manualInputs").querySelectorAll("*");
            for (let i = 0; i < elements.length; ++i)
                elements[i].setAttribute("disabled", "disabled");
        } else {
            let elements = document.getElementById("deriveInputs").querySelectorAll("*");
            for (let i = 0; i < elements.length; ++i)
                elements[i].setAttribute("disabled", "disabled");
            elements = document.getElementById("manualInputs").querySelectorAll("*");
            for (let i = 0; i < elements.length; ++i)
                elements[i].removeAttribute("disabled");
        }
    }
}
