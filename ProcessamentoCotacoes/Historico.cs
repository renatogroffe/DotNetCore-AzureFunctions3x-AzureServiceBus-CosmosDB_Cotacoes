using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProcessamentoCotacoes.Data;

namespace ProcessamentoCotacoes
{
    public class Historico
    {
        private readonly CotacoesRepository _repository;

        public Historico(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("Historico")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Function Historico - HTTP GET");
            return new OkObjectResult(_repository.GetAll());
        }
    }
}