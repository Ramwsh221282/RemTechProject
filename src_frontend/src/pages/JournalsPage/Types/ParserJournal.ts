export type ParserJournalResponse = {
    count: number;
    journals: ParserJournal[]
}

export type ParserJournal = {
    id: string;
    isSuccess: boolean;
    description: string;
    source: string;
    hours: number;
    minutes: number;
    seconds: number;
    itemsParsed: number;
    error: string;
    createdOn: string;
}