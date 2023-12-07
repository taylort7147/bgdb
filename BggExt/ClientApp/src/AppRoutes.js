import {FetchData} from "./components/FetchData";
import {Home} from "./components/Home";
import {BoardGameTable} from "./components/BoardGameTable";

const AppRoutes = [
    {
        index: true,
        element: <Home/>
    },
    {
        path: '/fetch-data',
        element: <FetchData/>
    },
    {
        path: "/board-game-table",
        element: <BoardGameTable/>
    }
];

export default AppRoutes;