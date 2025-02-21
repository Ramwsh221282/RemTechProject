import {AdvertisementsBoard} from "./Components/AdvertisementsBoard.tsx";
import {FilterBar} from "./Components/FilterBar.tsx";
import {useEffect, useState} from "react";
import {emptyStatistics, getStatistics, Statistics} from "./Utils/AdvertisementsApi.ts";
import {StatisticsInfo} from "./Components/StatisticsInfo.tsx";

export const AdvertisementsPage = () => {
    const [statistics, setStatistics] = useState<Statistics>(emptyStatistics())
    const [isLoading, setIsLoading] = useState<boolean>(false);

    useEffect(() => {
        setIsLoading(true);
        const fetchStatistics = async () => {
            const response = await getStatistics();
            const newStatistics = {...statistics, ...response};
            setStatistics(newStatistics);
            setIsLoading(false)
        }
        fetchStatistics();
    }, []);

    return (
        <div className="flex flex-row gap-5 py-5 px-5">
            <div className="flex flex-col gap-3">
                <FilterBar/>
                <StatisticsInfo isLoaded={isLoading} statistics={statistics}/>
            </div>
            <AdvertisementsBoard/>
        </div>
    )
}