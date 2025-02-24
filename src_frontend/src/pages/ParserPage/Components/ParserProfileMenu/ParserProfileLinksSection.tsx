import {ParserProfile, ParserProfileLinks} from "../../Types/ParserProfile.ts";
import {useState} from "react";
import {List} from "@mui/material";
import {ParserProfileAddNewLinkBar} from "./ParserProfileAddNewLinkBar.tsx";
import {ParserProfileLinkCard} from "./ParserProfileLinkCard.tsx";

export function ParserProfileLinksSection({profile}: { profile: ParserProfile }) {
    const [profileLinks, updateProfileLinks] = useState<ParserProfileLinks[]>([...profile.links]);

    return (
        <List sx={{
            borderRadius: '10px',
            padding: '1rem',
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
        }}
              component={"nav"}>
            <ParserProfileAddNewLinkBar/>
            {profileLinks.map((link) => (<ParserProfileLinkCard key={link.id} link={link}/>))}
        </List>
    )
}