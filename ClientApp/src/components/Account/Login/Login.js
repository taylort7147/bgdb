import React, { useContext, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AppContext } from "../../../AppContext";
import { ErrorList } from "../../Error/Error";
import "./Login.css";

async function loginUser(credentials) {
    return await fetch("api/account/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    });
}

export default Login;
export function Login() {
    const { setToken } = useContext(AppContext);
    const [email, setEmail] = useState();
    const [password, setPassword] = useState();
    const [error, setError] = useState([]);
    const navigate = useNavigate();

    const handleSubmit = async e => {
        e.preventDefault();
        var response = await loginUser({ email, password });
        if (response.status === 200) {
            var token = await response.json();
            if (token && token.accessToken) {
                setToken(token);
                navigate(-1);
                return;
            }
            else {
                setError("An invalid token was issued from the server");
            }
        }
        else if (response.status === 401) {
            setError("Incorrect email/password");
        }
        else {
            setError("An unexpected error occurred");
        }

    }

    return (
        <div className="login-wrapper">
            <h1>Please Log In</h1>
            <div>
                <form className="form-group" onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label>Email</label>
                        <input className="form-control" type="text" onChange={e => setEmail(e.target.value)} />
                    </div>
                    <div className="mb-3">
                        <label>Password</label>
                        <input className="form-control" type="password" onChange={e => setPassword(e.target.value)} />
                    </div>
                    <div className="mb-3">
                        <ErrorList errors={[error]} />
                    </div>
                    <div className="mb-3">
                        <button className="btn btn-primary form-control" type="submit">Submit</button>
                    </div>
                    <div className="mb-3">
                        <Link className="btn btn-secondary form-control" to="/account/_register">Register</Link>
                    </div>
                </form>
            </div>
        </div>
    )
}
