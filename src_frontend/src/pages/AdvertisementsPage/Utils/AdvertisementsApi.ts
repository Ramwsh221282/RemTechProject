import axios from "axios";
import {Envelope} from "../../../app/models/Envelope.ts";
import {Advertisement, Statistics} from "../Types/AdvertisementsPageTypes.ts";

export const advertisementsApiUrl = "http://localhost:5256/TransportAdvertisements";
export const statisticsApiUrl = "http://localhost:5256/statistics";

export async function getStatistics() {
    const response = await axios.get<Envelope<Statistics>>(statisticsApiUrl);
    return response.data.result;
}

export async function getAdvertisements(page: number, pageSize: number) {
    const params = {page: page, size: pageSize};
    const response = await axios.get<Envelope<Advertisement[]>>(advertisementsApiUrl, {
        params: params
    });
    return response.data.result;
}





