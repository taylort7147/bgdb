import React, { useState } from "react";
import { NavLink, Link } from "react-router-dom";
import { Row, Col, Form, Button } from "reactstrap";
import { useToken } from "../../useToken";

function logoutUser(accessToken) {
    console.log("logoutUser");
    fetch("account/logout", {
        method: "POST",
        headers: new Headers({
            'Authorization': `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        }),
        body: "{}"
    })
        .then(data => data.json())
    window.location.href = "/";
}

export default function Logout() {
    const { token, removeToken } = useToken();

    const handleSubmit = async e => {
        e.preventDefault();
        removeToken();
        logoutUser(token?.accessToken);
    }
    return (
        <Form onSubmit={handleSubmit}>
            <Row>
                <Col>
                    <Button type="submit">Log out</Button>
                </Col>
            </Row>
        </Form>
    )
}
