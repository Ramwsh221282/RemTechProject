import axios from "axios";
import {Envelope} from "../../../app/models/Envelope.ts";

export const advertisementsApiUrl = "http://localhost:5256/TransportAdvertisements";
export const statisticsApiUrl = "http://localhost:5256/statistics";

export type Statistics = {
    averagePrice: number;
    count: number;
    maxPrice: number;
    minPrice: number;
}

export function emptyStatistics(): Statistics {
    return {
        averagePrice: 0,
        count: 0,
        maxPrice: 0,
        minPrice: 0,
    }
}

export async function getStatistics() {
    const response = await axios.get<Envelope<Statistics>>(statisticsApiUrl);
    return response.data.result;
}