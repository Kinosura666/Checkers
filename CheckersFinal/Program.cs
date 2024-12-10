using System;

namespace CheckersFinal
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Шашки\nОберiть рiвень складностi");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1 - Рандом бот");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("2 - Бот не ходить пiд бiй");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("3 - Прорахунок ходiв наперед");
            Console.ResetColor();

            int difficulty;
            while (!int.TryParse(Console.ReadLine(), out difficulty) || difficulty < 1 || difficulty > 3)
            {
                UI.ShowError("Введiть число вiд 1 до 3");
            }

            //Console.ForegroundColor = ConsoleColor.Gray;
            //Console.WriteLine("Оберiть сторону\n1 - бiлi\n2 - чорнi");
            //Console.ResetColor();

            bool playerIsWhite = true;
            //while (true)
            //{
            //    string input = Console.ReadLine();
            //    if (input == "1")
            //    {
            //        playerIsWhite = true;
            //        break;
            //    }
            //    else if (input == "2")
            //    {
            //        playerIsWhite = false;
            //        break;
            //    }
            //    else UI.ShowError("Невiрний вибiр! Введiть 1 (бiлi) або 2 (чорнi).");
            //}

            Player player = playerIsWhite ? new Player('W', 'w', true) : new Player('B', 'b', false);
            Bot bot = playerIsWhite ? new Bot('B', 'b', false, difficulty) : new Bot('W', 'w', true, difficulty);

            Game game = new Game(player, bot);

            game._board.InitializeBoard(player, bot, playerIsWhite);

            game.Start();
        }
    }
}
