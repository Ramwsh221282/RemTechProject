import {useEffect, useState} from "react";
import {TransportType, TransportTypeResponse} from "../../../../../Types/TransportType.ts";
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

    useEffect(() => {
        const fetching = async () => {
            const response = await fetchTypes(pagination);
            if (typeof response === 'string') {
                notification.showNotification({severity: 'error', message: response})
                return;
            }
            setTransportTypes(response.items);
            setCount(response.count);
        }
        fetching();
    }, [])

    async function fetchTypes(pagination: CustomLinksViewPagination): Promise<TransportTypeResponse | string> {
        setIsLoading(true);
        const response = await TransportTypesService.getTransportTypes(pagination.page, pagination.size, pagination.sort, pagination.searchTerm, 'USER');
        setIsLoading(false);
        return response;
    }

    async function onPageChange(page: number) {
        const newPagination = {...pagination, page: page};
        const response = await fetchTypes(newPagination);
        if (typeof response === 'string') {
            notification.showNotification({severity: 'error', message: response})
            return;
        }
        setPagination({...newPagination});
        setTransportTypes(response.items);
        setCount(response.count);
    }

    async function onSearchSubmit(searchTerm: string | null) {
        const newPagination = {...pagination, searchTerm: searchTerm};
        const response = await fetchTypes(newPagination);
        if (typeof response === 'string') {
            notification.showNotification({severity: 'error', message: response})
            return;
        }
        setPagination({...newPagination});
        setTransportTypes(response.items);
        setCount(response.count);
    }

    async function addTransportType(type: TransportType) {
        const result = await TransportTypesService.createCustomTransportType(type.name, type.link, []);
        if (result.trim().length > 0) {
            notification.showNotification({severity: 'error', message: result})
            return;
        }
        const copy = transportTypes.slice();
        copy.push(type);
        setTransportTypes(copy.reverse())
        notification.showNotification({severity: 'success', message: `Добавлена пользовательская ссылка: ${type.name}`})
    }

    async function removeTransportType(type: TransportType) {
        const result = await TransportTypesService.deleteTransportType(type.name, type.link);
        if (result.trim().length > 0) {
            notification.showNotification({severity: 'error', message: result})
            return;
        }
        const copy = transportTypes.filter(t => t.name !== type.name && t.link !== type.link);
        setTransportTypes(copy.reverse())
        notification.showNotification({severity: 'success', message: `Удалена пользовательская ссылка: ${type.name}`})
    }


    return (
        <section className="flex flex-col gap-2">
            <GeneralLinksSearchBar onSearchSubmit={onSearchSubmit}/>
            <CustomLinksCreateForm onCreateSubmit={addTransportType}/>
            {isLoading ? <CircularProgress size={100}/> :
                <TransportTypesLinkContainer onDelete={removeTransportType} onSelect={props.onTransportTypeClick}
                                             types={transportTypes} columnSize={10}/>}
            <GeneralLinkspagination onPageChange={onPageChange}
                                    itemsCount={count}
                                    pageSize={pagination.size}
                                    page={pagination.page}/>
        </section>
    )
}