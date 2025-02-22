import {useStatisticsService} from "../Services/StatisticsApiService.ts";
import {useEffect} from "react";
import {CircularProgress, LinearProgress, Skeleton, TextField} from "@mui/material";
import {NotificationAlert, useNotification} from "../../../components/Notification.tsx";

export function StatisticsInfo() {
    const service = useStatisticsService();
    const notifications = useNotification();

    useEffect(() => {
        service.fetchStatistics();
    }, []);

    if (service.error.trim().length > 0) {
        notifications.showNotification({severity: "error", message: service.error})
        return (
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        )
    }

    if (service.isLoading) {
        return (
            <div
                className="flex
                flex-col
                gap-3
                py-3
                px-3
                bg-amber-950
                border-2
                border-amber-900
                rounded-md
                shadow-neutral-800
                shadow-md">
                <h3 className="text-2xl text-amber-50 underline">{"Статистическая информация"}</h3>
                <LinearProgress color="primary"/>
                <Skeleton variant={"rectangular"} width={"maxWidth"} height={40}></Skeleton>
                <LinearProgress color="primary"/>
                <Skeleton variant={"rectangular"} width={"maxWidth"} height={40}></Skeleton>
                <LinearProgress color="primary"/>
                <Skeleton variant={"rectangular"} width={"maxWidth"} height={40}></Skeleton>
            </div>
        )
    }

    return (
        <div
            className="flex
                flex-col
                gap-3
                py-3
                px-3
                bg-amber-950
                border-2
                border-amber-900
                rounded-md
                shadow-neutral-800
                shadow-md">
            <h3 className="text-2xl text-amber-50 underline">{"Статистическая информация"}</h3>
            <TextField size={"small"}
                       value={service.statistics.count}
                       aria-readonly={true}
                       label={"Количество"}>
                <LinearProgress color="secondary"/>
                <LinearProgress color="success"/>
                <LinearProgress color="inherit"/>
                <CircularProgress color={"primary"}/>
            </TextField>
            <TextField size={"small"}
                       value={service.statistics.averagePrice}
                       aria-readonly={true}
                       label={"Средняя цена"}>
            </TextField>
            <TextField size={"small"} value={service.statistics.maxPrice} aria-readonly={true}
                       label={"Максимальная цена"}></TextField>
            <TextField size={"small"} value={service.statistics.minPrice} aria-readonly={true}
                       label={"Минимальная цена"}></TextField>
        </div>
    )
}