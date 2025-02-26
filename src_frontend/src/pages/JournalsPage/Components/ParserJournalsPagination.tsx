import {Pagination} from "@mui/material";

type Props = {
    totalCount: number,
    page: number;
    pageSize: number;
    changePage: (page: number) => void;
}

export function ParserJournalsPagination(props: Props) {
    const pagesCount = Math.ceil(props.totalCount / props.pageSize);

    return (
        <Pagination sx={{
            backgroundColor: '#1E1E1E', marginTop: 'auto', alignSelf: 'flex-start', borderRadius: '5px',
            boxShadow: '0 0 5px 1px', padding: '4px'
        }}
                    size={"small"}
                    count={pagesCount}
                    page={props.page}
                    onChange={(_, page) => props.changePage(page)}>

        </Pagination>
    )
}