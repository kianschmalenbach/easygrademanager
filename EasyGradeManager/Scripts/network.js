const baseUrl = "https://" + window.location.href.split("/")[2];

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
        if (response.redirected)
            window.location.href = response.url;
        else if (!response.ok && response.status === 401) {
            document.getElementById("Login.status").innerText = "Invalid Identifier or Password";
            document.getElementById("password").value = "";
        }
    });
    return false;
}

function fetchData(path) {
    const url = baseUrl + path;
    return new Promise((resolve, reject) => {
        fetch(url, {
            method: 'GET',
            credentials: 'include'
        })
            .then(data => {
                if (data.status !== 200)
                    reject(handleError(data));
                return data.json();
            })
            .then(json => resolve(json))
            .catch(error => handleError(error))
    });
}

function sendData(method, type, data, id = 0) {
    const url = baseUrl + "/api/" + type + "s" + (id > 0 ? "/" + id : "");
    console.log(method + " " + url);
    console.log(data);
    fetch(url, {
        method: method,
        body: JSON.stringify(data),
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(data => {
            switch (data.status) {
                case 200:
                    window.location.href = data.url;
                    break;
                case 201:
                    alert("The account has been created successfully. Please log in now.");
                    break;
                default:
                    handleError(data);
            }
        });
    return false;
}

function handleError(error) {
    switch (error.status) {
        case 401:
            alert("The last request was not performed because you do not have the rights to perform this action.");
            break;
        default:
            alert("The last request was not performed because of the following error:\n\n" + error.statusText);
    }
}
