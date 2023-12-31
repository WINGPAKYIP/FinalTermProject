﻿using System;
using System.Xml.Linq;

public class ConnectFourGame
{
    private const int Rows = 6;
    private const int Columns = 7;
    private char[,] board;
    private bool isGameOver;

    public ConnectFourGame()
    {
        board = new char[Rows, Columns];
        InitializeBoard();
        isGameOver = false;
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

    public void PrintBoard()
    {
        Console.Clear();
        Console.WriteLine("Connect Four Game Development Project:");
        Console.WriteLine();

        for (int row = 0; row < Rows; row++)
        {
            Console.Write("|");
            for (int col = 0; col < Columns; col++)
            {
                Console.Write($"{board[row, col]}|");
            }
            Console.WriteLine();
        }

        Console.WriteLine("-----------------");
        Console.WriteLine(" 1 2 3 4 5 6 7");
        Console.WriteLine();
    }

    public bool IsMoveValid(int column)
    {
        return column >= 1 && column <= Columns && board[0, column - 1] == ' ';
    }

    public void MakeMove(int column, char symbol)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (board[row, column - 1] == ' ')
            {
                board[row, column - 1] = symbol;
                PrintBoard(); // to print the board after each move
                break;
            }
        }
    }

    public bool IsGameOver(Player player1, Player player2, out bool restart, out Player winningPlayer)
    {
        if (CheckWinCondition(player1.Symbol))
        {
            Console.WriteLine($"It is a Connect Four. {player1.Name} wins!");
            restart = PromptRestart();
            winningPlayer = player1;
            return true;
        }
        else if (CheckWinCondition(player2.Symbol))
        {
            Console.WriteLine($"It is a Connect Four. {player2.Name} wins!");
            restart = PromptRestart();
            winningPlayer = player2;
            return true;
        }
        else if (IsBoardFull())
        {
            Console.WriteLine("It's a draw!");
            restart = PromptRestart();
            winningPlayer = null;
            return true;
        }

        restart = false;
        winningPlayer = null;
        return false;
    }


    private bool PromptRestart()
    {
        Console.Write("Restart? Yes (1) No (0): ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 0 && choice != 1))
        {
            Console.WriteLine("Invalid input. Please enter 0 or 1.");
        }

        return choice == 1;
    }



    private bool CheckWinCondition(char symbol)
    {
        // Check rows
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col <= Columns - 4; col++)
            {
                if (board[row, col] == symbol &&
                    board[row, col + 1] == symbol &&
                    board[row, col + 2] == symbol &&
                    board[row, col + 3] == symbol)
                {
                    return true;
                }
            }
        }

        // Check columns
        // Check columns
        for (int row = 0; row <= Rows - 4; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (board[row, col] == symbol &&
                    board[row + 1, col] == symbol &&
                    board[row + 2, col] == symbol &&
                    board[row + 3, col] == symbol)
                {
                    return true;
                }
            }
        }

        // Check diagonals (top-left to bottom-right)
        for (int row = 0; row <= Rows - 4; row++)
        {
            for (int col = 0; col <= Columns - 4; col++)
            {
                if (board[row, col] == symbol &&
                    board[row + 1, col + 1] == symbol &&
                    board[row + 2, col + 2] == symbol &&
                    board[row + 3, col + 3] == symbol)
                {
                    return true;
                }
            }
        }

        // Check diagonals (top-right to bottom-left)
        for (int row = 0; row <= Rows - 4; row++)
        {
            for (int col = Columns - 1; col >= 3; col--)
            {
                if (board[row, col] == symbol &&
                    board[row + 1, col - 1] == symbol &&
                    board[row + 2, col - 2] == symbol &&
                    board[row + 3, col - 3] == symbol)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsBoardFull()
    {
        for (int col = 0; col < Columns; col++)
        {
            if (board[0, col] == ' ')
            {
                return false;
            }
        }

        return true;
    }
}

public abstract class Player
{
    public char Symbol { get; }
    public string Name { get; }

    public Player(char symbol, string name)
    {
        Symbol = symbol;
        Name = name;
    }

    public abstract int GetNextMove();
}

public class HumanPlayer : Player
{
    public HumanPlayer(char symbol, string name) : base(symbol, name)
    {
    }

    public override int GetNextMove()
    {
        Console.Write($"{Name}, enter column number (1-7): ");
        int column;
        while (!int.TryParse(Console.ReadLine(), out column) || column < 1 || column > 7)
        {
            Console.WriteLine("Invalid input. Please enter a valid column number (1-7).");
        }

        return column;
    }
}


public class GameController
{
    private ConnectFourGame game;
    private Player player1;
    private Player player2;

    public GameController()
    {
        game = new ConnectFourGame();
        player1 = CreatePlayer('X', "Player 1");
        player2 = CreatePlayer('O', "Player 2");
    }

    private Player CreatePlayer(char symbol, string playerLabel)
    {
        Console.Write($"{playerLabel}, please enter your name: ");
        string name = Console.ReadLine();
        return new HumanPlayer(symbol, name);
    }

    public void StartGame()
    {
        bool isPlayer1Turn = true;
        bool restart = true;
        Player winningPlayer = null;

        while (restart)
        {
            game = new ConnectFourGame();
            isPlayer1Turn = true;

            while (!game.IsGameOver(player1, player2, out restart, out winningPlayer))
            {
                game.PrintBoard();

                Player currentPlayer = isPlayer1Turn ? player1 : player2;
                int column = currentPlayer.GetNextMove();

                if (game.IsMoveValid(column))
                {
                    game.MakeMove(column, currentPlayer.Symbol);
                    isPlayer1Turn = !isPlayer1Turn;
                }
                else
                {
                    Console.WriteLine("Invalid move. Please select a different column.");
                }
            }
        }

        game.PrintBoard();
        if (winningPlayer != null)
        {
            Console.WriteLine($"It is a Connect Four. {winningPlayer.Name} wins!");
        }
        Console.ReadLine();
    }

class Program
    {
        static void Main(string[] args)
        {
            GameController controller = new GameController();
            controller.StartGame();
        }
    }

}
