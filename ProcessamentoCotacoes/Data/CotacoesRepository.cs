using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using ProcessamentoCotacoes.Models;
using ProcessamentoCotacoes.Documents;

namespace ProcessamentoCotacoes.Data
{
    public class CotacoesRepository
    {
        private const string DB_COTACOES = "DBCotacoes";
        private const string COLLECTION_HISTORICO = "HistoricoMoedas";
        private readonly string _DBAcoesEndpointUri;
        private readonly string _DBAcoesEndpointPrimaryKey;

        public CotacoesRepository(IConfiguration configuration)
        {
            _DBAcoesEndpointUri = configuration["DBAcoesEndpointUri"];
            _DBAcoesEndpointPrimaryKey = configuration["DBAcoesEndpointPrimaryKey"];

            using var client = GetDocumentClient();

            client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = DB_COTACOES }).Wait();

            DocumentCollection collectionInfo = new DocumentCollection();
            collectionInfo.Id = COLLECTION_HISTORICO;

            collectionInfo.IndexingPolicy =
                new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

            client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DB_COTACOES),
                collectionInfo,
                new RequestOptions { OfferThroughput = 400 }).Wait();
        }

        private DocumentClient GetDocumentClient()
        {
            return new DocumentClient(
                new Uri(_DBAcoesEndpointUri), _DBAcoesEndpointPrimaryKey);
        }

        public void Save(CotacaoMoeda cotacao)
        {
            var horario = DateTime.Now;
            var document = new CotacaoMoedaDocument()
            {
                id = $"{cotacao.Codigo}-{horario.ToString("yyyyMMdd-HHmmss")}",
                Sigla = cotacao.Codigo,
                DataReferencia = horario.ToString("yyyy-MM-dd HH:mm:ss"),
                Valor = cotacao.Valor.Value
            };

            using var client = GetDocumentClient();
            client.CreateDocumentAsync(
               UriFactory.CreateDocumentCollectionUri(
                   DB_COTACOES, COLLECTION_HISTORICO), document).Wait();
        }

        public object GetAll()
        {
            using var client = GetDocumentClient();
            FeedOptions queryOptions =
                new FeedOptions { MaxItemCount = -1 };
            return client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri(
                    DB_COTACOES, COLLECTION_HISTORICO),
                    "SELECT C.id, C.Sigla, C.Valor, C.DataReferencia " +
                    "FROM Cotacoes C " +
                    "ORDER BY C.DataReferencia DESC", queryOptions)
                .ToList();
        }
    }
}