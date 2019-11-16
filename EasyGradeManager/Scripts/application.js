const pageType = window.location.href.split("/")[3]
    .substring(0, window.location.href.split("/")[3].length-1);
let pageData = null;
const addButtons = [];
const editButtons = [];
const deleteButtons = [];

function fillPageWithData(data, type = "") {
    if(pageData === null && type === "")
        pageData = data;
    removeRoleSpecificElements();
    generateLinks(data, type);
    if (type !== "")
        type += ".";
    initializeButtons(type, data);
    const datalists = [];
    for (let key in data) {
        const element = data[key];
        switch (typeof element) {
            case "undefined":
                continue;
            case "object":
                if (element === null)
                    continue;
                if (element.constructor === Array)
                    datalists.push([key, element]);
                else
                    fillPageWithData(element, type + key);
                break;
            default:
                const elementList = document.querySelectorAll("*[data=\"" + type + key + "\"]");
                for (let index = 0; index < elementList.length; ++index)
                    handleElement(key, type, element, elementList[index]);
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
            if(href === "AuthorizedUser")
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
        newElement.setAttribute("data", type + key + "[" + element.Id + "]");
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
        for (let i = 0; i < childElements.length; ++i) {
            ["id", "data", "datalist", "link", "task"].forEach(attr => {
                if (childElements[i].hasAttribute(attr)) {
                    const data = childElements[i].getAttribute(attr).replace(oldValue, newValue);
                    childElements[i].setAttribute(attr, data);
                }
            });
        }
    }
}

function initializeButtons(type, data) {
    let entityType = type;
    if(/[[0-9]+].$/.test(type))
        entityType = entityType.split("[")[0] + ".";
    const addType = entityType;
    if(entityType !== "")
        entityType = entityType.substring(0, entityType.length-1);
    else
        entityType = pageType;
    let buttons = document.querySelectorAll("*[task=\"" + type + "Edit\"]");
    for(let i=0; i<buttons.length; ++i) {
        buttons[i].setAttribute("onclick",
            "editElement('" + entityType + "', " + data.Id + ", this)");
        editButtons.push(buttons[i]);
    }
    buttons = document.querySelectorAll("*[task=\"" + type + "Delete\"]");
    for(let i=0; i<buttons.length; ++i) {
        buttons[i].setAttribute("onclick",
            "deleteElement('" + entityType + "', " + data.Id + ", this)");
        deleteButtons.push(buttons[i]);
    }
    buttons = document.querySelectorAll("*[task=\"New" + addType + "Add\"]");
    for(let i=0; i<buttons.length; ++i) {
        buttons[i].setAttribute("onclick", "addElement('" + entityType + "', this)");
        addButtons.push(buttons[i]);
    }
}

function showLoaders() {
    const boxes = document.querySelectorAll("div[class=\"box\"]");
    for(let i=0; i<boxes.length; ++i) {
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
    for(let i=0; i<loaders.length; ++i)
        loaders[i].remove();
}

function addElement(type) {
    disableButtons();
    let data = null;
    switch(type) {
        case "Assignment":
            data = addAssignment();
            break;
        case "Course":
            data = addCourse();
            break;
        case "GradingScheme":
            data = addGradingScheme();
            break;
        case "Group":
            data = addGroup();
            break;
        case "Lesson":
            data = addLesson();
            break;
        case "Task":
            data = addTask();
            break;
        case "User":
            data = addUser();
            break;
    }
    if(data !== null)
        sendData("POST", type, data);
    enableButtons();
}

function editElement(type, id, button) {
    disableButtons();
    switch(type) {
        case "Assignment":
            editAssignment(id);
            break;
        case "Course":
            editCourse(id);
            break;
        case "GradingScheme":
            editGradingScheme(id);
            break;
        case "Group":
            editGroup(id);
            break;
        case "Lesson":
            editLesson(id);
            break;
        case "Task":
            editTask(id);
            break;
        case "User":
            editUser(id);
            break;
    }
    button.innerText = button.innerText.replace("Edit", "Save");
    button.setAttribute("onclick", button.getAttribute("onclick")
        .replace("edit", "save"));
    button.removeAttribute("disabled");
}

function saveElement(type, id, button) {
    disableButtons();
    let data = null;
    switch(type) {
        case "Assignment":
            data = saveAssignment(id);
            break;
        case "Course":
            data = saveCourse(id);
            break;
        case "GradingScheme":
            data = saveGradingScheme(id);
            break;
        case "Group":
            data = saveGroup(id);
            break;
        case "Lesson":
            data = saveLesson(id);
            break;
        case "Task":
            data = saveTask(id);
            break;
        case "User":
            data = saveUser(id);
            break;
    }
    if(data !== null)
        sendData("PUT", type, data, id);
    button.innerText = button.innerText.replace("Save", "Edit");
    button.setAttribute("onclick", button.getAttribute("onclick").replace("save", "edit"));
    enableButtons();
}

function deleteElement(type, id) {
    sendData("DELETE", type, {}, id);
}

function enableButtons() {
    for(let i=0; i<editButtons.length; ++i)
        editButtons[i].removeAttribute("disabled");
    for(let i=0; i<deleteButtons.length; ++i)
        deleteButtons[i].removeAttribute("disabled");
    for(let i=0; i<addButtons.length; ++i)
        addButtons[i].removeAttribute("disabled");
}

function disableButtons() {
    for(let i=0; i<editButtons.length; ++i)
        editButtons[i].setAttribute("disabled", "true");
    for(let i=0; i<deleteButtons.length; ++i)
        deleteButtons[i].setAttribute("disabled", "true");
    for(let i=0; i<addButtons.length; ++i)
        addButtons[i].setAttribute("disabled", "true");
}
