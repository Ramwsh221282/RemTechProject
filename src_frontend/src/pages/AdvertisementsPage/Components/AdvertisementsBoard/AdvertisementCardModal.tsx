import {Advertisement} from "../../Types/AdvertisementsPageTypes.ts";
import {Modal} from "../../../../components/Modal.tsx";
import {Box, Button, CircularProgress, ImageList, ImageListItem, Tab, Tabs, Typography} from "@mui/material";
import * as React from "react";
import {useEffect, useState} from "react";

type AdvertisementCardModalProps = {
    isOpen: boolean;
    handleClose: () => void;
    card: Advertisement
}

type TabPanelProps = {
    children?: React.ReactNode;
    index: number;
    value: number;
}

function TabPanel(props: TabPanelProps) {
    const {children, value, index} = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}>
            {value === index && <Box sx={{p: 3}}>{children}</Box>}
        </div>
    );
}

export function AdvertisementCardModal({isOpen, handleClose, card}: AdvertisementCardModalProps) {
    const [value, setValue] = useState(0);
    const [imagesLoaded, setImagesLoaded] = useState(false);

    useEffect(() => {
        if (isOpen && value === 2) {
            setImagesLoaded(false);

            const loadImage = (url: string) => {
                return new Promise((resolve, reject) => {
                    const img = new Image();
                    img.src = url;
                    img.onload = resolve;
                    img.onerror = reject;
                });
            };

            Promise.all(card.imageLinks.map((url) => loadImage(url)))
                .then(() => setImagesLoaded(true))
                .catch((_) => {
                    setImagesLoaded(true);
                });
        }
    }, [isOpen, value, card.imageLinks]);

    const handleChange = (_: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    }

    return (
        <>
            <Modal
                isOpen={isOpen}
                onClose={handleClose}
                title={card.title}
                actions={<Button onClick={handleClose}>Закрыть</Button>}>
                <Box sx={{borderBottom: 1, borderColor: 'divider'}}>
                    <Tabs value={value} onChange={handleChange} variant={"fullWidth"}>
                        <Tab label={"Характеристики"}/>
                        <Tab label={"Описание"}/>
                        <Tab label={"Изображения"}/>
                    </Tabs>
                </Box>
                <TabPanel index={0} value={value}>
                    <div className={"flex flex-col"}>
                        {card.characteristics.map((ctx, index) => (
                            <div key={index} className={"flex flex-row justify-between"}>
                                <Typography fontSize={"medium"}>
                                    {ctx.name}
                                </Typography>
                                <Typography fontSize={"medium"}>
                                    {ctx.value}
                                </Typography>
                            </div>
                        ))}
                    </div>
                </TabPanel>
                <TabPanel index={1} value={value}>
                    <Typography fontSize={"medium"}>
                        {card.description}
                    </Typography>
                </TabPanel>
                <TabPanel index={2} value={value}>
                    {value === 2 && !imagesLoaded ? (
                        <Box
                            sx={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                height: 450,
                            }}
                        >
                            <CircularProgress/>
                        </Box>
                    ) : (
                        <ImageList sx={{width: 500, height: 450}} cols={3} rowHeight={164}>
                            {card.imageLinks.map((item, index) => (
                                <ImageListItem key={index}>
                                    <img
                                        srcSet={`${item}?w=164&h=164&fit=crop&auto=format&dpr=2 2x`}
                                        src={`${item}?w=164&h=164&fit=crop&auto=format`}
                                        alt={"image-" + index}
                                        loading="lazy"
                                    />
                                </ImageListItem>
                            ))}
                        </ImageList>
                    )}
                </TabPanel>
            </Modal>
        </>
    )
}