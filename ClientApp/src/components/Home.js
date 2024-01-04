import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Card, CardBody } from 'reactstrap';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <Card>
                    <CardBody>
                        <h1 className="card-title">BoardGameDB</h1>

                        <p className="card-text">
                            BoardGameDB is an extension to <a href="https://boardgamegeek.com">BoardGameGeek.com</a> that allows
                            you to attach extra data to your library games and easily search and filter by various game details.
                        </p>

                        <h5 className="card-title">Getting Started</h5>

                        <p className="card-text">
                            <ul>
                                <li>
                                    <Link to="/account/_register">Register</Link> to add your library.
                                    Contact the administrator after registering to enable syncing.
                                </li>
                                <li>
                                    View others' <Link to={"/library"}>libraries</Link>.
                                </li>
                            </ul>
                        </p>
                    </CardBody>
                </Card>
            </div>
        );
    }
}
