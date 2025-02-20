import {createRoot} from 'react-dom/client'
import './index.css'
import {ThemeProvider} from "@mui/material";
import {amberDarkTheme} from "./theme.ts";
import {RouterProvider} from "react-router";
import {router} from "./app/router.tsx";

createRoot(document.getElementById('root')!).render(
    <ThemeProvider theme={amberDarkTheme}>
        <RouterProvider router={router}/>
    </ThemeProvider>
)
