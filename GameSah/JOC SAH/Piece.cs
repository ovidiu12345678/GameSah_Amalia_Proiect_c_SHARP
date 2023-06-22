using System.Collections.Generic;

namespace ChessConsole
{
    /// <rezumat>
    /// Reprezintă o piesă abstractă de șah
    /// </rezumat>
    public abstract class Piece
    {
        /// <rezumat>
        /// Culoarea piesei
        /// </rezumat>
        public PlayerColor Color
        {
            private set;
            get;
        }

        /// <rezumat>
        /// Fals în mod implicit, setat la adevărat la prima mișcare
        /// </rezumat>
        public bool Moved
        {
            protected set;
            get;
        }

        /// <rezumat>
        /// Toate mișcările posibile de făcut cu această piesă
        /// </rezumat>
        public abstract IEnumerable<ChessBoard.Cell> PossibleMoves
        {
            get;
        }

        /// <rezumat>
        /// Toate mișcările legale de făcut cu această piesă. Este un subset al <see cref="PossibleMoves"/>.
        /// Vezi și <seealso cref="ChessBoard.isMoveLegal(Piece, ChessBoard.Cell)"/>.
        /// </rezumat>
        public List<ChessBoard.Cell> LegalMoves
        {
            private set;
            get;
        }

        public ChessBoard.Cell Parent
        {
            private set;
            get;
        }

        public Piece(PlayerColor color)
        {
            Color = color;
            Moved = false;
            LegalMoves = new List<ChessBoard.Cell>();
        }

        /// <rezumat>
        /// Apelat atunci când piesa este plasată pentru prima dată sau când piesa este înlocuită după promovare.
        /// Nu recalculează încă, trebuie să suni la <see cref="Recalculate"/> pentru asta.
        /// </rezumat>
        public void OnPlace(ChessBoard.Cell cell)
        {
            Parent = cell;
        }

        /// <rezumat>
        /// Apelat când piesa este mutată.
        /// Nu recalculează încă, trebuie să suni la <see cref="Recalculate"/> pentru asta.
        /// </rezumat>
        public void OnMove(ChessBoard.Cell cell)
        {
            Parent = cell;
            Moved = true;
        }

        /// <rezumat>
        /// Recalculează mișcările posibile și actualizează graficul de lovituri
        /// </rezumat>
        public abstract void Recalculate();

        /// <rezumat>
        /// Spune dacă piesa mutată pe celulă a schimbat starea de lovire a celui blocat
        /// </rezumat>
        /// <param name="from">Unde se află piesa chiar acum</param>
        /// <param name="to">Unde este mutată piesa</param>
        /// <param name="blocked">Hit testează această piesă</param>
        /// <returns>Dacă blocat poate fi lovit după mutarea de la</returns>
        public abstract bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked);

        public abstract char Char { get; }

        protected virtual bool canHit(ChessBoard.Cell cell)
        {
            return cell != null && cell.Piece != null && cell.Piece.Color != Color;
        }
    }
}
