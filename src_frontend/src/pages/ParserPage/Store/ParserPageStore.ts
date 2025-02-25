import {combineReducers, configureStore} from "@reduxjs/toolkit";
import {parserPageProfilesReducer} from "./Slices/ParserPageProfilesState.ts";
import {parserPageTransportTypesReducer} from "./Slices/ParserPageTransportTypesState.ts";

const reducer = combineReducers({
    parserPageProfilesReducer,
    parserPageTransportTypesReducer,
})

export const parserPageStore = configureStore({
    reducer: reducer,
});

export type RootParserPageState = ReturnType<typeof parserPageStore.getState>;
export type RootParserPageDispatch = typeof parserPageStore.dispatch;