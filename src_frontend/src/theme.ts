import {amber} from "@mui/material/colors";
import {createTheme} from "@mui/material/styles";

export const amberDarkTheme = createTheme({
    palette: {
        mode: "dark",
        primary: {
            light: amber[400],
            main: amber[500],
            dark: amber[600],
            contrastText: "#fff",
        }
    }
})