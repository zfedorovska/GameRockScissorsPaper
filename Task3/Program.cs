using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Task3
{
    class Program
    {
            static void Main(string[] args)
            {
                var errorsCount = CheckArgs(args);
                if (errorsCount > 0)
                {
                    PrintMessageExample();
                    Environment.Exit(0);
                }
                while (true)
                {
                    Play(args);
                }
            }

            static void Play(string[] args)
            {
                int numOfOptions = args.Length;
                var key = new byte[16];
                RandomNumberGenerator.Create().GetNonZeroBytes(key);
                var compChoice = NextRnd(BitConverter.ToInt32(key), 1, numOfOptions);
                string compMessage = "Computer move: " + args[compChoice - 1];
                var HMAC = HashHMAC(compMessage, key);
                Console.WriteLine("HMAC: " + GetHexStringFromByteArray(HMAC));
                DisplayUserMenu(args);
                Console.Write("Enter your move: ");
                var userChoice = Console.ReadLine();
                var userChoiceErrors = CheckUserChoice(userChoice, args);
                while (userChoiceErrors > 0)
                {
                Console.WriteLine("Your input is incorrect. Please enter a number between 1 and " + args.Length);
                    DisplayUserMenu(args);
                    Console.Write("Enter your move: ");
                    userChoice = Console.ReadLine();
                    userChoiceErrors = CheckUserChoice(userChoice, args);
                }
                var userChoiceInt = Int32.Parse(userChoice);
                if (userChoiceInt == 0)
                    Environment.Exit(0);
                Console.WriteLine("Your move: " + args[userChoiceInt - 1]);
                Console.WriteLine(compMessage);
                string result = GetResult(numOfOptions, userChoiceInt, compChoice);
                Console.WriteLine(result);
                Console.WriteLine("HMAC key: " + GetHexStringFromByteArray(key));
            }

            static int NextRnd(int val, int min, int max)
            {
                max = max - 1;
                var result = ((val - min) % (max - min + 1) + (max - min + 1)) % (max - min + 1) + min;
                return result;
            }

            static byte[] HashHMAC(string msg, byte[] key)
            {
                byte[] toEncryptArray = Encoding.GetEncoding(28591).GetBytes(msg);
                HMACSHA256 hash = new HMACSHA256(key);
                return hash.ComputeHash(toEncryptArray);
            }
            
            static string GetHexStringFromByteArray(byte[] bytes)
            {
                string hex = BitConverter.ToString(bytes);
                hex = hex.Replace("-", "");
                return hex;
            }

            static string GetResult(int numOfOptions, int userChoice, int compChoice)
            {
                int numOfWinningCases = numOfOptions / 2;
                if (userChoice == compChoice)
                    return "Draw";
                if (userChoice > compChoice)
                {
                    if ((userChoice - compChoice) <= numOfWinningCases)
                        return "You win!";
                }
                else if ((userChoice + numOfOptions) - compChoice <= numOfWinningCases)
                {
                    return "You win!";
                }
                return "Comp wins";
            }

            static int CheckArgs(string[] n)
            {
                int errorsCount = 0;
                if (!(n.Length>=3))
                {
                    Console.WriteLine("moves number should be >=3");
                    errorsCount++;
                }
                if (n.Length % 2 == 0)
                {
                    Console.WriteLine("moves number should be odd number");
                    errorsCount++;
                }
                var dublicatesExist = CheckForDublicates_HashSet(n);
                if (dublicatesExist)
                    errorsCount++;
                return errorsCount;
            }

            static bool CheckForDublicates_HashSet(string[] n)
            {
                var set = new HashSet<string>();
                foreach (var item in n)
                    if (!set.Add(item))
                    {
                        Console.WriteLine("In your moves list you have duplicates: " + item);
                        return true;
                    }
                return false;
            }

            static void PrintMessageExample()
            {
                Console.WriteLine("Please enter an odd number >= 3 non-repeating moves for game");
                Console.WriteLine("For example: rock paper scissors lizard Spock");
            }

            static void DisplayUserMenu(string[] n)
            {
                for (int i = 0; i < n.Length; i++)
                    Console.WriteLine(i + 1 + " - " + n[i]);
                Console.WriteLine("0 - Exit");
            }

            static int CheckUserChoice(string userChoice, string[] n)
            {
                var errors = 0;
                int c;
                bool isNumeric = int.TryParse(userChoice, out c);
                if (!(isNumeric))
                    errors++;
                else
                {
                    if ((c < 0) || (c > n.Length))
                        errors++;
                }
                return errors;
            }
    }
}
