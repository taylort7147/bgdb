import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { AppContext } from '../../../AppContext';

export default Index;
export function Index() {
    const { token } = useContext(AppContext)
    const [libraries, setLibraries] = useState([]);

    useEffect(() => {
        fetch(`api/library`, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${token?.accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => setLibraries(data))
    }, []);

    if (libraries == null) {
        return null;
    }

    return (
        <div className="library-table">
            <ul className="list-group list-group-flush">
                {libraries.map((library, i) =>
                    <li key={i} className="list-group-item">
                        <Link to={`/library/${library.id}`} className="link-dark">{library.id}</Link>
                    </li>
                )}
            </ul>
        </div >
    );
};
