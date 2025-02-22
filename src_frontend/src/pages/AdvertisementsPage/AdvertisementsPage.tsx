import {AdvertisementsBoard} from "./Components/AdvertisementsBoard/AdvertisementsBoard.tsx";
import {FilterBar} from "./Components/FilterBar/FilterBar.tsx";
import {StatisticsInfo} from "./Components/StatisticsInfo.tsx";
import {usePaginationService} from "./Services/PaginationService.ts";
import {useAdvertisementsFilterService} from "./Services/FilterAdvertismentsService.ts";

export const AdvertisementsPage = () => {
    const paginationService = usePaginationService({page: 1, size: 12});
    const filterService = useAdvertisementsFilterService();

    // useEffect(() => {
    //     setIsLoading(true);
    //     const fetchStatistics = async () => {
    //         const response = await getStatistics();
    //         const newStatistics = {...statistics, ...response};
    //         setStatistics(newStatistics);
    //         setIsLoading(false)
    //     }
    //     fetchStatistics();
    // }, []);

    return (
        <div className="flex flex-row gap-5 py-5 px-5">
            <div className="flex flex-col gap-3">
                <FilterBar filterService={filterService}/>
                <StatisticsInfo/>
            </div>
            <AdvertisementsBoard paginationService={paginationService}/>
        </div>
    )
}