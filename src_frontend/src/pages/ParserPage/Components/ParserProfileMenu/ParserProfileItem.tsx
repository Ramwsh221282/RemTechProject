import {ParserProfile, ParserProfileLinks} from "../../Types/ParserProfile.ts";
import {Accordion, AccordionDetails, AccordionSummary, Button, Divider, Fab, Typography} from "@mui/material";
import Grid from '@mui/material/Grid2';
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import {ParserProfileItemHeadPanel} from "./ParserProfileItemHeadPanel.tsx";
import {useEffect, useRef, useState} from "react";
import {parserPageProfilesActions} from "../../Store/Slices/ParserPageProfilesState.ts";
import {useDispatch, useSelector} from "react-redux";
import {RootParserPageDispatch, RootParserPageState} from "../../Store/ParserPageStore.ts";
import DeleteIcon from '@mui/icons-material/Delete';

export function ParserProfileItem({profile, orderedNumber, notify, onItemUpdate, onItemDelete}: {
    profile: ParserProfile,
    orderedNumber: number,
    notify: (severity: 'error' | 'info' | 'warning' | 'success', message: string) => void,
    onItemUpdate: (profile: ParserProfile) => void,
    onItemDelete: (profile: ParserProfile) => Promise<void>;
}) {
    const error = useSelector((state: RootParserPageState) => state.parserPageProfilesReducer.error);
    const currentProfile = useSelector((state: RootParserPageState) => state.parserPageProfilesReducer.currentProfile);
    const [currentProfileLinks, setCurrentProfileLinks] = useState<ParserProfileLinks[]>(currentProfile ? currentProfile.links : [])
    const [isActive, setActive] = useState(false);
    const actions = parserPageProfilesActions;
    const dispatch = useDispatch<RootParserPageDispatch>();
    const isFirstRender = useRef(true);

    function onItemLinkDelete(link: ParserProfileLinks) {
        if (!currentProfile) return;
        const newLinks = currentProfileLinks.filter(element => element.mark !== link.mark);
        dispatch(actions.setCurrentProfile({...currentProfile, links: newLinks}));
        setCurrentProfileLinks(newLinks);
    }

    useEffect(() => {
        if (currentProfile) {
            setCurrentProfileLinks(currentProfile.links);
        }
    }, [currentProfile]);

    useEffect(() => {
        if (error.trim().length > 0) {
            notify('error', error);
            dispatch(actions.cleanError())
        }
    }, [error]);

    useEffect(() => {
        if (isFirstRender.current) {
            isFirstRender.current = false;
            return;
        }
        if (isActive) {
            dispatch(actions.setCurrentProfile(profile));
            notify('info', `Редактирование Профиль ${orderedNumber + 1}`);
        } else {
            dispatch(actions.updateProfileAsync(currentProfile!))
            dispatch(actions.setCurrentProfile(null));
            onItemUpdate(currentProfile!);
            notify('success', `Профиль ${orderedNumber + 1} сохранён.`);
        }
    }, [isActive]);

    return (
        <Accordion>
            <AccordionSummary onClick={() => setActive(!isActive)} expandIcon={<ArrowDropDownIcon/>}
                              id={`profile-${profile.id}`}>
                <ParserProfileItemHeadPanel profile={currentProfile ? currentProfile : profile}
                                            orderedNumber={orderedNumber}/>
            </AccordionSummary>
            {currentProfile ? <AccordionDetails
                sx={{
                    backgroundColor: '#404040',
                }}>
                <Grid container spacing={10} sx={{maxHeight: "300px", overflow: 'auto'}}>
                    <Grid sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyItems: 'center',
                        alignItems: 'center',
                        gap: '10px',
                        marginLeft: '120px'
                    }}>
                        {profile.state ?
                            <Button size={"small"} variant={"contained"}
                                    onClick={() => dispatch(actions.updateProfileState())
                                    }>
                                {"Деактивировать"}
                            </Button> :
                            <Button size={"small"} variant={"contained"}
                                    onClick={() => dispatch(actions.updateProfileState())
                                    }>
                                {"Активировать"}
                            </Button>}
                        <Typography variant={"subtitle2"}>
                            {"Для добавления бренда"}
                        </Typography>
                        <Typography variant={"subtitle2"}>
                            {"нажмите на него в правой панели"}
                        </Typography>
                        <Button size={"small"} variant={"contained"} onClick={() => {
                            dispatch(actions.setCurrentProfile(profile))
                        }}>{"Сбросить изменения"}</Button>
                        <Button size={"small"} variant={"contained"} color={'error'} onClick={() => {
                            onItemDelete(profile)
                        }}>{"Удалить профиль"}</Button>
                    </Grid>
                    <Grid>
                        <Divider orientation={"vertical"}></Divider>
                    </Grid>
                    <Grid>
                        <Typography variant={"h6"}>
                            {"Бренды текущего профиля:"}
                        </Typography>
                        {currentProfileLinks.map(link => <ProfileItemLink key={link.mark} link={link}
                                                                          onDelete={onItemLinkDelete}/>)}
                    </Grid>
                </Grid>
            </AccordionDetails> : null}
        </Accordion>
    )
}

type ProfileItemLinkProps = {
    link: ParserProfileLinks;
    onDelete: (link: ParserProfileLinks) => void;
}

function ProfileItemLink(props: ProfileItemLinkProps) {
    return (
        <div className="flex flex-row gap-1 items-center p-1">
            <Fab onClick={() => props.onDelete(props.link)} sx={{fontSize: 10, height: '1px', width: '35px'}}
                 size={"small"}>
                <DeleteIcon sx={{fontSize: 20}}/>
            </Fab>
            <Typography sx={{color: '#FFC107'}} fontSize={16} variant={"h6"}>
                {props.link.mark}
            </Typography>
        </div>
    )
}