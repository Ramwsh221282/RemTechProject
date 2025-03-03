import {ParserProfilesMenu} from "./Components/ParserProfilesMenu/ParserProfilesMenu.tsx";
import {ParserLinksSection} from "./Components/ParserLinksMenu/ParserLinksSection.tsx";
import {TransportType} from "../../Types/TransportType.ts";
import {useEffect, useState} from "react";
import {ParserProfile, ParserProfileLink} from "../../Types/ParserProfile.ts";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";
import {ParserProfileService} from "../../Services/ParserProfileService.ts";

export function ParserProfilesSection() {
    const [selectedProfile, setSelectedProfile] = useState<ParserProfile | null>(null);
    const [profiles, setProfiles] = useState<ParserProfile[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false)
    const notifications = useNotification();

    useEffect(() => {
        const fetch = async () => {
            setIsLoading(true);
            const profiles = await ParserProfileService.getParserProfiles();
            setIsLoading(false);
            if (typeof profiles === 'string') {
                notifications.showNotification({severity: 'error', message: profiles})
                return;
            }
            setProfiles(profiles);
        }
        fetch();
    }, []);

    function addProfile(profile: ParserProfile | string) {
        if (typeof profile === 'string') {
            notifications.showNotification({severity: 'error', message: profile})
            return;
        }
        const copy = profiles.slice();
        copy.push(profile);
        setProfiles(copy);
        notifications.showNotification({severity: 'success', message: `Добавлен профиль: ${profile.name}`})
    }

    function removeProfile(profile: ParserProfile | string) {
        if (typeof profile === 'string') {
            notifications.showNotification({severity: 'error', message: profile})
            return;
        }
        setProfiles(profiles.filter((p => p.id !== profile.id)));
    }

    function onTransportTypeClick(transportType: TransportType) {
        if (!selectedProfile) {
            notifications.showNotification({severity: "error", message: 'Необходимо выбрать профиль'})
            return;
        }
        const profileLink: ParserProfileLink = {
            link: transportType.link,
            name: transportType.name,
            additions: null
        }
        const copy = profiles.slice();
        const index = copy.findIndex(p => p.id === selectedProfile.id);
        copy[index].links.push(profileLink);
        setProfiles(copy);
    }

    function updateParserProfile(profile: ParserProfile | string): void {
        if (!selectedProfile) {
            notifications.showNotification({severity: 'error', message: "Необходимо выбрать профиль"})
            return;
        }
        if (typeof profile === 'string') {
            notifications.showNotification({severity: 'error', message: profile})
            return;
        }
        const copy = profiles.slice();
        const index = copy.findIndex(p => p.id === profile.id);
        copy[index] = {...profile}
        setSelectedProfile({...profile})
        setProfiles(copy);
        notifications.showNotification({severity: 'success', message: `Обновлен профиль: ${profile.name}`})
        return;
    }

    function selectParserProfile(parserProfile: ParserProfile) {
        if (!selectedProfile) {
            setSelectedProfile({...parserProfile});
        } else
            setSelectedProfile(null);
    }

    function removeParserProfileLink(link: ParserProfileLink) {
        if (!selectedProfile) {
            notifications.showNotification({severity: "error", message: 'Необходимо выбрать профиль'});
            return;
        }
        const copy = profiles.slice();
        const index = copy.findIndex(p => p.id === selectedProfile.id);
        copy[index].links = copy[index].links.filter(l => l.name !== link.name);
        setProfiles(copy);
    }

    return (
        <section className="inline-flex flex-row gap-1 w-full">
            <ParserProfilesMenu isLoading={isLoading}
                                profiles={profiles}
                                onParserProfileLinkRemove={removeParserProfileLink}
                                onParserProfileAdd={addProfile}
                                onParserProfileDelete={removeProfile}
                                onParserProfileUpdate={updateParserProfile}
                                onParserProfileSelect={selectParserProfile}/>
            <ParserLinksSection onTransportTypeClick={onTransportTypeClick}/>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </section>
    )
}