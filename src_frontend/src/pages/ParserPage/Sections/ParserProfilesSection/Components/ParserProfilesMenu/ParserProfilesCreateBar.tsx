import {Fab, TextField} from "@mui/material";
import * as React from "react";
import {useState} from "react";
import AddIcon from "@mui/icons-material/Add";
import {ParserProfile} from "../../../../Types/ParserProfile.ts";
import {ParserProfileService} from "../../../../Services/ParserProfileService.ts";

type Props = {
    onCreate: (profile: ParserProfile | string) => void;
}

export function ParserProfilesCreateBar(props: Props) {
    const [name, setName] = useState<string>("")

    function onNameChange(e: React.ChangeEvent<HTMLInputElement>) {
        setName(e.target.value);
    }

    async function onCreateSubmit(e: React.ChangeEvent<HTMLFormElement>) {
        e.preventDefault();
        e.stopPropagation();
        props.onCreate(await ParserProfileService.createNewProfile(name));
        setName("");
    }

    return (
        <form onSubmit={onCreateSubmit} className="flex flex-row gap-1">
            <TextField fullWidth={true} autoComplete={"off"} size={"small"} label={"Название профиля"} value={name}
                       onChange={onNameChange}></TextField>
            <Fab type={"submit"} size={"small"}>
                <AddIcon/>
            </Fab>
        </form>
    )
}