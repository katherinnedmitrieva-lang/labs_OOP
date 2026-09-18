using System;

internal class task8
{
    public static void Run()
    {
        Console.Write("Input departments count: ");
        int departments = int.Parse(Console.ReadLine());
        Console.Write("Input weeks count: ");
        int weeks = int.Parse(Console.ReadLine());

        int[,,] arr = new int[departments, weeks, 2];

        for (int d = 0; d < departments; d++)
        {
            for (int w = 0; w < weeks; w++)
            {
                Console.Write("Dep " + (d + 1) + ", Week " + (w + 1) + ", morning (0): ");
                arr[d, w, 0] = int.Parse(Console.ReadLine());

                Console.Write("Dep " + (d + 1) + ", Week " + (w + 1) + ", evening (1): ");
                arr[d, w, 1] = int.Parse(Console.ReadLine());
            }
        }

        int maxDepIndex = 0;
        int maxDepLoad = -1;

        Console.WriteLine("Department Stats:");
        for (int d = 0; d < departments; d++)
        {
            int depTotal = 0;
            for (int w = 0; w < weeks; w++)
            {
                depTotal += arr[d, w, 0] + arr[d, w, 1];
            }
            Console.WriteLine("Department " + (d + 1) + ": total loads = " + depTotal);

            if (depTotal > maxDepLoad)
            {
                maxDepLoad = depTotal;
                maxDepIndex = d;
            }
        }

        Console.WriteLine("Busiest department: " + (maxDepIndex + 1) + " (Loads: " + maxDepLoad + ")");
        Console.ReadLine();
    }
}
