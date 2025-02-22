import {useCallback, useMemo, useState} from "react";
import {Advertisement, Pagination} from "../Types/AdvertisementsPageTypes.ts";
import axios, {AxiosError} from "axios";
import {Envelope, getResult} from "../../../app/models/Envelope.ts";

const advertisementsApiUrl: string = "http://localhost:5256/TransportAdvertisements";

export type AdvertisementsService = {
    error: string,
    advertisements: Advertisement[],
    isLoading: boolean,
    fetchAdvertisements: (pagination: Pagination) => Promise<void>;
}

export function useAdvertisementsApiService() {
    const [currentAdvertisements, setCurrentAdvertisements] = useState<Advertisement[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string>("");

    const fetchAdvertisements = useCallback(async (pagination: Pagination) => {
        setIsLoading(true);
        try {
            const response = await axios.get<Envelope<Advertisement[]>>(advertisementsApiUrl, {
                params: {
                    page: pagination.page,
                    size: pagination.size,
                }
            })
            if (response.data.error.trim().length > 0) {
                setError(response.data.error.trim());
                setIsLoading(false);
                return;
            }
            const advertisements = getResult<Advertisement[]>(response.data);
            setCurrentAdvertisements(advertisements);
            await new Promise(resolve => setTimeout(resolve, 3000));
            setIsLoading(false);
        } catch (error) {
            const axiosError = error as AxiosError;
            setError(axiosError.message);
            setIsLoading(false);
        }
    }, [])

    const service: AdvertisementsService = useMemo(() => ({
        error: error,
        isLoading: isLoading,
        advertisements: currentAdvertisements,
        fetchAdvertisements: fetchAdvertisements
    }), [currentAdvertisements, isLoading, error])

    return service;
}