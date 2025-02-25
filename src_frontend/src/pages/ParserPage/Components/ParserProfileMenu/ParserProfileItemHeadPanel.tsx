import {Typography} from "@mui/material";
import {ParserProfile} from "../../Types/ParserProfile.ts";
import {useState} from "react";

export function ParserProfileItemHeadPanel({profile, orderedNumber}: {
    profile: ParserProfile,
    orderedNumber: number,
}) {
    const [isEditing, setIsEditing] = useState(false);

    return (
        <div className="flex flex-row gap-10 w-full" onClick={() => {
            setIsEditing(!isEditing);
        }}>
            <Typography
                sx={{
                    borderRadius: '10px',
                    padding: '4px',
                    textDecoration: 'underline'
                }}>
                {`Профиль ${orderedNumber + 1}`}
            </Typography>
            {profile.state ? <Typography
                sx={{backgroundColor: 'green', borderRadius: '10px', padding: '4px'}}>
                {profile.stateDescription}
            </Typography> : <Typography
                sx={{backgroundColor: 'red', borderRadius: '10px', padding: '4px'}}>
                {profile.stateDescription}
            </Typography>}
            <Typography
                sx={{borderRadius: '10px', padding: '4px'}}>
                {`Дата создания: ${profile.createdOn}`}
            </Typography>
            <Typography
                sx={{
                    borderRadius: '10px',
                    padding: '4px',
                    textDecoration: 'underline'
                }}>
                {isEditing ? "Нажмите для сохранения" : "Нажмите для редактирования"}
            </Typography>
        </div>
    )
}