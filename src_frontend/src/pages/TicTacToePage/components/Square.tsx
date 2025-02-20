export type SquareValue = {
    value: string | null;
    onSquareClick: () => void;
}

export const Square = ({value, onSquareClick}: SquareValue) => {
    return (
        <button className="square bg-amber-950" onClick={onSquareClick}>
            {value}
        </button>
    );
}