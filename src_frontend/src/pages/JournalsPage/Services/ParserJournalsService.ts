import {ParserJournalResponse} from "../Types/ParserJournal.ts";
import {Pagination} from "../../AdvertisementsPage/Types/AdvertisementsPageTypes.ts";
import axios from "axios";
import {Envelope} from "../../../common/Types/Envelope.ts";

const apiUrl = 'http://localhost:5256/ParserJournals/parser-journals'

export class ParserJournalsService {
    public static async getJournals(pagination: Pagination): Promise<ParserJournalResponse | string> {
        try {
            const response = await axios.get<Envelope<ParserJournalResponse>>(apiUrl, {
                params: {
                    page: pagination.page,
                    size: pagination.size,
                }
            })
            const data = response.data;
            if (data.error) {
                return data.error;
            }
            return data.result;
        } catch (error) {
            return "Что-то пошло не так"
        }
    }
}