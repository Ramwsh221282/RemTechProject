import {Button, MenuItem, OutlinedInput, Select, SelectChangeEvent} from "@mui/material";
import {FormEvent, useEffect} from "react";
import {FilterRow} from "./FilterRow.tsx";
import {Characteristic, CharacteristicsBar} from "./CharacteristicsBar/CharacteristicsBar.tsx";
import {FilterInput} from "./FilterInput.tsx";
import {FilterService, FilterState} from "../../Services/FilterAdvertismentsService.ts";
import {NotificationAlert, useNotification} from "../../../../components/Notification.tsx";

export function FilterBar({filterService}: { filterService: FilterService }) {
    const pricePredicates: string[] = ['Больше', 'Меньше', 'Равно'];
    const placeHolder: string = "Метод сравнения";
    const notifications = useNotification();

    useEffect(() => {
        if (filterService.error.trim().length > 0) {
            notifications.showNotification({severity: "error", message: filterService.error});
            filterService.clearError();
        }
    }, [filterService.error]);

    function onCharacteristicsChange(characteristics: Characteristic[]): void {
        const stateCopy: FilterState = {...filterService.filter};
        stateCopy.characteristics = [...characteristics];
        filterService.handleSetFilters(stateCopy);
    }

    function onPricePredicateChange(event: SelectChangeEvent) {
        const {target: {value}} = event;
        const nextValue = "" + value;
        const stateCopy: FilterState = {...filterService.filter};
        stateCopy.pricePredicate = nextValue;
        filterService.handleSetFilters(stateCopy);
        if (filterService.error.trim().length === 0) {
            notifications.showNotification({severity: "success", message: `Выбран метод сравнения цены: ${nextValue}`});
        }
    }

    function onFilterSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        const formData = new FormData(event.currentTarget);
        const address = formData.get("address-input") as string;
        const pricePredicate = formData.get("price-predicate-input") as string;
        const priceMinRange = Number(formData.get("price-min-input"));
        const priceMaxRange = Number(formData.get("price-max-input"));
        const priceExact = Number(formData.get("price-exact-input"));
        const stateCopy: FilterState = {...filterService.filter};
        stateCopy.address = address;
        stateCopy.pricePredicate = pricePredicate;
        stateCopy.priceMinRange = priceMinRange;
        stateCopy.priceMaxRange = priceMaxRange;
        stateCopy.priceExact = priceExact;
        filterService.handleSetFilters(stateCopy);
        if (filterService.error.trim().length === 0) {
            notifications.showNotification({severity: "success", message: "Фильтры применены"});
        }
    }

    function onFilterClean() {
        filterService.cleanFilters();
        if (filterService.error.trim().length === 0) {
            notifications.showNotification({severity: "success", message: "Фильтры очищены"});
        }
    }

    return (
        <div
            className="w-100 flex flex-col py-3 px-3 bg-amber-950 border-amber-900 border-2 shadow-neutral-800 shadow-md rounded-md text-amber-50 gap-3">
            <h3 className="text-2xl underline">Фильтры</h3>
            <form onSubmit={onFilterSubmit} className="flex flex-col gap-3">
                <FilterInput type={"text"} fullWidth={true} name={"address-input"} id={"address-input"} label={"Адрес"}
                             variant={"filled"}/>
                <FilterRow>
                    <FilterInput type={"number"} name={"price-min-input"} id={"price-min-input"} label={"Цена от"}
                                 variant={"outlined"}/>
                    <FilterInput type={"number"} name={"price-max-input"} id={"price-max-input"} label={"Цена до"}
                                 variant={"outlined"}/>
                </FilterRow>
                <FilterRow>
                    <FilterInput type={"number"} name={"price-exact-input"} fullWidth={true} id={"price-exact-input"}
                                 label={"Цена"}
                                 variant={"outlined"}/>
                    <div className="order-2 w-full">
                        <Select
                            size={"small"}
                            name={"price-predicate-input"}
                            fullWidth={true}
                            value={filterService.filter.pricePredicate}
                            onChange={onPricePredicateChange}
                            displayEmpty={true}
                            renderValue={(selected) => {
                                if (!selected)
                                    return <em className="w-full">{placeHolder}</em>
                                return <em className="w-full">{selected}</em>;
                            }}
                            input={<OutlinedInput/>}>
                            <MenuItem disabled>{placeHolder}</MenuItem>
                            {pricePredicates.map((item, index) => (
                                <MenuItem key={index} value={item}>
                                    {item}
                                </MenuItem>
                            ))}
                        </Select>
                    </div>
                </FilterRow>
                <CharacteristicsBar onCharacteristicsChange={onCharacteristicsChange}/>
                <Button size={"small"} type={"submit"} variant={"contained"}>Применить фильтр</Button>
                <Button size={"small"} onClick={onFilterClean} variant={"contained"}>Очистить фильтры</Button>
            </form>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </div>
    )
}