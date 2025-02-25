import {Pagination} from "@mui/material";
import {ChangeEvent, memo, useCallback, useEffect, useMemo, useRef, useState} from "react";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";

type Props = {
    totalCount: number;
    pageSize: number;
}

function TransportMarksPagination({totalCount, pageSize}: Props) {
    const [page, setPage] = useState(totalCount === 0 ? 0 : 1);
    const pagesCount = useMemo(() => Math.ceil(totalCount / pageSize), [totalCount, pageSize])
    const isFirstRender = useRef(true);
    const fetchPayload = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer.fetchingPayload);
    const actions = parserPageTransportTypesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();

    useEffect(() => {
        if (isFirstRender.current) {
            isFirstRender.current = false;
            return;
        }
        const copy = {...fetchPayload, page: page};
        dispatch(actions.setFetchingPayload(copy))
        dispatch(actions.fetchTypesAsync(copy))
    }, [page]);

    const handlePageChange = useCallback((_: ChangeEvent<unknown>, newPage: number) => {
        if (page === newPage) return;
        setPage(newPage);
    }, [page])

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