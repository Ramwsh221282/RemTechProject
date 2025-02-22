import {ContentBlockChildren} from "./RootLayout.tsx";

export function ContentBlock(children: ContentBlockChildren) {
    return (
        <div className="py-5 px-5 w-full h-full overflow-auto bg-amber-100 rounded-xl flex-1">
            {children.children}
        </div>
    )
}