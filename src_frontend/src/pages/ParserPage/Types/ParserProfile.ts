export type ParserProfile = {
    id: string;
    createdOn: string;
    name: string;
    state: boolean;
    stateDescription: string;
    links: ParserProfileLink[];
}

export type ParserProfileLink = {
    name: string;
    link: string;
    additions: string[] | null;
}
