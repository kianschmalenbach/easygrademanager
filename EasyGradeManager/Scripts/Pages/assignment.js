const lessonField = document.getElementById("NewLessonNumber");
const groupNumberField = document.getElementById("NewGroupNumber");
const groupPasswordField = document.getElementById("NewGroupPassword");

function initPage() {
    if (lessonField !== null)
        lessonField.setAttribute("onchange", "toggle('Create')");
    if (groupNumberField !== null)
        groupNumberField.setAttribute("onchange", "toggle('Join')");
    if (groupPasswordField !== null)
        groupPasswordField.setAttribute("onchange", "toggle('Join')");
}

function toggle(mode) {
    mode = mode === "Join";
    disableButton(mode, document.getElementById("CreateGroup"));
    disableButton(!mode, document.getElementById("JoinGroup"));
    if (mode)
        lessonField.value = null;
    else {
        groupNumberField.value = null;
        groupPasswordField.value = null;
    }
}

function disableButton(disable, button) {
    if (disable)
        button.setAttribute("disabled", "disabled");
    else
        button.removeAttribute("disabled");
}
