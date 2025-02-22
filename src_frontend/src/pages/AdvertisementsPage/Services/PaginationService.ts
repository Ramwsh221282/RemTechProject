import {useState} from "react";
import {Pagination} from "../Types/AdvertisementsPageTypes.ts";

export type PaginationService = {
    pagination: Pagination;
    setPagination: (pagination: Pagination) => void;
}

export function usePaginationService(initialPagination: Pagination) {
    const [currentPagination, setCurrentPagination] = useState<Pagination>({
        page: initialPagination.page,
        size: initialPagination.size
    });

    function setPagination(newPagination: Pagination) {
        setCurrentPagination((prev) => ({...prev, ...newPagination}));
    }

    const service: PaginationService = {pagination: currentPagination, setPagination: setPagination}
    return service;
}