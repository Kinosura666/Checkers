using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Bot : Player
    {
        public int _difficulty { get; private set; }

        public Bot(char botpiece, char queenmark, bool side, int difficulty) : base(botpiece, queenmark, side)
        {
            _difficulty = difficulty;
        }
    }
}
