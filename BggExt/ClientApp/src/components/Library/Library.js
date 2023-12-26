import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import PropTypes from "prop-types";
import { useToken } from "../../useToken";
import { LibrarySyncStateSwitch } from './LibrarySyncStateSwitch';


export default function Library() {
    const { libraryId } = useParams();
    const { token } = useToken();
    const accessToken = token?.accessToken;

    // Library
    const [library, setLibrary] = useState();
    useEffect(() => {
        fetch(`api/library/${libraryId}?includeGames=true`, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => setLibrary(data))
    }, []);

    // Permission
    const [canEdit, setCanEdit] = useState();
    useEffect(() => {
        fetch(`api/library/canedit?id=${libraryId}`, {
            method: 'GET',
            headers: new Headers({
                'Authorization': `Bearer ${accessToken}`
            }),
        })
            .then(response => response.json())
            .then(data => setCanEdit(data));
    }, []);

    const renderRange = (a, b) => {
        a = a ?? b;
        b = b ?? a;
        if (a != null) {
            var min = Math.min(a, b);
            var max = Math.max(a, b);
            if (min == max) {
                return <span>{min}</span>;
            } else {
                return <span>{min}-{max}</span>
            }
        }
    };

    const setLocation = () => { };
    const renderLocation = (game, i) => {
        if (canEdit) {
            return <form><input type="text" className="form-control" placeholder="Location" value={game.location} onSubmit={setLocation} /></form>
        }
        else {
            return <span>{game.location}</span>;
        }
    };

    if (library == null) {
        return null;
    }

    return (
        <table className="table table-striped board-game-table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Players</th>
                    <th>Play Time</th>
                    <th>Weight</th>
                    <th>Location</th>
                </tr>
            </thead>
            <tbody>

                {library.libraryData.map((data, i) => {
                    var game = data.boardGame;
                    return <tr key={i}>
                        <td>{game.name}</td>
                        <td>{renderRange(game.minPlayers, game.maxPlayers)}</td>
                        <td>{renderRange(game.minPlayTimeMinutes, game.maxPlayTimeMinutes)}</td>
                        <td>{game.averageWeight}</td>
                        <td>{renderLocation(game, i)}</td>
                    </tr>
                })}
            </tbody>

        </table>
    );
};
