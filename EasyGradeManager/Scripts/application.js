function fillPageWithData(data, type = "") {
    generateLinks(data, type);
    if (type !== "")
        type += ".";
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
            const href = array[array.length - 1].replace(/\[[0-9]+]/, "");
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
            ["data", "datalist", "link", "task"].forEach(attr => {
                if (childElements[i].hasAttribute(attr)) {
                    const data = childElements[i].getAttribute(attr).replace(oldValue, newValue);
                    childElements[i].setAttribute(attr, data);
                }
            });
        }
    }
}
