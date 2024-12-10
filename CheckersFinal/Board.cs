using CheckersFinal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Board
    {
        public Piece[,] _board = new Piece[8, 8];

        public void InitializeBoard(Player player, Player bot, bool side)
        {
            if (side) 
            {
                PlacePieces(player, 5, 8); 
                PlacePieces(bot, 0, 3); 
            }
            else 
            {
                PlacePieces(player, 5, 8);
                PlacePieces(bot, 0, 3);
            }
        }
        
        private void PlacePieces(Player user, int startRow, int endRow)
        {
            for (int i = startRow; i < endRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0) 
                    {
                        _board[i, j] = new Piece(user, i, j);
                    }
                }
            }
        } // support initialize

        public bool PlayerMove()
        {
            var captureMoves = Rules.GetAllCaptureMoves(_board, true); // all caps for player

            if (captureMoves.Count > 0)
            {
                UI.ShowHints("У вас є доступнi бої. Ви повиннi виконати один iз них.");

                foreach (var move in captureMoves)
                {
                    Console.WriteLine($"Бiй: {move.startx}, {move.starty} -> {move.endx}, {move.endy}");
                }

                Console.WriteLine("Введiть хiд у форматi x1 y1 x2 y2:");
            }
            else
            {
                Console.WriteLine("Введiть хiд у форматi x1 y1 x2 y2:");
            }

            string[] inputMove = Console.ReadLine()?.Split();
            if (inputMove != null && inputMove.Length == 4)
            {
                int startx, starty, endx, endy;
                if (int.TryParse(inputMove[0], out startx) &&
                    int.TryParse(inputMove[1], out starty) &&
                    int.TryParse(inputMove[2], out endx) &&
                    int.TryParse(inputMove[3], out endy))
                {
                    if (captureMoves.Count > 0) // cap
                    {
                        if (Rules.IsCaptureMove(_board, startx, starty, endx, endy))
                        {
                            Rules.PerformCapture(_board, startx, starty, endx, endy);
                            // multi cap
                            while (true)
                            {
                                var additionalCaptures = Rules.GetAllCaptureMovesForPiece(_board, endx, endy);
                                if (additionalCaptures.Count > 0)
                                {
                                    UI.ShowHints("Ви повиннi виконати наступний бiй тiєю ж шашкою.");
                                    foreach (var move in additionalCaptures)
                                    {
                                        Console.WriteLine($"Бiй: {move.startx}, {move.starty} -> {move.endx}, {move.endy}");
                                    }

                                    Console.WriteLine("Введiть наступний бiй у форматi x1 y1 x2 y2:");
                                    inputMove = Console.ReadLine()?.Split();
                                    if (inputMove == null || inputMove.Length != 4 ||
                                        !int.TryParse(inputMove[0], out startx) ||
                                        !int.TryParse(inputMove[1], out starty) ||
                                        !int.TryParse(inputMove[2], out endx) ||
                                        !int.TryParse(inputMove[3], out endy) ||
                                        !Rules.IsCaptureMove(_board, startx, starty, endx, endy))
                                    {
                                        UI.ShowError("Некоректний бiй. Ви повиннi виконати наступний бiй!");
                                        continue;
                                    }
                                    Rules.PerformCapture(_board, startx, starty, endx, endy);
                                }
                                else
                                {
                                    break; // no caps
                                }
                            }
                            return true;
                        }
                        else
                        {
                            UI.ShowError("Ви повиннi виконати бiй!");
                            return false;
                        }
                    }
                    else // no must caps
                    {
                        if (Rules.ValidatePlayerMove(_board, startx, starty, endx, endy))
                        {
                            var temp = _board[startx, starty];
                            _board[endx, endy] = temp;
                            _board[startx, starty] = null;

                            // king check
                            if (!temp.IsKing && ((temp.owner._side && endx == 0) || (!temp.owner._side && endx == 7)))
                            {
                                temp.IsKing = true;
                            }

                            return true;
                        }
                        else
                        {
                            UI.ShowError("Хiд некоректний");
                            return false;
                        }
                    }
                }
                else
                {
                    UI.ShowError("Координати мають бути цiлими числами");
                    return false;
                }
            }
            else
            {
                UI.ShowError("Введiть рiвно 4 координати");
                return false;
            }
        } // main player logic

        public void StartBot(int difficulty)
        {
            if (difficulty == 1)
                RandomBotMove();
            else if (difficulty == 2)
                BotSafeMove();
            else if (difficulty == 3)
                BotAdvancedMove();
        } // main bot logic

        private void MakeBotMove(int startx, int starty, int endx, int endy)
        {
            var temp = _board[startx, starty];
            _board[endx, endy] = temp;
            _board[startx, starty] = null;

            UI.ShowBotMove(startx, starty, endx, endy);
            if (!temp.IsKing && ((!temp.owner._side && endx == 7) || (temp.owner._side && endx == 0)))
            {
                temp.IsKing = true;
            }
        }

        private void RandomBotMove()
        {
            var captureMoves = Rules.GetAllCaptureMoves(_board, false);
            if (captureMoves.Count > 0)
            {
                var random = new Random();
                var move = captureMoves[random.Next(captureMoves.Count)];
                Rules.PerformCapture(_board, move.startx, move.starty, move.endx, move.endy);
                int currentX = move.endx;
                int currentY = move.endy;
                while (true)
                {
                    var additionalCaptures = Rules.GetAllCaptureMovesForPiece(_board, currentX, currentY);
                    if (additionalCaptures.Count > 0)
                    {
                        move = additionalCaptures[random.Next(additionalCaptures.Count)];
                        Rules.PerformCapture(_board, move.startx, move.starty, move.endx, move.endy);
                        currentX = move.endx;
                        currentY = move.endy;
                    }
                    else
                    {
                        break; 
                    }
                }
            }
            else
            {
                var validMoves = new List<(int startx, int starty, int endx, int endy)>();
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (_board[i, j]?.owner is Bot)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                for (int dy = -1; dy <= 1; dy++)
                                {
                                    int endx = i + dx;
                                    int endy = j + dy;

                                    if (Rules.ValidateBotMove(_board, i, j, endx, endy))
                                    {
                                        validMoves.Add((i, j, endx, endy));
                                    }
                                }
                            }
                        }
                    }
                }
                if (validMoves.Count > 0)
                {
                    var random = new Random();
                    var move = validMoves[random.Next(validMoves.Count)];
                    MakeBotMove(move.startx, move.starty, move.endx, move.endy);
                }
            }
        } // lvl 1

        private void BotSafeMove()
        {
            var captureMoves = Rules.GetAllCaptureMoves(_board, false);
            if (captureMoves.Count > 0)
            {
                var random = new Random();
                var move = captureMoves[random.Next(captureMoves.Count)];
                Rules.PerformCapture(_board, move.startx, move.starty, move.endx, move.endy);

                int currentX = move.endx;
                int currentY = move.endy;

                while (true)
                {
                    var additionalCaptures = Rules.GetAllCaptureMovesForPiece(_board, currentX, currentY);
                    if (additionalCaptures.Count > 0)
                    {
                        move = additionalCaptures[random.Next(additionalCaptures.Count)];
                        Rules.PerformCapture(_board, move.startx, move.starty, move.endx, move.endy);
                        currentX = move.endx;
                        currentY = move.endy;
                    }
                    else
                    {
                        break; 
                    }
                }
                return; 
            }

            var possibleMoves = new List<(int startx, int starty, int endx, int endy)>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = _board[i, j];
                    if (piece != null && piece.owner is Bot)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int endx = i + dx;
                                int endy = j + dy;
                                if (Rules.ValidateBotMove(_board, i, j, endx, endy))
                                {
                                    possibleMoves.Add((i, j, endx, endy));
                                }
                            }
                        }
                    }
                }
            }

            var safeMoves = possibleMoves.Where(move =>
            {
                var tempBoard = CloneBoard(_board);
                Rules.SimulateMove(tempBoard, move.startx, move.starty, move.endx, move.endy);

                return !Rules.GetAllCaptureMoves(tempBoard, true).Any();
            }).ToList();

            if (safeMoves.Count > 0)
            {
                // save move - do save move
                var random = new Random();
                var move = safeMoves[random.Next(safeMoves.Count)];
                MakeBotMove(move.startx, move.starty, move.endx, move.endy);
            }
            else
            {
                // no save moves - do any
                var random = new Random();
                var move = possibleMoves[random.Next(possibleMoves.Count)];
                MakeBotMove(move.startx, move.starty, move.endx, move.endy);
            }
        } // lvl 2

        private Piece[,] CloneBoard(Piece[,] board)
        {
            var newBoard = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        var piece = board[i, j];
                        newBoard[i, j] = new Piece(piece.owner, piece.x, piece.y)
                        {
                            IsKing = piece.IsKing
                        };
                    }
                }
            }
            return newBoard;
        } // support minimax

        private void BotAdvancedMove()
        {
            const int depth = 8; 

            var captureMoves = Rules.GetAllCaptureMoves(_board, false);
            if (captureMoves.Count > 0)
            {

                var topMoves = captureMoves
                    .Select(move => (move, eval: EvaluateBoardAfterMove(_board, move.startx, move.starty, move.endx, move.endy)))
                    .OrderByDescending(x => x.eval)
                    .Take(3) 
                    .ToList();

                var random = new Random();
                var bestCapture = topMoves[random.Next(topMoves.Count)].move;

                Rules.PerformCapture(_board, bestCapture.startx, bestCapture.starty, bestCapture.endx, bestCapture.endy);

                int currentX = bestCapture.endx;
                int currentY = bestCapture.endy;

                while (true)
                {
                    var additionalCaptures = Rules.GetAllCaptureMovesForPiece(_board, currentX, currentY);
                    if (additionalCaptures.Count > 0)
                    {
                        var nextTopMoves = additionalCaptures
                            .Select(move => (move, eval: EvaluateBoardAfterMove(_board, move.startx, move.starty, move.endx, move.endy)))
                            .OrderByDescending(x => x.eval)
                            .Take(3) 
                            .ToList();

                        var nextBestCapture = nextTopMoves[random.Next(nextTopMoves.Count)].move;
                        Rules.PerformCapture(_board, nextBestCapture.startx, nextBestCapture.starty, nextBestCapture.endx, nextBestCapture.endy);

                        currentX = nextBestCapture.endx;
                        currentY = nextBestCapture.endy;
                    }
                    else
                    {
                        break; 
                    }
                }
                return; 
            }

            var minimaxResult = Minimax(_board, depth, false, int.MinValue, int.MaxValue, Game._player, Game._bot);
            var bestMove = minimaxResult.Item2;

            if (bestMove != null)
            {
                Rules.SimulateMove(_board, bestMove.Value.startx, bestMove.Value.starty, bestMove.Value.endx, bestMove.Value.endy);
            }
        } // lvl 3

        private int EvaluateBoardAfterMove(Piece[,] board, int startx, int starty, int endx, int endy) // minimax support
        {
            var tempBoard = CloneBoard(board);
            Rules.PerformCapture(tempBoard, startx, starty, endx, endy);
            return EvaluateBoard(tempBoard);
        }

        private (int, (int startx, int starty, int endx, int endy)?) Minimax(Piece[,] board, int depth, bool isMaximizing, int alpha, int beta, Player player, Bot bot)
        {
            if (depth == 0 || Rules.CheckGameOver(board, player, bot, out _))
            {
                return (EvaluateBoard(board), null);
            }

            var moves = isMaximizing
                ? Rules.GetAllCaptureMoves(board, true).Concat(Rules.GetAllMoves(board, true)).ToList()
                : Rules.GetAllCaptureMoves(board, false).Concat(Rules.GetAllMoves(board, false)).ToList();

            (int, (int startx, int starty, int endx, int endy)?) bestMove = (isMaximizing ? int.MinValue : int.MaxValue, null);

            foreach (var move in moves)
            {
                var tempBoard = CloneBoard(board);
                Rules.SimulateMove(tempBoard, move.startx, move.starty, move.endx, move.endy);

                var eval = Minimax(tempBoard, depth - 1, !isMaximizing, alpha, beta, player, bot).Item1;

                if (isMaximizing)
                {
                    if (eval > bestMove.Item1)
                    {
                        bestMove = (eval, move);
                    }
                    alpha = Math.Max(alpha, eval);
                }
                else
                {
                    if (eval < bestMove.Item1)
                    {
                        bestMove = (eval, move);
                    }
                    beta = Math.Min(beta, eval);
                }

                if (beta <= alpha) break;
            }

            return bestMove;
        } // i wanna cry

        private int EvaluateBoard(Piece[,] board)
        {
            int[,] positionWeights = {
        { 4, 4, 4, 4, 4, 4, 4, 4 },
        { 3, 3, 3, 3, 3, 3, 3, 3 },
        { 2, 2, 2, 2, 2, 2, 2, 2 },
        { 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1 },
        { 2, 2, 2, 2, 2, 2, 2, 2 },
        { 3, 3, 3, 3, 3, 3, 3, 3 },
        { 4, 4, 4, 4, 4, 4, 4, 4 }
    };

            int score = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null)
                    {
                        int value = piece.IsKing ? 10 : 5;
                        // position value only for basic piece
                        if (!piece.IsKing)
                        {
                            value += positionWeights[i, j];
                            value += piece.owner._side ? (7 - i) : i; // stimulate for kings
                        }

                        score += (piece.owner == Game._bot ? value : -value);
                    }
                }
            }
            return score;
        } // support evaluate


    }
}
