using System;

internal class task6
{
    public static void Run()
    {
        Console.Write("Input doctors count: ");
        int doctorsCount = int.Parse(Console.ReadLine());

        float[][] jagged = new float[doctorsCount][];

        for (int i = 0; i < doctorsCount; i++)
        {
            Console.Write("Appointments for doctor " + (i + 1) + ": ");
            int appointments = int.Parse(Console.ReadLine());

            jagged[i] = new float[appointments];
            for (int j = 0; j < appointments; j++)
            {
                Console.Write("  Income for appointment " + (j + 1) + ": ");
                jagged[i][j] = float.Parse(Console.ReadLine());
            }
        }

        int bestDoctor = 0;
        float maxIncome = -1;

        Console.WriteLine("Doctor Income Stats:");
        for (int i = 0; i < doctorsCount; i++)
        {
            float doctorSum = 0;
            for (int j = 0; j < jagged[i].Length; j++)
            {
                doctorSum += jagged[i][j];
            }
            Console.WriteLine("Doctor " + (i + 1) + ": appointments = " + jagged[i].Length + ", sum = " + doctorSum);

            if (doctorSum > maxIncome)
            {
                maxIncome = doctorSum;
                bestDoctor = i;
            }
        }

        Console.WriteLine("Top doctor: Doctor " + (bestDoctor + 1) + " (Income: " + maxIncome + ")");
        Console.ReadLine();
    }
}
