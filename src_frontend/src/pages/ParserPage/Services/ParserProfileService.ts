import {Envelope} from "../../../common/Types/Envelope.ts";
import {ParserProfile} from "../Types/ParserProfile.ts";
import axios, {AxiosResponse} from "axios";

const parserProfileApiUri: string = 'http://localhost:5256/parser-profile';

export class ParserProfileService {

    public static async createNewProfile(name: string): Promise<ParserProfile | string> {
        try {
            const response: AxiosResponse<Envelope<ParserProfile>> = await axios.post(parserProfileApiUri, {
                name: name,
            });
            if (response.data.error.trim().length > 0)
                return response.data.error;
            return response.data.result;
        } catch (error) {
            return "Что-то пошло не так";
        }
    }

    public static async getParserProfiles(): Promise<ParserProfile[] | string> {
        try {
            const response: AxiosResponse<Envelope<ParserProfile[]>> = await axios.get(parserProfileApiUri);
            if (response.data.error.trim().length > 0)
                return response.data.error;
            return response.data.result;
        } catch (error) {
            return "Что-то пошло не так";
        }
    }

    public static async updateParserProfile(profile: ParserProfile): Promise<string> {
        try {
            const response: AxiosResponse<Envelope<any>> = await axios.put(`${parserProfileApiUri}/${profile.id}`, {
                id: profile.id,
                name: profile.name,
                state: profile.state,
                links: profile.links
            });
            const data = response.data;
            if (data.error) {
                return data.error;
            }
            return "";
        } catch (error) {
            return "Что-то пошло не так";
        }
    }

    public static async removeParserProfile(profile: ParserProfile): Promise<string> {
        try {
            const response: AxiosResponse<Envelope<any>> = await axios.delete(`${parserProfileApiUri}/${profile.id}`);
            const data = response.data;
            if (data.error) {
                return data.error;
            }
            return "";
        } catch (error) {
            return "Что-то пошло не так";
        }
    }
}