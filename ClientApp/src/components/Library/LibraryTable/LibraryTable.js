import React, { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { AppContext } from '../../../AppContext';
import * as BoardGameTable from '../BoardGameTable/BoardGameTable';

export default LibraryTable;
export function LibraryTable() {
    const { libraryId } = useParams();
    const { token } = useContext(AppContext);

    // Library
    const [library, setLibrary] = useState();
    useEffect(() => {
        fetch(`api/library/${libraryId}?includeGames=true`, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${token?.accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => setLibrary(data));
    }, []);

    // Permission
    const [canEdit, setCanEdit] = useState();
    useEffect(() => {
        fetch(`api/library/canedit?id=${libraryId}`, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${token?.accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => setCanEdit(data));
    }, []);


    if (library == null) {
        return null;
    }

    return (
        <table className="table table-striped board-game-table">
            <thead>
                <BoardGameTable.HeaderRow/>
            </thead>
            <tbody>
                {library.libraryData.map((data, i) => <BoardGameTable.DataRow key={i} game={data.boardGame} canEdit={canEdit} />)}
            </tbody>

        </table>
    );
};
