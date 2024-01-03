import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import * as Error from "../../Error/Error";
import "./Register.css";

async function registerUser(credentials) {
    return fetch("api/account/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    });
}

function renderErrors(errors, key) {
    if (errors !== undefined && errors[key] !== undefined) {
        return <Error.ErrorList errors={errors[key]} />
    }
}


export default Register;
export function Register() {
    const [email, setEmail] = useState();
    const [boardGameGeekUsername, setBoardGameGeekUsername] = useState();
    const [password, setPassword] = useState();
    const [passwordConfirm, setPasswordConfirm] = useState();
    const [errors, setErrors] = useState();
    const navigate = useNavigate();

    const handleSubmit = async e => {
        e.preventDefault();
        var userName = email;
        var response = await registerUser({
            email,
            userName,
            boardGameGeekUsername,
            password,
            passwordConfirm
        });
        if (response.status === 201) {
            navigate("/account/login");
        }
        var result = await response.json();
        setErrors(result.errors);
    }

    return (
        <div className="register-wrapper">
            <h1>Register</h1>
            <div>
                {errors && renderErrors(errors, "")}
                <form className="form-group" onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label>Email</label>
                        <input className="form-control" type="text" onChange={e => setEmail(e.target.value)} />
                        {renderErrors(errors, "email")}
                    </div>

                    <div className="mb-3">
                        <label>BoardGameGeek Username</label>
                        <input className="form-control" type="text" onChange={e => setBoardGameGeekUsername(e.target.value)} />
                        {renderErrors(errors, "boardGameGeekUsername")}
                    </div>

                    <div className="mb-3">
                        <label>Password</label>
                        <input className="form-control" type="password" onChange={e => setPassword(e.target.value)} />
                        {renderErrors(errors, "passwordConfirm")}
                    </div>

                    <div className="mb-3">
                        <label>Confirm Password</label>
                        <input className={`form-control ${(password === passwordConfirm) ? "check-mark-success" : ""}`}
                            type="password" onChange={e => setPasswordConfirm(e.target.value)} />
                        {renderErrors(errors, "passwordConfirm")}
                    </div>
                    <div>
                        <button className="btn btn-primary form-control" type="submit">Submit</button>
                    </div>
                </form>
            </div>
        </div>
    )
}
