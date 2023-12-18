import React, { useState } from "react";
import { NavLink, Link } from "react-router-dom";
import { Row, Col, Form, Button } from "reactstrap";
import useToken from "../../useToken";

function logoutUser() {
    console.log("logoutUser");
    fetch("logout", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: "{}"
    })
        .then(data => data.json())
    window.location.href = "/";
}

export default function Logout() {
    const { removeToken } = useToken();

    const handleSubmit = async e => {
        e.preventDefault();
        removeToken();
        logoutUser();
    }
    return (
        <Form inline onSubmit={handleSubmit}>
            <Row>
                <Col>
                    <Button type="submit">Log out</Button>
                </Col>
            </Row>
        </Form>
    )
}
