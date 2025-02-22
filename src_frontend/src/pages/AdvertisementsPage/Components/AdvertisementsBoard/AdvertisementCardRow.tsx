import {Advertisement} from "../../Types/AdvertisementsPageTypes.ts";
import {AdvertisementCard} from "./AdvertisementCard.tsx";

export type AdvertisementCardRowProps = {
    cards: Advertisement[]
}

export function AdvertisementCardRow(row: AdvertisementCardRowProps) {
    return (
        <div className="flex flex-row gap-3">
            {row.cards.map((card, _) => (
                <AdvertisementCard key={card.advertisementId} {...card} />
            ))}
        </div>
    )
}