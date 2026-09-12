using System;
using System.Globalization; 

namespace MedicalApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Input (1-7): ");
            int day = int.Parse(Console.ReadLine());
            Console.Write("Schedule: ");
            switch (day)
            {
                case 1:
                    Console.WriteLine("Monday 08:00–18:00");
                    break;
                case 2:
                    Console.WriteLine("Tuesday 08:00–18:00");
                    break;
                case 3:
                    Console.WriteLine("Wednesday 09:00–17:00");
                    break;
                case 4:
                    Console.WriteLine("Thursday 08:00–18:00");
                    break;
                case 5:
                    Console.WriteLine("Friday 08:00–16:00");
                    break;
                case 6:
                    Console.WriteLine("Saturday 09:00–14:00");
                    break;
                case 7:
                    Console.WriteLine("Sunday off");
                    break;
                default:
                    Console.WriteLine("Invalid day number");
                    break;
            }
            Console.ReadLine();
        }


        static void Task4() {
            Console.Write("Enter systolic blood pressure (upper value, e.g., 120): ");
            int systolic = int.Parse(Console.ReadLine());
            Console.Write("Enter diastolic blood pressure (lower value, e.g., 80): ");
            int diastolic = int.Parse(Console.ReadLine());
            if (systolic < 120 && diastolic < 80)
            {
                Console.WriteLine("normal");
            }
            else if (systolic < 130 && diastolic < 80)
            {
                Console.WriteLine("elevated");
            }
            else if (systolic < 140 || diastolic < 90)
            {
                Console.WriteLine("hypertension 1 degree");
            }
            else
            {
                Console.WriteLine("hypertension 2 degree");
            }
            Console.ReadLine();
        }

        static void Task3()
        {
            Console.Write("Input birth year: ");
            int birthYear = int.Parse(Console.ReadLine());

            int age = 2026 - birthYear;

            if (age <= 17)
            {
                Console.WriteLine("child");
            }
            else if (age <= 59)
            {
                Console.WriteLine("adult");
            }
            else
            {
                Console.WriteLine("pensioner");
            }

            Console.ReadLine();
        }


            static void Task2() {
            Console.Write("input price: ");
            double price = double.Parse(Console.ReadLine());
            Console.Write("input quantity: ");
            int count = int.Parse(Console.ReadLine());
            Console.Write("input discount: ");
            double discount = double.Parse(Console.ReadLine());
            double sum = price * count * (1 - discount / 100);
            Console.WriteLine("Sum: " + sum);
        }


        static void Task1()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            Console.Write("input weight (kg): ");
            string weightInput = Console.ReadLine();
            weightInput = weightInput.Replace(',', '.');
            double weight = Convert.ToDouble(weightInput, culture);
            Console.Write("input height (e.g., 1.64): ");
            string heightInput = Console.ReadLine();
            heightInput = heightInput.Replace(',', '.');
            double height = Convert.ToDouble(heightInput, culture);
            double bmi = weight / (height * height);
            double finalBmi = Math.Round(bmi, 2);
            Console.WriteLine($"\nBMI: {finalBmi.ToString(culture)}");
        }
    }
}
