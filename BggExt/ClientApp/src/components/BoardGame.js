import React, { useEffect, useState } from 'react';
import PropTypes from "prop-types";
import useToken from "../useToken";

// type BoardGame = {
//     id: number;
//     name: string;
//     description: string;
//     yearPublished: number;
//     minPlayers: number;
//     maxPlayers: number;
//     playingTimeMinutes: number;
//     minPlayTimeMinutes: number;
//     maxPlayTimeMinutes: number;
//     minAge: number;
//     averageWeight: number;
//     mechanics: Mechanic[];
//     categories: Category[];
//     families: Family[];
//     thumbnail?: Image;
//     image?: Image;
// };

// type Mechanic = {
//     id: number;
//     name: string;
//     boardGames: BoardGame[] | null;
// };

// type Category = {
//     id: number;
//     name: string;
//     boardGames: BoardGame[] | null;
// };

// type Family = {
//     id: number;
//     name: string;
//     boardGames: BoardGame[] | null;
// };

// type Image = {
//     id: number;
//     imageData: string;
// };
export default function BoardGame({ boardGameId }) {
    const { token, setToken } = useToken();
    console.log(token);
    const accessToken = token?.accessToken;
    console.log(`accessToken: ${accessToken}`);
    // fetch games and map to type
    const [game, setGames] = useState();
    const url = `BoardGame/${boardGameId}`;
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
                setGames(data);
            });
    }, []);

    const renderArray = (array) => {
        console.log("renderArray");
        console.log(array);
        // Filter out null or undefined entries
        const filteredArray = array.filter((item) => item);
        return filteredArray.map((item) => item.name).join(', ');
    };

    if (game == null) {
        return null;
    }
    console.log("game");
    console.log(game);
    return (
        <table className='table table-striped' aria-labelledby='tabelLabel'>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Year Published</th>
                    <th>Min Players</th>
                    <th>Max Players</th>
                    <th>Playing Time</th>
                    <th>Min Play Time</th>
                    <th>Max Play Time</th>
                    <th>Min Age</th>
                    <th>Average Weight</th>
                    <th>Mechanics</th>
                    <th>Categories</th>
                    <th>Families</th>
                </tr>
            </thead>
            <tbody>
                <tr key={game.id}>
                    <td>{game.name}</td>
                    <td>{game.yearPublished}</td>
                    <td>{game.minPlayers}</td>
                    <td>{game.maxPlayers}</td>
                    <td>{game.playingTimeMinutes}</td>
                    <td>{game.minPlayTimeMinutes}</td>
                    <td>{game.maxPlayTimeMinutes}</td>
                    <td>{game.minAge}</td>
                    <td>{game.averageWeight}</td>
                    <td>{renderArray(game.mechanics)}</td>
                    <td>{renderArray(game.categories)}</td>
                    <td>{renderArray(game.families)}</td>
                </tr>
            </tbody>
        </table>
    );
};

BoardGame.propTypes = {
    boardGameId: PropTypes.func.isRequired
};
