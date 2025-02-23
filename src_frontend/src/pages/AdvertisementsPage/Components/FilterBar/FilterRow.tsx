import * as React from "react";

type FilterRowProps = {
    children: React.ReactNode;
}

export function FilterRow({children}: FilterRowProps) {
    return (
        <div className="flex flex-row gap-3 w-full">
            {children}
        </div>
    )
}