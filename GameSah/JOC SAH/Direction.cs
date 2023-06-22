using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessConsole
{
    /// <rezumat>
    /// Conține posibilele mișcări și se ocupă de verificări ale liniei de vedere
    /// </rezumat>
    public class Direction
    {
        /// <rezumat>
        /// Piesa ale cărei mișcări sunt reprezentate de acest obiect
        /// </rezumat>
        public Piece Piece
        {
            private set;
            get;
        }
        /// <rezumat>
        /// Direcția X
        /// </rezumat>
        public int X
        {
            private set;
            get;
        }

        /// <rezumat>
        /// Direcția Y
        /// </rezumat>
        public int Y
        {
            private set;
            get;
        }

        /// <rezumat>
        /// Mișcările posibile pe care le puteți face în această direcție, inclusiv piesa de blocare a liniei de vedere care poate fi lovită sau nu.
        /// Vezi și <seealso cref="GetPossibleMoves"/>
        /// </rezumat>
        private List<ChessBoard.Cell> possibleMoves;

        /// <rezumat>
        /// Mișcările posibile pe care le puteți face în această direcție
        /// </rezumat>
        /// <param name="enemyHittable">Piesele inamice pot fi lovite</param>
        /// <returns>O enumerare a posibilelor mișcări</returns>
        public IEnumerable<ChessBoard.Cell> GetPossibleMoves(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                yield break;

            for (int i = 0; i < possibleMoves.Count - 1; i++)
            {
                yield return possibleMoves[i];
            }

            if (possibleMoves.Last().Piece == null)
                yield return possibleMoves.Last();
            else if (enemyHittable && possibleMoves.Last().Piece.Color != Piece.Color)
                yield return possibleMoves.Last();
        }
        /// <rezumat>
        /// Numărul de mișcări posibile
        /// </rezumat>
        /// <param name="enemyHittable">Piesele inamice pot fi lovite</param>
        /// <returns>Numărul de mișcări posibile</returns>
        public int GetPossibleMoveCount(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                return 0;

            if (possibleMoves.Last().Piece == null)
                return possibleMoves.Count;
            else if (!enemyHittable || possibleMoves.Last().Piece.Color == Piece.Color)
                return possibleMoves.Count - 1;
            else
                return possibleMoves.Count;
        }

        /// <rezumat>
        /// Numărul de mișcări pe care le-am putea face, luând în considerare nicio blocare sau în afara bordului.
        /// </rezumat>
        public int DesiredCount
        {
            private set;
            get;
        }

        /// <rezumat>
        /// Indică dacă direcția ar trebui să actualizeze graficul de lovituri al posibilelor celule de mișcare
        /// </rezumat>
        private bool updateHitGraph;

        public Direction(Piece piece, int x, int y, int desiredCount = 8, bool updateHitGraph = true)
        {
            Piece = piece;
            X = x;
            Y = y;
            DesiredCount = desiredCount;
            this.updateHitGraph = updateHitGraph;

            possibleMoves = new List<ChessBoard.Cell>();
            possibleMoves.AddRange(piece.Parent.OpenLineOfSight(x, y, desiredCount));

            foreach (ChessBoard.Cell move in possibleMoves)
            {
                if (updateHitGraph)
                    move.HitBy.Add(Piece);
            }
        }

        /// <rezumat>
        /// Spune dacă piesa mutată pe celulă a schimbat starea de lovire a celui blocat
        /// </rezumat>
        /// <param name="from">Unde se află piesa chiar acum</param>
        /// <param name="to">Unde este mutată piesa</param>
        /// <param name="blocked">Hit testează această piesă</param>
        /// <returns>Dacă blocat poate fi lovit după mutarea de la</returns>
        public bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            if (possibleMoves.Contains(blocked) && !possibleMoves.Contains(to))
            {
                //Blocarea poate fi lovită de la început și nu îl blocăm cu un nou blocant
                //To poate fi încă blocat, dar direcției nu ar trebui să-i pese de asta
                return false;
            }
            else if (possibleMoves.Contains(from))
            {
                int toIndex = possibleMoves.IndexOf(to);
                if (0 <= toIndex && toIndex < possibleMoves.Count - 1)
                    return true; //Blocatorul mai aproape de piesă
                else
                {
                    //Dacă ne-am muta mai departe
                    foreach (ChessBoard.Cell move in from.OpenLineOfSight(X, Y, DesiredCount - possibleMoves.Count))
                    {
                        if (move == to) //Blocatorul s-a mutat pe noua cale
                            return true;
                        if (move == blocked) //Blocarea poate fi lovită
                            return false;
                    }
                }
            }

            //Se întâmplă când blocantul nu a fost inclus și blocat nu a fost conținut o combinație perfectă pentru ca nimic să nu se întâmple
            return true;
        }
    }
}
