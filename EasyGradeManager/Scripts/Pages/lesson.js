function init() {
    if (typeof authorizedUser !== "undefined")
        fillPageWithData(authorizedUser, "AuthorizedUser");
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/Lessons/" + entityId)
            .then(data => {
                fillPageWithData(data);
                hideLoaders();
            });
    }
}
