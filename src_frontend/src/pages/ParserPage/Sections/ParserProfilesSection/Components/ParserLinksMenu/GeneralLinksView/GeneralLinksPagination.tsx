import {Pagination} from "@mui/material";

type Props = {
    onPageChange: (page: number) => void,
    itemsCount: number,
    pageSize: number,
    page: number,
}

export function GeneralLinkspagination(props: Props) {
    const totalItemsCount = Math.ceil(props.itemsCount / props.pageSize);

    return (
        <>
            <Pagination count={totalItemsCount} page={props.page} onChange={(_, page) => props.onPageChange(page)}/>
        </>
    )
}