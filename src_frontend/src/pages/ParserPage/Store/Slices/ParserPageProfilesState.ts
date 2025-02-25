import {ParserProfile} from "../../Types/ParserProfile.ts";
import {createAsyncThunk, createSlice, PayloadAction} from "@reduxjs/toolkit";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";
import {Envelope} from "../../../../common/Types/Envelope.ts";

interface ParserPageProfilesState {
    profiles: ParserProfile[];
    error: string;
    isLoading: boolean;
}

const initialParserPageProfilesState: ParserPageProfilesState = {
    profiles: [],
    error: '',
    isLoading: false,
};

const parserPageProfilesSlice = createSlice({
    name: "parser_page_profiles",
    initialState: initialParserPageProfilesState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(fetchProfilesAsync.pending, (state) => {
            state.isLoading = true;
        }).addCase(fetchProfilesAsync.fulfilled, (state, action: PayloadAction<Envelope<ParserProfile[]>>) => {
            if (action.payload.error.trim().length > 0)
                state.error = action.payload.error;
            state.profiles = action.payload.result;
            state.isLoading = false;
        })
    }
});

const fetchProfilesAsync = createAsyncThunk(
    "parserProfiles/fetchProfiles",
    async () => {
        return await ParserProfileService.getParserProfiles();
    }
)

export const parserPageProfilesActions = {fetchProfilesAsync};

export const parserPageProfilesReducer = parserPageProfilesSlice.reducer;