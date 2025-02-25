import {ParserProfile} from "../../Types/ParserProfile.ts";
import {useEffect, useState} from "react";
import {CircularProgress, Typography} from "@mui/material";
import {ParserProfileItem} from "./ParserProfileItem.tsx";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";

type Props = {
    profiles: ParserProfile[];
    initializeProfiles: (profiles: ParserProfile[] | string) => void;
    updateProfile: (profile: ParserProfile) => void;
    removeProfile: (profile: ParserProfile) => void;
}

export function ParserProfileMenuContent(props: Props) {
    const [isLoading, setIsLoading] = useState(false);
    const notification = useNotification();

    useEffect(() => {
        setIsLoading(true);
        const fetching = async () => {
            setIsLoading(true);
            const result = await ParserProfileService.getParserProfiles();
            if (typeof result === 'string') {
                setIsLoading(false);
                showNotification('error', result)
                return;
            }
            props.initializeProfiles(result);
            setIsLoading(false);
        }
        fetching();
    }, []);

    function showNotification(severity: 'error' | 'info' | 'warning' | 'success', message: string) {
        notification.showNotification({severity, message: message});
    }

    function onItemUpdate(item: ParserProfile) {
        props.updateProfile(item);
    }

    async function onItemDelete(item: ParserProfile) {
        setIsLoading(true);
        const result = await ParserProfileService.removeParserProfile(item);
        if (result.length > 0) {
            notification.showNotification({severity: 'error', message: result})
            setIsLoading(false);
            return;
        }
        setIsLoading(false);
        props.removeProfile(item);
    }

    if (isLoading) {
        return (
            <>
                <CircularProgress sx={{margin: 'auto'}} size={100}/>
            </>
        )
    }

    if (!isLoading && props.profiles.length === 0) {
        return (
            <div className="flex flex-col gap-2 justify-center items-center m-auto">
                <Typography variant={'h6'}>{"Не найдено профилей"}</Typography>
                <Typography variant={'h6'}>{"Создайте новый профиль"}</Typography>
            </div>
        )
    }

    return (
        <>
            {props.profiles.map((item, index) => <ParserProfileItem onItemDelete={onItemDelete}
                                                                    onItemUpdate={onItemUpdate}
                                                                    key={item.id} profile={item}
                                                                    orderedNumber={index}
                                                                    notify={showNotification}/>)}
            <NotificationAlert notification={notification.notification}
                               hideNotification={notification.hideNotification}/>
        </>
    )
}