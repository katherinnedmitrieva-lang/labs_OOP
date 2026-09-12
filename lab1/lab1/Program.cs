using System;
using System.Globalization; 

namespace MedicalApp
{
    class Program
    {
        static void Run()
        {
            Console.WriteLine("Task 1: BMI Calculation");
            Console.Write("Input weight (kg): ");
            double weight = double.Parse(Console.ReadLine());

            Console.Write("Input height (m): ");
            double height = double.Parse(Console.ReadLine());

            double bmi = CalculateBMI(weight, height);
            string bmiCategory = GetBMICategory(bmi);
            Console.WriteLine("BMI Result: " + bmiCategory);


            Console.WriteLine("\nTask 2: Cost of Visits");
            Console.Write("Input price for one visit: ");
            double price = double.Parse(Console.ReadLine());

            Console.Write("Input number of visits: ");
            int count = int.Parse(Console.ReadLine());

            Console.Write("Input discount (0-100): ");
            int discount = int.Parse(Console.ReadLine());

            double cost = CalculateCost(price, count, discount);
            Console.WriteLine("Cost Result: " + cost);


            Console.WriteLine("\nTask 3: patient age ");
            Console.Write("Input patient's birth year: ");
            int birthYear = int.Parse(Console.ReadLine());

            int age = 2026 - birthYear;
            string ageCategory = GetAgeCategory(age);
            Console.WriteLine("Age Category: " + ageCategory);


            Console.WriteLine("\nTask 4: blood pressure ");
            Console.Write("Input systolic pressure (upper): ");
            int systolic = int.Parse(Console.ReadLine());

            Console.Write("Input diastolic pressure (lower): ");
            int diastolic = int.Parse(Console.ReadLine());

            string pressureStatus = GetPressureStatus(systolic, diastolic);
            Console.WriteLine("Pressure Status: " + pressureStatus);
        }


        static double CalculateBMI(double weight, double height)
        {
            return weight / (height * height);
        }

        static string GetBMICategory(double bmi)
        {
            if (bmi < 18.5) return "low";
            if (bmi < 25) return "normal";
            if (bmi < 30) return "overweight";
            return "obesity";
        }

        static double CalculateCost(double price, int count, int discount)
        {
            return price * count * (1 - (double)discount / 100);
        }

        static string GetAgeCategory(int age)
        {
            if (age <= 17) return "child";
            if (age <= 59) return "adult";
            return "senior";
        }

        static string GetPressureStatus(int systolic, int diastolic)
        {
            if (systolic < 120 && diastolic < 80) return "normal";
            if (systolic < 130 && diastolic < 80) return "elevated";
            if (systolic < 140 || diastolic < 90) return "hypertension stage 1";
            return "hypertension stage 2";
        }

        static void Main(string[] args)
        {
            Run();
            Console.ReadLine();
        }
        static void Task7()
        {
            Console.Write("Input number of visits (e.g., 3): ");
            int n = int.Parse(Console.ReadLine());

            double[] prices = new double[n];
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                Console.Write("Input price for visit #" + (i + 1) + ": ");
                prices[i] = double.Parse(Console.ReadLine());
                sum += prices[i];
            }

            double min = prices[0];
            double max = prices[0];

            for (int i = 1; i < n; i++)
            {
                if (prices[i] < min) min = prices[i];
                if (prices[i] > max) max = prices[i];
            }

            double average = sum / n;

            Console.WriteLine("Total sum of all visits: " + sum);
            Console.WriteLine("Average cost of one visit: " + average);
            Console.WriteLine("Minimum cost: " + min);
            Console.WriteLine("Maximum cost: " + max);

            Console.ReadLine();
        }

        static void Task6() {
            Console.Write("Input medical card number: ");
            int number = int.Parse(Console.ReadLine());

            Console.WriteLine("\nAnalysis Results");

            int lastDigit = number % 10;

            if (lastDigit == 0 || lastDigit == 1)
                Console.WriteLine("Department: General Therapy");
            else if (lastDigit == 2 || lastDigit == 3)
                Console.WriteLine("Department: Surgery");
            else if (lastDigit == 4 || lastDigit == 5)
                Console.WriteLine("Department: Cardiology");
            else if (lastDigit == 6 || lastDigit == 7)
                Console.WriteLine("Department: Neurology");
            else if (lastDigit == 8 || lastDigit == 9)
                Console.WriteLine("Department: Ophthalmology");

            if (number % 2 == 0)
                Console.WriteLine("Discount card: yes");

            if (number % 3 == 0)
                Console.WriteLine("Routine examination: yes");

            Console.ReadLine();
        }

        static void Task5()
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
