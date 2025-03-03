import * as React from "react";

type Props = {
    children?: React.ReactNode | React.ReactNode[] | null;
}

export function RowsContainer(props: Props) {
    if (!props.children) return null;

    if (Array.isArray(props.children)) {
        return (
            <div className="flex flex-row p-1 gap-1">
                {props.children.map((child, index) => (
                    <div key={index}>
                        {child}
                    </div>
                ))}
            </div>
        )
    }

    return (
        <div className="flex flex-row p-1 gap-1">
            {props.children}
        </div>
    )
}