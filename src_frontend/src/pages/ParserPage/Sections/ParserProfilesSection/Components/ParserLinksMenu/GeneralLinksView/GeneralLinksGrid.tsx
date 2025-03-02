import {TransportType} from "../../../../../Types/TransportType.ts";
import {Fade, Typography} from "@mui/material";

type Props = {
    gridColumnLength: number;
    transportTypes: TransportType[];
    onTransportTypeClick: (transportType: TransportType) => void;
}

export function GeneralLinksGrid(props: Props) {
    const columns: TransportType[][] = [];
    for (let i = 0; i < props.transportTypes.length; i += props.gridColumnLength) {
        const slice = props.transportTypes.slice(i, i + props.gridColumnLength);
        columns.push(slice);
    }

    return (
        <div style={{display: 'flex', flexDirection: 'column', gap: '8px'}}>
            <div>
                <Typography variant={"subtitle1"}>{"Нажмите на название"}</Typography>
                <Typography variant={"subtitle1"}>{"Чтобы добавить в редактируемый профиль"}</Typography>
            </div>
            <div style={{display: 'flex', gap: '15px'}}>
                {columns.map((column, i) => {
                    return (
                        <div key={i} style={{
                            flexBasis: '20%',
                            textAlign: 'center',
                            flexGrow: 1,
                            display: 'flex',
                            flexDirection: 'column',
                            gap: '8px'
                        }}>
                            {column.map((item) => {
                                return (
                                    <Fade in={true} timeout={500} key={item.name}>
                                        <Typography onClick={() => props.onTransportTypeClick(item)}
                                                    sx={{
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
                                            {item.name}
                                        </Typography>
                                    </Fade>
                                )
                            })}
                        </div>
                    )
                })}
            </div>
        </div>
    )
}