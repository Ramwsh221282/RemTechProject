import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import {Fab, Fade, TextField} from "@mui/material";
import {ChangeEvent, useState} from "react";
import {FilterService} from "../Services/FilterAdvertismentsService.ts";


// TODO Fix Search Bar.
export function AdvertisementsTextSearchBar({service}: { service: FilterService }) {
    const [text, setText] = useState("");

    // @ts-ignore
    function onEnterPress(event: KeyboardEvent<HTMLDivElement>) {
        if (event.key === "Enter") {
            const copy = {...service.filter}
            copy.textSearch = text;
            service.handleSetFilters(copy);
        }
    }

    function onChange(event: ChangeEvent<HTMLInputElement>) {
        setText(event.target.value);
    }

    function onSearchPress(): void {
        const copy = {...service.filter}
        copy.textSearch = text;
        service.handleSetFilters(copy);
    }


    return (
        <Fade in={true} timeout={500}>
            <div className="inline-flex flex-row gap-1 bg-[#1E1E1E] py-2 px-1 rounded-md items-center">
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
        </Fade>
    )
}