import {ParserProfile} from "../../Types/ParserProfile.ts";
import {Fab, TextField} from "@mui/material";
import {useState} from "react";
import AddIcon from "@mui/icons-material/Add";
import * as React from "react";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";

type Props = {
    createProfile: (profile: ParserProfile | string) => void,
}

export function ParserProfileCreateBar(props: Props) {
    const [profileName, setProfileName] = useState<string>("");

    async function onSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        const response = await ParserProfileService.createNewProfile(profileName);
        props.createProfile(response);
        setProfileName("");
    }

    return (
        <>
            <form onSubmit={onSubmit} className={"flex flex-row justify-between gap-1"}>
                <TextField
                    onChange={(event: React.ChangeEvent<HTMLInputElement>) => setProfileName(event.target.value)}
                    size={"small"} label={"Название"} value={profileName}></TextField>
                <Fab type={"submit"} size={"small"}>
                    <AddIcon/>
                </Fab>
            </form>
        </>
    )
}