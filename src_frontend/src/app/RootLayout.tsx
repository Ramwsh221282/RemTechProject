import {Header} from "../components/Header.tsx";
import {Outlet} from "react-router";

export function RootLayout() {
    return (
        <div>
            <Header/>
            <Outlet/>
        </div>
    )
}