import {Button, Card, CardContent, Divider, Fade, Typography} from "@mui/material";
import {RowsContainer} from "./Components/RowsContainer.tsx";
import {NavLink} from "react-router";


export const ParserPage = () => {
    function createPageNavigationCardProps(): ParserPageNavigationCardProps[] {
        const props: ParserPageNavigationCardProps[] = [
            {
                title: 'Профили парсинга',
                description: ['Настройка ссылок парсера', 'Создание профилей парсера'],
                path: 'profiles',
                development: 'Done'
            },
            {
                title: 'Управление парсером Avito',
                description: ['Настройка времени ожидания', 'Включение, выключение, перезапуск'],
                path: 'parser/avito-settings',
                development: 'WIP'
            },
        ];
        return props;
    }


    return (
        <>
            <Fade in={true} timeout={500}>
                <div className="flex flex-col w-full h-full">
                    <RowsContainer
                        children={createPageNavigationCardProps().map((prop, index) => <ParserPageNavigationCard
                            key={index} {...prop} />)}/>
                </div>
            </Fade>
        </>
    )
}

type ParserPageNavigationCardProps = {
    title: string;
    description: string[];
    path: string;
    development?: string | null;
}

function ParserPageNavigationCard(props: ParserPageNavigationCardProps) {
    return (
        <Card>
            <CardContent>
                <div className="flex flex-row gap-1">
                    <Typography variant={"h5"}>{props.title}</Typography>
                    {props.development ? props.development == 'Done' ?
                        <Typography sx={{padding: '5px', backgroundColor: 'green', borderRadius: '10px'}}
                                    variant={"h5"}>{props.development}</Typography> :
                        <Typography
                            sx={{padding: '5px', backgroundColor: 'yellow', color: 'black', borderRadius: '10px'}}
                            variant={"h5"}>{props.development}</Typography> : null}
                </div>
                <Divider/>
                <div className="flex flex-col">
                    {props.description.map((description) => <Typography sx={{fontSize: '1rem'}} variant={"overline"}
                                                                        key={description}>{description}</Typography>)}
                </div>
                <Divider/>
                <div className="flex flex-row justify-center my-1">
                    {props.development ? props.development == 'Done' ? <NavLink to={props.path}>
                        <Button>{"Перейти"}</Button>
                    </NavLink> : null : null}
                </div>
            </CardContent>
        </Card>
    )
}

