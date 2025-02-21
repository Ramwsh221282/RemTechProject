import {useCallback, useEffect, useState} from "react";
import {Alert, Snackbar} from "@mui/material";

type NotificationProps = {
    severity: "info" | "warning" | "error" | "success";
    message: string;
    duration?: number;
};

export function useNotification() {
    const [notification, setNotification] = useState<NotificationProps | null>(null);

    const hideNotification = useCallback(() => {
        setNotification(null);
    }, []);

    useEffect(() => {
        if (notification) {
            const timer = setTimeout(() => {
                setNotification(null);
            }, notification.duration ?? 5000);
            return () => clearTimeout(timer);
        }
    }, [notification]);

    const showNotification = useCallback(
        (
            props: Omit<NotificationProps, "duration">,
            duration?: number
        ) => {
            setNotification({...props, duration: duration ?? 5000});
        },
        []
    );

    return {notification, showNotification, hideNotification};
}

type NotificationAlertProps = {
    notification: NotificationProps | null;
    hideNotification: () => void;
};

export function NotificationAlert({notification, hideNotification}: NotificationAlertProps) {
    if (!notification) return null;

    return (
        <Snackbar
            open={true}
            autoHideDuration={notification.duration}
            onClose={hideNotification}
            anchorOrigin={{vertical: "bottom", horizontal: "right"}}
        >
            <Alert severity={notification.severity} onClose={hideNotification}>
                {notification.message}
            </Alert>
        </Snackbar>
    );
}
