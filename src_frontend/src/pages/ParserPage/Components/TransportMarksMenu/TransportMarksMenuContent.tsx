import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import {Button, CircularProgress, Typography} from "@mui/material";
import {TransportType} from "../../Types/TransportType.ts";
import {useEffect} from "react";

export function TransportMarksMenuContent() {
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);
    const actions = parserPageTransportTypesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();

    useEffect(() => {
        dispatch(actions.fetchTypesAsync(state.fetchingPayload))
    }, [dispatch]);

    function createNewBrands() {
        dispatch(actions.createTypesAsync())
    }

    return (
        <div className="flex flex-col w-full gap-5 m-auto justify-center items-center p-5 h-170">
            {state.isLoading && state.types.length === 0 ? <CircularProgress size={100}/> : null}
            {state.types.length === 0 && !state.isLoading ? (
                <>
                    <Typography variant={"subtitle1"}>{"В данный момент в базе нет брендов"}</Typography>
                    <Button size={"small"} variant={"outlined"} onClick={createNewBrands}>{"Получить бренды"}</Button>
                </>
            ) : <BrandColumns/>}
        </div>
    )
}

function BrandColumns() {
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);

    function createBrandColumns(maxSize: number): Array<Array<TransportType>> {
        const columns: Array<Array<TransportType>> = []
        for (let i = 0; i < state.types.length; i += maxSize) {
            const column = state.types.slice(i, i + maxSize);
            columns.push(column);
        }
        return columns;
    }

    return (
        <>
            <div className={"flex flex-row gap-8 p-2 items-center justify-center"}>
                {createBrandColumns(10).map((column, index) => <BrandColumn key={index} types={column}/>)}
            </div>
        </>
    )
}

function BrandColumn({types}: { types: TransportType[] }) {
    return (
        <div className="flex flex-col gap-5 p-2 items-center text-center justify-between">
            {types.map((type, index) => <Typography sx={{
                fontSize: '1rem',
                color: '#FFC107',
                cursor: 'pointer',
                transform: 'scale(1)',
                transition: 'transform .5s, text-decoration .5s',
                "&::before": {
                    transform: 'scale(1)',
                    transition: 'transform .3s',
                },
                ":hover": {
                    transform: 'scale(1.5)',
                    textDecoration: 'underline',
                    transition: 'transform .5s, text-decoration .3s',
                    "&::before": {
                        transform: 'scale(1)',
                        textDecoration: 'underline',
                        transition: 'transform .5s, text-decoration .3s',
                    }
                }
            }} key={index}
                                                    variant={"caption"}>{type.name}</Typography>)}
        </div>
    )
}