import {AppBar, Button} from "@mui/material";
import {NavLink} from "react-router";

type NavigationBodyProps = {
    routerLink: string,
    displayName: string,
}

export function Header() {
    const navigations: NavigationBodyProps[] =
        [
            {routerLink: "advertisements", displayName: "Данные"},
            {routerLink: "parser", displayName: "Парсинг"},
        ]

    const navigationButtons = navigations.map((navigation, index) => {
        return (
            <NavLink to={navigation.routerLink} key={index}>
                <Button color="inherit">{navigation.displayName}</Button>
            </NavLink>
        )
    });

    return (
        <AppBar position="static">
            <div className="flex flex-row items-center py-2 px-3">
                <div className="flex flex-row gap-4 py-2 px-3">
                    <span className="text-3xl">App Name</span>
                    <div className="flex flex-row gap-4 pl-20 text-2xl">
                        {navigationButtons}
                    </div>
                </div>
            </div>
        </AppBar>
    )
}