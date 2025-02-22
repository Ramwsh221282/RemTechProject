import {useCallback, useState} from "react";
import {Statistics} from "../Types/AdvertisementsPageTypes.ts";
import axios, {AxiosError} from "axios";
import {Envelope, getResult} from "../../../app/models/Envelope.ts";
import {FilterDto} from "./FilterAdvertismentsService.ts";

const statisticsApiUrl: string = "http://localhost:5256/statistics";

export type StatisticsService = {
    error: string
    statistics: Statistics;
    isLoading: boolean;
    fetchStatistics(filtersDto?: FilterDto | null): Promise<void>
}

export function useStatisticsService() {
    const [currentStatistics, setCurrentStatistics] = useState<Statistics>({
        minPrice: 0,
        maxPrice: 0,
        averagePrice: 0,
        count: 0,
    });
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>("");

    const fetchStatistics = useCallback(async (filtersDto: FilterDto | null = null) => {
        if (isLoading) return;
        setIsLoading(true);
        try {
            const response = await axios.post<Envelope<Statistics>>(statisticsApiUrl, filtersDto);
            if (response.data.error.trim().length > 0) {
                setError(response.data.error);
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
    }, [isLoading]);

    const service: StatisticsService = {
        error,
        statistics: currentStatistics,
        isLoading,
        fetchStatistics,
    };

    return service;
}