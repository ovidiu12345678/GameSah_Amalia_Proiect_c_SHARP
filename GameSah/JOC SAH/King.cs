using System.Collections.Generic;

namespace ChessConsole.Pieces
{
    public class King : Piece
    {
        /// <rezumat>
        /// Reprezintă direcțiile de mișcare
        /// </rezumat>
        private Direction[] directions = new Direction[8];

        /// <rezumat>
        /// Afișează dacă putem turna la stânga
        /// </rezumat>
        private bool canCastleLeft;

        /// <rezumat>
        /// Arată dacă putem castel la dreapta
        /// </rezumat>
        private bool canCastleRight;

        public King(PlayerColor color)
            : base(color)
        {
            for (int i = 0; i < 8; i++)
            {
                directions[i] = null;
            }
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

                    if (canCastleLeft)
                    {
                        yield return Parent.Parent.GetCell(2, (Color == PlayerColor.White) ? 0 : 7);
                    }

                    if (canCastleRight)
                    {
                        yield return Parent.Parent.GetCell(6, (Color == PlayerColor.White) ? 0 : 7);
                    }
                }
            }
        }

        public override void Recalculate()
        {
            //Dacă înrocul mutat nu mai este posibil, ar trebui să eliminam și ascultătorii
            if (!Moved)
            {
                //Setați-l la adevărat, îl vom seta pe false dacă nu a fost adevărat
                canCastleLeft = true;

                //Verifică dacă turnul din stânga este încă pe loc și nu s-a mutat încă
                ChessBoard.Cell leftRookCell = Parent.Parent.GetCell(0, (Color == PlayerColor.White) ? 0 : 7);
                if (leftRookCell.Piece == null || !(leftRookCell.Piece is Rook) || leftRookCell.Piece.Color != Color || leftRookCell.Piece.Moved)
                    canCastleLeft = false;
                else
                {
                    //Verifică piese care ar putea bloca castelul
                    for (int i = 1; i <= 3; i++)
                    {
                        if (Parent.Parent.GetCell(i, (Color == PlayerColor.White) ? 0 : 7).Piece != null)
                            canCastleLeft = false;
                    }
                }

                //Setați-l la adevărat, îl vom seta pe false dacă nu a fost adevărat
                canCastleRight = true;

                //Verifică dacă turnul potrivit este încă pe loc și nu s-a mutat încă
                ChessBoard.Cell rightRookCell = Parent.Parent.GetCell(7, (Color == PlayerColor.White) ? 0 : 7);
                if (rightRookCell.Piece == null || !(rightRookCell.Piece is Rook) || rightRookCell.Piece.Color != Color || rightRookCell.Piece.Moved)
                    canCastleRight = false;
                else
                {
                    //Verifică piese care ar putea bloca castelul
                    for (int i = 5; i <= 6; i++)
                    {
                        if (Parent.Parent.GetCell(i, (Color == PlayerColor.White) ? 0 : 7).Piece != null)
                            canCastleRight = false;
                    }
                }
            }

            //Deschideți direcția în sus și ascultați-o
            directions[0] = new Direction(this, 0, 1, 1);
            //Deschideți direcția în jos și ascultați-o
            directions[1] = new Direction(this, 0, -1, 1);
            //Deschideți direcția spre stânga și ascultați-o
            directions[2] = new Direction(this, -1, 0, 1);
            //Deschideți direcția spre dreapta și ascultați-o
            directions[3] = new Direction(this, 1, 0, 1);
            //Deschide direcția din stânga și ascultă-l
            directions[4] = new Direction(this, -1, 1, 1);
            //Deschide direcția corectă și ascultă-o
            directions[5] = new Direction(this, 1, 1, 1);
            //Deschide în jos direcția stângă și ascultă-l
            directions[6] = new Direction(this, -1, -1, 1);
            //Deschide direcția corectă și ascultă-l
            directions[7] = new Direction(this, 1, -1, 1);
        }

        public override bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            foreach (Direction direction in directions)
            {
                //Dacă orice direcție poate atinge, returnarea blocată este false
                if (!direction.IsBlockedIfMove(from, to, blocked))
                    return false;
            }

            return true;
        }

        public override char Char => 'K';
    }
}
