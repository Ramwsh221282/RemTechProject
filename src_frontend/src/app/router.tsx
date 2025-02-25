import {createBrowserRouter} from "react-router";
import {RootLayout} from "./RootLayout.tsx";
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
        ]
    },
]);