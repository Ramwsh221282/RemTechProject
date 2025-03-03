import {Advertisement} from "../../Types/AdvertisementsPageTypes.ts";
import {Modal} from "../../../../components/Modal.tsx";
import {Box, Button, CircularProgress, ImageList, ImageListItem, Link, Typography} from "@mui/material";
import {useEffect, useState} from "react";
import {CustomTabs} from "../../../../components/CustomTabPanel.tsx";

type AdvertisementCardModalProps = {
    isOpen: boolean;
    handleClose: () => void;
    card: Advertisement
}

export function AdvertisementCardModal({isOpen, handleClose, card}: AdvertisementCardModalProps) {
    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title={card.title}
            actions={<Button onClick={handleClose}>Закрыть</Button>}
        >
            <CustomTabs
                panels={[
                    {
                        index: 0,
                        title: "Характеристики",
                        children: <CharacteristicsPanel card={card}/>,
                    },
                    {
                        index: 1,
                        title: "Описание",
                        children: <DescriptionPanel card={card}/>,
                    },
                    {
                        index: 2,
                        title: "Изображения",
                        children: <ImagesPanel card={card}/>,
                    },
                ]}
            />
        </Modal>
    );
}

function CharacteristicsPanel({card}: { card: Advertisement }) {

    function onLinkClick() {
        window.open(card.sourceUrl, "_blank", "noreferrer");
    }


    return (
        <div className={"flex flex-col"}>
            {card.characteristics.map((ctx, index) => (
                <div key={index} className={"flex flex-row justify-between"}>
                    <Typography fontSize={"medium"}>{ctx.name}</Typography>
                    <Typography fontSize={"medium"}>{ctx.value}</Typography>
                </div>
            ))}
            <Link onClick={onLinkClick} sx={{cursor: 'pointer', userSelect: 'none'}}>{"Ссылка на источник"}</Link>
        </div>
    );
}

function DescriptionPanel({card}: { card: Advertisement }) {
    return (
        <Typography fontSize={"medium"}>{card.description}</Typography>
    );
}

function ImagesPanel({card}: { card: Advertisement }) {
    const [imagesLoaded, setImagesLoaded] = useState(false);

    useEffect(() => {
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
    }, [imagesLoaded]);

    return (
        <>
            {!imagesLoaded ? (
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
        </>
    );
}