import {createBrowserRouter} from "react-router";
import {RootLayout} from "./RootLayout.tsx";
import {TicTacToe} from "../pages/TicTacToePage/TicTacToe.tsx";
import {AdvertisementsPage} from "../pages/AdvertisementsPage/AdvertisementsPage.tsx";
import {ParserPage} from "../pages/ParserPage/ParserPage.tsx";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <RootLayout/>,
        children: [
            {
                path: "advertisements",
                element: <AdvertisementsPage/>
            },
            {
                path: "parser",
                element: <ParserPage></ParserPage>
            },
            {
                path: "tic-tac-toe",
                element: <TicTacToe/>
            }
        ]
    },
]);