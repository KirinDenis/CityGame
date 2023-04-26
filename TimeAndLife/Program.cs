
using System.Globalization;

static DateTime DaysToDate(double days)
{
    // Дата 1 января 1900 года
    DateTime baseDate = new DateTime(1900, 1, 1, 0, 0, 0);

    // Добавляем указанное количество часов
    TimeSpan time = TimeSpan.FromDays(days);

    // Возвращаем дату и время в формате DateTime
    return baseDate + time;
}


int[] factrors = new int[10];

Random r = new Random();

for (int i = 0; i < 10; i++)
{
    factrors[i] = r.Next(-100, 100);
}

while (true)
{
    int i = r.Next(9);
    if (r.Next(1000) <= 500)
    {
        factrors[i]++;
    }
    else
    {
        factrors[i]--;
    }
    if (factrors[i] < -100)
    {
        factrors[i] = -100;
    }
    else
    if (factrors[i] > 100)
    {
        factrors[i] = 100;
    }

    double dayTrand = 0;
    for (i = 0; i < 10; i++)
    {
        dayTrand += factrors[i];

        Console.SetCursorPosition(i * 4, 2);
        Console.WriteLine(factrors[i]);
    }

    dayTrand = dayTrand / 10 / 100;
    Console.SetCursorPosition(0, 0);
    Console.WriteLine(dayTrand);


    Thread.Sleep(300);
}

/*
double days = 200;

int upCitezen = 255;
double citizenDinamicPerDay = 0.5;
int curentCit = 0;
double startingDay = 100;


int[] factrors = new int[10];

Random r = new Random();

for (int i=0; i < 10; i++)
{
    factrors[i] = r.Next(-100, 100);
}

while (true)
{
    days+=100;

    int i = r.Next(9);
    if (r.Next(1000) <= 500)
    {
        factrors[i]++;
    }
    else
    {
        factrors[i]--;
    }
    if (factrors[i] < -100)
    {
        factrors[i] = -100;
    }
    else
    if (factrors[i] > 100)
    {
        factrors[i] = 100;
    }

    double dayTrand = 0;
    for (i = 0; i < 10; i++)
    {
        dayTrand +=factrors[i] / 100.0f;

        Console.SetCursorPosition(i*4, 2);
        Console.WriteLine(factrors[i]);

    }
    Console.SetCursorPosition(0, 3);
    Console.WriteLine(dayTrand);

    curentCit += (int)dayTrand;

    CultureInfo culture = new CultureInfo("en-US");
    Console.SetCursorPosition(0, 0);    
    Console.WriteLine(DaysToDate(days).ToString("d MMM yyyy", culture));

    if (curentCit > upCitezen)
    {
        curentCit = upCitezen;
    }

    if (curentCit < 0)
    {
        curentCit = 0;
    }

    Console.SetCursorPosition(0, 1);
    Console.WriteLine(curentCit);

    //Thread.Sleep(200);
}
*/