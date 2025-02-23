export interface Envelope<T> {
    error: string;
    result: T;
    code: number;
}

export function getResult<T>(envelope: Envelope<T>): T {
    return envelope.result;
}