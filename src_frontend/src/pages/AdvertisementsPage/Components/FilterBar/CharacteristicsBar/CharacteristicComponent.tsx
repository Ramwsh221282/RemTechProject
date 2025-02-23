import {MenuItem} from "@mui/material";
import BallotIcon from "@mui/icons-material/Ballot";
import DisabledByDefaultIcon from "@mui/icons-material/DisabledByDefault";
import {Characteristic} from "./CharacteristicsBar.tsx";

export function CharacteristicComponent(ctxProps: Characteristic) {
    return (
        <MenuItem>
            <div className="flex gap-2 justify-between items-center">
                <BallotIcon/>
                <span>{ctxProps.characteristicsName}:</span>
                <span>{ctxProps.characteristicsValue}</span>
                <DisabledByDefaultIcon onClick={() => ctxProps.onDelete?.(ctxProps)}/>
            </div>
        </MenuItem>
    )
}