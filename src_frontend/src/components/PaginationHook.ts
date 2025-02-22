import {useState} from "react";

type PaginationProps = {
    page: number;
    pageSize: number;
}

export function usePagination({page = 1, pageSize = 1}: PaginationProps) {
    const [pageValue, setPageValue] = useState(page);
    const [pageSizeValue, setPageSizeValue] = useState(pageSize);

    function changePage(page: number) {
        setPageValue(page);
    }

    function changePageSize(page: number) {
        setPageSizeValue(page);
    }

    return {
        pageValue,
        pageSizeValue,
        changePage,
        changePageSize
    }
}