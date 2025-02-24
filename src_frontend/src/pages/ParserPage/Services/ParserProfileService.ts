import {useCallback, useMemo, useState} from "react";
import {ParserProfile} from "../Types/ParserProfile";
import axios from "axios";
import {Envelope, getResult} from "../../../common/Types/Envelope.ts";

const parserProfileApiUri: string = 'http://localhost:5256/parser-profile';

export type ParserProfileService = {
    isLoading: boolean;
    error: string;
    profiles: ParserProfile[];
    fetchProfiles: () => Promise<void>;
    createProfile: () => Promise<void>;
}

export function useParserProfile() {
    const [isLoading, setIsLoading] = useState(false);
    const [parserProfiles, setParserProfiles] = useState<ParserProfile[]>([])
    const [error, setError] = useState<string>('');

    const createNewProfile = useCallback(async () => {
        if (isLoading) return;
        setIsLoading(true);
        try {
            const response = await axios.post<Envelope<ParserProfile>>(parserProfileApiUri);
            const data = response.data;
            const result = getResult<ParserProfile>(data);
            setParserProfiles((prev) => [...prev, result])
            setIsLoading(false);
        } catch (error) {
            setError("Что-то пошло не так");
            setIsLoading(false);
        }
    }, [])

    const fetchProfiles = useCallback(async () => {
        if (isLoading) return;
        setIsLoading(true);
        try {
            const response = await axios.get<Envelope<ParserProfile[]>>(parserProfileApiUri)
            if (response.data.error.trim().length > 0) {
                setError(response.data.error)
                setIsLoading(false);
                return;
            }
            const result = getResult<ParserProfile[]>(response.data);
            setParserProfiles((prev) => ([...prev, ...result]));
            setIsLoading(false);
        } catch (error) {
            setError('Что-то пошло не так')
            setIsLoading(false);
        }
    }, [])

    const service: ParserProfileService = useMemo(() => ({
        isLoading: isLoading,
        error: error,
        profiles: parserProfiles,
        fetchProfiles: fetchProfiles,
        createProfile: createNewProfile,
    }), [isLoading, error, parserProfiles, fetchProfiles, createNewProfile]);

    return service;
}