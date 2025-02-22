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

    let rows: AdvertisementCardRowProps[] = [];
    const advertisements = service.advertisements.slice();
    while (advertisements.length !== 0) {
        const rowSize = 6;
        const row: AdvertisementCardRowProps = {cards: []};
        while (row.cards.length !== rowSize && advertisements.length !== 0) {
            row.cards.push(advertisements.pop()!)
        }
        rows.push(row);
    }
    rows = rows.reverse();

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











