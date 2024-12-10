using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Game
    {
        public Board _board;
        public static Player _player;
        public static Bot _bot;
        private bool _turn; //true = player, false = botyara

        public Game(Player player, Bot bot)
        {
            _board = new Board();
            _player = player;
            _bot = bot;
            _turn = true;
        }

        public void Start()
        {
            _turn = _player._side;

            while (true)
            {
                if (Rules.CheckGameOver(_board._board, _player, _bot, out string winner))
                {
                    Console.Clear();
                    UI.ShowHints($"Гра завершена! {winner}");
                    break;
                }

                UI.PrintBoard(_board._board, _turn); // true — player, false — botyara

                if (_turn)
                {
                    if (_board.PlayerMove())
                    {
                        Console.Clear();
                        _turn = !_turn;
                    }
                }
                else
                {
                    _board.StartBot(_bot._difficulty);
                    UI.ShowHints("Ви бачите хiд бота. Натиснiть будь-яку клавiшу щоб перейти до вашого ходу");
                    Console.ReadLine();
                    Console.Clear();
                    _turn = !_turn;
                }
            }
        }

    }

}
