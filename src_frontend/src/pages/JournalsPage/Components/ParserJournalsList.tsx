import {ParserJournal} from "../Types/ParserJournal.ts";
import {Card, CardContent, Divider, Typography} from "@mui/material";

type Props = {
    journals: ParserJournal[]
}

export function ParserJournalsList(props: Props) {
    const rows: ParserJournal[][] = createJournalRows(props.journals);

    function createJournalRows(journals: ParserJournal[]) {
        const rowSize: number = 5;
        const rows: Array<Array<ParserJournal>> = [];
        for (let i = 0; i < journals.length; i += rowSize) {
            const slice = journals.slice(i, i + rowSize);
            rows.push(slice);
        }
        return rows;
    }


    return (
        <div className="flex flex-col gap-2">
            {rows.map((row, index: number) => <ParserJournalRow key={index} journals={row}/>)}
        </div>
    )
}

function ParserJournalRow(props: Props) {
    return (
        <div className="flex flex-row gap-2">
            {props.journals.map((journal) => <ParserJournalCard key={journal.id} journal={journal}/>)}
        </div>
    )
}

type JournalCardProps = {
    journal: ParserJournal;
}

function ParserJournalCard(props: JournalCardProps) {
    return (
        <>
            <Card>
                <CardContent>
                    <Typography variant={"h6"}>
                        {`Дата формирования: ${props.journal.createdOn}`}
                    </Typography>
                    <Divider/>
                    <Typography variant={"subtitle1"}>{"Затраченное время:"}</Typography>
                    <Typography
                        variant={"subtitle1"}>{`${props.journal.hours} ч. ${props.journal.minutes} м. ${props.journal.seconds} с.`}</Typography>
                    <Divider/>
                    {props.journal.isSuccess ? <Typography
                        sx={{margin: '5px', padding: '5px', backgroundColor: 'green', borderRadius: '10px'}}>
                        {"Успешен"}
                    </Typography> : <Typography
                        sx={{margin: '5px', padding: '5px', backgroundColor: 'red', borderRadius: '10px'}}>
                        {"Неуспешен"}
                    </Typography>}
                    <Divider/>
                    {props.journal.error.trim().length > 0 ?
                        <Typography variant={"subtitle2"}>{props.journal.error}</Typography> : null}
                    <Divider></Divider>
                    {props.journal.isSuccess ? <Typography
                        variant={"subtitle2"}>{`Получено данных: ${props.journal.itemsParsed}`}</Typography> : null}
                    <Divider></Divider>
                </CardContent>
            </Card>
        </>
    )
}