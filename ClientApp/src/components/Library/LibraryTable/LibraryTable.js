import React, { useContext, useEffect, useState } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';
import { AppContext } from '../../../AppContext';
import { BoardGameFilterCollapse } from '../BoardGameFilter/BoardGameFilter';
import { BoardGameCard } from '../BoardGameCard/BoardGameCard';

function filterLibrary(library, params) {
    var result = library.filter(libraryData => {
        const game = libraryData.boardGame;
        const isPresent = param => (param != null && param.length > 0);
        const testParam = (name, fn) => !isPresent(params.get(name)) || fn(JSON.parse(params.get(name)));
        return testParam("minAge", p => game.minAge >= p)
            && testParam("minWeight", p => game.averageWeight >= p)
            && testParam("maxWeight", p => game.averageWeight <= p)
            && testParam("players", p => game.minPlayers <= p && p <= game.maxPlayers);
    });
    return result;
}
export default LibraryTable;
export function LibraryTable() {
    var { libraryId } = useParams();
    const [searchParams] = useSearchParams();
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
    const getSortKey = data => data.boardGame.name;
    const compareMethod = (a, b) => {
        const keyA = getSortKey(a);
        const keyB = getSortKey(b);
        if(keyA < keyB) return -1;
        if(keyA > keyB) return 1;
        return 0;
    }
    const [filteredLibrary, setFilteredLibrary] = useState([]);
    useEffect(() => {
        if (library) {
            setFilteredLibrary(filterLibrary(library.libraryData, searchParams).sort(compareMethod));
        } else {
            setFilteredLibrary();
        }
    }, [library, searchParams]);

    if (library == null) {
        return null;
    }

    return <>
        <BoardGameFilterCollapse />
        {filteredLibrary?.map((data, i) => {
            return <div key={i} className="mb-3">
                <BoardGameCard game={data.boardGame} />
            </div>
        })}
    </>;
};
