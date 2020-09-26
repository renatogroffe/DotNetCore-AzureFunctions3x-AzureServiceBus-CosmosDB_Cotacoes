using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProcessamentoCotacoes.Data;
using ProcessamentoCotacoes.Models;
using ProcessamentoCotacoes.Validators;

namespace ProcessamentoCotacoes
{
    public class ProcessarCotacao
    {
        private readonly CotacoesRepository _repository;

        public ProcessarCotacao(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("ProcessarCotacao")]
        public void Run([ServiceBusTrigger("queue-cotacoes", Connection = "AzureServiceBus")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"ProcessarCotacao - Dados: {myQueueItem}");

            CotacaoMoeda cotacao = null;
            try
            {
                cotacao = JsonSerializer.Deserialize<CotacaoMoeda>(myQueueItem,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                log.LogInformation($"ProcessarCotacao - Erro durante a deserialização");
            }

            if (cotacao == null)
                return;

            var validationResult = new CotacaoMoedaValidator().Validate(cotacao);
            if (validationResult.IsValid)
            {
                log.LogInformation($"ProcessarCotacao - Dados pós formatação: {JsonSerializer.Serialize(cotacao)}");
                _repository.Save(cotacao);
                log.LogInformation("ProcessarCotacao - Cotação registrada com sucesso!");
            }
            else
            {
                log.LogError("ProcessarCotacao - Dados invalidos para a Cotação");
                foreach (var error in validationResult.Errors)
                    log.LogError($"ProcessarCotacao - {error.ErrorMessage}");
            }
        }
    }
}