import React, { useContext, useState } from "react";
import PropTypes from "prop-types";
import "./Login.css";
import { useToken } from "../../../useToken";
import { useNavigate } from "react-router-dom";
import { AppContext } from "../../../AppContext";

async function loginUser(credentials) {
    console.log("loginUser");
    return fetch("api/account/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    })
        .then(data => data.json())
}

export default Login;
export function Login() {
    const {setToken} = useContext(AppContext);
    const [username, setUserName] = useState();
    const [password, setPassword] = useState();
    const navigate = useNavigate();
    
    const handleSubmit = async e => {
        e.preventDefault();
        var email = username;
        const token = await loginUser({
            email,
            password
        });
        setToken(token);
        navigate(-1);
    }

    return (
        <div className="login-wrapper">
            <h1>Please Log In</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    <p>Username</p>
                    <input type="text" onChange={e => setUserName(e.target.value)} />
                </label>
                <label>
                    <p>Password</p>
                    <input type="password" onChange={e => setPassword(e.target.value)} />
                </label>
                <div>
                    <button type="submit">Submit</button>
                </div>
            </form>
        </div>
    )
}
