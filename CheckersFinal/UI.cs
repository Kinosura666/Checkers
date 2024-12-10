using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class UI
    {
        public static void PrintBoard(Piece[,] board, bool isPlayerTurn)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(isPlayerTurn ? "Ваш хiд!" : "Хiд бота!");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   0 1 2 3 4 5 6 7");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ╔════════════════╗");
            Console.ResetColor();
            for (int i = 0; i < 8; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{i}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" ║");
                Console.ResetColor();
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write(board[i, j].GetSymbol() + " ");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("║ ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{i}");
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ╚════════════════╝");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   0 1 2 3 4 5 6 7");
            Console.ResetColor();
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowBotMove(int startx, int starty, int endx, int endy)
        {
            Console.ForegroundColor  = ConsoleColor.Green;
            Console.WriteLine($"Хiд бота - {startx},{starty} -> {endx},{endy}");
            Console.ResetColor();
        }

        public static void ShowHints(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message}");
            Console.ResetColor();
        }
    }
}