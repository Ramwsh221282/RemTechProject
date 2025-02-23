import {AdvertisementsService} from "../../Services/AdvertisementsApiService.ts";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";
import {useEffect} from "react";
import {AdvertisementCardRowProps} from "./AdvertisementCardRow.tsx";
import {AdvertisementCardCol} from "./AdvertisementCardCol.tsx";

export function AdvertisementsBoard({service}: { service: AdvertisementsService }) {
    const notifications = useNotification();

    useEffect(() => {
        if (service.error.trim().length > 0) {
            notifications.showNotification({severity: "error", message: service.error});
        }
    }, [service.error]);

    const rowSize = 6;
    const rows: AdvertisementCardRowProps[] = [];
    const advertisements = service.advertisements;

    for (let i = 0; i < advertisements.length; i += rowSize) {
        const rowAdvertisements = advertisements.slice(i, i + rowSize);
        rows.push({cards: rowAdvertisements});
    }

    return (
        <>
            <div className="h-164">
                <AdvertisementCardCol rows={rows}/>
                <NotificationAlert notification={notifications.notification}
                                   hideNotification={notifications.hideNotification}/>
            </div>
        </>
    )
}