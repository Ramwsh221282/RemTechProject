import {Button, MenuItem, OutlinedInput, Select, SelectChangeEvent} from "@mui/material";
import {FormEvent, useEffect, useState} from "react";
import {FilterInput, FilterRow} from "./FilterRow.tsx";
import {Characteristic, CharacteristicsBar} from "./CharacteristicsBar.tsx";

type FilterState = {
    address: string;
    priceMinRange: number;
    priceMaxRange: number;
    priceExact: number;
    pricePredicate: string;
    characteristics: Characteristic[];
}

function createEmptyFilterState(): FilterState {
    return {
        address: '',
        priceMinRange: 0,
        priceMaxRange: 0,
        priceExact: 0,
        pricePredicate: '',
        characteristics: []
    }
}

export function FilterBar() {
    const pricePredicates: string[] = ['Больше', 'Меньше', 'Равно'];
    const placeHolder: string = "Метод сравнения";
    const [currentFilterState, setCurrentFilterState] = useState<FilterState>(createEmptyFilterState())

    useEffect(() => {
        console.log(currentFilterState)
    }, [currentFilterState]);

    function onCharacteristicsChange(characteristics: Characteristic[]): void {
        const newFilterState = {...currentFilterState}
        newFilterState.characteristics = [...characteristics];
        setCurrentFilterState(newFilterState);
    }

    function onPricePredicateChange(event: SelectChangeEvent) {
        const {target: {value}} = event;
        const nextValue = "" + value;
        const newFilterState = {...currentFilterState}
        newFilterState.pricePredicate = nextValue;
        setCurrentFilterState(newFilterState);
    }

    function initializeFilterStateFromForm(formData: FormData): void {
        const address = formData.get("address-input") as string;
        const pricePredicate = formData.get("price-predicate-input") as string;
        const priceMinRange = Number(formData.get("price-min-input"));
        const priceMaxRange = Number(formData.get("price-max-input"));
        const priceExact = Number(formData.get("price-exact-input"));

        const newFilterState = {...currentFilterState}
        newFilterState.address = address;
        newFilterState.pricePredicate = pricePredicate;
        newFilterState.priceMinRange = priceMinRange;
        newFilterState.priceMaxRange = priceMaxRange;
        newFilterState.priceExact = priceExact;

        setCurrentFilterState(newFilterState);
    }

    function onFilterSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        event.stopPropagation();
        const formData = new FormData(event.currentTarget);
        initializeFilterStateFromForm(formData);
    }

    function onFilterClean() {
        const newState = createEmptyFilterState();
        setCurrentFilterState(newState);
    }

    return (
        <div
            className="flex flex-col py-3 px-3 bg-amber-950 border-amber-900 border-2 shadow-neutral-800 shadow-md rounded-md text-amber-50 gap-3">
            <h3 className="text-3xl underline">Фильтры</h3>
            <form onSubmit={onFilterSubmit} className="flex flex-col py-3 px-3 gap-3">
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
                            value={currentFilterState.pricePredicate}
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
        </div>
    )
}