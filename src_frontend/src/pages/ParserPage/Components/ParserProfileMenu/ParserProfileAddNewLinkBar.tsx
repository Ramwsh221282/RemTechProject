import {Fab, TextField} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {useState} from "react";

type AddNewLinkBarProps = {
    link: string;
    mark: string;
}

export function ParserProfileAddNewLinkBar() {
    const [addNewLinkBarProps, updateAddNewLinkBarProps] = useState<AddNewLinkBarProps>({mark: '', link: ''})

    return (
        <form className={"flex flex-row justify-between items-center gap-2 p-2 rounded-md bg-[#121212]"}>
            <TextField autoComplete="off" fullWidth={true} size={"small"} label={'Ссылка'}
                       value={addNewLinkBarProps.link}></TextField>
            <TextField autoComplete="off" fullWidth={true} size={"small"} label={'Марка'}
                       value={addNewLinkBarProps.mark}></TextField>
            <Fab
                sx={{width: '80px', height: '10px'}}><AddIcon/>
            </Fab>
        </form>
    )
}