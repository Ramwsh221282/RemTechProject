import {ParserProfile, ParserProfileLink} from "../../../../Types/ParserProfile.ts";
import {Card, CardContent, CircularProgress, Divider, Typography} from "@mui/material";
import {ParserProfilesCreateBar} from "./ParserProfilesCreateBar.tsx";
import {ParserProfileCard} from "./ParserProfileCard.tsx";

type Props = {
    onParserProfileSelect(profile: ParserProfile): void;
    onParserProfileDelete(profile: ParserProfile | string): void;
    onParserProfileAdd(profile: ParserProfile | string): void;
    onParserProfileUpdate(profile: ParserProfile | string): void;
    onParserProfileLinkRemove(link: ParserProfileLink): void;
    isLoading: boolean;
    profiles: ParserProfile[];
}

export function ParserProfilesMenu(props: Props) {
    return (
        <Card sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: '0.5rem',
            width: '50%',
            height: '850px',
            overflow: 'auto'
        }}>
            <CardContent>
                <Typography variant={"h5"}>Профили парсинга</Typography>
                <Divider/>
                <ParserProfilesCreateBar onCreate={props.onParserProfileAdd}/>
                {props.isLoading ? <CircularProgress size={100}/> : props.profiles.map((profile: ParserProfile) =>
                    <ParserProfileCard key={profile.id}
                                       profile={profile}
                                       onLinkRemove={props.onParserProfileLinkRemove}
                                       onDelete={props.onParserProfileDelete}
                                       onUpdate={props.onParserProfileUpdate}
                                       onToggle={props.onParserProfileSelect}/>)}
            </CardContent>
        </Card>
    )
}