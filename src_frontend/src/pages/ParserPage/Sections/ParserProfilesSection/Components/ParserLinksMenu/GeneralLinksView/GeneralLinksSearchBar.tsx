import * as React from "react";
import {Fab, TextField} from "@mui/material";
import SearchOutlinedIcon from "@mui/icons-material/SearchOutlined";
import ClearIcon from '@mui/icons-material/Clear';
import {useState} from "react";

type Props = {
    onSearchSubmit: (searchTerm: string | null) => void;
}

export function GeneralLinksSearchBar(props: Props) {
    const [searchTerm, setSearchTerm] = useState("");

    function onSearchSubmit(event: React.FormEvent<HTMLFormElement> | null) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
            props.onSearchSubmit(searchTerm);
            return;
        }
        setSearchTerm("");
    }

    function onInputChange(event: React.ChangeEvent<HTMLInputElement>) {
        setSearchTerm(event.target.value);
    }

    function onClear() {
        setSearchTerm("");
        props.onSearchSubmit(null);
    }

    return (
        <form onSubmit={onSearchSubmit} className="flex flex-row gap-1">
            <TextField onChange={onInputChange} size={"small"} fullWidth={true} label={"Поиск по названию"}
                       value={searchTerm}></TextField>
            <Fab size={"small"} type={"submit"}>
                <SearchOutlinedIcon/>
            </Fab>
            <Fab size={"small"} onClick={onClear}>
                <ClearIcon/>
            </Fab>
        </form>
    )
}