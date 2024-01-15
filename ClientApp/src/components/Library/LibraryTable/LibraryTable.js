import React, { useCallback, useContext, useEffect, useState } from 'react';
import { redirect, useParams, useSearchParams } from 'react-router-dom';
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
export function LibraryTable({ isEditing }) {
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

    // Permission
    if (isEditing) {
        const getCanEdit = async () => {
            return await fetch(`api/library/canedit?id=${libraryId}`, {
                method: 'GET',
                headers: new Headers({
                    'Authorization': `Bearer ${token?.accessToken}`
                }),
            })
                .then(response => response.json());
        }
        const canEdit = getCanEdit();
        if(!canEdit)
        {
            redirect(`/library/${libraryId}`);
        }
    }

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

    const lastSynchronized = library.lastSynchronized ? new Date(library.lastSynchronized).toLocaleString() : "never";
    const libraryData = library.libraryData;
    return <>
        <BoardGameFilter.FilterCollapse
            filter={filter}
            setFilter={setFilter}
            collection={library.libraryData}
            onFilter={handleFilter}
            getBoardGame={getBoardGame} />
        <div className="bgdb-library-summary">
            <h5>{`${libraryId}'s library`}</h5>
            <div>{`Last synchronized: ${lastSynchronized}`}</div>
            <div>{`Showing ${filteredLibrary?.length} of ${libraryData.length} game${libraryData.length != 1 && "s"}`}</div>
        </div>
        {filteredLibrary?.map((data, i) => {
            return <div key={i} className="mb-3">
                <BoardGameCard libraryData={data} isEditing={isEditing} />
            </div>
        })}
    </>;
};
