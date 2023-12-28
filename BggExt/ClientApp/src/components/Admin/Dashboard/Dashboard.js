import React from 'react';
import { Link } from 'react-router-dom';
import { NavLink } from 'reactstrap';

export default Dashboard;
export function Dashboard() {

    return (
        <div className="admin-dashboard">
            <h1>Admin Dashboard</h1>
            <ul className="list-group list-group-flush">
                <li className="list-group-item">
                    <NavLink tag={Link} to="/admin/users">Manage Users</NavLink>
                </li>
            </ul>
        </div>
    );
}
