import { useState } from "react";

export default function useToken() {
    const getToken = () => {
        console.log("getting token from local storage")
        const tokenString = localStorage.getItem("token");
        console.log(tokenString)
        const token = JSON.parse(tokenString);
        console.log(token);
        return token;
    };
    
    const [token, setToken] = useState(getToken());
    
    const saveToken = userToken => {
        console.log("saving token to local storage");
        console.log(userToken)
        localStorage.setItem("token", JSON.stringify(userToken));
        setToken(userToken);
    };

    return {
        setToken: saveToken,
        token
    }
}
