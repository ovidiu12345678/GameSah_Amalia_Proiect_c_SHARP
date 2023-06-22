using System.Collections.Generic;

namespace ChessConsole.Pieces
{
    public class Rook : Piece
    {
        /// <rezumat>
        /// Reprezintă direcțiile de mișcare
        /// </rezumat>
        private Direction[] directions = new Direction[4];

        public Rook(PlayerColor color)
            : base(color)
        {
            for (int i = 0; i < 4; i++)
            {
                directions[i] = null;
            }
        }

        public Rook(Piece promote)
            : this(promote.Color)
        {
            Moved = promote.Moved;
        }

        public override IEnumerable<ChessBoard.Cell> PossibleMoves
        {
            get
            {
                foreach (Direction direction in directions)
                {
                    foreach (ChessBoard.Cell cell in direction.GetPossibleMoves())
                    {
                        yield return cell;
                    }
                }
            }
        }

        public override void Recalculate()
        {
            //Deschideți direcția în sus și ascultați-o
            directions[0] = new Direction(this, 0, 1);
            //Deschideți direcția în jos și ascultați-o
            directions[1] = new Direction(this, 0, -1);
            //Deschideți direcția spre stânga și ascultați-o
            directions[2] = new Direction(this, -1, 0);
            //Deschideți direcția spre dreapta și ascultați-o
            directions[3] = new Direction(this, 1, 0);
        }

        public override bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            //Dacă orice direcție poate atinge, returnarea blocată este false
            foreach (Direction direction in directions)
            {
                //Dacă orice direcție poate atinge, returnarea blocată este false
                if (!direction.IsBlockedIfMove(from, to, blocked)) return false;
            }

            return true;
        }

        public override char Char => 'R';
    }
}
