import {ParserProfile} from "../../Types/ParserProfile.ts";
import {Accordion, AccordionDetails, AccordionSummary} from "@mui/material";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import {ParserProfileItemHeadPanel} from "./ParserProfileItemHeadPanel.tsx";
import {ParserProfileLinksSection} from "./ParserProfileLinksSection.tsx";

export function ParserProfileItem({profile, orderedNumber}: { profile: ParserProfile, orderedNumber: number }) {
    return (
        <Accordion>
            <AccordionSummary expandIcon={<ArrowDropDownIcon/>} id={'profile-0'}>
                <ParserProfileItemHeadPanel profile={profile} orderedNumber={orderedNumber}/>
            </AccordionSummary>
            <AccordionDetails
                sx={{
                    backgroundColor: '#404040',
                }}>
                <ParserProfileLinksSection profile={profile}/>
            </AccordionDetails>
        </Accordion>
    )
}