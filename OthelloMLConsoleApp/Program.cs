using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Othello;


namespace OthelloMLConsoleApp
{
    class Program
    {
        static void Main()
        {
            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Hello, World!");

            FastTreeWithOptions.Example();

            Console.WriteLine("Press any key to exit the program.");
            Console.ReadLine();
        }
    }
}