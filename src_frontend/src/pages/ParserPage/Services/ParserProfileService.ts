import {Envelope} from "../../../common/Types/Envelope.ts";
import {ParserProfile} from "../Types/ParserProfile.ts";
import axios, {AxiosResponse} from "axios";

const parserProfileApiUri: string = 'http://localhost:5256/parser-profile';

export class ParserProfileService {
    public static async getParserProfiles(): Promise<Envelope<ParserProfile[]>> {
        try {
            const response: AxiosResponse<Envelope<ParserProfile[]>> = await axios.get(parserProfileApiUri);
            return response.data;
        } catch (error) {
            return {result: [], code: 400, error: 'Something went wrong'}
        }
    }
}