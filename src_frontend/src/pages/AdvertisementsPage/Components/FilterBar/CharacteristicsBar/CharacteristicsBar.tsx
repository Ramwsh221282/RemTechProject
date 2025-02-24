import {FormEvent} from "react";
import {useModal} from "../../../../../components/Modal.tsx";
import {NotificationAlert, useNotification} from "../../../../../components/Notification.tsx";
import {CharacteristicsRemoveForm} from "./CharacteristicsRemoveForm.tsx";
import {CharacteristicsCreateForm} from "./CharacteristicsCreateForm.tsx";
import {CharacteristicsList} from "./CharacteristicsList.tsx";
import {FilterService} from "../../../Services/FilterAdvertismentsService.ts";

export type Characteristic = {
    characteristicsName: string;
    characteristicsValue: string;
    onDelete?: (characteristic: Characteristic) => void;
}

export function CharacteristicsBar({service}: { service: FilterService }) {
    const createModal = useModal();
    const removeModal = useModal();
    const placeHolder = "Добавить характеристику";
    const {showNotification, notification, hideNotification} = useNotification();

    function confirmDelete(characteristic: Characteristic): void {
        const newCharacteristics = service.filter.characteristics.filter(ctx =>
            ctx.characteristicsName !== characteristic.characteristicsName ||
            ctx.characteristicsValue !== characteristic.characteristicsValue);
        const filterCopy = {...service.filter};
        filterCopy.characteristics = newCharacteristics;
        service.handleSetFilters(filterCopy);
        showNotification({severity: "info", message: "Характеристика удалена"});
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
        const filterCopy = {...service.filter};
        filterCopy.characteristics.push(newCharacteristic);
        service.handleSetFilters(filterCopy);
        createModal.close();
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
                characteristics={service.filter.characteristics}
                handleOpen={createModal.open}
                placeHolder={placeHolder}
            />
            <NotificationAlert notification={notification} hideNotification={hideNotification}/>
        </div>
    );
}




