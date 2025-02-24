import {Button, OutlinedInput, Select} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {Characteristic} from "./CharacteristicsBar.tsx";
import {CharacteristicComponent} from "./CharacteristicComponent.tsx";
import {useVisibility} from "../../../../../common/hooks/VisibilityHook.ts";

type CharacteristicsSelectProps = {
    characteristics: Characteristic[],
    handleOpen: () => void,
    placeHolder: string,
}

export function CharacteristicsList({
                                        characteristics,
                                        handleOpen,
                                        placeHolder,
                                    }: CharacteristicsSelectProps) {

    const visibility = useVisibility(false);

    return (
        <Select
            onClick={visibility.toggleVisibility}
            open={visibility.visible}
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