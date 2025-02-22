import {TextField} from "@mui/material";

type FilterInputProps = {
    type: string;
    name?: string;
    id: string;
    label: string;
    variant: "outlined" | "filled" | "standard";
    fullWidth?: boolean;
}

export function FilterInput({name, id, label, variant, fullWidth}: FilterInputProps) {
    return (
        <div className="flex flex-row gap-3 w-full">
            <TextField size={"small"} type={"type"} autoComplete={"off"} name={name}
                       fullWidth={fullWidth ? fullWidth : false} id={id}
                       label={label}
                       variant={variant}/>
        </div>
    )
}