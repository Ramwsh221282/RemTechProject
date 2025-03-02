import {useEffect, useState} from "react";
import {TransportTypesService} from "../../../../../Services/TransportTypesService.ts";
import {useNotification} from "../../../../../../../components/Notification.tsx";
import {TransportType} from "../../../../../Types/TransportType.ts";
import {Button, CircularProgress, Typography} from "@mui/material";
import {GeneralLinksGrid} from "./GeneralLinksGrid.tsx";
import {GeneralLinksSearchBar} from "./GeneralLinksSearchBar.tsx";
import {GeneralLinkspagination} from "./GeneralLinksPagination.tsx";

type GeneralLinksPagination = {
    page: number,
    size: number,
    sort: string | null,
    searchTerm: string | null
}

type Props = {
    onTransportTypeClick: (transportType: TransportType) => void;
}

export function GeneralLinksView(props: Props) {
    const [transportTypes, setTransportTypes] = useState<TransportType[]>([]);
    const [count, setCount] = useState(0);
    const [pagination, setPagination] = useState<GeneralLinksPagination>({
        page: 1,
        size: 55,
        sort: null,
        searchTerm: null,
    })
    const [isLoading, setIsLoading] = useState<boolean>(false)
    const notification = useNotification();
    const [hasItems, setHasItems] = useState<boolean>(false);

    useEffect(() => {
        const fetching = async () => {
            setIsLoading(true);
            const result = await TransportTypesService.getTransportTypes(pagination.page, pagination.size, pagination.sort, null, 'SYSTEM');
            setIsLoading(false)
            if (typeof result === 'string') {
                notification.showNotification({severity: 'error', message: result})
                return;
            }
            if (result.items.length === 0) {
                setHasItems(false);
                return;
            }
            setHasItems(true)
            setTransportTypes(result.items);
            setCount(result.count);
        }
        fetching();
    }, []);

    useEffect(() => {
        const fetching = async () => {
            setIsLoading(true);
            const result = await TransportTypesService.getTransportTypes(pagination.page, pagination.size, pagination.sort, pagination.searchTerm, 'SYSTEM');
            setIsLoading(false)
            if (typeof result === 'string') {
                notification.showNotification({severity: 'error', message: result})
                return;
            }
            setTransportTypes(result.items);
            setCount(result.count);
        }
        fetching();
    }, [pagination]);

    async function createGeneralLinks() {
        setIsLoading(true);
        const response = await TransportTypesService.createSystemTransportTypes();
        setIsLoading(false);
        if (typeof response === "string") {
            notification.showNotification({severity: 'error', message: response})
            return;
        }
        setTransportTypes(response.items);
        setCount(response.count);
        setHasItems(true)
    }

    function pageChange(page: number) {
        setPagination({...pagination, page: page});
    }

    if (isLoading) {
        return (
            <>
                <CircularProgress size={100}/>
            </>
        )
    }

    return (
        <section className="flex flex-col gap-1">
            <GeneralLinksSearchBar
                onSearchSubmit={(searchTerm: string | null) => setPagination({...pagination, searchTerm: searchTerm})}/>
            {!hasItems ? <div className="flex flex-row gap-1 items-center">
                <Typography>{"Общих ссылок нет"}</Typography>
                <Button onClick={createGeneralLinks}>{"Создать общие ссылки"}</Button>
            </div> : <GeneralLinksGrid gridColumnLength={11} transportTypes={transportTypes}
                                       onTransportTypeClick={props.onTransportTypeClick}/>}
            <GeneralLinkspagination onPageChange={pageChange} itemsCount={count} pageSize={pagination.size}
                                    page={pagination.page}/>
        </section>
    )
}