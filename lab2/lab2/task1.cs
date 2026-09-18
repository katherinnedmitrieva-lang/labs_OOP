using System;

internal class task1
{
    public static void Run()
    {
        Console.Write("Input N patients: ");
        int n = int.Parse(Console.ReadLine());

        float[] weights = new float[n];
        float sum = 0;

        for (int i = 0; i < n; i++)
        {
            Console.Write("Input weight of patient " + (i + 1) + ": ");
            weights[i] = float.Parse(Console.ReadLine());
            sum += weights[i];
        }

        float average = sum / n;
        float min = weights[0];
        float max = weights[0];

        for (int i = 1; i < n; i++)
        {
            if (weights[i] < min) min = weights[i];
            if (weights[i] > max) max = weights[i];
        }

        int countAboveAverage = 0;
        for (int i = 0; i < n; i++)
        {
            if (weights[i] > average)
            {
                countAboveAverage++;
            }
        }

        Console.WriteLine("Results:");
        Console.WriteLine("Average weight: " + average);
        Console.WriteLine("Min weight: " + min);
        Console.WriteLine("Max weight: " + max);
        Console.WriteLine("Patients above average: " + countAboveAverage);
        Console.ReadLine();
    }
}
