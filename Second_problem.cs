namespace KOD_Enigma;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Привет, мир!");
        Console.WriteLine("Введите число");
        int a = int.Parse(Console.ReadLine());
        if (a % 2 == 0) {
            Console.WriteLine("Введённое число чётное");
        }
        else {
            Console.WriteLine("Введённое число нечётное");
        }

    }
}
