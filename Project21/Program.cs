using System.Text.Json;

Dictionary<string, double> rates;

try
{
    using var client = new HttpClient();
    string json = await client.GetStringAsync("https://www.cbr-xml-daily.ru/daily_json.js");

    using var doc = JsonDocument.Parse(json);
    var valute = doc.RootElement.GetProperty("Valute");

    double usd = valute.GetProperty("USD").GetProperty("Value").GetDouble();

    double eur = valute.GetProperty("EUR").GetProperty("Value").GetDouble();

    rates = new Dictionary<string, double>
    {
        ["RUB"] = 1,
        ["USD"] = usd,
        ["EUR"] = eur,

    };
}
catch (HttpRequestException)
{
    Console.WriteLine("Не удалось получить курсы: проверьте своё подключение к интернету.");
    return;
}
catch (Exception ex) when (ex is JsonException or KeyNotFoundException)
{

    Console.WriteLine("Сервер прислал данные в неожиданном виде.");
    return;
}



while (true)
{
    Console.Write("Сумма: ");
    string input = Console.ReadLine();

    if (!double.TryParse(input, out double amount))
    {
        Console.WriteLine("Нужно ввести число");
        continue;
    }

    Console.Write("Из валюты (USD, EUR, RUB): ");
    string from = Console.ReadLine().ToUpper();

    Console.Write("В валюту (USD, EUR, RUB): ");
    string to = Console.ReadLine().ToUpper();

    Console.WriteLine($"Ты хочешь перевести {amount} {from} в {to}");



    if (!rates.ContainsKey(from) || !rates.ContainsKey(to))
    {
        Console.WriteLine("Такой валюты не существует");
    }

    else
    {
        Console.WriteLine($"Итог перевода из {from} в {to}: " + amount * rates[from] / rates[to]);
    }

    Console.Write("Если хотите выйти из программы напишите 'exit': ");
    string exit = Console.ReadLine().ToLower();

    if (exit == "exit")
    {
        break;
    }
}