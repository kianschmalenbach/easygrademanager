function initPage() {

}

function findCustomDataFields(type, isArray = false, id = 0) {
    if (type !== "Group")
        return findDataFields(type, isArray, id);
    const allFields = document.querySelectorAll("*[data]");
    const dataFields = {};
    for (let i = 0; i < allFields.length; ++i) {
        const fieldData = allFields[i].getAttribute("data");
        let match = false;
        if (fieldData === "IsFinal") {
            dataFields[fieldData] = allFields[i];
            match = true;
        } else {
            const array = fieldData.split(".");
            if (array.length !== 2 || !/Task[[0-9]+]/.test(array[0]) || array[1] !== "Score")
                continue;
            dataFields[fieldData] = allFields[i];
            match = true;
        }
        if (match) {
            allFields[i].setAttribute("flag", "editField");
            allFields[i].setAttribute("previousValue", allFields[i].innerText);
        }
    }
    return dataFields;
}
