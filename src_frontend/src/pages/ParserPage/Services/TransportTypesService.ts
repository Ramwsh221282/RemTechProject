import {TransportTypeResponse} from "../Types/TransportType.ts";
import {Envelope} from "../../../common/Types/Envelope.ts";
import axios from "axios";


const apiUrl = "http://localhost:5256/TransportTypes/transport-types";

export class TransportTypesService {
    public static async getTransportTypes(page: number, size: number, sort: string | null, mark: string | null, implementor: string | null): Promise<TransportTypeResponse | string> {
        try {
            const response = await axios.get<Envelope<TransportTypeResponse>>(apiUrl, {
                params: {
                    page: page,
                    size: size,
                    sort: sort,
                    mark: mark,
                    implementor: implementor,
                }
            });
            if (response.data.error) {
                return response.data.error;
            }
            return response.data.result;
        } catch (error) {
            return 'Что-то пошло не так'
        }
    }

    public static async createSystemTransportTypes(): Promise<TransportTypeResponse | string> {
        try {
            const response = await axios.post<Envelope<TransportTypeResponse>>(`${apiUrl}/parse`);
            if (response.data.error) {
                return response.data.error;
            }
            return response.data.result;
        } catch (error) {
            return "Что-то пошло не так"
        }
    }

    public static async createCustomTransportType(name: string, link: string, additions: string[]): Promise<string> {
        try {
            const response = await axios.post<Envelope<any>>(`${apiUrl}/${name}`, {
                name: name,
                link: link,
                additions: additions,
            })
            if (response.data.error)
                return response.data.error;
            return "";
        } catch (error) {
            return "Что-то пошло не так";
        }
    }

    public static async deleteTransportType(name: string, link: string) {
        try {
            const response = await axios.delete<Envelope<any>>(`${apiUrl}/${name}`, {
                data: {
                    name: name,
                    link: link,
                }
            })
            if (response.data.error) {
                return response.data.error;
            }
            return "";
        } catch (error) {
            return "Что-то пошло не так"
        }
    }
}