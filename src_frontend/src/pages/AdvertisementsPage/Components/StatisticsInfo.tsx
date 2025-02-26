import {Fade, LinearProgress, Skeleton, TextField} from "@mui/material";
import {NotificationAlert, useNotification} from "../../../components/Notification.tsx";
import {StatisticsService} from "../Services/StatisticsApiService.ts";
import {useEffect} from "react";

export function StatisticsInfo({service}: { service: StatisticsService }) {
    const notifications = useNotification();

    useEffect(() => {
        if (service.error.trim().length > 0) {
            notifications.showNotification({severity: "error", message: service.error});
        }
    }, [service.error]);

    if (service.isLoading) {
        return (
            <div
                className="flex flex-col gap-3 py-3 px-3 bg-[#1E1E1E] rounded-md shadow-neutral-800 shadow-md">
                <h3 className="text-2xl text-amber-50 underline">Статистическая информация</h3>
                <LinearProgress color="primary"/>
                <Skeleton variant="rectangular" width="maxWidth" height={40}/>
                <LinearProgress color="primary"/>
                <Skeleton variant="rectangular" width="maxWidth" height={40}/>
                <LinearProgress color="primary"/>
                <Skeleton variant="rectangular" width="maxWidth" height={40}/>
            </div>
        );
    }

    return (
        <Fade in={true} timeout={500}>
            <div
                className="flex flex-col gap-3 py-3 px-3 bg-[#1E1E1E] rounded-md shadow-neutral-800 shadow-md">
                <h3 className="text-2xl text-amber-50 underline">Статистическая информация</h3>
                <TextField size="small" value={service.statistics.count} aria-readonly={true} label="Количество"/>
                <TextField size="small" value={service.statistics.averagePrice} aria-readonly={true}
                           label="Средняя цена"/>
                <TextField size="small" value={service.statistics.maxPrice} aria-readonly={true}
                           label="Максимальная цена"/>
                <TextField size="small" value={service.statistics.minPrice} aria-readonly={true}
                           label="Минимальная цена"/>
                <NotificationAlert notification={notifications.notification}
                                   hideNotification={notifications.hideNotification}/>
            </div>
        </Fade>
    );
}