export interface Envelope<T> {
    error: string;
    result: T;
    code: number;
}