import {Header} from "../components/Header.tsx";
import {ContentBlock} from "./ContentBlock.tsx";
import {Outlet} from "react-router";

export type ContentBlockChildren = {
    children: React.ReactNode;
}

export function RootLayout() {
    return (
        <div className="flex flex-col h-screen">
            <Header/>
            <main className="flex flex-col flex-1 px-2 sm:px-8 py-2 sm:py-5">
                <ContentBlock>
                    <Outlet/>
                </ContentBlock>
            </main>
        </div>
    )
}