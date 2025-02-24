import {ReactNode} from "react";

type ParserPageRootProps = {
    children?: ReactNode | null
}

export function ParserPageRoot({children}: ParserPageRootProps) {
    return (
        <section className="flex flex-col p-2 w-full h-full">
            {children}
        </section>
    )
}