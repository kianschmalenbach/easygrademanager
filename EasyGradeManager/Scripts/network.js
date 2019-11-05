function fetchData(path) {
    const url = "https://" + window.location.href.split("/")[2] + path;
    return new Promise(resolve => {
        fetch(url)
            .then(data => data.json())
            .then(json => {
                resolve(json);
            });
    });
}

function post() {
    //TODO
    return success;
}

function handleError() {
    //TODO
    //potentially redirect client to home page
    return errorMessage;
}
