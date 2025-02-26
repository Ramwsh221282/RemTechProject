import * as React from "react";
import {useState} from "react";
import NorthIcon from '@mui/icons-material/North';
import SouthIcon from '@mui/icons-material/South';
import {Fab, Fade, Typography} from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import {PaginationService} from "../Services/PaginationService.ts";

type SortButton = {
    id: string;
    disabled: boolean;
}

export function SortingBar({service}: { service: PaginationService }) {
    const [sortButton, setSortButton] = useState<SortButton>({
        id: 'sort-NONE',
        disabled: true
    })

    function onSortModeSelected(event: React.MouseEvent<HTMLButtonElement>) {
        const id = event.currentTarget.id;
        setSortButton((prev) => ({...prev, id: id, disabled: true}))
        const mode = id.split('-')[1];
        const copy = {...service.pagination};
        copy.sort = mode;
        service.setPagination(copy);
    }

    return (
        <Fade in={true} timeout={500}>
            <div className="flex flex-col gap-2 p-1 rounded-md shadow-neutral-800 shadow-md bg-[#1E1E1E]">
                <Typography sx={{textDecoration: 'underline'}} variant={"subtitle1"}
                            color="textPrimary">{"Сортировка"}</Typography>
                <div className="flex flex-row gap-3 justify-center">
                    <Fab
                        disabled={sortButton.id === "sort-ASC" && sortButton.disabled}
                        onClick={onSortModeSelected}
                        id={"sort-ASC"}
                        size={"small"}
                        color={"primary"}
                        aria-label={"sort-ASC"}>
                        <NorthIcon sx={{color: '#000000'}}/>
                    </Fab>
                    <Fab
                        disabled={sortButton.id === "sort-DESC" && sortButton.disabled}
                        onClick={onSortModeSelected}
                        id={"sort-DESC"}
                        size={"small"}
                        color={"primary"}
                        aria-label={"sort-DESC"}>
                        <SouthIcon sx={{color: '#000000'}}/>
                    </Fab>
                    <Fab disabled={sortButton.id === "sort-NONE" && sortButton.disabled}
                         onClick={onSortModeSelected}
                         id={"sort-NONE"}
                         size={"small"}
                         color={"primary"}
                         aria-label={"sort-NONE"}>
                        <ClearIcon sx={{color: '#000000'}}/>
                    </Fab>
                </div>
            </div>
        </Fade>
    )
}