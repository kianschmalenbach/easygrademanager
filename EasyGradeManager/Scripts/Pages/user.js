function initPage() {

}

function findCustomDataFields(type, isArray = false, id = 0, button) {
    if (type !== "User" || !button.hasAttribute("id"))
        return findDataFields(type, isArray, id);
    const buttonId = button.getAttribute("id");
    let fields;
    switch (buttonId) {
        case "UserButton":
            if (document.getElementById("RoleFields") !== null)
                document.getElementById("RoleFields").setAttribute("hidden", "hidden");
            if (document.getElementById("RoleText") !== null)
                document.getElementById("RoleText").removeAttribute("hidden");
            fields = [
                document.getElementById("Identifier"),
                document.getElementById("NewPassword"),
                document.getElementById("Name"),
                document.getElementById("Email")
            ];
            break;
        case "RoleButton":
            fields = [
                document.getElementById("NewIdentifier"),
                document.getElementById("NewRole")
            ];
            break;
        default:
            return findDataFields(type, isArray, id);
    }
    const ret = {};
    for (const field in fields) {
        fields[field].setAttribute("flag", "editField");
        fields[field].setAttribute("previousValue", fields[field].innerText);
        ret[fields[field].getAttribute("data")] = fields[field];
    }
    return ret;
}
