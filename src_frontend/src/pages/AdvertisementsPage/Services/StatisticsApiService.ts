import {useCallback, useMemo, useState} from "react";
import {Statistics} from "../Types/AdvertisementsPageTypes.ts";
import axios, {AxiosError} from "axios";
import {Envelope, getResult} from "../../../app/models/Envelope.ts";

const statisticsApiUrl: string = "http://localhost:5256/statistics";

export type StatisticsService = {
    error: string
    statistics: Statistics;
    isLoading: boolean;
    fetchStatistics(): Promise<void>
}

export function useStatisticsService() {
    const [currentStatistics, setCurrentStatistics] = useState<Statistics>({
        minPrice: 0,
        maxPrice: 0,
        averagePrice: 0,
        count: 0
    });
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>("");

    const fetchStatistics = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await axios.get<Envelope<Statistics>>(statisticsApiUrl);
            if (response.data.error.trim().length > 0) {
                setError(response.data.error)
                setIsLoading(false);
                return;
            }
            const statistics = getResult<Statistics>(response.data);
            setCurrentStatistics(statistics);
            await new Promise(resolve => setTimeout(resolve, 3000));
            setIsLoading(false);
        } catch (error) {
            const axiosError = error as AxiosError;
            setError(axiosError.message);
            setIsLoading(false);
        }
    }, [])

    const service: StatisticsService = useMemo(() => ({
        error: error,
        statistics: currentStatistics,
        isLoading: isLoading,
        fetchStatistics: fetchStatistics
    }), [currentStatistics, isLoading, error])
    return service;
}