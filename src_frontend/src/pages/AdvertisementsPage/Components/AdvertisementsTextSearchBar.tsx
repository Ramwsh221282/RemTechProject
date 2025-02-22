import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import {Fab, TextField} from "@mui/material";
import {ChangeEvent, useState} from "react";
import {FilterService} from "../Services/FilterAdvertismentsService.ts";


// TODO Fix Search Bar.
export function AdvertisementsTextSearchBar({service, textSearchTurn}: {
    service: FilterService,
    textSearchTurn: () => void;
}) {
    const [text, setText] = useState("");

    // @ts-ignore
    function onEnterPress(event: KeyboardEvent<HTMLDivElement>) {
        if (event.key === "Enter") {
            const copy = {...service.filter}
            copy.textSearch = text;
        }
    }

    function onChange(event: ChangeEvent<HTMLInputElement>) {
        setText(event.target.value);
    }

    function onSearchPress(): void {
        const copy = {...service.filter}
        copy.textSearch = text;
    }


    return (
        <div className="inline-flex flex-row gap-1 bg-amber-950 py-2 px-1 rounded-md items-center">
            <TextField
                value={text}
                onChange={onChange}
                onKeyDown={onEnterPress}
                autoComplete={"off"}
                size={"small"}
                fullWidth={true}
                label={"Текстовый поиск"}
                variant={"outlined"}>
            </TextField>
            <Fab size={"small"} onClick={onSearchPress}>
                <SearchOutlinedIcon fontSize={"small"}/>
            </Fab>
        </div>
    )
}