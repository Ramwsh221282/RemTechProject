import {Modal} from "../../../../../components/Modal.tsx";
import {Button, DialogContentText, TextField} from "@mui/material";
import {Characteristic} from "./CharacteristicsBar.tsx";

type CharacteristicsRemoveFormProps = {
    isOpen: boolean,
    handleClose: () => void,
    handleSubmit: (characteristic: Characteristic) => void;
    characteristic: Characteristic;
}

export function CharacteristicsRemoveForm({
                                              isOpen,
                                              handleClose,
                                              handleSubmit,
                                              characteristic
                                          }: CharacteristicsRemoveFormProps) {
    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title="Удаление характеристики из фильтра"
            actions={
                <>
                    <Button onClick={handleClose}>Нет</Button>
                    <Button onClick={() => handleSubmit(characteristic)}>Да</Button>
                </>
            }
        >
            <DialogContentText>Удалить характеристику?</DialogContentText>
            <div className="flex flex-row gap-3">
                <TextField
                    autoComplete="off"
                    value={characteristic.characteristicsName}
                    required={false}
                    aria-readonly={true}
                    margin="dense"
                    label="Название характеристики"
                    fullWidth
                    variant="standard"
                />
                <TextField
                    autoComplete="off"
                    value={characteristic.characteristicsValue}
                    required={false}
                    aria-readonly={true}
                    margin="dense"
                    label="Значение характеристики"
                    fullWidth
                    variant="standard"
                />
            </div>
        </Modal>
    );
}