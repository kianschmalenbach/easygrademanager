function init() {
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/Lessons/" + entityId)
            .then(data => {
                fillPageWithData(data);
                if (typeof authorizedUser !== "undefined")
                    fillPageWithData(authorizedUser, "AuthorizedUser");
                hideLoaders();
            });
    }
}
