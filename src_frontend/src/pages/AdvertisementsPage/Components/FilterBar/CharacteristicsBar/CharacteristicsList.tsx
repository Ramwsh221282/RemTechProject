import {Button, OutlinedInput, Select} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {Characteristic} from "./CharacteristicsBar.tsx";
import {CharacteristicComponent} from "./CharacteristicComponent.tsx";

type CharacteristicsSelectProps = {
    characteristics: Characteristic[],
    handleOpen: () => void,
    placeHolder: string,
    visibility: boolean;
    onVisibilityChange: () => void;
}

export function CharacteristicsList({
                                        characteristics,
                                        handleOpen,
                                        placeHolder,
                                        visibility,
                                        onVisibilityChange
                                    }: CharacteristicsSelectProps) {
    return (
        <Select
            onClick={onVisibilityChange}
            open={visibility}
            size={"small"}
            fullWidth={true}
            value={""}
            displayEmpty={true}
            renderValue={(_) => {
                const ctxAmount = characteristics.length;
                const ctxLabel = `Указано характеристик: ${ctxAmount}`;
                return (
                    <div>{ctxLabel}</div>
                )
            }}
            input={<OutlinedInput/>}>
            <div className="w-full px-1 flex flex-row items-center justify-center">
                <Button variant="outlined" onClick={handleOpen} startIcon={<AddIcon/>}>
                    {placeHolder}
                </Button>
            </div>
            {characteristics.map((item, index) => (
                <CharacteristicComponent key={index} {...item}/>
            ))}
        </Select>
    )
}