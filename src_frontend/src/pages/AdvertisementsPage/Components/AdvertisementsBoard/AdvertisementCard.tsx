import {useModal} from "../../../../components/Modal.tsx";
import {Advertisement} from "../../Types/AdvertisementsPageTypes.ts";
import {Button, Card, CardContent, Divider, Typography} from "@mui/material";
import {AdvertisementCardModal} from "./AdvertisementCardModal.tsx";

export function AdvertisementCard(advertisement: Advertisement) {
    const modal = useModal();

    return (
        <div className="max-h-22 max-w-52">
            <Card sx={{boxShadow: '0 0 5px 1px #000'}}>
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
                    <Divider orientation={"horizontal"} flexItem={true}/>
                    <div className="flex flex-col underline items-center">
                        <Typography fontSize={"smaller"} noWrap={true} component={"span"}>
                            {"Адрес:"}
                        </Typography>
                        <Typography fontSize={"medium"} textOverflow={"ellipsis"}
                                    noWrap={true}
                                    component={"span"}>
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
                        <Button fullWidth={false} onClick={modal.open} size={"small"} variant={"outlined"}>
                            <span className="text-sm">Подробнее</span>
                        </Button>
                    </div>
                </CardContent>
            </Card>
            <AdvertisementCardModal isOpen={modal.isOpen} handleClose={modal.close} card={advertisement}/>
        </div>
    )
}