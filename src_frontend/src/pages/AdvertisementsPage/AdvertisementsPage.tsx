import {FilterBar} from "./Components/FilterBar/FilterBar.tsx";
import {usePaginationService} from "./Services/PaginationService.ts";
import {createFilterDto, useAdvertisementsFilterService} from "./Services/FilterAdvertismentsService.ts";
import {useStatisticsService} from "./Services/StatisticsApiService.ts";
import {StatisticsInfo} from "./Components/StatisticsInfo.tsx";
import {useEffect} from "react";
import {AdvertisementsBoard} from "./Components/AdvertisementsBoard/AdvertisementsBoard.tsx";
import {useAdvertisementsApiService} from "./Services/AdvertisementsApiService.ts";

export const AdvertisementsPage = () => {
    const paginationService = usePaginationService({page: 1, size: 12});
    const filterService = useAdvertisementsFilterService();
    const statisticsService = useStatisticsService();
    const advertisementsService = useAdvertisementsApiService();

    useEffect(() => {
        statisticsService.fetchStatistics(createFilterDto(filterService.filter));
        advertisementsService.fetchAdvertisements(paginationService.pagination, createFilterDto(filterService.filter));
    }, [filterService.filter]);

    return (
        <div className="flex flex-row gap-5 py-5 px-5">
            <div className="flex flex-col gap-3">
                <FilterBar filterService={filterService}/>
                <StatisticsInfo service={statisticsService}/>
            </div>
            <AdvertisementsBoard service={advertisementsService}/>
        </div>
    );
}