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
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
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
            int lastFoodTime = 0;
            int foodDissapearTime = 8000; // time for food to dissapear in milliseconds
            int negativePoints = 0; //

            // Snake Default value
            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };

            double sleepTime = 100; // Velocity of snake
            int direction = right; // Initialize default snake direction
            Random randomNumbersGenerator = new Random(); // Random number generator
            Console.BufferHeight = Console.WindowHeight; // Intialize the height of the game window
            lastFoodTime = Environment.TickCount; // Get time since program has started

            // Initialize obstacle locations
            List<Position> obstacles = new List<Position>()
            {
                new Position(12, 12),
                new Position(14, 20),
                new Position(7, 7),
                new Position(19, 19),
                new Position(6, 9),
            };

            // Initialize obstacle color
            foreach (Position obstacle in obstacles)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                Console.Write("=");
            }

            // Initialize length of snake
            Queue<Position> snakeElements = new Queue<Position>();
            for (int i = 0; i <= 5; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            // Initialize food
            Position food;
            do
            {
                food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth)); // Initialize coordinate of food (random)
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food)); // To detect whether the food collides with the obstacles/snake body 
            Console.SetCursorPosition(food.col, food.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");

            // Initialize snake body
            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");
            }

            // Main game loop
            while (true)
            {
                negativePoints++; // This variable will be used to deduct from the total score

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    // Prevents snake from going backwards
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right) direction = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left) direction = right;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down) direction = up;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up) direction = down;
                    }
                }

                Position snakeHead = snakeElements.Last(); // Returns last element in the queue, assigns the element as coordinate of snakeHead
                Position nextDirection = directions[direction]; // The direction will be converted to integer as index, nextDirection will store the difference of the next coordinate

                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                    snakeHead.col + nextDirection.col); //

                // Makes sure that the snake doesn't go out of bounds
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                // If the snake head collides with the snake body or the snake head hits an obstacle, the game loop ends
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    int userPoints = (snakeElements.Count - 6) * 100 - negativePoints;
                    if (userPoints < 0) userPoints = 0;
                    userPoints = Math.Max(userPoints, 0);
                    Console.WriteLine("Your points are: {0}", userPoints);
                    Console.ReadLine();
                    return;
                }

                Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");

                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (direction == right) Console.Write(">");
                if (direction == left) Console.Write("<");
                if (direction == up) Console.Write("^");
                if (direction == down) Console.Write("v");


                if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                {
                    // feeding the snake
                    do
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign two random values into food position
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food)); // To detect whether the snake/obstacle collides with food

                    lastFoodTime = Environment.TickCount; // 
                    Console.SetCursorPosition(food.col, food.row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("@");
                    sleepTime--; // Increase the velocity that the snake is travelling

                    Position obstacle = new Position(); // Initialize position of obstacle

                    do
                    {
                        obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)); // Assign to random values into obstacle position
                    }
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row)); // Makes sure the obstacles do not spawn inside the food or other obstacles
                    obstacles.Add(obstacle); // Adds a new obstacle to the queue
                    Console.SetCursorPosition(obstacle.col, obstacle.row); // Moves cursor to the obstacle about to drawn
                    Console.ForegroundColor = ConsoleColor.Cyan; // Join
                    Console.Write("="); // 
                }
                else
                {
                    // moving...
                    Position last = snakeElements.Dequeue(); // Deletes the last element of the list
                    Console.SetCursorPosition(last.col, last.row); // Moves cursor to the position of deleted element
                    Console.Write(" "); // Replaces the space in the position with space
                }

                if (Environment.TickCount - lastFoodTime >= foodDissapearTime) // Food destructor
                {
                    negativePoints = negativePoints + 50; // Deduct points for not eating food
                    Console.SetCursorPosition(food.col, food.row); // Moves cursor to the food about to be deleted
                    Console.Write(" "); // Deletes the food drawn at that position
                    do
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)); // Assigns new position to the food
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food)); // Loops for a new position for food until a valid position is set and drawn
                    lastFoodTime = Environment.TickCount; // 
                }

                Console.SetCursorPosition(food.col, food.row); // Moves cursor to the food about to be deleted
                Console.ForegroundColor = ConsoleColor.Yellow; // Set the food color to yellow
                Console.Write("@"); // Draw the @ on the food position

                sleepTime -= 0.01; // Increase the velocity of the snake each time the loop is run

                Thread.Sleep((int)sleepTime); // 
            }
        }
    }
}
