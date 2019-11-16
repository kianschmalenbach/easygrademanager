function init() {
    if (typeof authorizedUser !== "undefined")
        fillPageWithData(authorizedUser, "AuthorizedUser");
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/Courses/" + entityId)
            .then(data => {
                fillPageWithData(data);
                hideLoaders();
            });
    }
}

function addAssignment() {
    console.log("Adding Assignment");
}

function editAssignment(id) {
    console.log("Editing Assignment " + id);
}

function editCourse(id) {
    console.log("Editing Course " + id);
}

function editGradingScheme(id) {
    console.log("Editing GradingScheme " + id);
}

function saveAssignment(id) {
    console.log("Saving Assignment " + id);
    return {};
}

function saveCourse(id) {
    console.log("Saving Course " + id);
    return {};
}

function saveGradingScheme(id) {
    console.log("Saving GradingScheme " + id);
    return {};
}
