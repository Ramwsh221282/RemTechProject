import {FormEvent, useState} from "react";
import {Button, DialogContentText, MenuItem, OutlinedInput, Select, TextField} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import BallotIcon from '@mui/icons-material/Ballot';
import DisabledByDefaultIcon from '@mui/icons-material/DisabledByDefault';
import {Modal, useModal} from "../../../components/Modal.tsx";

export type Characteristic = {
    characteristicsName: string;
    characteristicsValue: string;
    onDelete?: (characteristic: Characteristic) => void;
}

type CharacteristicsBarProps = {
    onCharacteristicsChange: (characteristics: Characteristic[]) => void;
}

export function CharacteristicsBar({onCharacteristicsChange}: CharacteristicsBarProps) {
    const [characteristics, setNewCharacteristics] = useState<Characteristic[]>([]);
    const createModal = useModal();
    const removeModal = useModal();
    const placeHolder = "Добавить характеристику";

    function confirmDelete(characteristic: Characteristic): void {
        const newCharacteristics = characteristics.filter(ctx =>
            ctx.characteristicsName !== characteristic.characteristicsName ||
            ctx.characteristicsValue !== characteristic.characteristicsValue);
        setNewCharacteristics(newCharacteristics);
        onCharacteristicsChange(newCharacteristics);
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        const formData = new FormData(event.currentTarget);
        const newCharacteristic: Characteristic = {
            characteristicsName: formData.get("name") as string,
            characteristicsValue: formData.get("value") as string,
            onDelete: removeModal.open,
        }
        const newCharacteristics = [...characteristics, newCharacteristic];
        setNewCharacteristics(newCharacteristics);
        onCharacteristicsChange(newCharacteristics);
        createModal.close();
    }

    return (
        <div className="flex flex-col w-full">
            {removeModal.data && (
                <CharacteristicsRemoveForm
                    isOpen={removeModal.isOpen}
                    handleClose={removeModal.close}
                    handleSubmit={() => {
                        confirmDelete(removeModal.data);
                        removeModal.close();
                    }}
                    characteristic={removeModal.data}
                />
            )}
            <CharacteristicsCreateForm
                isOpen={createModal.isOpen}
                handleClose={createModal.close}
                handleSubmit={handleSubmit}
            />
            <CharacteristicsList
                characteristics={characteristics}
                handleOpen={createModal.open}
                placeHolder={placeHolder}
            />
        </div>
    );
}

type CharacteristicsSelectProps = {
    characteristics: Characteristic[],
    handleOpen: () => void,
    placeHolder: string
}

function CharacteristicsList({characteristics, handleOpen, placeHolder}: CharacteristicsSelectProps) {
    return (
        <Select
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

type CharacteristicsRemoveFormProps = {
    isOpen: boolean,
    handleClose: () => void,
    handleSubmit: (characteristic: Characteristic) => void;
    characteristic: Characteristic;
}

function CharacteristicsRemoveForm({
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

type CharacteristicsAddFormProps = {
    isOpen: boolean,
    handleClose: () => void,
    handleSubmit: (event: FormEvent<HTMLFormElement>) => void,
}

function CharacteristicsCreateForm({isOpen, handleClose, handleSubmit}: CharacteristicsAddFormProps) {
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

function CharacteristicComponent(ctxProps: Characteristic) {
    return (
        <MenuItem>
            <div className="flex gap-2 justify-between items-center">
                <BallotIcon/>
                <span>{ctxProps.characteristicsName}:</span>
                <span>{ctxProps.characteristicsValue}</span>
                <DisabledByDefaultIcon onClick={() => ctxProps.onDelete?.(ctxProps)}/>
            </div>
        </MenuItem>
    )
}