using System;

internal class task3
{
    public static void Run()
    {
        string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        int[] patients = new int[7];
        int total = 0;

        for (int i = 0; i < 7; i++)
        {
            Console.Write("Patients for " + days[i] + ": ");
            patients[i] = int.Parse(Console.ReadLine());
            total += patients[i];
        }

        int maxIdx = 0;
        int minIdx = 0;

        for (int i = 1; i < 7; i++)
        {
            if (patients[i] > patients[maxIdx]) maxIdx = i;
            if (patients[i] < patients[minIdx]) minIdx = i;
        }

        Console.WriteLine("\n--- Weekly Schedule ---");
        for (int i = 0; i < 7; i++)
        {
            Console.WriteLine(days[i] + ": " + patients[i]);
        }

        Console.WriteLine("\nTotal patients: " + total);
        Console.WriteLine("Busiest day: " + days[maxIdx] + " (" + patients[maxIdx] + ")");
        Console.WriteLine("Quietest day: " + days[minIdx] + " (" + patients[minIdx] + ")");
        Console.ReadLine();
    }
}
