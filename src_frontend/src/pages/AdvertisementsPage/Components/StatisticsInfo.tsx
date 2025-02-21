import {Statistics} from "../Utils/AdvertisementsApi.ts";
import {CircularProgress, LinearProgress, Skeleton, TextField} from "@mui/material";

type StatisticsInfoProps = {
    statistics: Statistics;
    isLoaded: boolean;
}

export function StatisticsInfo({statistics, isLoaded}: StatisticsInfoProps) {

    function ContentLoading() {
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

    function Content() {
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
                           value={statistics.count}
                           aria-readonly={true}
                           label={"Количество"}>
                    <LinearProgress color="secondary"/>
                    <LinearProgress color="success"/>
                    <LinearProgress color="inherit"/>
                    <CircularProgress color={"primary"}/>
                </TextField>
                <TextField size={"small"}
                           value={statistics.averagePrice}
                           aria-readonly={true}
                           label={"Средняя цена"}>
                </TextField>
                <TextField size={"small"} value={statistics.maxPrice} aria-readonly={true}
                           label={"Максимальная цена"}></TextField>
                <TextField size={"small"} value={statistics.minPrice} aria-readonly={true}
                           label={"Минимальная цена"}></TextField>
            </div>
        )
    }

    return (
        isLoaded ? <ContentLoading/> : <Content/>
    )
}