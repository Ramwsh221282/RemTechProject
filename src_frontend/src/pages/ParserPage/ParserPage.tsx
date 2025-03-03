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
}

function ParserPageNavigationCard(props: ParserPageNavigationCardProps) {
    return (
        <Card>
            <CardContent>
                <div className="flex flex-row gap-1">
                    <Typography variant={"h5"}>{props.title}</Typography>
                </div>
                <Divider/>
                <div className="flex flex-col">
                    {props.description.map((description) => <Typography sx={{fontSize: '1rem'}} variant={"overline"}
                                                                        key={description}>{description}</Typography>)}
                </div>
                <Divider/>
                <div className="flex flex-row justify-center my-1">
                    <NavLink to={props.path}>
                        <Button>{"Перейти"}</Button>
                    </NavLink>
                </div>
            </CardContent>
        </Card>
    )
}

