import {TextField} from "@mui/material";

export const AdvertisementsPage = () => {
    return (
        <section className="flex flex-col">
            <div className="flex flex-col md:flex-row flex-grow">
                {/*Вынести в сайдбар компонент*/}
                <aside
                    className="flex flex-col p-4 bg-amber-200 order-1 md:order-1 items-center py-5 px-5 gap-5">
                    <h3 className="text-2xl underline">Фильтры</h3>
                    {/*{Вынести в компонент формы}*/}
                    <div
                        className="flex flex-col p-4 items-center py-5 px-5 gap-5 bg-amber-950 rounded-md shadow-sm shadow-neutral-800">
                        <TextField id="addressInput" label="Адрес" fullWidth={true} variant="outlined"></TextField>
                        <div className="flex flex-row flex-grow items-center">
                            <div className="p-2 order-1">
                                <TextField id="priceFrom" label="Цена от"
                                           variant="standard"></TextField>
                            </div>
                            <div className="p-2 order-2">
                                <TextField id="priceFrom" label="Цена до" variant="standard"></TextField>
                            </div>
                        </div>
                        
                    </div>
                </aside>
                {/*{Вынести в компонент объявлений}*/}
                <div className="flex-grow p-4 order-2 md:order-2 w-full bg-amber-100">

                </div>
            </div>
        </section>
    )
}