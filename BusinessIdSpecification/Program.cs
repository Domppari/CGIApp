using System;

namespace BusinessIdSpecification
{
    class Program
    {
        static void Main(string[] args)
        {
            var businessIdSpecification = new BusinessIdSpecification();
            var businessIdsToCheckFor = new string[] {
                "1234567-9",  // Fail, checksum
                "0357502-9",  // Pass, CGI Suomi
                "357502-9",   // Fail, CGI Suomi, old style - not sure if should make a special case for this
                "1234567a",   // Fail, format
                null,         // Fail, null
                string.Empty, // Fail, Empty string
                };

            foreach (var businessId in businessIdsToCheckFor)
            {
                Console.Write("Check for " + businessId + " ");
                if (!businessIdSpecification.IsSatisfiedBy(businessId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL");
                    int i = 1;
                    foreach (var reason in businessIdSpecification.ReasonsForDissatisfaction)
                        Console.WriteLine("  " + i++ + ": " + reason);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
