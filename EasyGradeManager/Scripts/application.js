const pageType = window.location.href.split("/")[3]
    .substring(0, window.location.href.split("/")[3].length - 1);
let pageData = null;
const addButtons = [];
const editButtons = [];
const deleteButtons = [];

function init() {
    removeRoleSpecificElements();
    showModeSpecificElements("show");
    if (typeof authorizedUser !== "undefined")
        fillPageWithData(authorizedUser, "AuthorizedUser");
    else
        document.getElementById("header").remove();
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/" + pageType + "s/" + entityId)
            .then(data => {
                pageData = data;
                fillPageWithData(data);
                insertCustomElements();
                initPage();
                hideLoaders();
            });
    } else
        initPage();
}

function fillPageWithData(data, type = "") {
    if (data !== null)
        generateLinks(data, type);
    if (type !== "")
        type += ".";
    initializeButtons(type, data);
    if (data === null) {
        const showIfs = document.querySelectorAll("*[showif]");
        for (let index = 0; index < showIfs.length; ++index)
            handleShowIfs(showIfs[index], type.substring(0, type.length - 1), null);
        return;
    }
    const datalists = [];
    for (let key in data) {
        const element = data[key];
        const showIfs = document.querySelectorAll("*[showif]");
        for (let index = 0; index < showIfs.length; ++index)
            handleShowIfs(showIfs[index], type + key, element);
        switch (typeof element) {
            case "undefined":
                continue;
            case "object":
                if (element !== null && element.constructor === Array)
                    datalists.push([key, element]);
                else
                    fillPageWithData(element, type + key);
                break;
            default:
                const elementList = document.querySelectorAll("*[data=\"" + type + key + "\"]");
                for (let index = 0; index < elementList.length; ++index)
                    handleElement(key, type, element, elementList[index]);
                const inputList = document.querySelectorAll("*[autoValue=\"" + type + key + "\"]");
                for (let index = 0; index < inputList.length; ++index)
                    inputList[index].setAttribute("value", element.toString());
        }
    }
    datalists.forEach(entry => {
        const key = entry[0].substring(0, entry[0].length - 1);
        const elementList = document.querySelectorAll("*[datalist=\"" + type + key + "\"]");
        for (let index = 0; index < elementList.length; ++index)
            handleArray(key, type, entry[1], elementList[index]);
    });
}

function removeRoleSpecificElements() {
    const roleSpecificElements = document.querySelectorAll("*[roles]");
    for (let i = 0; i < roleSpecificElements.length; ++i) {
        const element = roleSpecificElements[i];
        const roles = element.getAttribute("roles").split(" ");
        let show = false;
        for (const index in roles) {
            if (authorizedUser.Roles.includes(roles[index])) {
                show = true;
                break;
            }
        }
        if (!show)
            element.remove();
        else
            element.removeAttribute("roles");
    }
}

function generateLinks(data, type) {
    if (!data.hasOwnProperty("Id"))
        return;
    const id = data.Id;
    const linkList = document.querySelectorAll("*[link=\"" + type + "\"]");
    for (let index = 0; index < linkList.length; ++index) {
        const element = linkList[index];
        if (element.getElementsByTagName("a").length === 0) {
            const link = document.createElement("a");
            const array = type.split(".");
            let href = array[array.length - 1].replace(/\[[0-9]+]/, "");
            if (href === "AuthorizedUser")
                href = "User";
            link.setAttribute("href", "/" + href + "s/" + id);
            if (element.hasAttribute("data")) {
                const id = element.getAttribute("data");
                element.removeAttribute("data");
                link.setAttribute("data", id);
            }
            while (element.childNodes.length > 0)
                link.appendChild(element.childNodes[0]);
            element.appendChild(link);
        }
    }
}

function handleElement(key, type, element, htmlElement) {
    if (htmlElement !== null) {
        if (htmlElement.tagName.toLowerCase() === "input")
            htmlElement.setAttribute("value", element.toString());
        else
            htmlElement.innerText = element.toString();
    }
}

function handleArray(key, type, array, rootElement) {
    if (rootElement === null)
        return;
    const listElement = rootElement.parentElement;
    rootElement.remove();
    if (rootElement.hasAttribute("filter"))
        array = filterArray(array, rootElement.getAttribute("filter"));
    if (array.length === 0 && rootElement.hasAttribute("removeOnEmpty")) {
        initializeButtons(type + key + "s", null);
        const toRemove = document.getElementById(rootElement.getAttribute("removeOnEmpty"));
        if (toRemove !== null)
            toRemove.remove();
    }
    if (rootElement.hasAttribute("order"))
        array = sortArray(array);
    for (let index = 0; index < array.length; ++index) {
        const element = array[index];
        if (!element.hasOwnProperty("Id"))
            continue;
        const newElement = rootElement.cloneNode(true);
        newElement.setAttribute("entity", type + key + "[" + element.Id + "]");
        newElement.removeAttribute("datalist");
        newElement.removeAttribute("order");
        newElement.removeAttribute("removeonempty");
        listElement.appendChild(newElement);
        modifyChildDataAttrs(newElement, key, key + "[" + element.Id + "]");
        fillPageWithData(element, type + key + "[" + element.Id + "]");
    }

    function filterArray(array, filter) {
        const typeClass = type.replace(/\[[0-9]+]/, "");
        if (!filter.startsWith(typeClass + key))
            return array;
        filter = filter
            .substring((typeClass + key).length + 1)
            .replace(" =", "=")
            .replace("= ", "=")
            .split("=");
        if (filter.length !== 2)
            return array;
        return array.filter(element => {
            let criterion = filter[0];
            const value = filter[1];
            while (element && criterion.split(".").length > 1) {
                const filterStep = criterion.split(".")[0];
                element = element[filterStep];
                criterion = criterion.substring(filterStep.length + 1);
            }
            return element && element.hasOwnProperty(criterion) && element[criterion].toString() === value;
        });
    }

    function sortArray(array) {
        array.sort((a, b) => {
            if (!rootElement.getAttribute("order").startsWith(type + key))
                return -1;
            const orderInfo = rootElement.getAttribute("order").substring((type + key).length + 1).split(" ");
            const descending = (orderInfo.length > 1 && orderInfo[1] === "DESC");
            const criteria = orderInfo[0].split(".");

            function sort(a, b, index = 0) {
                const criterion = criteria[index];
                if (!a.hasOwnProperty(criterion) || !b.hasOwnProperty(criterion))
                    return descending ? 1 : -1;
                if (index === criteria.length - 1)
                    return a[criterion].toString().localeCompare(b[criterion].toString());
                return sort(a[criterion], b[criterion], index + 1);
            }

            return descending ? sort(b, a) : sort(a, b);
        });
        return array;
    }

    function modifyChildDataAttrs(root, oldValue, newValue) {
        const childElements = root.getElementsByTagName("*");
        const elementList = [root];
        for (let i = 0; i < childElements.length; ++i)
            elementList.push(childElements[i]);
        for (let i = 0; i < elementList.length; ++i) {
            ["id", "data", "datalist", "link", "task", "showif"].forEach(attr => {
                if (elementList[i].hasAttribute(attr)) {
                    const data = elementList[i].getAttribute(attr).replace(oldValue, newValue);
                    elementList[i].setAttribute(attr, data);
                }
            });
        }
    }
}

function handleShowIfs(htmlElement, type, element) {
    const array = htmlElement.getAttribute("showif").split("=");
    if (array.length !== 2)
        return;
    let key = array[0].replace(" ", "");
    let equal = true;
    if (key.endsWith("!")) {
        equal = false;
        key = key.substring(0, key.length - 1);
    }
    if (key === type) {
        if (array[1] !== null)
            array[1] = array[1].replace(" ", "");
        if (element !== null)
            element = element.toString().replace(" ", "");
        else
            element = "null";
        if ((equal && array[1] !== element) || (!equal && array[1] === element))
            htmlElement.remove();
    }
}

function initializeButtons(type, data) {
    let entityType = type;
    let arrayType = false;
    if (/[[0-9]+].$/.test(type)) {
        entityType = entityType.split("[")[0] + ".";
        arrayType = true;
    }
    if (entityType !== "")
        entityType = entityType.substring(0, entityType.length - 1);
    else
        entityType = pageType;
    let addType = "";
    const array = entityType.split(".");
    for (let i = 0; i < array.length - 1; ++i)
        addType += array[i] + ".";
    addType += "New" + array[array.length - 1] + ".";
    let buttons = document.querySelectorAll("*[task=\"" + addType + "Add\"]");
    for (let i = 0; i < buttons.length; ++i) {
        const custom = buttons[i].hasAttribute("custom") && buttons[i].getAttribute("custom") === "true";
        buttons[i].setAttribute("onclick", "addElement('" + array[array.length - 1] + "'" + (custom ? ", true" : "") + ")");
        addButtons.push(buttons[i]);
    }
    if (data === null)
        return;
    buttons = document.querySelectorAll("*[task=\"" + type + "Edit\"]");
    for (let i = 0; i < buttons.length; ++i) {
        const custom = buttons[i].hasAttribute("custom") && buttons[i].getAttribute("custom") === "true";
        buttons[i].setAttribute("onclick",
            "editElement('" + entityType + "', " + data.Id + ", " + arrayType + ", this" + (custom ? ", true" : " ") + ")");
        editButtons.push(buttons[i]);
    }
    buttons = document.querySelectorAll("*[task=\"" + type + "Delete\"]");
    for (let i = 0; i < buttons.length; ++i) {
        buttons[i].setAttribute("onclick",
            "deleteElement('" + entityType + "', " + data.Id + ", this)");
        deleteButtons.push(buttons[i]);
    }
}

function insertCustomElements() {
    /*const tables = document.querySelectorAll("table");
    const tablesToModify = [];
    for(let i=0; i<tables.length; ++i) {
        const rows = tables[i].querySelectorAll("tr");
        if(rows.length <= 1)
            tablesToModify.push(tables[i]);
    }
    for(let i=0; i<tablesToModify.length; ++i) {
        const text = document.createTextNode("This table is empty.");
        const wrapper = document.createElement("div");
        wrapper.appendChild(text);
        const parent = tablesToModify[i].parentElement;
        console.log(parent);
        let index;
        for(let j=0; j<parent.children.length; ++j) {
            if(parent.children.item(j) === tablesToModify[i]) {
                index = j;
                break;
            }
        }
        for(let j=0; j<tablesToModify[i].attributes.length; ++j) {
            wrapper.setAttribute(tablesToModify[i].attributes[j].name, tablesToModify[i].attributes[j].value);
        }
        tablesToModify[i].remove();
        parent.insertBefore(wrapper, parent.children[index]);
    }*/
}

function showLoaders() {
    const boxes = document.querySelectorAll("div[class=\"box\"]");
    for (let i = 0; i < boxes.length; ++i) {
        const element = boxes[i];
        const loaderWrapper = document.createElement("div");
        loaderWrapper.setAttribute("class", "loaderWrapper");
        const loaderContainer = document.createElement("div");
        loaderContainer.setAttribute("class", "loaderContainer");
        const loader = document.createElement("div");
        loader.setAttribute("class", "loader");
        loaderContainer.appendChild(loader);
        loaderWrapper.appendChild(loaderContainer);
        element.appendChild(loaderWrapper);
    }
}

function hideLoaders() {
    const loaders = document.querySelectorAll("div[class=\"loaderWrapper\"]");
    for (let i = 0; i < loaders.length; ++i)
        loaders[i].remove();
}

function addElement(type) {
    disableButtons();
    const dataFields = findDataFields("New" + type);
    createInputFields(dataFields);
    const data = getInputData();
    if (data !== null)
        sendData("POST", type, data);
    removeInputFields();
    enableButtons();
    return false;
}

function editElement(type, id, isArray, button, customFields = false) {
    disableButtons();
    showModeSpecificElements("edit", type);
    const dataFields = customFields ? findCustomDataFields(type, isArray, id, button) : findDataFields(type, isArray, id);
    createInputFields(dataFields);
    button.innerText = button.innerText.replace("Edit", "Save");
    button.setAttribute("onclick", button.getAttribute("onclick")
        .replace("edit", "save"));
    button.removeAttribute("disabled");
}

function saveElement(type, id, isArray, button) {
    disableButtons();
    const data = getInputData(type + isArray ? ("[" + id + "]") : "");
    if (data !== null)
        sendData("PUT", type, data, id);
    removeInputFields();
    button.innerText = button.innerText.replace("Save", "Edit");
    button.setAttribute("onclick", button.getAttribute("onclick").replace("save", "edit"));
    showModeSpecificElements("show");
    enableButtons();
    return false;
}

function findDataFields(type, isArray = false, id = 0) {
    const allFields = document.querySelectorAll("*[data]");
    const dataFields = {};
    for (let i = 0; i < allFields.length; ++i) {
        const fieldData = allFields[i].getAttribute("data");
        if ((allFields[i].hasAttribute("edit") &&
            allFields[i].getAttribute("edit") === "false") ||
            allFields[i].tagName.toLowerCase() === "button")
            continue;
        let match = false;
        if (type === pageType && fieldData.split(".").length === 1) {
            dataFields[fieldData] = allFields[i];
            match = true;
        } else {
            const array = fieldData.split(".");
            let matchString = "";
            let matchType = type;
            if (isArray)
                matchType += "[" + id + "]";
            for (let j = 0; j < array.length - 1; ++j)
                matchString += array[j] + ".";
            if (matchType + "." === matchString) {
                dataFields[array[array.length - 1]] = allFields[i];
                match = true;
            }
        }
        if (match) {
            allFields[i].setAttribute("flag", "editField");
            allFields[i].setAttribute("previousValue", allFields[i].innerText);
        }
    }
    return dataFields;
}

function createInputFields(dataFields) {
    for (const field in dataFields) {
        let inputElement = dataFields[field];
        if (inputElement.tagName.toLowerCase() !== "select") {
            let inputType = "text";
            if (dataFields[field].hasAttribute("type"))
                inputType = dataFields[field].getAttribute("type");
            if (inputElement.tagName.toLowerCase() !== "input") {
                inputElement = document.createElement("input");
                if (inputType === "checkbox" && dataFields[field].innerText === "true")
                    inputElement.setAttribute("checked", "checked");
                else if (inputType === "date")
                    inputElement.setAttribute("value", "");
                inputElement.setAttribute("value", dataFields[field].innerText);
                dataFields[field].innerText = "";
                dataFields[field].appendChild(inputElement);
                inputElement.setAttribute("type", inputType);
                ["min", "max", "step", "size", "maxlength", "required"].forEach(attr => {
                    if (dataFields[field].hasAttribute(attr))
                        inputElement.setAttribute(attr, dataFields[field].getAttribute(attr));
                });
                inputElement.setAttribute("placeholder", field);
            }
        }
        inputElement.setAttribute("data", field);
    }
}

function getInputData() {
    const data = {};
    const fields = document.querySelectorAll("*[flag=\"editField\"]");
    for (let i = 0; i < fields.length; ++i) {
        let inputField = fields[i];
        const selectField = inputField;
        let select = false;
        if (inputField.tagName.toLowerCase() === "select") {
            inputField = inputField.options[inputField.selectedIndex];
            select = true;
        } else if (inputField.tagName.toLowerCase() !== "input")
            inputField = fields[i].getElementsByTagName("input")[0];
        if (inputField === undefined || inputField === null || inputField.value === "")
            continue;
        let value = select ? inputField.text : inputField.value;
        if (inputField.hasAttribute("type") && inputField.getAttribute("type").toLowerCase() === "checkbox")
            value = inputField.checked;
        const fieldName = (select ? selectField : inputField).getAttribute("data");
        const array = fieldName.split(".");
        if (array.length === 1)
            data[fieldName] = value;
        else {
            let cursor = data;
            for (let i = 0; i < array.length - 1; ++i) {
                if (/[[0-9]+]$/.test(array[i])) {
                    const innerArray = array[i].split("[");
                    const datalist = innerArray[0] + "s";
                    if (cursor[datalist] === undefined)
                        cursor[datalist] = [];
                    const id = parseInt(innerArray[1].split("]")[0]);
                    cursor[datalist].push({
                        "Id": id
                    });
                    cursor = cursor[datalist][cursor[datalist].length - 1];
                } else {
                    cursor[array[i]] = {};
                    cursor = cursor[array[i]];
                }
            }
            cursor[array[array.length - 1]] = value;
        }
    }
    return data;
}

function removeInputFields() {
    const fields = document.querySelectorAll("*[flag=\"editField\"]");
    for (let i = 0; i < fields.length; ++i) {
        const value = fields[i].getAttribute("previousValue");
        if (fields[i].getElementsByTagName("input").length > 0)
            fields[i].getElementsByTagName("input")[0].remove();
        else if (fields[i].tagName.toLowerCase() === "input")
            fields[i].value = fields[i].getAttribute("previousValue");
        fields[i].innerText = value;
        fields[i].removeAttribute("flag");
        fields[i].removeAttribute("previousValue");
    }
}

function deleteElement(type, id) {
    sendData("DELETE", type, {}, id);
}

function enableButtons() {
    for (let i = 0; i < editButtons.length; ++i)
        editButtons[i].removeAttribute("disabled");
    for (let i = 0; i < deleteButtons.length; ++i)
        deleteButtons[i].removeAttribute("disabled");
    for (let i = 0; i < addButtons.length; ++i)
        addButtons[i].removeAttribute("disabled");
}

function disableButtons() {
    for (let i = 0; i < editButtons.length; ++i)
        editButtons[i].setAttribute("disabled", "true");
    for (let i = 0; i < deleteButtons.length; ++i)
        deleteButtons[i].setAttribute("disabled", "true");
    for (let i = 0; i < addButtons.length; ++i)
        addButtons[i].setAttribute("disabled", "true");
}

function showModeSpecificElements(mode, scope = null) {
    let elements = document.querySelectorAll("*[mode]");
    if (scope !== null) {
        const filteredElements = [];
        for (let i = 0; i < elements.length; ++i) {
            if (!elements[i].hasAttribute("scope") ||
                elements[i].getAttribute("scope") === scope)
                filteredElements.push(elements[i]);
        }
        elements = filteredElements;
    }
    for (let i = 0; i < elements.length; ++i) {
        const modes = elements[i].getAttribute("mode").split(" ");
        if (modes.includes(mode))
            elements[i].removeAttribute("hidden");
        else
            elements[i].setAttribute("hidden", "hidden");
    }
}
