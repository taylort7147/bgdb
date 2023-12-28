import React, { useContext, useEffect } from "react";
import { AppContext } from "../../AppContext";

function logout(accessToken) {
    console.log("logout");
    return fetch("api/account/logout", {
        method: "POST",
        headers: new Headers({
            'Authorization': `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        }),
        body: "{}"
    });
}

export default function Logout() {
    const { token, removeToken } = useContext(AppContext);

    useEffect(() => {
        logout(token?.accessToken)
            .then(() => {
                removeToken();
                window.location.href = "/";
            });

    }, []);
}
