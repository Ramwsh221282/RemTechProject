import {FormEvent, useState} from "react";
import {Fab, TextField, Typography} from "@mui/material";
import NorthIcon from '@mui/icons-material/North';
import SouthIcon from '@mui/icons-material/South';
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import ClearIcon from '@mui/icons-material/Clear';


export function TransportMarksMenuBar() {
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);
    const actions = parserPageTransportTypesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();
    const [sort, setSort] = useState<string>('ASC');
    const [mark, setMark] = useState<string>('')

    function onSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        const markSearch = mark?.trim().length > 0 ? mark : null
        const payloadCopy = {...state.fetchingPayload, sort: sort, mark: markSearch};
        dispatch(actions.setFetchingPayload(payloadCopy));
        dispatch(actions.fetchTypesAsync(payloadCopy));
    }

    function onSortClick(order: string) {
        const payloadCopy = {...state.fetchingPayload, sort: order};
        dispatch(actions.setFetchingPayload(payloadCopy));
        dispatch(actions.fetchTypesAsync(payloadCopy));
    }

    function onClearClick() {
        const payloadCopy = {...state.fetchingPayload, mark: null};
        dispatch(actions.setFetchingPayload(payloadCopy));
        dispatch(actions.fetchTypesAsync(payloadCopy));
        setMark((prev) => {
            prev = '';
            return prev;
        });
    }

    return (
        <form onSubmit={onSubmit} className="flex flex-row gap-2 p-2 items-center justify-start">
            <Typography variant={"h6"}>{"Сортировка:"}</Typography>
            <Fab disabled={sort === 'ASC'} onClick={() => {
                const order = 'ASC';
                setSort(order);
                onSortClick(order);
            }} size={"small"}>
                <NorthIcon/>
            </Fab>
            <Fab disabled={sort === 'DESC'} onClick={() => {
                const order = 'DESC'
                setSort(order)
                onSortClick(order);
            }} size={"small"}>
                <SouthIcon/>
            </Fab>
            <TextField value={mark} onChange={(event) => setMark(event.target.value)} size={"small"}
                       variant={"outlined"}
                       label={'Бренд'}></TextField>
            <Fab type={'submit'} size={"small"}>
                <SearchOutlinedIcon/>
            </Fab>
            <Fab size={"small"} onClick={onClearClick}>
                <ClearIcon/>
            </Fab>
        </form>
    )
}
