import * as React from "react";
import {useCallback, useState} from "react";
import {Box, Tab, Tabs} from "@mui/material";

type CustomTabPanelProps = {
    children?: React.ReactNode;
    index: number;
};

function useCustomTab() {
    const [value, setValue] = useState(0);

    const onTabChange = useCallback((_: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    }, []);

    return {value, onTabChange};
}

export function CustomTabPanel({children, index}: CustomTabPanelProps) {
    return (
        <div role="tabpanel" id={`simple-tabpanel-${index}`}
             aria-labelledby={`simple-tab-${index}`}>
            {<Box sx={{p: 3}}>{children}</Box>}
        </div>
    );
}

type CustomTabProps = {
    panels: { index: number; title: string; children: React.ReactNode }[];
};

export function CustomTabs({panels}: CustomTabProps) {
    const {value, onTabChange} = useCustomTab();

    return (
        <>
            <Box sx={{borderBottom: 1, borderColor: "divider"}}>
                <Tabs value={value} onChange={onTabChange} variant="fullWidth">
                    {panels.map((panel, index) => (
                        <Tab key={index} label={panel.title}/>
                    ))}
                </Tabs>
            </Box>
            <CustomTabPanel key={panels[value].index} index={panels[value].index}>
                {panels[value].children}
            </CustomTabPanel>
        </>
    );
}