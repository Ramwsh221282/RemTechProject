import {ParserProfile, ParserProfileLink} from "../../../../Types/ParserProfile.ts";
import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Button,
    Divider,
    Fab, Fade,
    Grid2,
    TextField,
    Typography
} from "@mui/material";
import {ExpandMore} from "@mui/icons-material";
import DeleteIcon from '@mui/icons-material/Delete';
import {ParserProfileService} from "../../../../Services/ParserProfileService.ts";
import {useState} from "react";

type Props = {
    onDelete: (profile: ParserProfile | string) => void,
    onUpdate: (profile: ParserProfile | string) => void,
    onToggle: (profile: ParserProfile) => void,
    onLinkRemove: (link: ParserProfileLink) => void,
    profile: ParserProfile,
}

export function ParserProfileCard(props: Props) {
    const [profile, setProfile] = useState<ParserProfile>(props.profile);

    async function deleteProfile() {
        const result = await ParserProfileService.removeParserProfile(props.profile);
        if (result.trim().length > 0) {
            props.onDelete(result);
            return;
        }
        props.onDelete(props.profile);
    }

    function ProfileLinkRow(link: ParserProfileLink) {
        return (
            <div
                className="flex flex-row gap-1 justify-center items-center p-1 bg-neutral-800 rounded-md">
                <div className="flex flex-col gap-1 w-100">
                    <TextField variant={"standard"} autoComplete={"off"} fullWidth size={"small"}
                               aria-readonly={true} value={link.name}/>
                    <TextField variant={"standard"} autoComplete={"off"} fullWidth size={"small"}
                               aria-readonly={true} value={link.link}/>
                </div>
                <Fab onClick={() => props.onLinkRemove(link)} size={"small"}>
                    <DeleteIcon/>
                </Fab>
            </div>
        )
    }

    function toggleProfileState(): void {
        const copy = {...profile};
        copy.state = !copy.state;
        setProfile(copy);
    }

    async function updateProfile() {
        const updatedProfile = {...profile};
        const result = await ParserProfileService.updateParserProfile(updatedProfile);
        if (result.trim().length > 0) {
            props.onUpdate(result)
            return;
        }
        props.onUpdate(updatedProfile);
    }

    return (
        <>
            <Fade in={true} timeout={500}>
                <Accordion>
                    <AccordionSummary onClick={() => props.onToggle(profile)} expandIcon={<ExpandMore/>}>
                        <div className="flex flex-col">
                            <div className="inline-flex flex-row justify-between gap-1">
                                <Typography sx={{fontSize: '1rem'}}
                                            variant={"overline"}>{`${profile.name} /`}</Typography>
                                <Typography sx={{fontSize: '1rem'}}
                                            variant={"overline"}>{`Дата создания: ${profile.createdOn} /`}</Typography>
                                <Typography sx={{fontSize: '1rem'}}
                                            variant={"overline"}>{`${profile.stateDescription} /`}</Typography>
                                <Typography sx={{fontSize: '1rem'}}
                                            variant={"overline"}>{`Ссылок: ${profile.links.length}`}</Typography>
                            </div>
                            <Typography variant={'button'}
                                        sx={{
                                            fontSize: '0.75rem',
                                            textDecoration: 'underline'
                                        }}>{"Редактировать"}</Typography>
                        </div>
                    </AccordionSummary>
                    <AccordionDetails sx={{backgroundColor: '#272727'}}>
                        <Grid2 container={true} spacing={4}>
                            <Grid2>
                                <div className="flex flex-col gap-1">
                                    {profile.state ? <Button color={'error'}
                                                             onClick={toggleProfileState}>{'Деактивировать'}</Button> :
                                        <Button color={'success'}
                                                onClick={toggleProfileState}>{'Активировать'}</Button>}
                                    <Button color={'info'} onClick={updateProfile}>{'Сохранить изменения'}</Button>
                                    <Button color={'error'} onClick={deleteProfile}>{'Удалить'}</Button>
                                </div>
                            </Grid2>
                            <Grid2>
                                <Divider orientation={'vertical'}/>
                            </Grid2>
                            <Grid2>
                                <div
                                    className="flex flex-col gap-1 w-full h-100 overflow-auto justify-start items-start">
                                    <Typography variant={"button"}>Ссылки:</Typography>
                                    {profile.links.length == 0 ? <Typography variant={"button"}>Ссылок
                                        нет</Typography> : profile.links.map((link: ParserProfileLink, index: number) =>
                                        <ProfileLinkRow
                                            key={index} {...link}/>)}
                                </div>
                            </Grid2>
                        </Grid2>
                    </AccordionDetails>
                </Accordion>
            </Fade>
        </>
    )
}