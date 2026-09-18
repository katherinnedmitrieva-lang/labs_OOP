using System;

internal class task5
{
    public static void Run()
    {
        Console.Write("Input matrix size N: ");
        int n = int.Parse(Console.ReadLine());

        int[,] matrix = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write("Element [" + i + "][" + j + "]: ");
                matrix[i, j] = int.Parse(Console.ReadLine());
            }
        }

        int mainSum = 0;
        int sideSum = 0;

        Console.Write("Main diagonal: ");
        for (int i = 0; i < n; i++)
        {
            Console.Write(matrix[i, i] + " ");
            mainSum += matrix[i, i];
        }
        Console.WriteLine("Main diagonal sum: " + mainSum);

        Console.Write("Side diagonal: ");
        for (int i = 0; i < n; i++)
        {
            Console.Write(matrix[i, n - 1 - i] + " ");
            sideSum += matrix[i, n - 1 - i];
        }
        Console.WriteLine("Side diagonal sum: " + sideSum);
        Console.ReadLine();
    }
}
