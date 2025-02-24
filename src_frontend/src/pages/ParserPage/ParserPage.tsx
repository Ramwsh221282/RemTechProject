import {ParserProfileService, useParserProfile} from "./Services/ParserProfileService.ts";
import {useEffect} from "react";
import {ParserPageRoot} from "./Components/ParserPageRoot.tsx";
import {Card, CircularProgress, Fade, Typography} from "@mui/material";
import {CreateParserProfileDialog} from "./Components/CreateParserProfileDialog.tsx";
import {ParserProfileMenu} from "./Components/ParserProfileMenu/ParserProfileMenu.tsx";

export const ParserPage = () => {
    const parserProfileService: ParserProfileService = useParserProfile();

    useEffect(() => {
        parserProfileService.fetchProfiles();
    }, []);

    if (parserProfileService.isLoading) {
        return (
            <ParserPageRoot>
                <CircularProgress size={200}/>
            </ParserPageRoot>
        )
    }

    if (parserProfileService.profiles.length === 0) {
        return (
            <ParserPageRoot>
                <CreateParserProfileDialog service={parserProfileService}/>
            </ParserPageRoot>
        )
    }

    return (

        <ParserPageRoot>
            <Fade in={true} timeout={500}>
                <div className="flex flex-row w-full h-full gap-5">
                    <div className="flex flex-col overflow-auto w-full h-full">
                        <ParserProfileMenu profiles={parserProfileService.profiles}/>
                    </div>
                    <div className="flex flex-col overflow-auto w-full h-full">
                        <Card>
                            <Typography sx={{padding: '5px'}} variant={"h5"}>
                                {"Марки:"}
                            </Typography>
                        </Card>
                    </div>
                </div>
            </Fade>
        </ParserPageRoot>
    )
}