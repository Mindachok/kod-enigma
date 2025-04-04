﻿namespace KOD_Enigma;

class Person {
    public string Name {get; set;}
    public int Age {get; set;}

    public Person(string name, int age) {
        Name = name;
        Age = age;
    }

    public void PrintInfo() {
        Console.WriteLine($"Имя: {Name}, Возраст: {Age}");
    }

}

class Program
{
    public static void SayHello(string name) {
        Console.WriteLine($"Привет, {name}!");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("1) Привет, мир!\n");

        Console.WriteLine("2) Введите число:");
        int a = int.Parse(Console.ReadLine());
        if (a % 2 == 0) {
            Console.WriteLine("Введённое число чётное\n");
        }
        else {
            Console.WriteLine("Введённое число нечётное\n");
        }

        Console.WriteLine("3) Таблица умножения на 5:");
        for (int b = 1; b < 11; b++) {
            Console.WriteLine($"• 5*{b} = {5*b}");
        }

        string[] massiv = {"Петя", "Вася", "Николай"};
        Console.WriteLine("\n4)Приветики:");
        foreach (string names in massiv) {
            SayHello(names);
        }

        Console.WriteLine("5) Люди:");
        
        Person person = new Person("Андрюша", 62);
        person.PrintInfo();




    }
}