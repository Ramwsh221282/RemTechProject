import {FormEvent, useEffect, useRef, useState} from "react";
import {Fab, TextField, Typography} from "@mui/material";
import NorthIcon from '@mui/icons-material/North';
import SouthIcon from '@mui/icons-material/South';
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import ClearIcon from '@mui/icons-material/Clear';


export function TransportMarksMenuBar() {
    const fetchPayload = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer.fetchingPayload);
    const dispatch = useDispatch<RootParserPageDispatch>();
    const [sort, setSort] = useState<string>('ASC');
    const [mark, setMark] = useState<string>('')
    const [isSubmit, setIsSubmit] = useState<boolean>(false);
    const isFirstRender = useRef<boolean>(true);
    const actions = parserPageTransportTypesActions;

    useEffect(() => {
        if (isFirstRender.current) {
            isFirstRender.current = false;
            return;
        }
        const newPayload = {...fetchPayload, sort: sort, mark: mark.trim().length > 0 ? mark.trim() : null}
        dispatch(actions.setFetchingPayload(newPayload));
        dispatch(actions.fetchTypesAsync(newPayload));
    }, [sort, isSubmit]);

    function onSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        setIsSubmit(!isSubmit);
    }

    function onClearClick() {
        setMark('');
        setSort((prev) => prev);
        setIsSubmit(!isSubmit);
    }

    return (
        <form onSubmit={onSubmit} className="flex flex-row gap-2 p-2 items-center justify-start">
            <Typography variant={"h6"}>{"Сортировка:"}</Typography>
            <Fab disabled={sort === 'ASC'} onClick={() => {
                const order = 'ASC';
                setSort(order);
            }} size={"small"}>
                <NorthIcon/>
            </Fab>
            <Fab disabled={sort === 'DESC'} onClick={() => {
                const order = 'DESC'
                setSort(order)
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
