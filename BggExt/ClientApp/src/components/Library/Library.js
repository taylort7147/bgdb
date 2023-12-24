import React, { useEffect, useState } from 'react';
import Switch from "react-switch";
import PropTypes from "prop-types";
import { useToken } from "../../useToken";
import { LibrarySyncStateSwitch } from './LibrarySyncStateSwitch';

export default function Library({ libraryId }) {
    const { token } = useToken();
    const accessToken = token?.accessToken;

    const [library, setLibrary] = useState();
    const url = `library/${libraryId}`;
    console.log(url);
    useEffect(() => {
        fetch(url, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => {
                setLibrary(data);
            });
    }, []);

    if (library == null) {
        return null;
    }
    
    return (
        <div className="bggext-library-item">
            <div className="row">
                <div className="col">
                    <p>{libraryId}</p>
                </div>
                <div className="col">
                    <LibrarySyncStateSwitch libraryId={libraryId} accessToken={token?.accessToken}></LibrarySyncStateSwitch>
                </div>
            </div>
        </div>
    );
};

Library.propTypes = {
    libraryId: PropTypes.func.isRequired
};
