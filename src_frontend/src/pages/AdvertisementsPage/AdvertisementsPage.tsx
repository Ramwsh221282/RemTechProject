import {AdvertisementsBoard} from "./Components/AdvertisementsBoard.tsx";
import {FilterBar} from "./Components/FilterBar.tsx";

export const AdvertisementsPage = () => {


    return (
        <div className="flex flex-row gap-5 py-5 px-5">
            <FilterBar/>
            <AdvertisementsBoard/>
        </div>
    )
}