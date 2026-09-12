using System;
using System.Globalization; 

namespace MedicalApp
{
    class Program
    {
        static void Main(string[] args)
        {
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
