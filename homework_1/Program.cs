using System;

namespace homework_1
{
    internal class Program
    {
        // methods

        static double Addition(double a, double b)
        {
            return a + b;
        }

        static double Subtraction(double a, double b)
        {
            return a - b;
        }

        static double Multiplication(double a, double b)
        {
            return a * b;
        }

        static double Division(double a, double b)
        {
            if (b != 0)
            {
                return a / b;
            }
            else
            {
                Console.WriteLine("You can't devide by zero");
                return -1;
            }
        }

        static double Exponentiation(double a, double b)
        {
            return Math.Pow(a, b);
        }


        static double Factorial(int number)
        {
            if (number == 0 || number == 1)
                return 1;
            else
                return number * Factorial(number - 1);
        }

        static void Main(string[] args)
        {
            double number1, number2;
            int operation;

            do
            { 

                Console.WriteLine("This is the Calculator App");

                Console.Write("Enter the first number:");
                number1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter the second number:");
                number2 = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Choose which math operation to perform:");  // choosing operation
                Console.WriteLine("1. Addition");
                Console.WriteLine("2. Subtraction");
                Console.WriteLine("3. Multiplication");
                Console.WriteLine("4. Division");
                Console.WriteLine("5. Exponentiation");
                Console.WriteLine("6. Factorial");
                Console.WriteLine("7. Exit program");

                Console.Write("Enter the operation number: ");
                operation = Convert.ToInt32(Console.ReadLine());

            
                switch (operation)
                {
                    case 1:
                        Console.WriteLine("Result= " + number1 + " + " + number2 + " = " + Addition(number1, number2));
                        break;
                    case 2:
                        Console.WriteLine("Result= " + number1 + " - " + number2 + " = " + Subtraction(number1, number2));
                        break;
                    case 3:
                        Console.WriteLine("Result= " + number1 + " * " + number2 + " = " + Multiplication(number1, number2));
                        break;
                    case 4:
                        if (number2 != 0)
                            Console.WriteLine("Result= " + number1 + " / " + number2 + " = " + Division(number1, number2));
                        else
                            Console.WriteLine("You can't divide by zero!");
                        break;
                    case 5:
                        Console.WriteLine("Result= " + number1 + " ^ " + number2 + " = " + Exponentiation(number1, number2));
                        break;
                    case 6:
                        Console.WriteLine("Result= " + number1 + "! = " + Factorial((int)number1));
                        break;
                    default:
                        Console.WriteLine("Invalid operation!");
                        break;
                }
            }

            while (operation <= 6);

        }
    }
}