import {useCallback, useEffect, useState} from "react";
import {ParserJournal} from "./Types/ParserJournal.ts";
import {ParserJournalsService} from "./Services/ParserJournalsService.ts";
import {Pagination} from "../AdvertisementsPage/Types/AdvertisementsPageTypes.ts";
import {NotificationAlert, useNotification} from "../../components/Notification.tsx";
import {CircularProgress, Fade} from "@mui/material";
import {ParserJournalsList} from "./Components/ParserJournalsList.tsx";
import {ParserJournalsPagination} from "./Components/ParserJournalsPagination.tsx";

export function JournalsPage() {
    const [journals, setJournals] = useState<ParserJournal[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [pagination, setPagination] = useState<Pagination>({page: 1, size: 10, sort: null})
    const [totalCount, setTotalCount] = useState(0);
    const notifications = useNotification();

    useEffect(() => {
        const fetching = async () => {
            setIsLoading(true);
            const response = await ParserJournalsService.getJournals(pagination);
            if (typeof response === 'string') {
                notifications.showNotification({severity: "error", message: response})
                setIsLoading(false);
                return;
            }
            setIsLoading(false);
            setTotalCount(response.count);
            setJournals(response.journals);
        }
        fetching();
    }, [])

    const onPageChange = useCallback((page: number) => {
        setPagination({...pagination, page: page});
    }, [])

    if (isLoading) {
        return (
            <div className="flex flex-col gap-2 p-2 w-full h-full">
                <div className="m-auto">
                    <CircularProgress size={128}/>
                </div>
            </div>
        )
    }

    return (
        <>
            <Fade in={true} timeout={500}>
                <div className="flex flex-col gap-2 p-2 h-full">
                    <ParserJournalsList journals={journals}/>
                    <ParserJournalsPagination totalCount={totalCount} page={pagination.page} pageSize={pagination.size}
                                              changePage={onPageChange}/>
                </div>
            </Fade>
            <NotificationAlert notification={notifications.notification}
                               hideNotification={notifications.hideNotification}/>
        </>
    )
}