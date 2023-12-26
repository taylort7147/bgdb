import React, { useEffect, useState } from 'react';
import PropTypes from "prop-types";
import { useToken } from "../../useToken";
import { Link } from 'react-router-dom';

export default function LibraryTable({ id }) {
    const { token } = useToken();
    console.log(token);
    const accessToken = token?.accessToken;
    console.log(`accessToken: ${accessToken}`);
    // fetch games and map to type
    const [libraries, setLibraries] = useState([]);
    const url = `api/library`;
    useEffect(() => {
        fetch(url, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => {
                setLibraries(data);
            });
    }, []);

    if (libraries == null) {
        return null;
    }

    return (
        <div className="library-table">
            <ul className="list-group">
                {libraries.map((library, i) =>
                    <li key={i} className="list-group-item">
                        <Link to={`/library/${library.id}`} className="link-dark">{library.id}</Link>
                    </li>
                )}
            </ul>
        </div >
    );
};
