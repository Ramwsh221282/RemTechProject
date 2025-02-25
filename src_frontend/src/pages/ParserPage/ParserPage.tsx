import {Provider} from "react-redux"
import {ParserPageRoot} from "./Components/ParserPageRoot.tsx";
import {Fade} from "@mui/material";
import {ParserProfileMenu} from "./Components/ParserProfileMenu/ParserProfileMenu.tsx";
import {parserPageStore} from "./Store/ParserPageStore.ts";
import {TransportMarksMenu} from "./Components/TransportMarksMenu/TransportMarksMenu.tsx";

export const ParserPage = () => {

    return (
        <Provider store={parserPageStore}>
            <ParserPageRoot>
                <Fade in={true} timeout={500}>
                    <div className="flex flex-row w-full h-full gap-5">
                        <div className="flex flex-col overflow-auto w-full h-full">
                            <ParserProfileMenu/>
                        </div>
                        <div className="flex flex-col overflow-auto w-full h-full">
                            <TransportMarksMenu/>
                        </div>
                    </div>
                </Fade>
            </ParserPageRoot>
        </Provider>
    )
}