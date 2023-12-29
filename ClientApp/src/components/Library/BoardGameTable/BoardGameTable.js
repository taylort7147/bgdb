import React from "react";

function renderRange(a, b) {
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

function renderLocation(game, canEdit, setLocation) {
    if (canEdit) {
        return <form><input type="text" className="form-control" placeholder="Location" value={game.location} onSubmit={setLocation} /></form>
    }
    else {
        return <span>{game.location}</span>;
    }
}

function renderImage(id) {
    
   return <img src={`/api/asset/img/${id}`} />;
}

export function HeaderRow() {
    return (
        <tr>
            <th>Name</th>
            <th>Players</th>
            <th>Play Time</th>
            <th>Complexity</th>
            <th>Location</th>
            <th>Thumbnail</th>
        </tr>
    );
}

export function DataRow({ game, canEdit }) {
    const setLocation = () => { };

    return (
        <tr>
            <td>{game.name}</td>
            <td>{renderRange(game.minPlayers, game.maxPlayers)}</td>
            <td>{renderRange(game.minPlayTimeMinutes, game.maxPlayTimeMinutes)}</td>
            <td>{game.averageWeight}</td>
            <td>{renderLocation(game, canEdit, setLocation)}</td>
            <td>{renderImage(game.thumbnailId)}</td>
        </tr>
    );
}
