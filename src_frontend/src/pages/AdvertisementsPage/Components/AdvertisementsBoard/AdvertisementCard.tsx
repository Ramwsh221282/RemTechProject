import {Advertisement} from "../../Types/AdvertisementsPageTypes.ts";
import {Button, Card, CardContent, Divider, Typography} from "@mui/material";

export function AdvertisementCard(advertisement: Advertisement) {
    return (
        <div className="max-h-24 max-w-52">
            <Card>
                <CardContent>
                    <Typography fontSize={"medium"} gutterBottom={true}>
                        <span className="line-clamp-3">
                            {advertisement.title}
                        </span>
                    </Typography>
                    <Divider orientation={"horizontal"} flexItem={true}/>
                    <div className="flex flex-row justify-between underline gap-5">
                        <Typography fontSize={"medium"} gutterBottom={true} component={"div"}>
                            {"Цена:"}
                        </Typography>
                        <Typography fontSize={"medium"} gutterBottom={true} component={"div"}>
                            {advertisement.price.value}
                        </Typography>
                    </div>
                    <div className="flex flex-row underline justify-between  gap-5">
                        <Typography fontSize={"medium"} gutterBottom={true} variant={"h5"} component={"div"}>
                            {"Адрес:"}
                        </Typography>
                        <Typography fontSize={"medium"} gutterBottom={true} variant={"h5"} component={"div"}>
                            {advertisement.address.split(',')[0]}
                        </Typography>
                    </div>
                    <Divider orientation={"horizontal"} flexItem={true}/>
                    <Typography fontSize={"small"}>
                        <span
                            className="line-clamp-2">{advertisement.description}</span>
                    </Typography>
                    <Divider orientation={"horizontal"} flexItem={true}/>
                    <div className="py-2 flex flex-col gap-3 items-center">
                        <Button fullWidth={false} size={"small"} variant={"outlined"}
                                color={"error"}><span className="text-sm">Удалить</span>
                        </Button>
                        <Button fullWidth={false} size={"small"} variant={"outlined"}>
                            <span className="text-sm">Характеристики</span>
                        </Button>
                    </div>
                </CardContent>
            </Card>
        </div>
    )
}