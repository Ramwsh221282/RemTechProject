import {useState} from "react";
import {Board} from "./Board.tsx";

export function Game() {
    const [xIsNext, setXIsNext] = useState<boolean>(true);
    const [squaresHistory, setSquaresHistory] = useState<Array<Array<string>>>([Array(9).fill("")]);
    const [currentMove, setCurrentMove] = useState(0);
    const currentSquares = squaresHistory[squaresHistory.length - 1];

    function handlePlay(nextSquares: string[]) {
        const nextHistory = [...squaresHistory.slice(0, currentMove + 1), nextSquares];
        setSquaresHistory(nextHistory);
        setCurrentMove(nextHistory.length - 1);
        setXIsNext(!xIsNext);
    }

    function jumpTo(move: number) {
        setCurrentMove(move);
        setXIsNext(move % 2 === 0);
    }

    return (
        <div className="game">
            <div className="game-board">
                <Board xIsNext={xIsNext} squares={currentSquares} onPlay={handlePlay}/>
            </div>
            <div className="game-info">
                <ol>{squaresHistory.map((_, move) => {
                    let description: string = '';
                    if (move > 0)
                        description = 'Go to move #' + move;
                    else
                        description = 'Go to game start';
                    return (
                        <li key={move}>
                            <button onClick={() => jumpTo((move))}>{description}</button>
                        </li>
                    )
                })}</ol>
            </div>
        </div>
    )
}