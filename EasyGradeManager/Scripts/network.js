const baseUrl = "https://" + window.location.href.split("/")[2];

function fetchData(path) {
    const url = baseUrl + path;
    return new Promise((resolve, reject) => {
        fetch(url, {
                method: 'GET',
                credentials: 'include'
            })
            .then(data => {
                if(data.status !== 200)
                    reject(handleError(data));
                return data.json();
            })
            .then(json => resolve(json))
            .catch(error => handleError(error))
    });
}

function handleError(error) {
    console.error(error);
}

function login() {
    const identifier = document.getElementById("identifier").value;
    const password = document.getElementById("password").value;
    fetch(baseUrl + "/api/Auth", {
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        method: "POST",
        redirect: "follow",
        body: "\"" + identifier + "&" + password + "\""
    }).then(response => {
        if(response.redirected)
            window.location.href = response.url;
        else if(!response.ok && response.status === 401) {
            document.getElementById("Login.status").innerText = "Invalid Identifier or Password";
            document.getElementById("password").value = "";
        }
    });
    return false;
}

function postData(type) {
    console.log("Posting " + type);
    return false;
}
