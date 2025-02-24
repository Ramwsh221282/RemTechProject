import {Fab, TextField} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import {ParserProfileLinks} from "../../Types/ParserProfile.ts";

export function ParserProfileLinkCard({link}: { link: ParserProfileLinks }) {
    return (
        <div className={"flex flex-row justify-between items-center gap-2 p-2 rounded-md bg-[#121212]"}>
            <TextField
                autoComplete="off"
                fullWidth={true} size={"small"}
                value={link.link}
                aria-readonly={true}
                label={link.mark}>
            </TextField>
            <Fab
                sx={{width: '38px', height: '10px'}}><DeleteIcon/>
            </Fab>
        </div>
    )
}