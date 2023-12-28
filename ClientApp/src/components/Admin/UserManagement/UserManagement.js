import React, { useContext, useEffect, useState } from "react";
import LibrarySyncStateSwitch from "../../Library/LibrarySyncStateSwitch/LibrarySyncStateSwitch";
import UserRoleConfiguration from "../UserRoleConfiguration/UserRoleConfiguration";
import { AppContext } from "../../../AppContext";

export default UserManagement;
export function UserManagement() {
    const { token } = useContext(AppContext);
    const [users, setUsers] = useState([]);
    useEffect(() => {
        fetch("/api/user", {
            method: "GET",
            headers: new Headers({
                'Authorization': `Bearer ${token?.accessToken}`,
            }),
        })
            .then(response => response.json())
            .then(data => setUsers(data));
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
                            <th>Enabled</th>
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
                                                ? <LibrarySyncStateSwitch libraryId={user.library.id} initialState={user.library.isEnabled} />
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
