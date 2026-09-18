using System;

internal class task7
{
    public static void Run()
    {
        Console.Write("Input patients count N: ");
        int n = int.Parse(Console.ReadLine());

        string[] names = new string[n];
        float[] bmi = new float[n];

        for (int i = 0; i < n; i++)
        {
            Console.Write("Patient " + (i + 1) + " name: ");
            names[i] = Console.ReadLine();
            Console.Write("Patient " + (i + 1) + " BMI: ");
            bmi[i] = float.Parse(Console.ReadLine());
        }

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (bmi[j] < bmi[j + 1])
                {
                    float tempBmi = bmi[j];
                    bmi[j] = bmi[j + 1];
                    bmi[j + 1] = tempBmi;

                    string tempName = names[j];
                    names[j] = names[j + 1];
                    names[j + 1] = tempName;
                }
            }
        }

        Console.WriteLine("Patients BMI Rating:");
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine((i + 1) + ". Name: " + names[i] + " | BMI: " + bmi[i]);
        }
        Console.ReadLine();
    }
}
