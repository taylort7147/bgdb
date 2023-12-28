import React from 'react';
import { Link } from 'react-router-dom';
import { Button, NavLink } from 'reactstrap';

export default function AdminDashboard() {
    
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
