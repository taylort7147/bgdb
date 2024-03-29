import { useContext, useEffect } from "react";
import { AppContext } from "../../../AppContext";

function logout(accessToken) {
    return fetch("api/account/logout", {
        method: "POST",
        headers: new Headers({
            'Authorization': `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        }),
        body: "{}"
    });
}

export default Logout;
export function Logout() {
    const { token, removeToken } = useContext(AppContext);

    useEffect(() => {
        logout(token?.accessToken)
            .then(() => {
                removeToken();
                window.location.href = "/";
            });

    }, [token, removeToken]);
}
