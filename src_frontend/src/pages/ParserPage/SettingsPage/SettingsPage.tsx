import {useEffect, useState} from "react";
import {ParserState} from "./Types/ParserState.ts";
import {ParserStateService} from "./Services/ParserStateService.ts";
import {Button, Card, CardContent, Typography} from "@mui/material";
import {NotificationAlert, useNotification} from "../../../components/Notification.tsx";

export function SettingsPage() {
    const [parserState, setParserState] = useState<ParserState>({isEnabled: false, description: 'Выключен'});
    const notifications = useNotification();

    useEffect(() => {
        const fetching = async () => {
            const state: ParserState = await ParserStateService.get();
            setParserState(state);
        }
        fetching();
    }, []);

    async function enable() {
        const result = await ParserStateService.enable();
        if (typeof result === 'string') {
            notifications.showNotification({severity: "error", message: result})
            return;
        }
        setParserState({...parserState, isEnabled: true, description: result.description});
        notifications.showNotification({severity: 'success', message: 'Парсер включен'});
    }

    async function disable() {
        const result = await ParserStateService.disable();
        if (typeof result === 'string') {
            notifications.showNotification({severity: "error", message: result})
            return;
        }
        setParserState({...parserState, isEnabled: false, description: result.description});
        notifications.showNotification({severity: 'success', message: 'Парсер выключен'});
    }

    async function restart() {
        const result = await ParserStateService.restart();
        if (typeof result === 'string') {
            notifications.showNotification({severity: "error", message: result})
            return;
        }
        setParserState({...parserState, isEnabled: true, description: result.description});
        notifications.showNotification({severity: 'success', message: 'Парсер перезапущен'});
    }

    return (
        <Card sx={{width: '20%'}}>
            <CardContent sx={{display: 'flex', flexDirection: 'column', gap: '2px'}}>
                <Typography variant={"h5"}>Состояние
                    парсера:</Typography>
                {parserState.isEnabled ? <Typography sx={{
                        padding: '10px',
                        backgroundColor: 'green',
                        borderRadius: '10px',
                        marginTop: '20px',
                        width: '30%'
                    }}
                                                     variant={"button"}>{parserState.description}</Typography> :
                    <Typography sx={{
                        padding: '10px',
                        backgroundColor: 'red',
                        borderRadius: '10px',
                        marginTop: '20px',
                        width: '30%'
                    }}
                                variant={"button"}>{parserState.description}</Typography>}
                <div className="flex flex-row gap-1 my-auto">
                    {parserState.isEnabled ? <Button onClick={disable} color={"error"}>Выключить</Button> :
                        <Button onClick={enable} color={"success"}>Включить</Button>}
                    <Button onClick={restart} color={"warning"}>Перезапустить</Button>
                </div>
            </CardContent>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </Card>
    )
}