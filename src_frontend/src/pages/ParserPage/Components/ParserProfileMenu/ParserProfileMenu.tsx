import {Card, Typography} from "@mui/material";
import {ParserProfileMenuBarMemo} from "./ParserProfileMenuBar.tsx";
import {ParserProfileMenuContent} from "./ParserProfileMenuContent.tsx";
import {useState} from "react";
import {ParserProfile} from "../../Types/ParserProfile.ts";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";

export function ParserProfileMenu() {
    const [profiles, setProfiles] = useState<ParserProfile[]>([]);
    const notifications = useNotification();

    function addProfile(profile: ParserProfile | string) {
        if (typeof profile === "string") {
            notifications.showNotification({severity: 'error', message: profile})
            return;
        }
        const copy = profiles.slice();
        copy.push(profile);
        setProfiles(copy);
    }

    function removeProfile(profile: ParserProfile) {
        const copy = profiles.filter(p => p.id !== profile.id);
        setProfiles(copy);
    }

    function updateProfile(profile: ParserProfile) {
        const copy = profiles.slice();
        const index = profiles.findIndex(i => i.id === profile.id);
        copy[index] = {...profile};
        setProfiles(copy);
    }

    function initializeProfiles(profiles: ParserProfile[] | string) {
        if (typeof profiles === "string") {
            notifications.showNotification({severity: 'error', message: profiles})
            return;
        }
        setProfiles(profiles);
    }


    return (
        <Card>
            <Typography sx={{padding: '5px'}} variant={"h5"}>
                {"Профили парсинга:"}
            </Typography>
            <ParserProfileMenuBarMemo addNewProfile={addProfile}/>
            <div className="flex flex-col overflow-auto h-185.5">
                <ParserProfileMenuContent profiles={profiles} initializeProfiles={initializeProfiles}
                                          removeProfile={removeProfile} updateProfile={updateProfile}/>
            </div>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </Card>
    )
}