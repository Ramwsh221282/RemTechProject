import {Fade} from "@mui/material";
import {RowsContainer} from "./Components/RowsContainer.tsx";
import {ParserProfilesSection} from "./Sections/ParserProfilesSection/ParserProfilesSection.tsx";

export const ParserPage = () => {

    return (
        <RowsContainer children={<ParserProfilesSection/>}/>
    )
}