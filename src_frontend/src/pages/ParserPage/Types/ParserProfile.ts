export type ParserProfile = {
    id: string;
    createdOn: string;
    state: boolean;
    stateDescription: string;
    links: ParserProfileLinks[];
}

export type ParserProfileLinks = {
    id: string;
    link: string;
    mark: string;
}
