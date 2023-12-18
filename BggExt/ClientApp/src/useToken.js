import { useState } from "react";


export default function useToken() {
    const tokenName = "token";

    const getToken = () => {
        const tokenString = localStorage.getItem(tokenName);
        console.log(tokenString)
        const token = JSON.parse(tokenString);
        console.log(token);
        return token;
    };
    
    const [token, setToken] = useState(getToken());
    
    const saveToken = userToken => {
        console.log(userToken)
        localStorage.setItem(tokenName, JSON.stringify(userToken));
        setToken(userToken);
    };

    const removeToken = () => {
        localStorage.removeItem(tokenName);
    }

    return {
        setToken: saveToken,
        removeToken: removeToken,
        token
    }
}
