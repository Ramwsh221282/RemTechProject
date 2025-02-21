import * as React from "react";
import {FormEvent, useCallback, useState} from "react";
import {Dialog, DialogActions, DialogContent, DialogTitle} from "@mui/material";

export function useModal(timeout: number = 300) {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [data, setData] = useState<any>(null);

    const open = useCallback((modalData: any = null) => {
        setIsOpen(true);
        if (modalData) {
            setData(modalData);
        }
    }, []);

    const close = useCallback(() => {
        setIsOpen(false);
        setTimeout(() => {
            setData(null);
        }, timeout);
    }, [timeout]);

    return {
        isOpen,
        data,
        open,
        close
    };
}

type ModalProps = {
    isOpen: boolean;
    onClose: () => void;
    title: string;
    children: React.ReactNode;
    actions: React.ReactNode;
    onSubmit?: (event: FormEvent<HTMLFormElement>) => void;
}

export function Modal({isOpen, onClose, title, children, actions, onSubmit}: ModalProps) {
    const content = (
        <>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>{children}</DialogContent>
            <DialogActions>{actions}</DialogActions>
        </>
    )

    return (
        <Dialog open={isOpen} onClose={onClose}>
            {onSubmit ? (
                <form onSubmit={onSubmit}>
                    {content}
                </form>
            ) : (content)}
        </Dialog>
    )
}