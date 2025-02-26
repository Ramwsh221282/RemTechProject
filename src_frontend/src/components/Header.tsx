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
            {routerLink: 'parser-journals', displayName: 'Журналы'}
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
            <div className="flex flex-row items-center py-1 px-3">
                <div className="flex flex-row gap-4">
                    <span className="text-2xl">App Name</span>
                    <div className="flex flex-row gap-4 pl-20">
                        {navigationButtons}
                    </div>
                </div>
            </div>
        </AppBar>
    )
}