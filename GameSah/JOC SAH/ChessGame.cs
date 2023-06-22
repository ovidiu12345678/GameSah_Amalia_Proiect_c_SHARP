using System;
using System.Linq;

namespace ChessConsole
{
    public enum PlayerColor
    {
        White, Black
    }

    public enum PlayerState
    {
        Idle, Holding, AwaitPromote, GameOver
    }

    public enum PromoteOptions
    {
        Queen = 0, Rook = 1, Bishop = 2, Knight = 3
    }

    public class ChessGame
    {
        /// <summary>
        ///Fals indică că jocul ar trebui să iasă
        /// </summary>
        public bool Running
        {
            private set;
            get;
        }

        private PlayerState playerState;

        /// <summary>
        /// Opțiune de promovare selectată în prezent
        /// </summary>
        private PromoteOptions promoteOption;

        /// <summary>
        ///Adevărat pentru alb, fals pentru negru
        /// </summary>
        private PlayerColor currentPlayer;

        /// <summary>
        /// Coordonatele cursorului virtual de pe tablă
        /// </summary>
        private int cursorX, cursorY;

        /// <summary>
        /// Tabla de șah propriu-zisă
        /// </summary>
        private ChessBoard board;

        /// <summary>
        ///  Celula părinte a piesei deținute în prezent
        /// </summary>
        private ChessBoard.Cell holdedNode = null;

        /// <summary>
        /// Unde să te muți
        /// </summary>
        private ChessBoard.Cell moveTo = null;

        public ChessGame()
        {
            Running = true;
            board = new ChessBoard();
            currentPlayer = PlayerColor.White;
            turnStart();
        }

        #region PublicInterfaceCommands
        public void Update()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.LeftArrow && cursorX > 0 && playerState != PlayerState.AwaitPromote)
                    cursorX--;
                else if (keyInfo.Key == ConsoleKey.RightArrow && cursorX < 7 && playerState != PlayerState.AwaitPromote)
                    cursorX++;
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (playerState != PlayerState.AwaitPromote && cursorY < 7)
                        cursorY++;
                    else if ((int)promoteOption > 0)
                        promoteOption--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (playerState != PlayerState.AwaitPromote && cursorY > 0)
                        cursorY--;
                    else if ((int)promoteOption < 3)
                        promoteOption++;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                    interact();
                else if (keyInfo.Key == ConsoleKey.D)
                    debugInteract();
                else if (keyInfo.Key == ConsoleKey.Escape)
                    cancel();
            }
        }

        /// <summary>
        /// Draws the game
        /// </summary>
        /// <param name="g">Obiect ConsoleGraphics cu care să desenați/to</param>
        public void Draw(ConsoleGraphics g)
        {
            g.FillArea(new CChar(' ', ConsoleColor.Black, ConsoleColor.DarkGray), 10, 5, 8, 8);

            //7-j peste tot pentru că este inversat la șah
            for (int i = 0; i < 8; i++)
           {
                for (int j = 0; j < 8; j++)
                {
                    // Desenați simbolul
                    ChessBoard.Cell cell = board.GetCell(i, j);
                    if (cell.Piece != null)
                    {
                        g.DrawTransparent(cell.Piece.Char, (cell.Piece.Color == PlayerColor.White) ? ConsoleColor.White : ConsoleColor.Black, 10 + i, 5 + (7 - j));
                        if (cell.Piece.LegalMoves.Count == 0)
                        {
                            g.SetBackground(ConsoleColor.DarkRed, 10 + i, 5 + (7 - j));
                        }
                    }

                    if (cell.HitBy.Contains(debugPiece))
                        g.SetBackground(ConsoleColor.DarkMagenta, 10 + i, 5 + (7 - j));
                }
            }

            if (holdedNode != null && playerState == PlayerState.Holding)
            {
                // Evidențiați mișcările legale
                foreach (ChessBoard.Cell move in holdedNode.Piece.LegalMoves)
                {
                    g.SetBackground(ConsoleColor.DarkGreen, 10 + move.X, 5 + (7 - move.Y));
                }
            }

            // Setează culoarea cursorului -> galben
            g.SetBackground(ConsoleColor.DarkYellow, 10 + cursorX, 5 + (7 - cursorY));

            //TODO: Eliminați testarea en passant
            /*dacă (board.EnPassant != null)
                g.SetBackground(ConsoleColor.DarkCyan, 10 + board.EnPassant.X, 5 + (7 - board.EnPassant.Y));

            if (board.EnPassantCapture != null)
                g.SetBackground(ConsoleColor.DarkMagenta, 10 + board.EnPassantCapture.X, 5 + (7 - board.EnPassantCapture.Y));*/

            //Ușurează pentru modelul de șah
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1) g.LightenBackground(10 + i, 5 + j);
                }
            }

            //Meniu de opțiuni de promovare

            if (playerState == PlayerState.AwaitPromote)
            {
                g.DrawTextTrasparent("Queen", promoteOption == PromoteOptions.Queen ? ConsoleColor.Yellow : ConsoleColor.White, 22, 7);
                g.DrawTextTrasparent("Rook", promoteOption == PromoteOptions.Rook ? ConsoleColor.Yellow : ConsoleColor.White, 22, 9);
                g.DrawTextTrasparent("Bishop", promoteOption == PromoteOptions.Bishop ? ConsoleColor.Yellow : ConsoleColor.White, 22, 11);
                g.DrawTextTrasparent("Knight", promoteOption == PromoteOptions.Knight ? ConsoleColor.Yellow : ConsoleColor.White, 22, 13);
            }
            else
            {
                g.ClearArea(22, 7, 6, 7);
            }
        }

        #endregion

        #region EventHandlerLikeMethods

        /// <rezumat>
        /// Se întâmplă când utilizatorul apasă tasta Enter
        /// </rezumat>
        private void interact()
        {
            switch (playerState)
            {
                case PlayerState.Idle:
                    holdedNode = board.GetCell(cursorX, cursorY);

                    if (holdedNode.Piece == null || holdedNode.Piece.Color != currentPlayer || holdedNode.Piece.LegalMoves.Count == 0)
                    {
                        holdedNode = null;
                        return;
                    }
                    else playerState = PlayerState.Holding;


                    break;
                case PlayerState.Holding:
                    playerState = PlayerState.Holding;

                    moveTo = board.GetCell(cursorX, cursorY);

                    if (!holdedNode.Piece.LegalMoves.Contains(moveTo))
                    {
                        moveTo = null;
                        return;
                    }

                    if (board.IsPromotable(holdedNode, moveTo))
                        showPromote();
                    else
                        turnOver();
                    
                    break;
                case PlayerState.AwaitPromote:
                    turnOver();
                    break;
                case PlayerState.GameOver:
                    Running = false;
                    break;
            }
        }


        private Piece debugPiece;
        private void debugInteract()
        {
            debugPiece = board.GetCell(cursorX, cursorY).Piece;
        }

        /// <rezumat>
        /// Se întâmplă când utilizatorul apasă tasta de evacuare
        /// </rezumat>
        private void cancel()
        {
            playerState = PlayerState.Idle;
            holdedNode = null;
        }

        #endregion

        #region EventLikeMethods
        /// <rezumat>
        /// Apelat la fiecare pornire de viraj
        /// </rezumat>
        private void turnStart()
        {
            board.TurnStart(currentPlayer);
        }

        /// <rezumat>
        /// Afișează dialogul de promovare (setează starea)
        /// </rezumat>
        private void showPromote()
        {
            playerState = PlayerState.AwaitPromote;
            promoteOption = PromoteOptions.Queen; //reseteaza meniu
        }

        /// <rezumat>
        /// Apelat când rândul este transmis celuilalt jucător
        /// </rezumat>
        private void turnOver()
        {
            board.Move(holdedNode, moveTo, promoteOption);
            holdedNode = null;
            moveTo = null;
            playerState = PlayerState.Idle;
            currentPlayer = currentPlayer == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            turnStart();
        }
        #endregion // regiune finală
    }
}
