import {Characteristic} from "../Components/FilterBar/CharacteristicsBar/CharacteristicsBar.tsx";
import {useCallback, useMemo, useState} from "react";

export type FilterDto = {
    characteristics: CharacteristicsListDto | null;
    address: AddressDto | null;
    price: PriceFilterDto | null;
    priceRange: PriceRangeDto | null;
    text: TextSearchDto | null;
}

export type CharacteristicsListDto = {
    characteristics: CharacteristicsDto[];
}

export type CharacteristicsDto = {
    name: string;
    value: string;
}

export type AddressDto = {
    text: string;
}

export type PriceFilterDto = {
    price: PriceDto;
    predicate: string;
}

export type PriceDto = {
    value: number;
}

export type PriceRangeDto = {
    valueMin: number;
    valueMax: number;
}

export type TextSearchDto = {
    text: string;
}

export function createFilterDto(filter: FilterState): FilterDto {
    const characteristics: CharacteristicsListDto | null = filter.characteristics.length > 0 ? {
        characteristics: filter.characteristics.map((ctx): CharacteristicsDto => ({
            name: ctx.characteristicsName.trim(),
            value: ctx.characteristicsValue.trim()
        }))
    } : null;
    const address: AddressDto | null = filter.address.trim().length > 0 ? {text: filter.address.trim()} : null;
    const priceFilter: PriceFilterDto | null = filter.pricePredicate.trim().length > 0 && filter.priceExact > 0 ? {
        predicate: filter.pricePredicate.trim(),
        price: {value: filter.priceExact}
    } : null;
    const priceRange: PriceRangeDto | null = filter.priceMinRange > 0 && filter.priceMaxRange > 0 ? {
        valueMax: filter.priceMaxRange,
        valueMin: filter.priceMinRange
    } : null;
    const textSearch: TextSearchDto | null = filter.textSearch.trim().length > 0 ? {text: filter.textSearch.trim()} : null;
    return {
        characteristics: characteristics,
        address: address,
        price: priceFilter,
        priceRange: priceRange,
        text: textSearch
    };
}

export type FilterService = {
    filter: FilterState;
    error: string;
    handleSetFilters: (filter: FilterState) => void;
    cleanFilters: () => void;
    clearError: () => void;
}

export type FilterState = {
    address: string;
    priceMinRange: number;
    priceMaxRange: number;
    priceExact: number;
    pricePredicate: string;
    characteristics: Characteristic[];
    textSearch: string;
}

function createEmptyState(): FilterState {
    return {
        address: '',
        priceMinRange: 0,
        priceMaxRange: 0,
        priceExact: 0,
        pricePredicate: '',
        characteristics: [],
        textSearch: '',
    }
}

function isFiltersEmpty(filter: FilterState) {
    return filter.pricePredicate.trim().length === 0 &&
        filter.characteristics.length === 0 &&
        filter.address.trim().length === 0 &&
        filter.priceMinRange === 0 &&
        filter.priceMaxRange === 0 &&
        filter.priceExact === 0 &&
        filter.textSearch.trim().length === 0
}

export function useAdvertisementsFilterService() {
    const [filter, setFilter] = useState<FilterState>(createEmptyState());
    const [error, setError] = useState<string>("");

    const cleanFilters = useCallback(() => {
        if (isFiltersEmpty(filter)) {
            setError((prev) => {
                prev = "Фильтры уже очищены";
                return prev;
            });
            return;
        }
        const emptyState = createEmptyState();
        setFilter((prev) => ({
            ...prev,
            address: emptyState.address,
            pricePredicate: emptyState.pricePredicate,
            priceExact: emptyState.priceExact,
            priceMinRange: emptyState.priceMinRange,
            priceMaxRange: emptyState.priceMaxRange,
            textSearch: emptyState.textSearch,
            characteristics: emptyState.characteristics
        }));
        setError((prev) => {
            prev = "";
            return prev;
        });
    }, [filter]);

    const clearError = useCallback(() => {
        setError((prev) => {
            prev = "";
            return prev;
        });
    }, []);

    const handleSetFilters = useCallback((filters: FilterState) => {
        const isEmpty: boolean = isFiltersEmpty(filters);
        if (isEmpty) {
            setError((prev) => {
                prev = "Фильтры пустые";
                return prev;
            });
            return;
        }
        setFilter((prev) => ({
            ...prev,
            address: filters.address,
            priceMinRange: filters.priceMinRange,
            priceMaxRange: filters.priceMaxRange,
            priceExact: filters.priceExact,
            pricePredicate: filters.pricePredicate,
            characteristics: filters.characteristics,
            textSearch: filters.textSearch,
        }));
        setError((prev) => {
            prev = "";
            return prev;
        });
        return createFilterDto(filters);
    }, []);

    const service: FilterService = useMemo(() => ({
        filter: filter,
        error: error,
        handleSetFilters: handleSetFilters,
        cleanFilters: cleanFilters,
        clearError: clearError,
    }), [filter, error, handleSetFilters, cleanFilters, clearError])

    return service;
}