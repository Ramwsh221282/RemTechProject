import {ParserProfile, ParserProfileLinks} from "../../Types/ParserProfile.ts";
import {createAsyncThunk, createSlice, PayloadAction} from "@reduxjs/toolkit";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";
import {TransportType} from "../../Types/TransportType.ts";

interface ParserPageProfilesState {
    profiles: ParserProfile[];
    currentProfile: ParserProfile | null;
    error: string;
    isLoading: boolean;
}

const initialParserPageProfilesState: ParserPageProfilesState = {
    profiles: [],
    error: '',
    currentProfile: null,
    isLoading: false,
};

const parserPageProfilesSlice = createSlice({
    name: "parser_page_profiles",
    initialState: initialParserPageProfilesState,
    reducers: {
        setCurrentProfile: (state, action: PayloadAction<ParserProfile | null>) => {
            state.currentProfile = action.payload;
        },
        addLinkToProfile: (state, action: PayloadAction<TransportType>) => {
            if (!state.currentProfile) {
                state.error = 'Не выбран профиль'
                return;
            }
            const newLink: ParserProfileLinks = {id: null, link: action.payload.link, mark: action.payload.name}
            if (state.currentProfile.links.findIndex(link => link.mark === newLink.mark) !== -1) {
                state.error = `Бренд ${newLink.mark} уже добавлен`
                return;
            }
            const links = state.currentProfile.links.slice();
            links.push(newLink);
            state.currentProfile.links = links;
        },
        updateProfileState: (state) => {
            if (!state.currentProfile) {
                state.error = 'Не выбран профиль'
                return;
            }
            state.currentProfile.state = !state.currentProfile.state;
            state.currentProfile.stateDescription = state.currentProfile.state ? 'Активный' : 'Неактивный';
        },
        cleanError: (state) => {
            if (state.error.trim().length > 0) {
                state.error = '';
            }
        }
    },
    extraReducers: (builder) => {
        builder.addCase(fetchProfilesAsync.pending, (state) => {
            state.isLoading = true;
        }).addCase(fetchProfilesAsync.fulfilled, (state, action: PayloadAction<ParserProfile[] | string>) => {
            if (typeof action.payload === "string") {
                state.error = action.payload;
                state.isLoading = false;
            }
            state.profiles = action.payload as ParserProfile[];
            state.isLoading = false;
        })
            .addCase(createNewProfileAsync.pending, (state) => {
                state.isLoading = true
            })
            .addCase(createNewProfileAsync.fulfilled, (state, action: PayloadAction<ParserProfile | string>) => {
                if (typeof action.payload === "string") {
                    state.error = action.payload;
                    state.isLoading = false;
                }
                state.profiles.push(action.payload as ParserProfile);
                state.isLoading = false;
            })
            .addCase(updateProfileAsync.pending, (state) => {
                state.isLoading = true
            })
            .addCase(updateProfileAsync.fulfilled, (state, action: PayloadAction<string>) => {
                if (action.payload.trim().length > 0) {
                    state.error = action.payload;
                    state.isLoading = false;
                    return
                }
                state.isLoading = false;
            })
    }
});

const createNewProfileAsync = createAsyncThunk(
    "parserProfiles/createNewProfile",
    async () => {
        return await ParserProfileService.createNewProfile();
    }
)

const fetchProfilesAsync = createAsyncThunk(
    "parserProfiles/fetchProfiles",
    async () => {
        return await ParserProfileService.getParserProfiles();
    }
)

const updateProfileAsync = createAsyncThunk(
    "parserProfiles/updateProfile",
    async (profile: ParserProfile) => {
        return await ParserProfileService.updateParserProfile(profile);
    }
)

const setCurrentProfile = parserPageProfilesSlice.actions.setCurrentProfile;

const addLinkToProfile = parserPageProfilesSlice.actions.addLinkToProfile;

const updateProfileState = parserPageProfilesSlice.actions.updateProfileState;

const cleanError = parserPageProfilesSlice.actions.cleanError;

export const parserPageProfilesActions = {
    fetchProfilesAsync,
    createNewProfileAsync,
    updateProfileAsync,
    setCurrentProfile,
    addLinkToProfile,
    updateProfileState,
    cleanError,
};

export const parserPageProfilesReducer = parserPageProfilesSlice.reducer;