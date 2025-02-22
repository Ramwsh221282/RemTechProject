import {Characteristic} from "../Components/FilterBar/CharacteristicsBar/CharacteristicsBar.tsx";
import {useCallback, useState} from "react";

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
}

function createEmptyState(): FilterState {
    return {
        address: '',
        priceMinRange: 0,
        priceMaxRange: 0,
        priceExact: 0,
        pricePredicate: '',
        characteristics: [],
    }
}

function isFiltersEmpty(filter: FilterState) {
    return filter.pricePredicate.trim().length === 0 &&
        filter.characteristics.length === 0 &&
        filter.address.trim().length === 0 &&
        filter.priceMinRange === 0 &&
        filter.priceMaxRange === 0 &&
        filter.priceExact === 0;
}

export function useAdvertisementsFilterService() {
    const [filter, setFilter] = useState<FilterState>(createEmptyState());
    const [error, setError] = useState<string>("");

    const cleanFilters = useCallback(() => {
        if (isFiltersEmpty(filter)) {
            setError("Фильтры уже очищены");
            return;
        }
        setFilter({...createEmptyState()});
        setError("");
    }, [filter]);

    const clearError = useCallback(() => {
        setError("");
    }, []);

    const handleSetFilters = useCallback((filters: FilterState) => {
        const isEmpty: boolean = isFiltersEmpty(filters);
        if (isEmpty) {
            setError("Фильтры пустые");
            return;
        }
        setFilter((prev) => ({
            ...prev,
            address: filters.address,
            priceMinRange: filters.priceMinRange,
            priceMaxRange: filters.priceMaxRange,
            priceExact: filters.priceExact,
            pricePredicate: filters.pricePredicate,
            characteristics: filters.characteristics
        }));
        setError("");
    }, []);

    const service: FilterService = {
        filter: filter,
        error: error,
        handleSetFilters: handleSetFilters,
        cleanFilters: cleanFilters,
        clearError: clearError,
    }

    return service;
}