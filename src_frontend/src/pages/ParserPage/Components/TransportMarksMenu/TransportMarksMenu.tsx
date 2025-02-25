import {Card, Typography} from "@mui/material";
import {TransportMarksMenuContent} from "./TransportMarksMenuContent.tsx";
import {useSelector} from "react-redux";
import {RootParserPageState} from "../../Store/ParserPageStore.ts";
import {TransportMarksMenuBar} from "./TransportMarksMenuBar.tsx";
import {TransportMarksPaginationMemo} from "./TransportMarksPagination.tsx";

export function TransportMarksMenu() {
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);

    return (
        <Card>
            <div className={"flex flex-col h-full"}>
                <Typography sx={{padding: '5px'}} variant={"h5"}>
                    {"Бренды:"}
                </Typography>
                <TransportMarksMenuBar/>
                <TransportMarksMenuContent/>
                <TransportMarksPaginationMemo pageSize={state.fetchingPayload.size}
                                              totalCount={state.count}/>
            </div>
        </Card>
    )
}