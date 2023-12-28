import React, { useEffect, useState } from 'react';
import { getToken } from '../../useToken';
import UserRoleSwitch from '../UserRoleSwitch/UserRoleSwitch';

export function addRole(userId, role) {
    var body = {
        userId: userId,
        role: role
    };
    return fetch(`api/user/addrole`, {
        method: "POST",
        headers: new Headers({
            'Authorization': `Bearer ${getToken()?.accessToken}`,
            'Content-Type': "application/json"
        }),
        body: JSON.stringify(body)
    });
}

export function removeRole(userId, role) {
    var body = {
        userId: userId,
        role: role
    };
    return fetch(`api/user/removerole`, {
        method: "POST",
        headers: new Headers({
            'Authorization': `Bearer ${getToken()?.accessToken}`,
            'Content-Type': "application/json"
        }),
        body: JSON.stringify(body)
    })
}

export async function getAllRoles() {
    return await fetch(`api/user/roles`, {
        method: "GET",
        headers: new Headers({
            'Authorization': `Bearer ${getToken()?.accessToken}`
        })
    }).then(response => response.json());
}

export async function getUserRoles(userId) {
    var response = await fetch(`api/user/roles?userId=${userId}`, {
        method: "GET",
        headers: new Headers({
            'Authorization': `Bearer ${getToken()?.accessToken}`
        })
    });
    var result = await response.json();
    return result;
}

export default function UserRoleConfiguration({ userId }) {
    var [allRoles, setAllRoles] = useState([]);
    var [userRoles, setUserRoles] = useState([]);
    var [roleStates, setRoleStates] = useState([]);

    const updateRoleStates = (allRoles, userRoles) => {
        var states = allRoles.map(role => ({
            role: role,
            checked: userRoles.includes(role)
        }));
        setRoleStates(states);
    }

    useEffect(() => {
        getAllRoles().then(data => setAllRoles(data));
    }, []);

    useEffect(() => {
        getUserRoles(userId).then(data => setUserRoles(data));
    }, []);

    useEffect(() => {
        updateRoleStates(allRoles, userRoles);
    }, [userRoles, allRoles]);

    const handleChange = async (index, checked, role) => {
        // Before going through the network, assume the action will be 
        // successful and preemptively set the new state. This will make the UI 
        // appear more responsive. If the call is unsuccessful, the state will 
        // be updated to match the server at the end of this function.
        var newStates = roleStates.slice();
        newStates[index].checked = checked;
        setRoleStates(newStates);

        // Make the API call to update the role
        var method = checked ? addRole : removeRole;
        var response = await method(userId, role);
        if (response.status != 200) {
            console.error(response);
        }

        // Now update the state based on the server state.
        Promise.all([getAllRoles(), getUserRoles(userId)]).then(data => {
            setAllRoles(data[0]);
            setUserRoles(data[1]);
        });
    };

    return <ul className="user-role-switches list-group">
        {
            roleStates.map((roleState, i) =>
                <li key={i} className="list-group-item">
                    <UserRoleSwitch className="col-auto" role={roleState.role} onChange={checked => handleChange(i, checked, roleState.role, i)} checked={roleState.checked} />
                </li>)
        }
    </ul>;
}
