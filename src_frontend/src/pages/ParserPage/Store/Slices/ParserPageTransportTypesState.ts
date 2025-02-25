import {TransportType, TransportTypeResponse} from "../../Types/TransportType.ts";
import {createAsyncThunk, createSlice, PayloadAction} from "@reduxjs/toolkit";
import {TransportTypesService} from "../../Services/TransportTypesService.ts";
import {Envelope} from "../../../../common/Types/Envelope.ts";

interface ParserPageTransportTypesState {
    types: TransportType[]
    isLoading: boolean;
    error: string;
    fetchingPayload: ParserPageFetchingPayload;
    count: number;
}

interface ParserPageFetchingPayload {
    page: number,
    size: number,
    sort: string | null
    mark: string | null,
}

const initialFetchingPayload: ParserPageFetchingPayload = {
    page: 1,
    size: 50,
    sort: null,
    mark: null,
}

const initialState: ParserPageTransportTypesState = {
    types: [],
    isLoading: false,
    error: "",
    fetchingPayload: initialFetchingPayload,
    count: 0,
};

const slice = createSlice({
    name: 'transport-types-state',
    initialState: initialState,
    reducers: {
        setFetchingPayload: (state, action: PayloadAction<ParserPageFetchingPayload>) => {
            state.fetchingPayload = action.payload;
        }
    },
    extraReducers: (builder) => {
        builder.addCase(fetchTypesAsync.pending, (state) => {
            state.isLoading = true
        }).addCase(fetchTypesAsync.fulfilled, (state, action: PayloadAction<Envelope<TransportTypeResponse>>) => {
            if (action.payload.error.trim().length > 0) {
                state.error = action.payload.error
                state.isLoading = false;
                return;
            }
            state.types = action.payload.result.items
            state.count = action.payload.result.count
            state.isLoading = false;
        }).addCase(createTypesAsync.pending, (state) => {
            state.isLoading = true
        }).addCase(createTypesAsync.fulfilled, (state, action: PayloadAction<Envelope<TransportTypeResponse>>) => {
            if (action.payload.error) {
                state.error = action.payload.error
                state.isLoading = false;
                return;
            }
            state.types = action.payload.result.items
            state.count = action.payload.result.count
            state.isLoading = false;
        })
    }
});

const fetchTypesAsync = createAsyncThunk(
    "transportTypesState/fetchTypes",
    async (payload: ParserPageFetchingPayload) => {
        return await TransportTypesService.getTransportTypes(payload.page, payload.size, payload.sort, payload.mark);
    }
)

const createTypesAsync = createAsyncThunk(
    "transportTypesState/createTypes",
    async () => {
        return await TransportTypesService.createTransportTypes();
    }
)

const setFetchingPayload = slice.actions.setFetchingPayload;

export const parserPageTransportTypesActions = {fetchTypesAsync, createTypesAsync, setFetchingPayload};

export const parserPageTransportTypesReducer = slice.reducer;
