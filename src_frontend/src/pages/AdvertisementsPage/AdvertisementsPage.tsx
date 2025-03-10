import {FilterBar} from "./Components/FilterBar/FilterBar.tsx";
import {usePaginationService} from "./Services/PaginationService.ts";
import {createFilterDto, useAdvertisementsFilterService} from "./Services/FilterAdvertismentsService.ts";
import {useStatisticsService} from "./Services/StatisticsApiService.ts";
import {StatisticsInfo} from "./Components/StatisticsInfo.tsx";
import {useEffect, useRef} from "react";
import {useAdvertisementsApiService} from "./Services/AdvertisementsApiService.ts";
import {AdvertisementsPaginationPanel} from "./Components/AdvertisementsPaginationPanel.tsx";
import {AdvertisementsTextSearchBar} from "./Components/AdvertisementsTextSearchBar.tsx";
import {AdvertisementsBoard} from "./Components/AdvertisementsBoard/AdvertisementsBoard.tsx";
import {CircularProgress} from "@mui/material";
import {SortingBar} from "./Components/SortingBar.tsx";

export const AdvertisementsPage = () => {
    const paginationService = usePaginationService({page: 1, size: 12, sort: null});
    const filterService = useAdvertisementsFilterService();
    const statisticsService = useStatisticsService();
    const advertisementsService = useAdvertisementsApiService();
    const initialRequest = useRef<boolean>(true);

    useEffect(() => {
        statisticsService.fetchStatistics(createFilterDto(filterService.filter));
        advertisementsService.fetchAdvertisements(paginationService.pagination, createFilterDto(filterService.filter));
    }, [filterService.filter]);

    useEffect(() => {
        if (initialRequest.current) {
            initialRequest.current = false;
            return;
        }
        advertisementsService.fetchAdvertisements(paginationService.pagination, createFilterDto(filterService.filter));
    }, [paginationService.pagination]);


    return (
        <div className="flex flex-col gap-2">
            <AdvertisementsTextSearchBar service={filterService}/>
            <div className="flex flex-row gap-3">
                <div className="flex flex-col gap-3">
                    <FilterBar filterService={filterService}/>
                    <StatisticsInfo service={statisticsService}/>
                    <SortingBar service={paginationService}/>
                </div>
                {advertisementsService.isLoading ? (
                    <div className="flex flex-col w-full h-full my-auto mx-auto justify-center items-center">
                        <CircularProgress size={128}/>
                        <span className="text-2xl">{"Загрузка..."}</span>
                    </div>) : <div className="flex flex-col">
                    <AdvertisementsBoard service={advertisementsService}/>
                </div>}
            </div>
            {advertisementsService.isLoading ? null :
                <AdvertisementsPaginationPanel paginationService={paginationService}
                                               statisticsService={statisticsService}/>}
        </div>
    );
}