using CheckersFinal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersFinal
{
    public class Piece
    {
        public Player owner { get; private set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool IsKing { get; set; } 

        public Piece(Player owner, int x, int y)
        {
            this.owner = owner;
            this.x = x;
            this.y = y;
            this.IsKing = false; 
        }

        public char GetSymbol()
        {
            return IsKing ? owner._queenmark : owner._piece;
        }
    }


}
