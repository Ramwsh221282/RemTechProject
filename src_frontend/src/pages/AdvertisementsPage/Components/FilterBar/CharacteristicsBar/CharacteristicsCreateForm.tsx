import {FormEvent} from "react";
import {Modal} from "../../../../../components/Modal.tsx";
import {Button, DialogContentText, TextField} from "@mui/material";

type CharacteristicsAddFormProps = {
    isOpen: boolean,
    handleClose: () => void,
    handleSubmit: (event: FormEvent<HTMLFormElement>) => void,
}

export function CharacteristicsCreateForm({isOpen, handleClose, handleSubmit}: CharacteristicsAddFormProps) {
    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title="Добавление характеристики"
            onSubmit={handleSubmit}
            actions={
                <>
                    <Button onClick={handleClose}>Закрыть</Button>
                    <Button type="submit">Добавить</Button>
                </>
            }
        >
            <DialogContentText>Укажите характеристику:</DialogContentText>
            <div className="flex flex-col gap-3">
                <TextField
                    autoComplete="off"
                    autoFocus
                    required
                    margin="dense"
                    name="name"
                    label="Название характеристики"
                    fullWidth
                    variant="standard"
                />
                <TextField
                    autoComplete="off"
                    required
                    margin="dense"
                    name="value"
                    label="Значение характеристики"
                    fullWidth
                    variant="standard"
                />
            </div>
        </Modal>
    )
}