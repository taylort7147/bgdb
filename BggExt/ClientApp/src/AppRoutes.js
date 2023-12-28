import { Home } from "./components/Home";
import { BoardGameTable } from "./components/BoardGameTable";
import * as Library from "./components/Library/Library";
import * as Account from "./components/Account/Account";
import * as Admin from "./components/Admin/Admin";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/account/dashboard',
        element: <Account.Dashboard />
    },
    {
        path: '/account/login',
        element: <Account.Login />
    },
    {
        path: '/account/logout',
        element: <Account.Logout />
    },
    {
        path: '/account/preferences',
        element: <Account.Preferences />
    },
    {
        path: '/admin',
        element: <Admin.Dashboard />
    },
    {
        path: "/admin/users",
        element: <Admin.UserManagement />
    },
    {
        path: "/board-game-table",
        element: <BoardGameTable />
    },
    {
        path: "/library",
        element: <Library.Index />
    },
    {
        path: "/library/:libraryId",
        element: <Library.LibraryTable />
    }
];

export default AppRoutes;
