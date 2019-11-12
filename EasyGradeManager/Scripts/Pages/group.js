function init() {
    if (typeof authorizedUser !== "undefined")
        fillPageWithData(authorizedUser, "AuthorizedUser");
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/Groups/" + entityId)
            .then(data => {
                fillPageWithData(data);
                hideLoaders();
            });
    }
}
