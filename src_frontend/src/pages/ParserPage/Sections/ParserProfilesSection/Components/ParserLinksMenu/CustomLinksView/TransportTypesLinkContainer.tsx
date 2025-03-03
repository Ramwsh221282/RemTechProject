import {TransportType} from "../../../../../Types/TransportType.ts";
import {Fab, Fade, Typography} from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';

type Props = {
    onDelete(result: TransportType): void;
    onSelect(result: TransportType): void;
    types: TransportType[];
    columnSize: number;
}

export function TransportTypesLinkContainer(props: Props) {
    function createColumnsOfItems(): TransportType[][] {
        const columns: TransportType[][] = [];
        for (let i = 0; i < props.types.length; i += props.columnSize) {
            const slice = props.types.slice(i, i + props.columnSize);
            columns.push(slice);
        }
        return columns;
    }

    return (
        <section className="flex flex-row gap-1 h-100">
            {createColumnsOfItems().map((column, index) => {
                return (
                    <div key={index} className="flex flex-col gap-2 items-center">
                        {column.map((type) => {
                            return <TransportTypeItem onDelete={props.onDelete} onSelect={props.onSelect}
                                                      transportType={type}/>
                        })}
                    </div>
                )
            })}
        </section>
    )
}

type TransportTypeItemProps = {
    onDelete(result: TransportType | string): void;
    onSelect(result: TransportType): void;
    transportType: TransportType;
}

function TransportTypeItem(props: TransportTypeItemProps) {

    function deleteTransportType() {
        props.onDelete(props.transportType);
    }

    function selectTransportType() {
        props.onSelect(props.transportType);
    }

    return (
        <Typography>
            <Fade in={true} timeout={500} key={props.transportType.name}>
                <div className="flex flex-row gap-1">
                    <Typography onClick={selectTransportType}
                                sx={{
                                    textAlign: 'center',
                                    width: '100px',
                                    padding: '5px',
                                    borderRadius: '10px',
                                    backgroundColor: '#272727',
                                    userSelect: 'none',
                                    fontSize: '1rem',
                                    color: '#FFC107',
                                    cursor: 'pointer',
                                    transform: 'scale(1)',
                                    transition: 'transform .5s, text-decoration .5s',
                                    "&::before": {
                                        transform: 'scale(1)',
                                        transition: 'transform .3s',
                                    },
                                    ":hover": {
                                        transform: 'scale(1.05)',
                                        textDecoration: 'underline',
                                        transition: 'transform .5s, text-decoration .3s',
                                        "&::before": {
                                            transform: 'scale(1)',
                                            textDecoration: 'underline',
                                            transition: 'transform .5s, text-decoration .3s',
                                        }
                                    }
                                }}
                    >
                        {props.transportType.name}
                    </Typography>
                    <Fab onClick={deleteTransportType}
                         sx={{height: '10px', width: '35px'}} size={"small"}>
                        <ClearIcon/>
                    </Fab>
                </div>
            </Fade>
        </Typography>
    )
}