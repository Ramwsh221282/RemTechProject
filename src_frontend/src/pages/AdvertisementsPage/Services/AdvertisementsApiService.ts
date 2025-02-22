import {useCallback, useState} from "react";
import {Advertisement, Pagination} from "../Types/AdvertisementsPageTypes.ts";
import axios, {AxiosError} from "axios";
import {Envelope, getResult} from "../../../app/models/Envelope.ts";
import {FilterDto} from "./FilterAdvertismentsService.ts";

const advertisementsApiUrl: string = "http://localhost:5256/advertisements";

export type AdvertisementsService = {
    error: string,
    advertisements: Advertisement[],
    isLoading: boolean,
    fetchAdvertisements: (pagination: Pagination, filtersDto?: FilterDto | null) => Promise<void>;
}

export function useAdvertisementsApiService() {
    const [currentAdvertisements, setCurrentAdvertisements] = useState<Advertisement[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string>("");

    const fetchAdvertisements = useCallback(async (pagination: Pagination, filtersDto: FilterDto | null = null) => {
        if (isLoading) return;
        setIsLoading(true);
        try {
            const response = await axios.post<Envelope<Advertisement[]>>(advertisementsApiUrl, filtersDto,
                {
                    params: {
                        page: pagination.page,
                        size: pagination.size,
                    },
                })
            if (response.data.error.trim().length > 0) {
                setError(response.data.error.trim());
                setIsLoading(false);
                return;
            }
            const advertisements = getResult<Advertisement[]>(response.data);
            setCurrentAdvertisements(advertisements);
            setIsLoading(false);
        } catch (error) {
            const axiosError = error as AxiosError;
            // @ts-ignore
            const message = axiosError.response!.data["error"]! as string;
            setError(message);
            setIsLoading(false);
        }
    }, [isLoading])

    const service: AdvertisementsService = {
        error: error,
        isLoading: isLoading,
        advertisements: currentAdvertisements,
        fetchAdvertisements: fetchAdvertisements
    }

    return service;
}