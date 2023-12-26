import Dashboard from "./components/Dashboard/Dashboard";
import Preferences from "./components/Preferences/Preferences";

import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import BoardGame from "./components/BoardGame";
import { BoardGameTable } from "./components/BoardGameTable";
import UserManagement from "./components/UserManagement/UserManagement";
import LibraryTable from "./components/Library/LibraryTable";
import Library from "./components/Library/Library";


const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/dashboard',
        element: <Dashboard />
    },
    {
        path: '/preferences',
        element: <Preferences />
    },
    {
        path: '/fetch-data',
        element: <FetchData />
    },
    {
        path: "/board-game",
        element: <BoardGame boardGameId={13} />
    },
    {
        path: "/board-game-table",
        element: <BoardGameTable />
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
        path: "/admin/users",
        element: <UserManagement />
    }
];

export default AppRoutes;
