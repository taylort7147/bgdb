import React, { useEffect, useState } from "react";
import { LibrarySyncStateSwitch } from "../../Library/LibrarySyncStateSwitch";
import { useToken } from "../../../useToken";
import UserRoleConfiguration from "../../UserRoleConfiguration/UserRoleConfiguration";

export default function UserManagement() {
    var { token } = useToken();
    var accessToken = token?.accessToken;
    const [users, setUsers] = useState([]);
    useEffect(() => {
        fetch("api/user", {
            method: "GET",
            headers: new Headers({
                'Authorization': `Bearer ${accessToken}`,
            }),
        })
            .then(response => response.json())
            .then(data => {
                setUsers(data);
            });
    }, []);
    console.log(users);
    return (
        <div className="user-management-wrapper">
            <h1>User Management</h1>
            <div className="user-management-body">
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Username</th>
                            <th>Library</th>
                            <th>Enable Synchronization</th>
                            <th>Roles</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            users.map((user, i) => {
                                console.log(user);
                                return (
                                    <tr key={i}>
                                        <td>{user.userName}</td>
                                        <td>{(user.library !== null)
                                            ? user.library.id
                                            : "N/A"}</td>
                                        <td>
                                            {(user.library != null)
                                                ? <LibrarySyncStateSwitch libraryId={user.library.id} initialState={user.library.isSynchronizationEnabled} />
                                                : null}
                                        </td>
                                        <td>
                                            <UserRoleConfiguration userId={user.id} />
                                        </td>
                                    </tr>);
                            })
                        }
                    </tbody>
                </table>
            </div>
        </div>
    )
}
