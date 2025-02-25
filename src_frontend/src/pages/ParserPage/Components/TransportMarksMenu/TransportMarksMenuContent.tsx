import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import {parserPageTransportTypesActions} from "../../Store/Slices/ParserPageTransportTypesState.ts";
import {Button, CircularProgress, Typography} from "@mui/material";
import {TransportType} from "../../Types/TransportType.ts";
import {useEffect} from "react";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";
import {parserPageProfilesActions} from "../../Store/Slices/ParserPageProfilesState.ts";

export function TransportMarksMenuContent() {
    const state = useSelector((state: RootParserPageState) => state.parserPageTransportTypesReducer);
    const actions = parserPageTransportTypesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();
    const notifications = useNotification();

    useEffect(() => {
        dispatch(actions.fetchTypesAsync(state.fetchingPayload))
    }, []);

    function createNewBrands() {
        dispatch(actions.createTypesAsync())
    }

    function notify(severity: 'info' | 'warning' | 'error' | 'success', message: string) {
        notifications.showNotification({severity, message: message})
    }

    return (
        <>
            <div className="flex flex-col w-full gap-5 m-auto justify-center items-center p-5 h-170">
                {state.isLoading && state.types.length === 0 ? <CircularProgress size={100}/> : null}
                {state.types.length === 0 && !state.isLoading ? (
                    <>
                        <Typography variant={"subtitle1"}>{"В данный момент в базе нет брендов"}</Typography>
                        <Button size={"small"} variant={"outlined"}
                                onClick={createNewBrands}>{"Получить бренды"}</Button>
                    </>
                ) : <BrandColumns notify={notify}/>}
            </div>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </>
    )
}

function BrandColumns({notify}: {
    notify: (severity: 'info' | 'warning' | 'error' | 'success', message: string) => void
}) {
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
                {createBrandColumns(10).map((column, index) => <BrandColumn notify={notify} key={index}
                                                                            types={column}/>)}
            </div>
        </>
    )
}

function BrandColumn({types, notify}: {
    types: TransportType[],
    notify: (severity: 'info' | 'warning' | 'error' | 'success', message: string) => void
}) {
    return (
        <div className="flex flex-col gap-5 p-2 items-center text-center justify-between">
            {types.map((type, index) => <BrandItem key={index} item={type} notify={notify}/>)}
        </div>
    )
}

function BrandItem({item, notify}: {
    item: TransportType,
    notify: (severity: 'info' | 'warning' | 'error' | 'success', message: string) => void
}) {

    const actions = parserPageProfilesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();
    const error = useSelector((state: RootParserPageState) => state.parserPageProfilesReducer.error)

    useEffect(() => {
        if (error.trim().length > 0)
            notify('error', error)
    }, [error]);

    function handleClick() {
        dispatch(actions.addLinkToProfile(item))
        //notify('success', `Добавлена бренд: ${item.name}`)
    }

    return (
        <>
            <Typography sx={{
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
            }}
                        variant={"caption"} onClick={handleClick}>{item.name}</Typography>
        </>
    )
}