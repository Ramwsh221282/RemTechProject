export type Pagination = {
    page: number;
    size: number;
}

export type Statistics = {
    averagePrice: number;
    count: number;
    maxPrice: number;
    minPrice: number;
}

export function emptyStatistics(): Statistics {
    return {
        averagePrice: 0,
        count: 0,
        maxPrice: 0,
        minPrice: 0,
    }
}

export type Filters = {
    address: string;
    priceMinRange: number;
    priceMaxRange: number;
    priceExact: number;
    pricePredicate: string;
    characteristics: CharacteristicFilter[];
}

export type CharacteristicFilter = {
    characteristicsName: string;
    characteristicsValue: string;
}

export type Advertisement = {
    id: string;
    advertisementId: number;
    title: string;
    description: string;
    imageLinks: string[];
    sourceUrl: string;
    address: string;
    characteristics: AdvertisementCharacteristics[];
    owner: AdvertisementOwner;
    price: AdvertisementPrice;
}

export type AdvertisementCharacteristics = {
    name: string;
    value: string;
}

export type AdvertisementOwner = {
    status: string;
    description: string;
}

export type AdvertisementPrice = {
    value: number;
    currency: string;
    extra: string
}

export type AdvertisementPageState = {
    pagination: Pagination;
    statistics: Statistics;
    filters: Filters;
    advertisements: Advertisement[];
}

export function createInitialAdvertisementPageState(): AdvertisementPageState {
    const pagination: Pagination = {page: 1, size: 12}
    const filters: Filters = {
        address: '',
        priceExact: 0,
        priceMinRange: 0,
        priceMaxRange: 0,
        pricePredicate: '',
        characteristics: []
    }
    const statistics: Statistics = {averagePrice: 0, minPrice: 0, maxPrice: 0, count: 0}
    const advertisements: Advertisement[] = [];
    return {pagination: pagination, statistics: statistics, filters: filters, advertisements: advertisements}
}

