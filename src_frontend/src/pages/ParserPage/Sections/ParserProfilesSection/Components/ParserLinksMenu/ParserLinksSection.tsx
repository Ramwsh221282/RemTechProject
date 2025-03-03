import {CustomTabs} from "../../../../../../components/CustomTabPanel.tsx";
import {Card, CardContent, Typography} from "@mui/material";
import {GeneralLinksView} from "./GeneralLinksView/GeneralLinksView.tsx";
import {TransportType} from "../../../../Types/TransportType.ts";
import {CustomLinksView} from "./CustomLinksView/CustomLinksView.tsx";

type Props = {
    onTransportTypeClick(transportType: TransportType): void;
}

export function ParserLinksSection(props: Props) {
    return (
        <Card sx={{
            width: '50%', overflow: 'auto'
        }}>
            <CardContent sx={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
                <Typography variant={"h5"}>Ссылки:</Typography>
                <CustomTabs panels=
                                {
                                    [
                                        {
                                            index: 0,
                                            title: 'Общие',
                                            children: <GeneralLinksView
                                                onTransportTypeClick={props.onTransportTypeClick}/>
                                        },
                                        {
                                            index: 1,
                                            title: 'Пользовательские',
                                            children: <CustomLinksView
                                                onTransportTypeClick={props.onTransportTypeClick}/>
                                        }
                                    ]}/>
            </CardContent>
        </Card>
    )
}