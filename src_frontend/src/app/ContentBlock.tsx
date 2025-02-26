import {ContentBlockChildren} from "./RootLayout.tsx";

export function ContentBlock(children: ContentBlockChildren) {
    return (
        <div className="p-1 w-full h-full overflow-auto bg-neutral-600 shadow-neutral-800 shadow-2xl rounded-xl flex-1">
            {children.children}
        </div>
    )
}