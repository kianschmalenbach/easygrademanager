function init() {
    if (typeof entityId !== "undefined") {
        fetchData("/api/Lessons/" + entityId)
            .then(data => {
                fillPageWithData(data);
                if (typeof authorizedUser !== "undefined")
                    fillPageWithData(authorizedUser, "AuthorizedUser");
            });
    }
}
