import {AdvertisementCardRow, AdvertisementCardRowProps} from "./AdvertisementCardRow.tsx";

type AdvertisementCardColProps = {
    rows: AdvertisementCardRowProps[]
}

export function AdvertisementCardCol({rows}: AdvertisementCardColProps) {
    return (
        <div className="flex flex-col gap-60">
            {rows.map((row, index) => (
                <AdvertisementCardRow key={index} {...row} />
            ))}
        </div>
    )
}