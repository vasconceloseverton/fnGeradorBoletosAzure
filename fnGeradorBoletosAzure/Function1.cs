using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;

namespace fnGeradorBoletosAzure
{
    public class GeradorCodigoBarras    
    {
        private readonly ILogger<GeradorCodigoBarras> _logger;
        private readonly string _serviceBusConectionString;
        private object queueName;
        private const string ServiceBusQueueName = "gerador-codigo-barras";


        public GeradorCodigoBarras(ILogger<GeradorCodigoBarras> logger)
        {
            _logger = logger;
            _serviceBusConectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString") ?? throw new InvalidOperationException("ServiceBusConnectionString is not set.");

        }

        [Function("barcode-generator")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try 
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string valor = data?.valor;
                string dataVencimento = data?.dataVencimento;

                string barccodeData = data?.barcodeData;

                // Validate input data
                if (string.IsNullOrEmpty(valor) || string.IsNullOrEmpty(dataVencimento) || string.IsNullOrEmpty(barccodeData))
                {
                    _logger.LogError("Invalid input data.");
                    return new BadRequestObjectResult("Invalid input data.");
                }

                // Validate barcode data YYYY-MM-DD format
                if (!DateTime.TryParseExact(dataVencimento, "yyyy-MM-dd", null, System.Globalization.DataTimeStyles.None, out DateTime dataObj))
                {
                    return new BadRequestObjectResult("Invalid date format for dataVencimento.");
                }

                string dataStr = dataObj.ToString("yyyyMMdd");

                // Validate valor as decimal 8 digits with 2 decimal places
                if (!decimal.TryParse(valor, out decimal valorDecimal) || valorDecimal < 0 || valorDecimal > 99999999.99m)
                {
                    return new BadRequestObjectResult("Invalid value format for valor.");
                }

         

                var resultObject = new
                {
                    barcode = "00012323123258888200203123734",
                    valorOriginal = 123.45,
                    DataVencimento = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                    ImageBase64 = "ImagemBase64"
                };

                SendFileFallback(resultObject, _serviceBusConectionString, queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the barcode.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        private async Task SendFileFallback(object resultObject, string serviceBusConectionString, object queueName)
        {
            await using var client = new ServiceBusClient(serviceBusConectionString);

            ServiceBusSender sender = client.CreateSender(ServiceBusQueueName);

           string messageBody = System.Text.Json.JsonSerializer.Serialize(resultObject);
            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            try
            {
                await sender.SendMessageAsync(message);
                _logger.LogInformation("Message sent successfully to the queue.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the message to the queue.");
                throw;
            }
        }
    }
}
