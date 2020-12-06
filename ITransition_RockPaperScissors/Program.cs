using System;
using System.Security.Cryptography;
using System.Text;

namespace ITransition_RockPaperScissors
{
    class Program
    {
        private static byte[] currentKey = new byte[16];
        private static byte[] currentHash;
        private static RandomNumberGenerator generator = RandomNumberGenerator.Create();
        private static HMACSHA256 hmac;
        private static String[] choices;
        static void Main(string[] args)
        {
            choices = args;
            CheckArguments();
            int chosen = GetCPUChoice();
            currentHash = GetHashFromString(choices[chosen]);
            int userChosen = GetUserChoice();
            WinnerCodes winner = DetermineWinner(chosen,userChosen);
            Console.WriteLine($"Computer choice: {choices[chosen]}\r\nYour choice: {choices[userChosen]}");
            switch (winner)
            {
                case WinnerCodes.CPU:
                    Console.WriteLine("Computer won!");
                    break;
                case WinnerCodes.USER:
                    Console.WriteLine("You won!");
                    break;
                case WinnerCodes.DRAW:
                    Console.WriteLine("Its a draw!");
                    break;
            }
            Console.WriteLine($"KEY: {BitConverter.ToString(currentKey).Replace("-", String.Empty)}");
        }
        private static void CheckArguments()
        {
            if ((choices.Length % 2) == 0 || choices.Length <= 1)
            {
                Console.WriteLine("Необходимо передать нечетное количество параметров! Для игры необходимо больше 1 параметра!");
                Environment.Exit(0);
            }
            for (int i=0;i<choices.Length-1;i++)
            {
                if (choices[i] == choices[i + 1])
                {
                    Console.WriteLine($"Параметры не должны быть одинаковыми!\r\n" +
                        $"{choices[i]} == {choices[i+1]}");
                    Environment.Exit(0);
                }
            }
        }

        private static int GetCPUChoice()
        {
            Random random = new Random();
            int number = random.Next(0, choices.Length);
            return number;
        }
        private static byte[] GetHashFromString(String value)
        {
            generator.GetBytes(currentKey);
            hmac = new HMACSHA256(currentKey);
            byte[] choiceEncoded = Encoding.UTF8.GetBytes(value);
            return hmac.ComputeHash(choiceEncoded);
        }
        private static void PrintWelcome()
        {
            Console.Clear();
            Console.WriteLine($"HMAC: {BitConverter.ToString(currentHash).Replace("-", String.Empty)}");
            Console.WriteLine("Choices:");
            for (int i = 0; i < choices.Length; i++)
            {
                Console.WriteLine($"[{i + 1}]{choices[i]}");
            }
            Console.WriteLine("[0]Exit");
        }
        private static int GetUserChoice()
        {
            int chosen;
            PrintWelcome();
            while (true)
            {
                if(int.TryParse(Console.ReadLine(),out chosen))
                {
                    if (chosen > 0 && chosen <= choices.Length)
                    {
                        return chosen - 1;
                    }
                    else if (chosen == 0) Environment.Exit(0);
                }
                PrintWelcome();
                Console.Write("Неверно введен пункт меню! Повторите ввод: ");
            }
        }
        private static WinnerCodes DetermineWinner(int CpuChoice,int UserChoice)
        {
            if (CpuChoice == UserChoice) return WinnerCodes.DRAW;
            int current = CpuChoice;
            int distance = 0;
            while (true)
            {
                if (current == UserChoice) break;
                current = Previous(current);
                distance++;
            }
            if ((distance % 2) == 0) return WinnerCodes.USER; else return WinnerCodes.CPU;
        }
        private static int Previous(int current)
        {
            if ((current - 1) < 0) return choices.Length - 1;
            return current - 1;
        }
    }
}
