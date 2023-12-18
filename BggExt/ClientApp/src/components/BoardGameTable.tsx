import React, { useEffect, useState } from 'react';

type BoardGame = {
    id: number;
    name: string;
    description: string;
    yearPublished: number;
    minPlayers: number;
    maxPlayers: number;
    playingTimeMinutes: number;
    minPlayTimeMinutes: number;
    maxPlayTimeMinutes: number;
    minAge: number;
    averageWeight: number;
    mechanics: Mechanic[];
    categories: Category[];
    families: Family[];
    thumbnail?: Image;
    image?: Image;
};

type Mechanic = {
    id: number;
    name: string;
    boardGames: BoardGame[] | null;
};

type Category = {
    id: number;
    name: string;
    boardGames: BoardGame[] | null;
};

type Family = {
    id: number;
    name: string;
    boardGames: BoardGame[] | null;
};

type Image = {
    id: number;
    imageData: string;
};

export const BoardGameTable = () => {
    // fetch games and map to type
    const [games, setGames] = useState<BoardGame[]>([]);
    useEffect(() => {
        fetch('BoardGame')
            .then(response => response.json())
            .then(data => {
                setGames(data);
            });
    }, []);

    const renderArray = (array: any) => {
        // Filter out null or undefined entries
        const filteredArray = array.filter((item: any) => item);
        return filteredArray.map((item: any) => item.name).join(', ');
    };

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
                {games.map((game, i) => (
                    <tr key={i}>
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
                    </tr>))}
            </tbody>
        </table>
    );
};
