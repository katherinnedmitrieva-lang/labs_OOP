using System;

internal class task4
{
    public static void Run()
    {
        Console.Write("Input doctors (N): ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("Input days (M): ");
        int m = int.Parse(Console.ReadLine());

        int[,] matrix = new int[n, m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write("Doctor " + (i + 1) + ", Day " + (j + 1) + ": ");
                matrix[i, j] = int.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("Doctor totals (rows):");
        for (int i = 0; i < n; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < m; j++)
            {
                rowSum += matrix[i, j];
            }
            Console.WriteLine("Doctor " + (i + 1) + ": " + rowSum);
        }

        Console.WriteLine("Day totals (columns):");
        for (int j = 0; j < m; j++)
        {
            int colSum = 0;
            for (int i = 0; i < n; i++)
            {
                colSum += matrix[i, j];
            }
            Console.WriteLine("Day " + (j + 1) + ": " + colSum);
        }

        int maxVal = matrix[0, 0];
        int maxRow = 0, maxCol = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (matrix[i, j] > maxVal)
                {
                    maxVal = matrix[i, j];
                    maxRow = i;
                    maxCol = j;
                }
            }
        }

        Console.WriteLine("Max value: " + maxVal + " (Doctor " + (maxRow + 1) + ", Day " + (maxCol + 1) + ")");
        Console.ReadLine();
    }
}
