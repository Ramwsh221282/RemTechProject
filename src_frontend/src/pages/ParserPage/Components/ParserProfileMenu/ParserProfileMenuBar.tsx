import {Fab} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {memo, useCallback} from "react";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";
import {ParserProfile} from "../../Types/ParserProfile.ts";

type Props = {
    addNewProfile: (result: ParserProfile | string) => void;
}

function ParserProfileMenuBar(props: Props) {
    const createProfile = useCallback(async () => {
        const response = await ParserProfileService.createNewProfile();
        props.addNewProfile(response);
    }, [props])

    return (
        <Fab onClick={createProfile} size={"small"} sx={{margin: '5px'}}>
            <AddIcon/>
        </Fab>
    )
}

export const ParserProfileMenuBarMemo = memo(ParserProfileMenuBar);