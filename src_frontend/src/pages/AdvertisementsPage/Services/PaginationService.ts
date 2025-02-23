import {useCallback, useMemo, useState} from "react";
import {Pagination} from "../Types/AdvertisementsPageTypes.ts";

export type PaginationService = {
    pagination: Pagination;
    setPagination: (pagination: Pagination) => void;
}

export function usePaginationService(initialPagination: Pagination) {
    const [currentPagination, setCurrentPagination] = useState<Pagination>({
        page: initialPagination.page,
        size: initialPagination.size,
        sort: null
    });

    const setPagination = useCallback((newPagination: Pagination) => {
        setCurrentPagination((prev) =>
            ({...prev, page: newPagination.page, size: newPagination.size, sort: newPagination.sort}));
    }, [])


    const service: PaginationService = useMemo(() => ({
        pagination: currentPagination,
        setPagination: setPagination
    }), [currentPagination, setPagination]);


    return service;
}