export type Pagination = {
    page: number;
    size: number;
    sort: string | null
}

export type Statistics = {
    averagePrice: number;
    count: number;
    maxPrice: number;
    minPrice: number;
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

