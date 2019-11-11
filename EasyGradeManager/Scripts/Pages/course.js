function init() {
    if (typeof entityId !== "undefined") {
        showLoaders();
        fetchData("/api/Courses/" + entityId)
            .then(data => {
                fillPageWithData(data);
                if (typeof authorizedUser !== "undefined")
                    fillPageWithData(authorizedUser, "AuthorizedUser");
                hideLoaders();
            });
    }
}
