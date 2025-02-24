import {ParserProfileService} from "../Services/ParserProfileService.ts";
import {Button, Typography} from "@mui/material";
import {NotificationAlert, useNotification} from "../../../components/Notification.tsx";
import {useEffect} from "react";

export function CreateParserProfileDialog({service}: { service: ParserProfileService }) {
    const notification = useNotification();

    useEffect(() => {
        if (service.error.trim().length > 0)
            notification.showNotification(({severity: 'error', message: service.error}))
        if (service.profiles.length > 0)
            notification.showNotification(({severity: 'success', message: 'Создан новый профиль парсинга'}))
    }, [service.profiles, service.error]);

    async function createNewProfileClick() {
        await service.createProfile();
    }

    return (
        <div
            className="flex
            flex-col
            w-auto
            h-auto
            rounded-md
            m-auto
            justify-center
            items-center p-4
            bg-amber-950
            gap-5
            shadow-md
            shadow-neutral-800">
            <Typography sx={{color: '#fff'}} variant="h5">
                {"Нет созданных профилей. Создать новый?"}
            </Typography>
            <Button onClick={createNewProfileClick}>{"Создать новый профиль"}</Button>
            <NotificationAlert notification={notification.notification}
                               hideNotification={notification.hideNotification}/>
        </div>
    )
}