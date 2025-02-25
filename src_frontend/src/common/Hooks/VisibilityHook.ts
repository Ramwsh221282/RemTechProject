import {useCallback, useMemo, useState} from "react";

export function useVisibility(initial: boolean) {
    const [visible, setVisible] = useState(initial);

    const toggleVisibility = useCallback(() => {
        setVisible((prev) => !prev);
    }, []);

    return useMemo(() => {
        return {visible, toggleVisibility};
    }, [visible, toggleVisibility])
}