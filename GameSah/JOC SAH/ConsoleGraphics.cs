using System;

namespace ChessConsole
{
    /// <rezumat>
    /// Caracter de consolă colorat pentru <see cref="ConsoleGraphics.backBuffer"/> și <see cref="ConsoleGraphics.frontBuffer"/>
    /// </rezumat>
    public struct CChar : IEquatable<CChar>
    {
        public ConsoleColor Foreground;
        public ConsoleColor Background;
        /// <rezumat>
        /// Valoarea reală a caracterului
        /// </rezumat>
        public char C;

        public CChar(char c = ' ', ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Foreground = foreground;
            Background = background;
            C = c;
        }

        public bool Equals(CChar other)
        {
            return other.Foreground == Foreground && other.Background == Background && other.C == C;
        }

        public static bool operator == (CChar lhs, CChar rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(CChar lhs, CChar rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    /// <rezumat>
    /// Se ocupă de consola C# cu tampon dublu
    /// </rezumat>
    public class ConsoleGraphics
    {
        /// <rezumat>
        /// Mai întâi totul este atras în buffer-ul din spate în scopuri de tamponare dublă
        /// puteți „schimba” bufferele cu <see cref="SwapBuffers"/>
        /// </rezumat>
        private CChar[,] backBuffer;

        /// <rezumat>
        /// Bufferul de caractere colorat al consolei curente
        /// puteți „schimba” bufferele cu <see cref="SwapBuffers"/>
        /// </rezumat>
        private CChar[,] frontBuffer;

        public ConsoleGraphics()
        {
            backBuffer = new CChar[Console.BufferWidth, Console.BufferHeight];
            frontBuffer = new CChar[Console.BufferWidth, Console.BufferHeight];
        }



        #region DrawMethods

        /// <rezumat>
        /// Șterge bufferul din spate, următorul SwapBuffer
        /// </rezumat>
        public void Clear()
        {
            for (int i = 0; i < backBuffer.GetLength(0); i++)
            {
                for (int j = 0; j < backBuffer.GetLength(1); j++)
                {
                    backBuffer[i, j] = new CChar();
                }
            }
        }

        /// <rezumat>
        /// Desenează un caracter colorat în tamponul din spate
        /// </rezumat>
        /// <param name="cchar">Personajul colorat de desenat</param>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void Draw(CChar cchar, int x, int y)
        {
            backBuffer[x, y] = cchar;
        }

        /// <rezumat>
        /// Desenează un caracter colorat în tamponul din spate, nu își schimbă culoarea de fundal
        /// </rezumat>
        /// <param name="c">Personajul de desenat</param>
        /// <param name="foreground">Culoarea personajului</param>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void DrawTransparent(char c, ConsoleColor foreground, int x, int y)
        {
            backBuffer[x, y].C = c;
            backBuffer[x, y].Foreground = foreground;
        }

        /// <rezumat>
        /// Desenează o zonă de caractere colorate în tamponul din spate.
        /// Lungimea matricei este folosită ca lățime și înălțime a zonei.
        /// </rezumat>
        /// <param name="cchars">Zona de caractere colorată de desenat</param>
        /// <param name="x">Pornirea X-coord în bufferul consolei</param>
        /// <param name="y">Pornirea Y-coord în bufferul consolei</param>
        public void DrawArea(CChar[,] cchars, int x, int y)
        {
            for (int i = 0; i < cchars.GetLength(0); i++)
            {
                for (int j = 0; j < cchars.GetLength(1); j++)
                {
                    backBuffer[x + i, y + j] = cchars[i, j];
                }
            }
        }

        /// <rezumat>
        /// Desenează text pe ecran. Multiline nu este gestionat.
        /// </rezumat>
        /// <param name="text">Textul de desenat</param>
        /// <param name="foreground">Culoarea primului plan a textului</param>
        /// <param name="background">Culoarea de fundal a textului</param>
        /// <param name="x">Pornirea X-coord în bufferul consolei</param>
        /// <param name="y">Pornirea Y-coord în bufferul consolei</param>
        public void DrawText(string text, ConsoleColor foreground, ConsoleColor background, int x, int y)
        {
            CChar[,] area = new CChar[text.Length, 1];
            for (int i = 0; i < text.Length; i++)
            {
                area[i, 0] = new CChar(text[i], foreground, background);
            }

            DrawArea(area, x, y);
        }

        /// <rezumat>
        /// Desenează text pe ecran cu un fundal transparent. Multiline nu este gestionat.
        /// </rezumat>
        /// <param name="text">Textul de desenat</param>
        /// <param name="foreground">Culoarea primului plan a textului</param>
        /// <param name="x">Pornirea X-coord în bufferul consolei</param>
        /// <param name="y">Pornirea Y-coord în bufferul consolei</param>
        public void DrawTextTrasparent(string text, ConsoleColor foreground, int x, int y)
        {
            CChar[,] area = new CChar[text.Length, 1];
            for (int i = 0; i < text.Length; i++)
            {
                area[i, 0] = new CChar(text[i], foreground, backBuffer[x + i, y].Background);
            }

            DrawArea(area, x, y);
        }

        /// <rezumat>
        /// Umple o zonă a memoriei tampon din spate cu un caracter colorat specific
        /// Lungimea matricei este folosită ca lățime și înălțime a zonei.
        /// </rezumat>
        /// <param name="cchar">Zona de caractere colorată de desenat</param>
        /// <param name="x">Pornirea X-coord în bufferul consolei</param>
        /// <param name="y">Pornirea Y-coord în bufferul consolei</param>
        /// <param name="width">Lățimea zonei</param>
        /// <param name="height">Înălțimea zonei</param>
        public void FillArea(CChar cchar, int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    backBuffer[x + i, y + j] = cchar;
                }
            }
        }
        /// <rezumat>
        /// Șterge zona de pe ecran
        /// </rezumat>
        /// <param name="x">Pornirea X-coord în bufferul consolei</param>
        /// <param name="y">Pornirea Y-coord în bufferul consolei</param>
        /// <param name="width">Lățimea zonei</param>
        /// <param name="height">Înălțimea zonei</param>
        public void ClearArea(int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    backBuffer[x + i, y + j] = new CChar();
                }
            }
        }

        #endregion

        #region Darken_Lighten

        /// <rezumat>
        /// Închide culoarea de fundal a unui caracter colorat din bufferul din spate.
        /// Dacă culoarea de fundal este deja întunecată sau nu există o versiune întunecată, aceasta o lasă neschimbată.
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void DarkenBackground(int x, int y)
        {
            switch (backBuffer[x, y].Background)
            {
                case ConsoleColor.Blue:
                    backBuffer[x, y].Background = ConsoleColor.DarkBlue;
                    break;
                case ConsoleColor.Green:
                    backBuffer[x, y].Background = ConsoleColor.DarkGreen;
                    break;
                case ConsoleColor.Yellow:
                    backBuffer[x, y].Background = ConsoleColor.DarkYellow;
                    break;
                case ConsoleColor.Magenta:
                    backBuffer[x, y].Background = ConsoleColor.DarkMagenta;
                    break;
                case ConsoleColor.Gray:
                    backBuffer[x, y].Background = ConsoleColor.DarkGray;
                    break;
                case ConsoleColor.Cyan:
                    backBuffer[x, y].Background = ConsoleColor.DarkCyan;
                    break;
                case ConsoleColor.Red:
                    backBuffer[x, y].Background = ConsoleColor.DarkRed;
                    break;
            }
        }

        /// <rezumat>
        /// Întunecă culoarea primului plan a unui caracter colorat din bufferul din spate.
        /// Dacă culoarea primului plan este deja întunecată sau nu există o versiune întunecată, aceasta o lasă neschimbată.
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void DarkenForeground(int x, int y)
        {
            switch (backBuffer[x, y].Foreground)
            {
                case ConsoleColor.Blue:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkBlue;
                    break;
                case ConsoleColor.Green:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkGreen;
                    break;
                case ConsoleColor.Yellow:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkYellow;
                    break;
                case ConsoleColor.Magenta:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkMagenta;
                    break;
                case ConsoleColor.Gray:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkGray;
                    break;
                case ConsoleColor.Cyan:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkCyan;
                    break;
                case ConsoleColor.Red:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkRed;
                    break;
            }
        }

        /// <rezumat>
        /// Luminează culoarea de fundal a unui caracter colorat din bufferul din spate.
        /// Dacă culoarea de fundal este deja deschisă sau nu există o versiune deschisă, aceasta o lasă neschimbată.
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void LightenBackground(int x, int y)
        {
            switch (backBuffer[x, y].Background)
            {
                case ConsoleColor.DarkBlue:
                    backBuffer[x, y].Background = ConsoleColor.Blue;
                    break;
                case ConsoleColor.DarkGreen:
                    backBuffer[x, y].Background = ConsoleColor.Green;
                    break;
                case ConsoleColor.DarkYellow:
                    backBuffer[x, y].Background = ConsoleColor.Yellow;
                    break;
                case ConsoleColor.DarkMagenta:
                    backBuffer[x, y].Background = ConsoleColor.Magenta;
                    break;
                case ConsoleColor.DarkGray:
                    backBuffer[x, y].Background = ConsoleColor.Gray;
                    break;
                case ConsoleColor.DarkCyan:
                    backBuffer[x, y].Background = ConsoleColor.Cyan;
                    break;
                case ConsoleColor.DarkRed:
                    backBuffer[x, y].Background = ConsoleColor.Red;
                    break;
            }
        }

        /// <rezumat>
        /// Luminează culoarea primului plan a unui caracter colorat din bufferul din spate.
        /// Dacă culoarea primului plan este deja deschisă sau nu există o versiune deschisă, aceasta o lasă neschimbată.
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void LightenForeground(int x, int y)
        {
            switch (backBuffer[x, y].Foreground)
            {
                case ConsoleColor.DarkBlue:
                    backBuffer[x, y].Foreground = ConsoleColor.Blue;
                    break;
                case ConsoleColor.DarkGreen:
                    backBuffer[x, y].Foreground = ConsoleColor.Green;
                    break;
                case ConsoleColor.DarkYellow:
                    backBuffer[x, y].Foreground = ConsoleColor.Yellow;
                    break;
                case ConsoleColor.DarkMagenta:
                    backBuffer[x, y].Foreground = ConsoleColor.Magenta;
                    break;
                case ConsoleColor.DarkGray:
                    backBuffer[x, y].Foreground = ConsoleColor.Gray;
                    break;
                case ConsoleColor.DarkCyan:
                    backBuffer[x, y].Foreground = ConsoleColor.Cyan;
                    break;
                case ConsoleColor.DarkRed:
                    backBuffer[x, y].Foreground = ConsoleColor.Red;
                    break;
            }
        }

        #endregion

        #region Color Getters/Setters
        /// <rezumat>
        /// Setează culoarea de fundal a bufferului din spate la (x, y)
        /// </rezumat>
        /// <param name="color">Culoare de fundal nouă</param>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void SetBackground(ConsoleColor color, int x, int y)
        {
            backBuffer[x, y].Background = color;
        }

        /// <rezumat>
        /// Obține culoarea de fundal a bufferului din spate la (x, y)
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        /// <returns>Culoarea de fundal la (x, y)</returns>
        public ConsoleColor GetBackground(int x, int y)
        {
            return backBuffer[x, y].Background;
        }

        /// <rezumat>
        /// Setează culoarea primului plan a tamponului din spate la (x, y)
        /// </rezumat>
        /// <param name="color">Culoare nouă de prim-plan</param>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        public void SetForeground(ConsoleColor color, int x, int y)
        {
            backBuffer[x, y].Foreground = color;
        }

        /// <rezumat>
        /// Obține culoarea primului plan a tamponului din spate la (x, y)
        /// </rezumat>
        /// <param name="x">X-coord în bufferul consolei</param>
        /// <param name="y">Y-coord în bufferul consolei</param>
        /// <returns>Culoarea primului plan la (x, y)</returns>
        public ConsoleColor GetForeground(int x, int y)
        {
            return backBuffer[x, y].Foreground;
        }
        #endregion

        /// <rezumat>
        /// Suprascrie FrontBuffer și redesenează caracterul dacă este diferit de BackBuffer
        /// </rezumat>
        public void SwapBuffers()
        {
            for (int i = 0; i < backBuffer.GetLength(0); i++)
            {
                for (int j = 0; j < backBuffer.GetLength(1); j++)
                {
                    if (frontBuffer[i, j] != backBuffer[i, j])
                    {
                        Console.SetCursorPosition(i, j);
                        Console.ForegroundColor = backBuffer[i, j].Foreground;
                        Console.BackgroundColor = backBuffer[i, j].Background;
                        Console.Write(backBuffer[i, j].C);
                        frontBuffer[i, j] = backBuffer[i, j];
                    }
                }
            }
        }
    }
}
