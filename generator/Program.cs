using System.Text;
using Serilog;
using Bogus;

public class SoapMessage
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int Number { get; set; }
    public string Text { get; set; }
}

public class SoapMessageGenerator
{
    // Инициализация логирования
    static SoapMessageGenerator()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()  // Логирование в консоль
            .CreateLogger();
    }

    // Генерация SOAP-сообщения с случайными данными
    public static string GenerateSoapMessage()
    {
        var faker = new Faker<SoapMessage>()
            .RuleFor(r => r.Name, f => f.Person.FullName)
            .RuleFor(r => r.Date, f => f.Date.Recent())  // Случайная дата
            .RuleFor(r => r.Number, f => f.Random.Number(1000, 9999))  // Случайное число
            .RuleFor(r => r.Text, f => f.Lorem.Sentence());  // Случайная строка текста

        var element = faker.Generate();

        // Генерация SOAP-сообщения с использованием случайных данных
        string payload = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                            <soapenv:Body>
                            <card112ChangedRequest xmlns=""http://www.protei.ru/emergency/integration"">
                            <globalId>{Guid.NewGuid()}</globalId>
                            <nEmergencyCardId>{element.Number}</nEmergencyCardId>
                            <dtCreate>{element.Date.ToString("yyyy-MM-ddTHH:mm:ss.fff+03:00")}</dtCreate>
                            <nCallTypeId>{element.Number}</nCallTypeId>
                            <nCardSyntheticState>{element.Number}</nCardSyntheticState>
                            <nCard01SyntheticState>{element.Number}</nCard01SyntheticState>
                            <nCard02SyntheticState>{element.Number}</nCard02SyntheticState>
                            <nCardCommServSyntheticState>{element.Number}</nCardCommServSyntheticState>
                            <nCardATSyntheticState>{element.Number}</nCardATSyntheticState>
                            <lWithCall>{element.Number}</lWithCall>
                            <strCreator>{element.Name}</strCreator>
                            <nIncidentTypeID>{element.Number}</nIncidentTypeID>
                            <strIncidentType>{element.Text}</strIncidentType>
                            <lNear>{element.Number}</lNear>
                            <lHumanThreat>{element.Number}</lHumanThreat>
                            </card112ChangedRequest>
                            </soapenv:Body>
                            </soapenv:Envelope>";

        // Логирование сгенерированного SOAP-сообщения
        Log.Information("Generated SOAP Request: {Request}", payload);

        return payload;
    }

    // Асинхронная отправка SOAP-сообщения
    public static async Task SendSoapMessageAsync()
    {
        string message = GenerateSoapMessage();

        var content = new StringContent(message, Encoding.UTF8, "text/xml");

        using (var client = new HttpClient())
        {
            Log.Information("Sending SOAP message to the endpoint");

            try
            {
                var response = await client.PostAsync("http://127.0.0.1:6295/api/Message", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Логирование ответа от сервера
                Log.Information("SOAP Response: {ResponseContent}", responseContent);
            }
            catch (Exception ex)
            {
                // Логирование ошибок
                Log.Error(ex, "Error while sending SOAP message");
            }
        }
    }

    // Метод для запуска отправки сообщений каждые 5 секунд
    public static void StartSendingMessages(CancellationToken cancellationToken)
    {
        Timer timer = new Timer(async _ =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Log.Information("Timer triggered: Sending SOAP message.");
                await SendSoapMessageAsync();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));  // Начать немедленно, отправлять каждые 5 секунд

        cancellationToken.Register(() =>
        {
            Log.Information("Cancellation requested, stopping the timer.");
            timer.Dispose();
        });
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.Title = "generator";
        // Создаем токен для отмены
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        // Запуск отправки сообщений каждые 5 секунд
        SoapMessageGenerator.StartSendingMessages(cancellationToken);

        // Даем программе работать 60 секунд (для примера)
        await Task.Delay(TimeSpan.FromSeconds(60));

        // Отменить таймер (если необходимо завершить выполнение)
        cancellationTokenSource.Cancel();
    }
}
