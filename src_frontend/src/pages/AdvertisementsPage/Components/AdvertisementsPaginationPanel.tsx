import {PaginationService} from "../Services/PaginationService.ts";
import {StatisticsService} from "../Services/StatisticsApiService.ts";
import {Fade, Pagination} from "@mui/material";

function calculateTotalPages(paginationService: PaginationService, statisticsService: StatisticsService) {
    return Math.ceil(statisticsService.statistics.count / paginationService.pagination.size)
}

export function AdvertisementsPaginationPanel({paginationService, statisticsService}: {
    paginationService: PaginationService,
    statisticsService: StatisticsService
}) {
    const pageCount = calculateTotalPages(paginationService, statisticsService);

    function onPageChange(_: any, value: number): void {
        const paginationCopy = {...paginationService.pagination};
        const newPaginations = {...paginationCopy, page: value};
        paginationService.setPagination(newPaginations);
    }

    return (
        <>
            <Fade in={true} timeout={500}>
                <Pagination sx={{
                    margin: "auto",
                    backgroundColor: '#1E1E1E',
                    width: '350px',
                    padding: '4px',
                    borderRadius: '5px',
                    boxShadow: '0 0 5px 1px',
                    alignItems: 'center',
                    justifyContent: 'center',
                    justifyItems: 'center',
                }}
                            page={paginationService.pagination.page}
                            count={pageCount}
                            color={"standard"}
                            onChange={onPageChange}/>
            </Fade>
        </>
    )
}
