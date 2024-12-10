using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Player 
    {
        public char _piece { get; set; }
        public char _queenmark { get; set; }
        public bool _side { get; private set; }

        public Player(char playerpiece, char queenmark, bool side)
        {
            _piece = playerpiece;
            _queenmark = queenmark;
            _side = side;
        }
    }
}
