import React, { useContext, useEffect, useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { AppContext } from '../AppContext';

export function NavMenu() {
    const [collapsed, setCollapsed] = useState();
    const [isLoggedIn, setIsLoggedIn] = useState();
    const [isAdmin, setIsAdmin] = useState();
    const { token } = useContext(AppContext);

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    };

    useEffect(() => {
        setIsLoggedIn(token != undefined);
    }, [token]);

    const getUserDetails = async () => {
        return fetch(`/api/account/details`,
            {
                method: "GET",
                headers: new Headers({
                    'Authorization': `Bearer ${token?.accessToken}`
                })
            }).then(response => response.json());
    };

    useEffect(() => {
        getUserDetails().then(details => {
            console.log(details);
            setIsAdmin(details.roles.includes("Administrator"));
        });
    }, [token]);


    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
                container light>
                <NavbarBrand tag={Link} to="/">BoardGameDB</NavbarBrand>
                <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                    <ul className="navbar-nav flex-grow">
                        {isAdmin &&
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/admin">Admin</NavLink>
                            </NavItem>
                        }
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/board-game-table">All Games</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/library">Libraries</NavLink>
                        </NavItem>
                        {isLoggedIn ?
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/account/logout">Logout</NavLink>
                            </NavItem>
                            :
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/account/login">Login/Register</NavLink>
                            </NavItem>

                        }
                    </ul>
                </Collapse>
            </Navbar>
        </header>
    );
}
