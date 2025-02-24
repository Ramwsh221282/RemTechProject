import {Card, Typography} from "@mui/material";
import {ParserProfile} from "../../Types/ParserProfile.ts";
import {ParserProfileItem} from "./ParserProfileItem.tsx";

export function ParserProfileMenu({profiles}: { profiles: ParserProfile[] }) {
    return (
        <Card sx={{overflow: 'auto', maxHeight: '770px'}}>
            <Typography sx={{padding: '5px'}} variant={"h5"}>
                {"Профили парсинга:"}
            </Typography>
            {profiles.map((profile: ParserProfile, index: number) => (
                <ParserProfileItem key={profile.id} profile={profile} orderedNumber={index}/>
            ))}
        </Card>
    )
}