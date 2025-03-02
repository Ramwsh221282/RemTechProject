import {useEffect, useRef, useState} from "react";
import {TransportType} from "../../../../../Types/TransportType.ts";
import {useNotification} from "../../../../../../../components/Notification.tsx";
import {TransportTypesService} from "../../../../../Services/TransportTypesService.ts";
import {GeneralLinksSearchBar} from "../GeneralLinksView/GeneralLinksSearchBar.tsx";
import {GeneralLinkspagination} from "../GeneralLinksView/GeneralLinksPagination.tsx";
import {CustomLinksCreateForm} from "./CustomLinksCreateForm.tsx";
import {TransportTypesLinkContainer} from "./TransportTypesLinkContainer.tsx";
import {CircularProgress} from "@mui/material";

type CustomLinksViewPagination = {
    page: number,
    size: number,
    sort: string | null,
    searchTerm: string | null
}

type Props = {
    onTransportTypeClick: (transportType: TransportType) => void;
}

function defaultPagination(): CustomLinksViewPagination {
    return {page: 1, size: 10, sort: null, searchTerm: null}
}

export function CustomLinksView(props: Props) {
    const [pagination, setPagination] = useState<CustomLinksViewPagination>(defaultPagination())
    const [transportTypes, setTransportTypes] = useState<TransportType[]>([]);
    const notification = useNotification();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [count, setCount] = useState(0);
    const firstInit = useRef<boolean>(true);

    function updatePagination(page: number): void {
        setPagination({...pagination, page: page});
    }

    function addTransportType(type: TransportType | string): void {
        if (typeof type === 'string') {
            notification.showNotification({severity: 'error', message: type})
            return;
        }
        const copy = transportTypes.slice();
        copy.push(type);
        setTransportTypes(copy)
        notification.showNotification({severity: 'success', message: `Добавлена пользовательская ссылка: ${type.name}`})
    }

    function removeTransportType(result: TransportType | string): void {
        if (typeof result === "string") {
            notification.showNotification({severity: 'error', message: result})
            return;
        }
        const copy = transportTypes.filter(t => t.name !== result.name);
        setTransportTypes(copy)
    }

    function onSearchSubmit(searchTerm: string | null) {
        setPagination({...pagination, searchTerm: searchTerm});
    }

    useEffect(() => {
        if (firstInit.current) {
            firstInit.current = false;
        }
        const fetching = async () => {
            setIsLoading(true);
            const response = await TransportTypesService.getTransportTypes(pagination.page, pagination.size, pagination.sort, pagination.searchTerm, 'USER');
            setIsLoading(false);
            if (typeof response === 'string') {
                notification.showNotification({severity: 'error', message: response})
                return;
            }
            setTransportTypes(response.items);
            setCount(response.count);
        }
        fetching();
    }, [])

    return (
        <section className="flex flex-col gap-1">
            <GeneralLinksSearchBar onSearchSubmit={onSearchSubmit}/>
            <CustomLinksCreateForm onCreateSubmit={addTransportType}/>
            {isLoading ? <CircularProgress size={100}/> :
                <TransportTypesLinkContainer onDelete={removeTransportType} onSelect={props.onTransportTypeClick}
                                             types={transportTypes} columnSize={10}/>}
            <GeneralLinkspagination onPageChange={updatePagination}
                                    itemsCount={count}
                                    pageSize={pagination.size}
                                    page={pagination.page}/>
        </section>
    )
}