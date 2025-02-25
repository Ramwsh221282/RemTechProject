import {Card, CircularProgress, Typography} from "@mui/material";
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import {parserPageProfilesActions} from "../../Store/Slices/ParserPageProfilesState.ts";
import {useEffect} from "react";
import {ParserProfile} from "../../Types/ParserProfile.ts";
import {ParserProfileItem} from "./ParserProfileItem.tsx";

export function ParserProfileMenu() {
    const state = useSelector((state: RootParserPageState) => state.parserPageProfilesReducer);
    const actions = parserPageProfilesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();

    useEffect(() => {
        dispatch(actions.fetchProfilesAsync())
    }, [dispatch]);

    function LoadingContent() {
        return (
            <div className="flex flex-col w-full justify-center items-center m-auto">
                <CircularProgress size={100}/>
            </div>
        )
    }

    return (
        <Card sx={{overflow: 'auto', height: '835px'}}>
            <Typography sx={{padding: '5px'}} variant={"h5"}>
                {"Профили парсинга:"}
            </Typography>
            {state.isLoading ? <LoadingContent/> : state.profiles.map((profile: ParserProfile, index: number) => (
                <ParserProfileItem key={profile.id} profile={profile} orderedNumber={index}/>
            ))}
        </Card>
    )
}