import React, { useContext, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AppContext } from "../../../AppContext";
import "./Login.css";

async function loginUser(credentials) {
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
    const { setToken } = useContext(AppContext);
    const [email, setEmail] = useState();
    const [password, setPassword] = useState();
    const navigate = useNavigate();

    const handleSubmit = async e => {
        e.preventDefault();
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
            <div>
                <form onSubmit={handleSubmit}>
                    <label>
                        <p>Email</p>
                        <input type="text" onChange={e => setEmail(e.target.value)} />
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
            <div>
                <Link className="text-dark" to="/account/_register">Register</Link>
            </div>
        </div>
    )
}
