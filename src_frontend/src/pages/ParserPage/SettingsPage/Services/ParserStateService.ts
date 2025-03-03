import {ParserState} from "../Types/ParserState.ts";
import axios from "axios";
import {Envelope} from "../../../../common/Types/Envelope.ts";

const apiUrl = 'http://localhost:5256/ParserState';

type ParserStateResponse = {
    isEnabled: boolean,
}

export class ParserStateService {
    public static async get() {
        const response = await axios.get<Envelope<ParserStateResponse>>(`${apiUrl}`);
        const data = response.data;
        const state: ParserState = data.result.isEnabled ? {
            isEnabled: true,
            description: 'Включен'
        } : {isEnabled: false, description: 'Выключен'};
        return state;
    }

    public static async enable(): Promise<string | ParserState> {
        try {
            const response = await axios.post<Envelope<ParserState>>(`${apiUrl}/enable`, {});
            if (response.data.error) {
                return response.data.error;
            }
            return {
                isEnabled: true,
                description: 'Включен'
            };
        } catch (error) {
            return "Что-то пошло не так"
        }
    }

    public static async disable(): Promise<string | ParserState> {
        try {
            const response = await axios.post<Envelope<ParserState>>(`${apiUrl}/disable`, {});
            if (response.data.error) {
                return response.data.error;
            }
            return {
                isEnabled: false,
                description: 'Выключен'
            };
        } catch (error) {
            return "Что-то пошло не так"
        }
    }

    public static async restart(): Promise<string | ParserState> {
        try {
            const response = await axios.post<Envelope<ParserState>>(`${apiUrl}/restart`, {});
            if (response.data.error) {
                return response.data.error;
            }
            return {
                isEnabled: true,
                description: 'Включен'
            };
        } catch (error) {
            return "Что-то пошло не так"
        }
    }
}