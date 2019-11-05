function fillPageWithData(data, type="") {
    const prefix = type.substring(0, 1);
    let id;
    for (const key in data) {
        const element = data[key];
        switch (typeof element) {
            case "undefined":
                return;
            case "object":
                if (element === null)
                    return;
                if (element.constructor === Array && element.length > 0) {
                    const list = document.getElementById(key);
                    if (list === null)
                        continue;
                    let cols = [];
                    if (list.getAttribute("cols") !== null) {
                        cols = list.getAttribute("cols").split(" ");
                    }
                    else {
                        for (const tableHead in element[0]) {
                            if (typeof element[0][tableHead] !== "object" && !tableHead.endsWith("Id")) {
                                cols.push(tableHead);
                            }
                        }
                    }
                    const tableRow = document.createElement("tr");
                    list.appendChild(tableRow);
                    for (const col in cols) {
                        const tableCell = document.createElement("th");
                        tableCell.innerText = cols[col];
                        tableRow.appendChild(tableCell);
                    }
                    for (const entry in element) {
                        const tableRow = document.createElement("tr");
                        list.appendChild(tableRow);
                        const dataRow = element[entry];
                        for (const col in cols) {
                            const dataCell = cols[col];
                            const tableCell = document.createElement("td");
                            tableRow.appendChild(tableCell);
                            if (dataRow[dataCell] !== undefined && typeof dataRow[dataCell] !== "object" && !dataCell.endsWith("Id")) {
                                if (list.getAttribute("link") !== undefined && dataCell === list.getAttribute("link")) {
                                    const link = document.createElement("a");
                                    link.setAttribute("href", "/" + key + "/" + dataRow.Id);
                                    link.innerText = dataRow[dataCell];
                                    tableCell.appendChild(link);
                                }
                                else {
                                    tableCell.innerText = dataRow[dataCell];
                                }
                            }
                        }
                    }
                }
                else {
                    fillPageWithData(element, key);
                }
                break;
            default:
                if (key === "Id")
                    id = element;
                const htmlElement = document.getElementById(prefix + key);
                if (htmlElement !== null)
                    htmlElement.innerText = element;
        }
    }
    if (prefix !== "" && document.getElementById(prefix + "Link") !== null) {
        document.getElementById(prefix + "Link").setAttribute("href", "/" + type + "s/" + id);
    }
}
