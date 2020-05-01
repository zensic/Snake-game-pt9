using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Media;
using System.IO;

namespace Snake
{
    // Stores the position of the food
    struct Position
    {
        public int row;
        public int col;
        public Position(int aRow, int aCol)
        {
            row = aRow;
            col = aCol;
        }
    }

    class Program
    {
        // Drawer function
        // Draws symbols based on specified color onto given coordinate
        public static void Draw(string aColor, int aCol, int aRow, string aSymbol)
        {
            switch (aColor)
            {
                case "Blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "Cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "Red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "Magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "Yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "DarkGray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "Green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.SetCursorPosition(aCol, aRow); // Moves cursor to the coordinate about to drawn
            Console.Write(aSymbol); // Draws symbol on the coordinate
        }

        public static void Main(string[] args)
        {
            // Assigning values to right, left, down, up as index
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int gLastFoodTime = 0;
            int gFoodDissapearTime = 12000; // Time for food to dissapear in milliseconds
            int gNegativePoints = 0; // Points to be deducted from the final score
            string gDifficulty = "normal";
            double gSleepTime = 100; // Velocity of snake

            while (true)
            {
                Console.BufferHeight = Console.WindowHeight; // Intialize the height of the game window
                Console.BufferWidth = Console.WindowWidth; // Initialize the width of the game window
                Console.Clear();
                Draw("White", Console.BufferWidth / 2 - 3, Console.BufferHeight / 2 - 6, "Main Menu");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 - 5, "------------------------");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 - 4, "[1] Start Game");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 - 3, "[2] Difficulty");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 - 2, "[3] Highscores");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 - 1, "[4] Instructions");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2, "[5] Exit\n");
                Draw("White", Console.BufferWidth / 2 - 8, Console.BufferHeight / 2 + 2, "Enter Your Selection: ");
                string gSelection = Console.ReadLine();

                if (gSelection == "1")
                {
                    break;
                }

                else if (gSelection == "2")
                {
                    Console.Clear();
                    Draw("White", Console.BufferWidth / 2 - 5, Console.BufferHeight / 2 - 6, "Current Difficulty: " + gDifficulty);
                    Draw("White", Console.BufferWidth / 2 - 5, Console.BufferHeight / 2 - 4, "Select difficulty level");
                    Draw("White", Console.BufferWidth / 2 - 2, Console.BufferHeight / 2 - 3, "[1] Easy");
                    Draw("White", Console.BufferWidth / 2 - 2, Console.BufferHeight / 2 - 2, "[2] Normal");
                    Draw("White", Console.BufferWidth / 2 - 2, Console.BufferHeight / 2 - 1, "[3] Hard\n");
                    Draw("White", Console.BufferWidth / 2 - 5, Console.BufferHeight / 2 + 1, "Enter Your Selection: ");

                    // Sets value of difficulty
                    gDifficulty = Console.ReadLine();
                }

                else if (gSelection == "3")
                {
                    Console.Clear(); //To clear out current screen
                    // Display Scoreboard
                    string[] content = File.ReadAllLines(@"..\Scores\score.txt");
                    Console.WriteLine("Scoreboard");
                    foreach (string x in content)
                    {
                        Console.WriteLine(x);
                    }

                    Console.WriteLine("\nPress enter to continue...");
                    Console.ReadLine();
                }

                else if (gSelection == "4")
                {
                    // Implement instructions here..
                    Console.WriteLine("\nPress enter to continue...");
                    Console.ReadLine();
                }

                else
                {
                    System.Environment.Exit(0);
                }
            }
            Console.Clear(); //To clear out current screen

            //Input for player name
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, 13); //Reposition the string
            Console.WriteLine("Type in your username: ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, 14); //Reposition the string
            Console.WriteLine("_______________________");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, 14); //Reposition the string
            string lPlyr_name = Console.ReadLine();
            Console.Clear(); //To clear out current screen
            Draw("Blue", 10, 0, "Player Name:  " + lPlyr_name);

            // Initialize Snake Movement value
            Position[] gDirections = new Position[]
            {
                new Position(0, 1), // Move right
                new Position(0, -1), // Move left
                new Position(1, 0), // Move down
                new Position(-1, 0), // Move up
            };

            // Changes speed of snake according to difficulty
            switch (gDifficulty)
            {
                case "1":
                    gSleepTime = 150;
                    break;
                case "2":
                    gSleepTime = 100;
                    break;
                case "3":
                    gSleepTime = 50;
                    break;
                default:
                    gSleepTime = 100;
                    break;
            }

            int gDirection = right; // Initialize default snake direction
            Random gRandomNumbersGenerator = new Random(); // Random number generator
            gLastFoodTime = Environment.TickCount; // Get time since program has started

            // Initialize obstacle locations
            List<Position> gObstacles = new List<Position>()
            {
                new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),gRandomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),gRandomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),gRandomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),gRandomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),gRandomNumbersGenerator.Next(0, Console.WindowWidth)),
            };

            // Initialize obstacle color
            foreach (Position obstacle in gObstacles)
            {
                Draw("Cyan", obstacle.col, obstacle.row, "=");
            }

            // Initialize length of snake
            Queue<Position> gSnakeElements = new Queue<Position>();
            for (int i = 0; i <= 3; i++)
            {
                gSnakeElements.Enqueue(new Position(1, i)); // Changed so that starts on second line
            }

            // Initialize food
            Position gFood;
            do
            {
                gFood = new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),
                    gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Initialize coordinate of food (random)
            }
            while (gSnakeElements.Contains(gFood) || gObstacles.Contains(gFood)); // To detect whether the food collides with the obstacles/snake body 
            Draw("Yellow", gFood.col, gFood.row, "@");

            // Initialize snake body
            foreach (Position position in gSnakeElements)
            {
                Draw("DarkGray", position.col, position.row, "*");
            }

            // Initialize soundplayer
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @"..\Sounds\bgm.wav";
            player.Play();

            // Main game loop
            while (true)
            {
                // Initialize negative points (deducted from total score)
                gNegativePoints++;

                // Initialize scoreboard
                int userPoints = (gSnakeElements.Count - 4) * 100 - gNegativePoints;
                if (userPoints < 0) userPoints = 0;
                userPoints = Math.Max(userPoints, 0);

                Draw("Green", 0, 0, "Score: " + userPoints);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    // Prevents snake from going backwards
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (gDirection != right) gDirection = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (gDirection != left) gDirection = right;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (gDirection != down) gDirection = up;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (gDirection != up) gDirection = down;
                    }
                }

                Position gSnakeHead = gSnakeElements.Last(); // Returns last element in the queue, assigns the element as coordinate of snakeHead
                Position gNextDirection = gDirections[gDirection]; // The direction will be converted to integer as index, nextDirection will store the difference of the next coordinate

                Position gSnakeNewHead = new Position(gSnakeHead.row + gNextDirection.row,
                    gSnakeHead.col + gNextDirection.col); // Adds to the next position of the snake head

                // Makes sure that the snake doesn't go out of bounds
                if (gSnakeNewHead.col < 0) gSnakeNewHead.col = Console.WindowWidth - 1;
                if (gSnakeNewHead.row < 1) gSnakeNewHead.row = Console.WindowHeight - 1;
                if (gSnakeNewHead.row >= Console.WindowHeight) gSnakeNewHead.row = 1;
                if (gSnakeNewHead.col >= Console.WindowWidth) gSnakeNewHead.col = 0;

                // If the snake is size 15, the player wins and the game ends
                if (gSnakeElements.Count == 15)
                {
                    Draw("Green", 0, 0, "");
                    Console.SetCursorPosition(Console.WindowWidth / 2, 10); //Reposition the string
                    Console.WriteLine("You won!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 4, 11); //Reposition the string
                    Console.WriteLine("Your points are: {0}", userPoints);
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 8, 13); //Reposition the string
                    string lScore = "Score: " + userPoints.ToString();
                    System.IO.File.WriteAllText(@"..\Scores\score.txt", lScore);
                    Console.WriteLine("Press Enter to exit the game");
                    Console.ReadLine();
                    return;
                }

                // If the snake head collides with the snake body or the snake head hits an obstacle, the game loop ends
                if (gSnakeElements.Contains(gSnakeNewHead) || gObstacles.Contains(gSnakeNewHead))
                {
                    Draw("Red", 0, 0, "");
                    Console.SetCursorPosition(Console.WindowWidth / 2, 10); //Reposition the string
                    Console.WriteLine("Game over!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 4, 11); //Reposition the string
                    Console.WriteLine("Your points are: {0}", userPoints);
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 8, 13); //Reposition the string
                    string lScore = "Score: " + userPoints.ToString();

                    //Updating score text file
                    using (StreamWriter lFile = File.AppendText(@"..\Scores\score.txt"))
                    {
                        lFile.WriteLine("\nPlayer : " + lPlyr_name);
                        lFile.WriteLine(lScore);
                        lFile.WriteLine("--------------------");
                    }
                    Console.WriteLine("Press Enter to exit the game");
                    Console.ReadLine();
                    return;
                }

                Draw("DarkGray", gSnakeHead.col, gSnakeHead.row, "*");

                gSnakeElements.Enqueue(gSnakeNewHead);
                Console.SetCursorPosition(gSnakeNewHead.col, gSnakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (gDirection == right) Console.Write(">");
                else if (gDirection == left) Console.Write("<");
                else if (gDirection == up) Console.Write("^");
                else Console.Write("v");

                if (gSnakeNewHead.col == gFood.col && gSnakeNewHead.row == gFood.row)
                {
                    // feeding the snake
                    do
                    {
                        gFood = new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),
                            gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign two random values into food position
                    }
                    while (gSnakeElements.Contains(gFood) || gObstacles.Contains(gFood)); // To detect whether the snake/obstacle collides with food

                    gLastFoodTime = Environment.TickCount; //
                    Draw("Yellow", gFood.col, gFood.row, "@");
                    gSleepTime--; // Increase the velocity that the snake is travelling

                    Position obstacle = new Position(); // Initialize position of obstacle

                    do
                    {
                        obstacle = new Position(gRandomNumbersGenerator.Next(1, Console.WindowHeight),
                            gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign to random values into obstacle position
                    }
                    while (gSnakeElements.Contains(obstacle) ||
                        gObstacles.Contains(obstacle) ||
                        (gFood.row != obstacle.row && gFood.col != obstacle.row)); // Makes sure the obstacles do not spawn inside the food or other obstacles
                    gObstacles.Add(obstacle); // Adds a new obstacle to the queue
                    Draw("Cyan", obstacle.col, obstacle.row, "="); // Draws obstacle
                }
                else
                {
                    // moving...
                    Position last = gSnakeElements.Dequeue(); // Deletes the last element of the list
                    Console.SetCursorPosition(last.col, last.row); // Moves cursor to the position of deleted element
                    Console.Write(" "); // Replaces the space in the position with space
                }

                if (Environment.TickCount - gLastFoodTime >= gFoodDissapearTime) // Food destructor
                {
                    gNegativePoints = gNegativePoints + 50; // Deduct points for not eating food
                    Console.SetCursorPosition(gFood.col, gFood.row); // Moves cursor to the food about to be deleted
                    Console.Write(" "); // Deletes the food drawn at that position
                    do
                    {
                        gFood = new Position(gRandomNumbersGenerator.Next(0, Console.WindowHeight),
                            gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Assigns new position to the food
                    }
                    while (gSnakeElements.Contains(gFood) || gObstacles.Contains(gFood)); // Loops for a new position for food until a valid position is set and drawn
                    gLastFoodTime = Environment.TickCount; // 
                }

                // Draws food
                Draw("Yellow", gFood.col, gFood.row, "@");

                gSleepTime -= 0.01; // Increase the velocity of the snake each time the loop is run

                Thread.Sleep((int)gSleepTime);
            }
        }
    }
}
