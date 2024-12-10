using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Rules
    {
        public static bool CheckGameOver(Piece[,] board, Player player, Player bot, out string winner)
        {
            bool playerHasPieces = HasPieces(board, player);
            bool botHasPieces = HasPieces(board, bot);

            bool playerCanMove = CanMove(board, player);
            bool botCanMove = CanMove(board, bot);

            if (!playerHasPieces || !playerCanMove)
            {
                winner = "Бот перемiг!";
                return true;
            }

            if (!botHasPieces || !botCanMove)
            {
                winner = "Гравець перемiг!";
                return true;
            }

            winner = null;
            return false;
        }

        private static bool HasPieces(Piece[,] board, Player player)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j]?.owner == player)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CanMove(Piece[,] board, Player player)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null && piece.owner == player)
                    {
                        // caps
                        var captureMoves = GetAllCaptureMovesForPiece(board, i, j);
                        if (captureMoves.Count > 0) return true;

                        // no cap
                        for (int dx = -1; dx <= 1; dx += 2)
                        {
                            for (int dy = -1; dy <= 1; dy += 2)
                            {
                                int endx = i + dx;
                                int endy = j + dy;
                                if (player._side)
                                {
                                    if (ValidatePlayerMove(board, i, j, endx, endy)) return true;
                                }
                                else
                                {
                                    if (ValidateBotMove(board, i, j, endx, endy)) return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool ValidatePlayerMove(Piece[,] board, int startx, int starty, int endx, int endy)
        {
            if (startx < 0 || startx > 7 || starty < 0 || starty > 7 || endx < 0 || endx > 7 || endy < 0 || endy > 7)
                return false;

            var startPiece = board[startx, starty];
            var endPiece = board[endx, endy];

            if (startPiece == null || endPiece != null) return false;

            int dx = Math.Abs(endx - startx);
            int dy = Math.Abs(endy - starty);

            if (startPiece.IsKing)
            {
                //king
                if (dx == dy)
                {
                    int stepX = (endx - startx) / dx;
                    int stepY = (endy - starty) / dx;

                    for (int i = 1; i < dx; i++)
                    {
                        if (board[startx + i * stepX, starty + i * stepY] != null)
                            return false; 
                    }
                    return true;
                }
            }
            else
            {
                // basic
                if (startPiece.owner._side) // player
                {
                    return dx == 1 && dy == 1 && startx > endx;
                }
                else // bot
                {
                    return dx == 1 && dy == 1 && startx < endx;
                }
            }
            return false;
        }

        public static bool ValidateBotMove(Piece[,] board, int startx, int starty, int endx, int endy)
        {
            if (startx < 0 || startx > 7 || starty < 0 || starty > 7 || endx < 0 || endx > 7 || endy < 0 || endy > 7)
                return false;

            var startPiece = board[startx, starty];
            var endPiece = board[endx, endy];

            if (startPiece == null || endPiece != null) return false;
            int dx = Math.Abs(endx - startx);
            int dy = Math.Abs(endy - starty);
            if (startPiece.IsKing)
            {
                if (dx == dy)
                {
                    int stepX = (endx - startx) / dx;
                    int stepY = (endy - starty) / dy;
                    for (int i = 1; i < dx; i++)
                    {
                        if (board[startx + i * stepX, starty + i * stepY] != null)
                            return false; 
                    }
                    return true;
                }
            }
            else
            {
                if (!startPiece.owner._side) 
                {
                    return dx == 1 && dy == 1 && startx < endx;
                }
            }
            return false;
        }

        public static bool IsCaptureMove(Piece[,] board, int startx, int starty, int endx, int endy)
        {
            if (startx < 0 || startx >= 8 || starty < 0 || starty >= 8 || endx < 0 || endx >= 8 || endy < 0 || endy >= 8)
                return false;

            var startPiece = board[startx, starty];
            if (startPiece == null || board[endx, endy] != null) return false;

            int dx = Math.Abs(endx - startx);
            int dy = Math.Abs(endy - starty);

            if (startPiece.IsKing) // king
            {
                if (dx == dy && dx > 1)
                {
                    int stepX = (endx - startx) / dx;
                    int stepY = (endy - starty) / dy;

                    bool foundEnemy = false;
                    for (int i = 1; i < dx; i++)
                    {
                        int midX = startx + i * stepX;
                        int midY = starty + i * stepY;
                        var midPiece = board[midX, midY];

                        if (midPiece != null)
                        {
                            if (midPiece.owner == startPiece.owner)
                                return false; // ally
                            if (foundEnemy)
                                return false; // enemy
                            foundEnemy = true;
                        }
                    }
                    return foundEnemy; // exact 1 
                }
            }
            else // baisc
            {
                if (dx == 2 && dy == 2)
                {
                    int midx = (startx + endx) / 2;
                    int midy = (starty + endy) / 2;
                    var midPiece = board[midx, midy];
                    if (midPiece != null && midPiece.owner != startPiece.owner)
                        return true;
                }
            }
            return false;
        }

        public static void PerformCapture(Piece[,] board, int startx, int starty, int endx, int endy)
        {
            var startPiece = board[startx, starty];
            int dx = Math.Abs(endx - startx);

            if (startPiece.IsKing)
            {
                int stepX = (endx - startx) / dx;
                int stepY = (endy - starty) / dx;

                for (int i = 1; i < dx; i++)
                {
                    int midX = startx + i * stepX;
                    int midY = starty + i * stepY;
                    if (board[midX, midY] != null && board[midX, midY].owner != startPiece.owner)
                    {
                        board[midX, midY] = null; 
                        break;
                    }
                }
            }
            else
            {
                int midX = (startx + endx) / 2;
                int midY = (starty + endy) / 2;
                board[midX, midY] = null; 
            }

            board[endx, endy] = startPiece;
            board[startx, starty] = null;

            // check for king
            if (!startPiece.IsKing && ((startPiece.owner._side && endx == 0) || (!startPiece.owner._side && endx == 7)))
            {
                startPiece.IsKing = true;
            }
        }

        public static List<(int startx, int starty, int endx, int endy)> GetAllCaptureMoves(Piece[,] board, bool isPlayer)
        {
            var captureMoves = new List<(int startx, int starty, int endx, int endy)>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];

                    if (piece != null && piece.owner._side == isPlayer)
                    {
                        captureMoves.AddRange(GetAllCaptureMovesForPiece(board, i, j));
                    }
                }
            }
            return captureMoves;
        } // every possible caps

        public static List<(int startx, int starty, int endx, int endy)> GetAllCaptureMovesForPiece(Piece[,] board, int startx, int starty)
        {
            var captureMoves = new List<(int startx, int starty, int endx, int endy)>();

            var piece = board[startx, starty];
            if (piece == null) return captureMoves;

            if (piece.IsKing) // king
            {
                for (int dx = -1; dx <= 1; dx += 2)
                {
                    for (int dy = -1; dy <= 1; dy += 2)
                    {
                        int x = startx, y = starty;
                        bool foundEnemy = false;

                        while (true)
                        {
                            x += dx;
                            y += dy;

                            if (x < 0 || x >= 8 || y < 0 || y >= 8) break;

                            var targetPiece = board[x, y];

                            if (targetPiece == null) 
                            {
                                if (foundEnemy) 
                                {
                                    captureMoves.Add((startx, starty, x, y));
                                }
                            }
                            else if (targetPiece.owner != piece.owner) 
                            {
                                if (foundEnemy) break; 
                                foundEnemy = true;
                            }
                            else //ally
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else // basic piece
            {
                for (int dx = -2; dx <= 2; dx += 4)
                {
                    for (int dy = -2; dy <= 2; dy += 4)
                    {
                        int endx = startx + dx;
                        int endy = starty + dy;

                        if (IsCaptureMove(board, startx, starty, endx, endy))
                        {
                            captureMoves.Add((startx, starty, endx, endy));
                        }
                    }
                }
            }

            return captureMoves;
        } // multi cap

        public static void SimulateMove(Piece[,] board, int startx, int starty, int endx, int endy)
        {
            var temp = board[startx, starty];
            board[endx, endy] = temp;
            board[startx, starty] = null;
        } // support minimax

        public static List<(int startx, int starty, int endx, int endy)> GetAllMoves(Piece[,] board, bool isPlayer)
        {
            var moves = new List<(int startx, int starty, int endx, int endy)>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null && piece.owner._side == isPlayer)
                    {
                        if (piece.IsKing) // king
                        {
                            for (int dx = -1; dx <= 1; dx += 2)
                            {
                                for (int dy = -1; dy <= 1; dy += 2)
                                {
                                    int x = i, y = j;
                                    while (true)
                                    {
                                        x += dx;
                                        y += dy;

                                        if (x < 0 || x >= 8 || y < 0 || y >= 8) break;

                                        if (board[x, y] == null)
                                        {
                                            moves.Add((i, j, x, y));
                                        }
                                        else
                                        {
                                            break; 
                                        }
                                    }
                                }
                            }
                        }
                        else // basic
                        {
                            int direction = piece.owner._side ? -1 : 1; 
                            for (int dy = -1; dy <= 1; dy += 2)
                            {
                                int endx = i + direction;
                                int endy = j + dy;

                                if (endx >= 0 && endx < 8 && endy >= 0 && endy < 8 && board[endx, endy] == null)
                                {
                                    moves.Add((i, j, endx, endy));
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        } // support minimax
    }
}
