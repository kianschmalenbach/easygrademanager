function fillPageWithData(data, type = "") {
    let id;
    if (data.hasOwnProperty("Id")) {
        id = data.Id;
        const linkList = document.querySelectorAll("*[link=\"" + type + "\"]");
        if (linkList.length > 0)
            for (let index = 0; index < linkList.length; ++index) {
                const element = linkList[index];
                if (element.getElementsByTagName("a").length === 0) {
                    const link = document.createElement("a");
                    const array = type.split(".");
                    link.setAttribute("href", "/" + array[array.length - 1] + "s/" + id);
                    if (element.hasAttribute("id")) {
                        const id = element.getAttribute("id");
                        element.removeAttribute("id");
                        link.setAttribute("id", id);
                    }
                    while (element.childNodes.length > 0)
                        link.appendChild(element.childNodes[0]);
                    element.appendChild(link);
                }
            }
    }
    if (type !== "")
        type += ".";
    for (const key in data) {
        const element = data[key];
        switch (typeof element) {
            case "undefined":
                continue;
            case "object":
                if (element === null)
                    continue;
                if (element.constructor === Array && element.length > 0)
                    handleArray(key.substring(0, key.length - 1), type, element);
                else
                    fillPageWithData(element, type + key);
                break;
            default:
                handleElement(key, type, element);
        }
    }
}

function handleElement(key, type, element) {
    const htmlElement = document.getElementById(type + key);
    if (htmlElement !== null)
        htmlElement.innerText = element.toString();
}

function handleArray(key, type, array) {
    const rootElement = document.getElementById(type + key);
    if (rootElement === null)
        return;
    const listElement = rootElement.parentElement;
    rootElement.remove();
    if (rootElement.hasAttribute("order"))
        array = sortArray(array);
    for (let index = 0; index < array.length; ++index) {
        const element = array[index];
        if (!element.hasOwnProperty("Id"))
            continue;
        const newElement = rootElement.cloneNode(true);
        listElement.appendChild(newElement);
        newElement.setAttribute("id", key + "[" + element.Id + "]");
        fillPageWithData(element, type + key);
        modifyChildIds(newElement, key, key + "[" + element.Id + "]");
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

    function modifyChildIds(root, oldValue, newValue) {
        const childElements = root.getElementsByTagName("*");
        for (let i = 0; i < childElements.length; ++i) {
            if (childElements[i].hasAttribute("id")) {
                const id = childElements[i].getAttribute("id").replace(oldValue, newValue);
                childElements[i].setAttribute("id", id);
            }
        }
    }
}
