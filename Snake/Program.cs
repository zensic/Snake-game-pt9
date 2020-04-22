using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace Snake
{
    //Stores the position of the food
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
        static void Main(string[] args)
        {
            // Assigning default value as index
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int gLastFoodTime = 0;
            int gFoodDissapearTime = 8000; // time for food to dissapear in milliseconds
            int gNegativePoints = 0; //

            // Snake Default value
            Position[] gDirections = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };

            double gSleepTime = 100; // Velocity of snake
            int gDirection = right; // Initialize default snake direction
            Random gRandomNumbersGenerator = new Random(); // Random number generator
            Console.BufferHeight = Console.WindowHeight; // Intialize the height of the game window
            gLastFoodTime = Environment.TickCount; // Get time since program has started

            // Initialize obstacle locations
            List<Position> gObstacles = new List<Position>()
            {
                new Position(12, 12),
                new Position(14, 20),
                new Position(7, 7),
                new Position(19, 19),
                new Position(6, 9),
            };

            // Initialize obstacle color
            foreach (Position obstacle in gObstacles)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                Console.Write("=");
            }

            // Initialize length of snake
            Queue<Position> gSnakeElements = new Queue<Position>();
            for (int i = 0; i <= 5; i++)
            {
                gSnakeElements.Enqueue(new Position(0, i));
            }

            // Initialize food
            Position gFood;
            do
            {
                gFood = new Position(gRandomNumbersGenerator.Next(0, Console.WindowHeight),
                    gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Initialize coordinate of food (random)
            }
            while (gSnakeElements.Contains(gFood) || gObstacles.Contains(gFood)); // To detect whether the food collides with the obstacles/snake body 
            Console.SetCursorPosition(gFood.col, gFood.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");

            // Initialize snake body
            foreach (Position position in gSnakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");
            }

            // Main game loop
            while (true)
            {
                gNegativePoints++; // This variable will be used to deduct from the total score

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
                    gSnakeHead.col + gNextDirection.col); //

                // Makes sure that the snake doesn't go out of bounds
                if (gSnakeNewHead.col < 0) gSnakeNewHead.col = Console.WindowWidth - 1;
                if (gSnakeNewHead.row < 0) gSnakeNewHead.row = Console.WindowHeight - 1;
                if (gSnakeNewHead.row >= Console.WindowHeight) gSnakeNewHead.row = 0;
                if (gSnakeNewHead.col >= Console.WindowWidth) gSnakeNewHead.col = 0;

                // If the snake head collides with the snake body or the snake head hits an obstacle, the game loop ends
                if (gSnakeElements.Contains(gSnakeNewHead) || gObstacles.Contains(gSnakeNewHead))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    int userPoints = (gSnakeElements.Count - 6) * 100 - gNegativePoints;
                    if (userPoints < 0) userPoints = 0;
                    userPoints = Math.Max(userPoints, 0);
                    Console.WriteLine("Your points are: {0}", userPoints);
                    Console.ReadLine();
                    return;
                }

                Console.SetCursorPosition(gSnakeHead.col, gSnakeHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");

                gSnakeElements.Enqueue(gSnakeNewHead);
                Console.SetCursorPosition(gSnakeNewHead.col, gSnakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (gDirection == right) Console.Write(">");
                if (gDirection == left) Console.Write("<");
                if (gDirection == up) Console.Write("^");
                if (gDirection == down) Console.Write("v");


                if (gSnakeNewHead.col == gFood.col && gSnakeNewHead.row == gFood.row)
                {
                    // feeding the snake
                    do
                    {
                        gFood = new Position(gRandomNumbersGenerator.Next(0, Console.WindowHeight),
                            gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign two random values into food position
                    }
                    while (gSnakeElements.Contains(gFood) || gObstacles.Contains(gFood)); // To detect whether the snake/obstacle collides with food

                    gLastFoodTime = Environment.TickCount; // 
                    Console.SetCursorPosition(gFood.col, gFood.row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("@");
                    gSleepTime--; // Increase the velocity that the snake is travelling

                    Position obstacle = new Position(); // Initialize position of obstacle

                    do
                    {
                        obstacle = new Position(gRandomNumbersGenerator.Next(0, Console.WindowHeight),
                            gRandomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign to random values into obstacle position
                    }
                    while (gSnakeElements.Contains(obstacle) ||
                        gObstacles.Contains(obstacle) ||
                        (gFood.row != obstacle.row && gFood.col != obstacle.row)); // Makes sure the obstacles do not spawn inside the food or other obstacles
                    gObstacles.Add(obstacle); // Adds a new obstacle to the queue
                    Console.SetCursorPosition(obstacle.col, obstacle.row); // Moves cursor to the obstacle about to drawn
                    Console.ForegroundColor = ConsoleColor.Cyan; // Join
                    Console.Write("="); // 
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

                Console.SetCursorPosition(gFood.col, gFood.row); // Moves cursor to the food about to be deleted
                Console.ForegroundColor = ConsoleColor.Yellow; // Set the food color to yellow
                Console.Write("@"); // Draw the @ on the food position

                gSleepTime -= 0.01; // Increase the velocity of the snake each time the loop is run

                Thread.Sleep((int)gSleepTime); // 
            }
        }
    }
}
