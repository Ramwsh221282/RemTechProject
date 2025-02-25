import {TransportTypeResponse} from "../Types/TransportType.ts";
import {Envelope} from "../../../common/Types/Envelope.ts";
import axios from "axios";


const apiUrl = "http://localhost:5256/transport-types";

export class TransportTypesService {
    public static async getTransportTypes(page: number, size: number, sort: string | null, mark: string | null): Promise<Envelope<TransportTypeResponse>> {
        try {
            const response = await axios.get<Envelope<TransportTypeResponse>>(apiUrl, {
                params: {
                    page: page,
                    size: size,
                    sort: sort,
                    mark: mark,
                }
            });
            return response.data;
        } catch (error) {
            const result: TransportTypeResponse = {count: 0, items: []}
            return {error: "Что-то пошло не так", result: result, code: 500}
        }
    }

    public static async createTransportTypes(): Promise<Envelope<TransportTypeResponse>> {
        try {
            const response = await axios.post<Envelope<TransportTypeResponse>>(apiUrl);
            return response.data;
        } catch (error) {
            const result: TransportTypeResponse = {count: 0, items: []}
            return {error: "Что-то пошло не так", result: result, code: 500}
        }
    }
}