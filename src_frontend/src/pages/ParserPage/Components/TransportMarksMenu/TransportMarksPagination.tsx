import {Pagination} from "@mui/material";
import {ChangeEvent, memo, useState} from "react";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";

type Props = {
    totalCount: number;
    pageSize: number;
}

function TransportMarksPagination({totalCount, pageSize}: Props) {
    const pagesCount = Math.ceil(totalCount / pageSize);
    const [page, setPage] = useState(totalCount === 0 ? 0 : 1);
    const actions = parserPageTransportTypesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);

    function handlePageChange(_: ChangeEvent<unknown>, newPage: number) {
        if (page === newPage) return;
        setPage(newPage);
        const newPayload = {...state.fetchingPayload, page: newPage};
        dispatch(actions.setFetchingPayload(newPayload))
        dispatch(actions.fetchTypesAsync(newPayload))
    }

    return (
        <>
            <Pagination
                sx={{padding: '12.5px'}}
                page={page}
                count={pagesCount}
                onChange={handlePageChange}
                color={"standard"}/>
        </>
    )
}

export const TransportMarksPaginationMemo = memo(TransportMarksPagination);