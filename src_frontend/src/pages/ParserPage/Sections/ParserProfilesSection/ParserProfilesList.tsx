import {ParserProfile} from "../../Types/ParserProfile.ts";
import {Button, Card, CardContent, Divider, Typography} from "@mui/material";
import {useModal} from "../../../../components/Modal.tsx";
import {ParserProfileCardModal} from "./ParserProfileCardModal.tsx";

type Props = {
    profiles: ParserProfile[];
    rowSize: number;
}

export function ParserProfilesList(props: Props) {
    function splitIntoRows(): ParserProfile[][] {
        const rows: ParserProfile[][] = [];
        for (let i = 0; i < props.profiles.length; i += props.rowSize) {
            const slice = props.profiles.slice(i, i + props.rowSize);
            rows.push(slice);
        }
        return rows;
    }


    return (
        <div className="flex flex-col">
            {splitIntoRows().map((row, i) => <ParserProfileRow key={i} profiles={row}/>)}
        </div>
    )
}

type RowProps = {
    profiles: ParserProfile[];
}

function ParserProfileRow(props: RowProps) {
    const modal = useModal();

    return (
        <div className="flex flex-row p-1 gap-1">
            {props.profiles.map((profile) => <ParserProfileCard key={profile.id} {...profile} />)}
        </div>
    )
}

function ParserProfileCard(profile: ParserProfile) {
    const modal = useModal();

    return (
        <Card sx={{backgroundColor: '#262626'}}>
            <CardContent>
                <div className="flex flex-col">
                    <div className="flex flex-row justify-end mx-[-15px] my-[-15px] p-1">
                        {profile.state ?
                            <Typography variant={"overline"} sx={{
                                backgroundColor: 'green',
                                borderRadius: '10px',
                                padding: '5px',
                                fontSize: '0.8rem'
                            }}>{"Активный"}</Typography> :
                            <Typography variant={"caption"}
                                        sx={{
                                            backgroundColor: 'red',
                                            borderRadius: '10px',
                                            padding: '5px',
                                            fontSize: '0.8rem'
                                        }}>{"Неактивный"}</Typography>}
                    </div>
                    <Typography sx={{marginTop: '10px'}} variant={"overline"}>{`Название: ${profile.name}`}</Typography>
                    <Divider/>
                </div>
                <Typography variant={"overline"}>{`Создан: ${profile.createdOn}`}</Typography>
                <Divider/>
                <Typography variant={"overline"}>{`Ссылок: ${profile.links.length}`}</Typography>
                <Divider/>
                <div className="flex flex-row p-1 justify-center">
                    <Button size={"small"} variant={"outlined"} onClick={() => modal.open()}>Управление</Button>
                </div>
            </CardContent>
            <ParserProfileCardModal isOpen={modal.isOpen} handleClose={modal.close} profile={profile}/>
        </Card>
    )
}