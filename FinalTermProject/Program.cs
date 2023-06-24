using System;

public class ConnectFourGame
{
    private const int Rows = 6;
    private const int Columns = 7;
    private char[,] board;
    private int currentPlayerIndex;

    public ConnectFourGame()
    {
        board = new char[Rows, Columns];
        currentPlayerIndex = 0;
        InitializeBoard();
    }

    public void PlayMove(int column)
    {
        if (column < 0 || column >= Columns || IsColumnFull(column))
        {
            Console.WriteLine("Invalid move. Please try again.");
            return;
        }

        int row = GetLowestEmptyRow(column);
        board[row, column] = GetPlayerSymbol(currentPlayerIndex);

        if (HasPlayerWon(row, column))
        {
            Console.WriteLine($"Player {currentPlayerIndex + 1} wins!");
            InitializeBoard();
        }
        else if (IsBoardFull())
        {
            Console.WriteLine("It's a draw!");
            InitializeBoard();
        }
        else
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }

    private void InitializeBoard()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                board[row, col] = ' ';
            }
        }
    }

    private bool IsColumnFull(int column)
    {
        return board[0, column] != ' ';
    }

    private int GetLowestEmptyRow(int column)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (board[row, column] == ' ')
            {
                return row;
            }
        }
        return -1; // Column is full, should not happen if checked before calling this method
    }

    private bool HasPlayerWon(int row, int column)
    {
        char symbol = GetPlayerSymbol(currentPlayerIndex);

        // Check horizontally
        for (int col = Math.Max(0, column - 3); col <= Math.Min(column + 3, Columns - 1); col++)
        {
            if (board[row, col] == symbol &&
                board[row, col + 1] == symbol &&
                board[row, col + 2] == symbol &&
                board[row, col + 3] == symbol)
            {
                return true;
            }
        }

        // Check vertically
        for (int r = Math.Max(0, row - 3); r <= Math.Min(row + 3, Rows - 1); r++)
        {
            if (board[r, column] == symbol &&
                board[r + 1, column] == symbol &&
                board[r + 2, column] == symbol &&
                board[r + 3, column] == symbol)
            {
                return true;
            }
        }

        // Check diagonally (top-left to bottom-right)
        for (int offset = -3; offset <= 0; offset++)
        {
            if (IsSymbolInDiagonal(board, row + offset, column + offset, symbol, 1, 1))
            {
                return true;
            }
        }

        // Check diagonally (bottom-left to top-right)
        for (int offset = -3; offset <= 0; offset++)
        {
            if (IsSymbolInDiagonal(board, row - offset, column + offset, symbol, -1, 1))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsSymbolInDiagonal(char[,] board, int startRow, int startColumn, char symbol, int rowIncrement, int columnIncrement)
    {
        for (int i = 0; i < 4; i++)
        {
            if (board[startRow + i * rowIncrement, startColumn + i * columnIncrement] != symbol)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsBoardFull()
    {
        for (int col = 0; col < Columns; col++)
        {
            if (!IsColumnFull(col))
            {
                return false;
            }
        }
        return true;
    }

    private char GetPlayerSymbol(int playerIndex)
    {
        return playerIndex == 0 ? 'X' : 'O';
    }

    public void PrintBoard()
    {
        Console.WriteLine();
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Console.Write(board[row, col] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

public abstract class Player
{
    public string Name { get; }

    protected Player(string name)
    {
        Name = name;
    }

    public abstract int GetMove();
}

public class HumanPlayer : Player
{
    public HumanPlayer(string name) : base(name)
    {
    }

    public override int GetMove()
    {
        Console.Write($"Player {Name}, enter your move (column number): ");
        string input = Console.ReadLine();
        int move;
        while (!int.TryParse(input, out move))
        {
            Console.Write("Invalid input. Please enter a valid column number: ");
            input = Console.ReadLine();
        }
        return move - 1;
    }
}

public class ComputerPlayer : Player
{
    public ComputerPlayer(string name) : base(name)
    {
    }

    public override int GetMove()
    {
        // Simple logic for computer player's move
        Random random = new Random();
        return random.Next(0, 7);
    }
}

public class GameController
{
    private ConnectFourGame game;
    private Player[] players;
    private ConsoleInputOutput inputOutput;
    private int currentPlayerIndex;

    public GameController()
    {
        game = new ConnectFourGame();
        players = new Player[2];
        inputOutput = new ConsoleInputOutput();
        currentPlayerIndex = 0;
    }

    public void Run()
    {
        inputOutput.DisplayMessage("Welcome to Connect Four!");

        string player1Name = inputOutput.GetPlayerName(1);
        string player2Name = inputOutput.GetPlayerName(2);

        players[0] = new HumanPlayer(player1Name);
        players[1] = new HumanPlayer(player2Name);

        while (true)
        {
            game.PrintBoard();

            Player currentPlayer = players[currentPlayerIndex];

            int move = currentPlayer.GetMove();
            game.PlayMove(move);

            inputOutput.DisplayMessage("");

            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }
}

public class ConsoleInputOutput
{
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public string GetPlayerName(int playerNumber)
    {
        Console.Write($"Enter the name for Player {playerNumber}: ");
        return Console.ReadLine();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        GameController gameController = new GameController();
        gameController.Run();
    }
}

