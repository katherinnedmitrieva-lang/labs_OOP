using System;

internal class task2
{
    public static void Run()
    {
        Console.Write("Input N appointments: ");
        int n = int.Parse(Console.ReadLine());

        double[] costs = new double[n];
        for (int i = 0; i < n; i++)
        {
            Console.Write("Input cost " + (i + 1) + ": ");
            costs[i] = double.Parse(Console.ReadLine());
        }

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (costs[j] > costs[j + 1])
                {
                    double temp = costs[j];
                    costs[j] = costs[j + 1];
                    costs[j + 1] = temp;
                }
            }
        }

        Console.WriteLine("\nSorted costs:");
        for (int i = 0; i < n; i++)
        {
            Console.Write(costs[i] + " ");
        }
        Console.WriteLine();
        Console.ReadLine();
    }
}

