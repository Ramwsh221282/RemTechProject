import {useState} from "react";
import * as React from "react";
import {Divider, Fab, TextField, Typography} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {TransportType} from "../../../../../Types/TransportType.ts";

type Props = {
    onCreateSubmit(result: TransportType | string): void;
}

export function CustomLinksCreateForm(props: Props) {
    const [name, setName] = useState('');
    const [link, setLink] = useState('');

    function clearInputs() {
        setName('');
        setLink('');
    }

    function submitCreation(e: React.FormEvent) {
        e.preventDefault();
        e.stopPropagation();
        clearInputs();
        if (name.trim().length === 0)
            return props.onCreateSubmit("Требуется имя ссылки")
        if (link.trim().length === 0)
            return props.onCreateSubmit("Требуется значение ссылки")
        const type: TransportType = {link: link, name: name}
        props.onCreateSubmit(type)
        setName("");
        setLink("");
    }

    return (
        <form onSubmit={submitCreation} className="flex flex-row items-center gap-1">
            <div className="flex flex-col gap-2 w-full">
                <Typography variant={"subtitle1"}>Создание пользовательской ссылки:</Typography>
                <TextField
                    autoComplete={"off"}
                    fullWidth={true}
                    size={"small"}
                    value={name}
                    label={'Название'}
                    onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => setName(event.currentTarget.value)}/>
                <TextField
                    autoComplete={"off"}
                    fullWidth={true}
                    size={"small"}
                    value={link}
                    label={'Ссылка'}
                    onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => setLink(event.currentTarget.value)}/>
            </div>
            <Fab size={"small"} type={"submit"}>
                <AddIcon/>
            </Fab>
            <Divider/>
        </form>
    )
}