import {createBrowserRouter} from "react-router";
import {RootLayout} from "./RootLayout.tsx";
import {AdvertisementsPage} from "../pages/AdvertisementsPage/AdvertisementsPage.tsx";
import {ParserPage} from "../pages/ParserPage/ParserPage.tsx";
import {JournalsPage} from "../pages/JournalsPage/JournalsPage.tsx";
import {ParserProfilesSection} from "../pages/ParserPage/Sections/ParserProfilesSection/ParserProfilesSection.tsx";
import {SettingsPage} from "../pages/ParserPage/SettingsPage/SettingsPage.tsx";

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
                element: <ParserPage/>,
            },
            {
                path: 'parser/profiles',
                element: <ParserProfilesSection/>
            },
            {
                path: 'parser-journals',
                element: <JournalsPage/>
            },
            {
                path: 'parser/settings',
                element: <SettingsPage/>
            }
        ]
    },
]);