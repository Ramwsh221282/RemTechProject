import {useState} from "react";
import {ParserProfile} from "../../Types/ParserProfile.ts";
import {Card, CardContent, Typography} from "@mui/material";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";
import {RowsContainer} from "../../Components/RowsContainer.tsx";
import {ParserProfileCreateBar} from "./ParserProfileCreateBar.tsx";
import {ParserProfilesList} from "./ParserProfilesList.tsx";

export function ParserProfilesSection() {
    const [profiles, setProfiles] = useState<ParserProfile[]>([]);
    const notification = useNotification();

    function addProfile(profile: ParserProfile | string) {
        if (typeof profile === "string") {
            notification.showNotification({severity: "error", message: profile})
            return;
        }
        const copy = profiles.slice();
        copy.push(profile);
        setProfiles(copy);
        notification.showNotification({severity: "error", message: `Создан профиль с именем: ${profile.name}`})
    }

    function removeProfile(profile: ParserProfile | string) {
        if (typeof profile === "string") {
            notification.showNotification({severity: "error", message: profile})
            return;
        }
        const copy = profiles.slice();
        copy.splice(copy.indexOf(profile), 1);
        setProfiles(copy);
        notification.showNotification({severity: "error", message: `Удалён профиль с именем: ${profile.name}`})
    }

    return (
        <Card>
            <CardContent>
                <RowsContainer children={<Typography variant={"h5"}>{"Профили парсинга"}</Typography>}/>
                <RowsContainer children={<ParserProfileCreateBar createProfile={addProfile}/>}/>
                <RowsContainer children={<ParserProfilesList profiles={profiles} rowSize={5}/>}/>
            </CardContent>
            <NotificationAlert notification={notification.notification}
                               hideNotification={notification.hideNotification}/>
        </Card>
    )
}