import React, { useCallback, useContext, useEffect, useState } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';
import { AppContext } from '../../../AppContext';
import * as BoardGameFilter from '../BoardGameFilter/BoardGameFilter';
import { BoardGameCard } from '../BoardGameCard/BoardGameCard';
import { createFilter } from '../BoardGameFilter/Filter';

function getBoardGame(item) {
    return item.boardGame;
}
function getSortKey(data) {
    return data.boardGame.name;
}

function compareMethod(a, b) {
    const keyA = getSortKey(a);
    const keyB = getSortKey(b);
    if (keyA < keyB) return -1;
    if (keyA > keyB) return 1;
    return 0;
};

function getLibrary(token, libraryId, includeGameProperties) {
    return fetch(`api/library/${libraryId}?includeGames=true&includeGameProperties=${includeGameProperties}`, {
        method: 'GET',
        headers: new Headers({
            'Authorization': `Bearer ${token?.accessToken}`
        }),
    });
}

export default LibraryTable;
export function LibraryTable() {
    var { libraryId } = useParams();
    const { token } = useContext(AppContext);

    // Library
    const [library, setLibrary] = useState();
    useEffect(() => {
        getLibrary(token, libraryId, false)
            .then(response => response.json())
            .then(data => {
                setLibrary(data);
                getLibrary(token, libraryId, true)
                    .then(response => response.json())
                    .then(data => setLibrary(data));
            });
    }, [token, libraryId]);

    // // Permission
    // const [canEdit, setCanEdit] = useState();
    // useEffect(() => {
    //     fetch(`api/library/canedit?id=${libraryId}`, {
    //         method: 'GET',
    //         headers: new Headers({
    //             'Authorization': `Bearer ${token?.accessToken}`
    //         }),
    //     })
    //         .then(response => response.json())
    //         .then(data => setCanEdit(data));
    // }, [token, libraryId]);

    // Filter
    const [searchParams] = useSearchParams();
    const [filter, setFilter] = useState(createFilter(searchParams));
    const [filteredLibrary, setFilteredLibrary] = useState(library);

    const handleFilter = useCallback(filteredLibrary => {
        setFilteredLibrary(filteredLibrary.sort(compareMethod));
    }, [setFilteredLibrary]);

    if (library == null) {
        return null;
    }

    return <>
        <BoardGameFilter.FilterCollapse
            filter={filter}
            setFilter={setFilter}
            collection={library.libraryData}
            onFilter={handleFilter}
            getBoardGame={getBoardGame} />
        {filteredLibrary?.map((data, i) => {
            return <div key={i} className="mb-3">
                <BoardGameCard game={data.boardGame} />
            </div>
        })}
    </>;
};
