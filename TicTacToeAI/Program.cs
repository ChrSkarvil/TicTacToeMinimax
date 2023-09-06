namespace TicTacToe
{
    class Program
    {
        static int Rows = 3;
        static int Columns = 3;
        static char[,] Board = new char[Rows, Columns];
        static bool player1Turn = true;
        static int pWin, aiWin, draw;

        public static void Main(string[] args)
        {
            SetupBoard();
            int toRow, toCol;
            while (true)
            {
                while (true)  // Inner loop for a single game
                {
                    //Print the board
                    PrintBoard(Rows, Columns);

                    if (IsGameOver())
                    {
                        char winner = GetWinner();
                        Console.WriteLine(ShowWinner(winner));
                        UpdateScoreboard(winner);
                        break;
                    }

                    Console.WriteLine(player1Turn ? "Your turn (X)" : "AI (O)");

                    //Your turn, enter where to place X
                    if (player1Turn)
                    {
                        Console.Write("Enter new column (0-2): ");
                        if (!int.TryParse(Console.ReadLine(), out toRow) || toRow < 0 || toRow > 2)
                        {
                            Console.WriteLine("Invalid input - Try again.");
                            Thread.Sleep(1000);
                            continue;
                        }

                        Console.Write("Enter new row (0-2): ");
                        if (!int.TryParse(Console.ReadLine(), out toCol) || toCol < 0 || toCol > 2)
                        {
                            Console.WriteLine("Invalid input - Try again.");
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                    else
                    {
                        Tuple<int, int> bestMove = GetBestMove();
                        toRow = bestMove.Item1;
                        toCol = bestMove.Item2;
                    }

                    //Check if move is valid, update board, and switch turns.
                    if (ValidMove(toRow, toCol))
                    {
                        Board[toRow, toCol] = player1Turn ? 'X' : 'O';
                        player1Turn = !player1Turn;

                    }
                    //Invalid move
                    else
                    {
                        Console.WriteLine("Invalid move - Try again.");
                        Console.ReadLine();
                    }

                }

                //Display scoreboard
                DisplayScoreboard(pWin, aiWin, draw);
                //Try again or break loop
                Console.Write("PRESS 'Y' TO PLAY AGAIN: ");
                if (Console.ReadLine().ToLower() != "y")
                    break;
                else
                {
                    //Reset board
                    SetupBoard();
                    player1Turn = true;
                }

            }
        }


        //Setup board, fill it with empty spaces
        static void SetupBoard()
        {
            for (var i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Board[i, j] = ' ';
                }
            }
        }

        //Print the board
        static void PrintBoard(int rows, int columns)
        {
            Console.Clear();
            Console.WriteLine("      0       1       2");
            for (int row = 0; row < rows; row++)
            {
                //Seperate columns
                Console.WriteLine("  +-------+-------+-------+");
                //Seperate rows
                Console.Write(row + " |");
                for (int col = 0; col < columns; col++)
                {
                    Console.Write("   " + Board[row, col] + "   |");
                }
                Console.WriteLine();
            }
            Console.WriteLine("  +-------+-------+-------+");
        }

        static bool ValidMove(int toRow, int toCol)
        {
            //Check if move is out of board
            if (toRow < 0 || toRow > Rows - 1 || toCol < 0 || toCol > Columns - 1)
                return false;

            //Check if cell is empty
            if (Board[toRow, toCol] == ' ')
            {
                return true;
            }

            return false;
        }


        static List<Tuple<int, int>> GetPossibleMoves()
        {
            List<Tuple<int, int>> possibleMoves = new List<Tuple<int, int>>();

            //Loop through board and check for valid moves
            for (int toRow = 0; toRow < Rows; toRow++)
            {
                for (int toCol = 0; toCol < Columns; toCol++)
                {
                    if (ValidMove(toRow, toCol))
                    {
                        possibleMoves.Add(Tuple.Create(toRow, toCol));
                    }
                }
            }

            return possibleMoves;
        }

        static Tuple<int, int> GetBestMove()
        {
            char aiPlayer = 'O';
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            Tuple<int, int> bestMove = null;

            //Loop through possible moves
            foreach (var move in GetPossibleMoves()) 
            {
                int row = move.Item1;
                int col = move.Item2;

                //Make the move
                Board[row, col] = aiPlayer;

                //Calculate the score for the move
                int score = Minimax(Board, 0, false, alpha, beta);

                //Undo the move
                Board[row, col] = ' ';

                //Check if this move gets a better score
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            return bestMove;
        }


        static int Minimax(char[,] board, int depth, bool isMaximizing, int alpha, int beta)
        {
            //Return score - depth
            if (IsWin('O'))
                return 10 - depth;
            if (IsWin('X'))
                return depth - 10;
            if (IsDraw())
                return 0;

            int score;
            //Find maximum score for AI
            if (isMaximizing)
            {
                //-INFINITY
                score = int.MinValue;
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        if (board[row, col] == ' ')
                        {
                            board[row, col] = 'O';
                            score = Math.Max(score, Minimax(board, depth + 1, false, alpha, beta));
                            board[row, col] = ' ';

                            alpha = Math.Max(alpha, score);

                            //Prune if beta is smaller or equal to alpha
                            if (beta <= alpha)
                                break;
                        }
                    }
                }
            }
            //Find minimum score for player X
            else
            {
                //+INFINITY
                score = int.MaxValue;
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        if (board[row, col] == ' ')
                        {
                            board[row, col] = 'X';
                            score = Math.Min(score, Minimax(board, depth + 1, true, alpha, beta));
                            board[row, col] = ' ';

                            alpha = Math.Min(alpha, score);

                            //Prune if beta is smaller or equal to alpha
                            if (beta <= alpha)
                                break;
                        }
                    }
                }
            }
            return score;
        }

        //Update the scoreboard
        static void UpdateScoreboard(char winner)
        {
            if (winner == 'X')
                pWin++;
            else if (winner == 'O')
                aiWin++;
            else
                draw++;
        }


        //Show scoreboard
        static void DisplayScoreboard(int pWin, int aiWin, int draw)
        {
            Console.WriteLine("\n-------- SCOREBOARD --------");
            Console.WriteLine($"Player Wins: {pWin}");
            Console.WriteLine($"AI Wins: {aiWin}");
            Console.WriteLine($"Draws: {draw}");
            Console.WriteLine("-----------------------------");
        }

        //Check for win
        static bool IsWin(char player)
        {
            for (int i = 0; i < 3; i++)
            {
                //Check for wins horizontal
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;
                //Check for wins vertical
                if (Board[0, i] == player && Board[1, i] == player && Board[2, i] == player)
                    return true;
            }
            //Check for win across board
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;

            return false;
        }



        //Check for draw
        static bool IsDraw()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    //Check if there is an empty spot
                    if (Board[row, col] == ' ')
                    {
                        return false;
                    }
                }
            }

            //No empty spots left, its a draw
            return true;
        }


        //Check for winner
        static bool IsGameOver()
        {
            return IsWin('X') || IsWin('O') || IsDraw();
        }

        //Get the winner
        static char GetWinner()
        {
            if (IsWin('X')) return 'X';
            if (IsWin('O')) return 'O';
            return 'D';
        }

        //Show who won
        static string ShowWinner(char winner)
        {
            if (winner == 'X') return "\n          YOU WIN!";
            if (winner == 'O') return "\n          AI WINS!";
            return "\n         ITS A DRAW!";
        }

    }
}