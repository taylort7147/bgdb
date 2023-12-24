import { useState } from "react";
const tokenName = "token";
var isTimedRefreshRunning;

function getToken() {
    const tokenString = localStorage.getItem(tokenName);
    const token = JSON.parse(tokenString);
    return token;
};

function refreshToken(token, setToken) {
    console.log("Refreshing token");
    const body = JSON.stringify({ refreshToken: token.refreshToken });
    fetch("./account/refresh", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: body
    })
        .then(response => response.json())
        .then(data => setToken(data));
}

function timedRefreshRequest(setToken, interval) {
    isTimedRefreshRunning = true;
    var token = getToken();
    if (token) {
        refreshToken(token, setToken);
        setTimeout(timedRefreshRequest, interval, setToken, interval);
    }
    else {
        isTimedRefreshRunning = false;
    }
}

function startRefreshTimer(setToken, interval) {
    if (!isTimedRefreshRunning) {
        setTimeout(timedRefreshRequest, interval, setToken, interval);
    }
}

function useToken() {
    const [token, setToken] = useState(getToken());

    const saveToken = userToken => {
        localStorage.setItem(tokenName, JSON.stringify(userToken));
        setToken(userToken);
    };

    const removeToken = () => {
        localStorage.removeItem(tokenName);
    }

    const refreshIntervalMs = 600 * 1000;
    startRefreshTimer(saveToken, refreshIntervalMs);

    return {
        setToken: saveToken,
        removeToken: removeToken,
        token
    }
}

export {
    getToken,
    useToken
}
