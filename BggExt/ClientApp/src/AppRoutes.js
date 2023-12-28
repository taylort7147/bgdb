import Dashboard from "./components/Dashboard/Dashboard";
import Preferences from "./components/Preferences/Preferences";

import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import BoardGame from "./components/BoardGame";
import { BoardGameTable } from "./components/BoardGameTable";
import UserManagement from "./components/Admin/UserManagement/UserManagement";
import LibraryTable from "./components/Library/LibraryTable";
import Library from "./components/Library/Library";
import Logout from "./components/Logout/Logout";
import Login from "./components/Login/Login";
import AdminDashboard from "./components/Admin/AdminDashboard/AdminDashboard";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/account/login',
        element: <Login />
    },
    {
        path: '/account/logout',
        element: <Logout />
    },
    {
        path: '/admin',
        element: <AdminDashboard />
    },
    {
        path: "/admin/users",
        element: <UserManagement />
    },
    {
        path: "/board-game-table",
        element: <BoardGameTable />
    },
    {
        path: '/dashboard',
        element: <Dashboard />
    },
    {
        path: "/library",
        element: <LibraryTable />
    },
    {
        path: "/library/:libraryId",
        element: <Library />
    },
    {
        path: '/preferences',
        element: <Preferences />
    }
];

export default AppRoutes;
