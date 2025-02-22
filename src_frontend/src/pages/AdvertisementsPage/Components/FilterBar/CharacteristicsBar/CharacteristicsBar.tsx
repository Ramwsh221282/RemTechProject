import {FormEvent, useState} from "react";
import {useModal} from "../../../../../components/Modal.tsx";
import {NotificationAlert, useNotification} from "../../../../../components/Notification.tsx";
import {CharacteristicsRemoveForm} from "./CharacteristicsRemoveForm.tsx";
import {CharacteristicsCreateForm} from "./CharacteristicsCreateForm.tsx";
import {CharacteristicsList} from "./CharacteristicsList.tsx";

function useVisibility() {
    const [visible, setVisible] = useState(false);

    function turnOn() {
        setVisible(true);
    }

    function turnOff() {
        setVisible(false);
    }

    return {
        visible,
        turnOn,
        turnOff
    }
}

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
    const {showNotification, notification, hideNotification} = useNotification();
    const selectVisibility = useVisibility();

    function confirmDelete(characteristic: Characteristic): void {
        const newCharacteristics = characteristics.filter(ctx =>
            ctx.characteristicsName !== characteristic.characteristicsName ||
            ctx.characteristicsValue !== characteristic.characteristicsValue);
        setNewCharacteristics(newCharacteristics);
        onCharacteristicsChange(newCharacteristics);
        showNotification({severity: "info", message: "Характеристика удалена"});
        selectVisibility.turnOff()
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
        selectVisibility.turnOff()
        showNotification({severity: "info", message: "Характеристика добавлена"});
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
                visibility={selectVisibility.visible}
                onVisibilityChange={selectVisibility.turnOn}
            />
            <NotificationAlert notification={notification} hideNotification={hideNotification}/>
        </div>
    );
}




