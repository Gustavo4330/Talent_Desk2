using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MySqlConnector;

namespace TalentDesk.Filters
{
    // Quando o MySQL do XAMPP está desligado, mostra uma página explicando
    // o que fazer em vez da tela de erro do ASP.NET.
    public class BancoIndisponivelFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider _metadata;
        private readonly ILogger<BancoIndisponivelFilter> _logger;

        public BancoIndisponivelFilter(IModelMetadataProvider metadata, ILogger<BancoIndisponivelFilter> logger)
        {
            _metadata = metadata;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not MySqlException erro)
            {
                return;
            }

            _logger.LogError(erro, "Erro ao acessar o banco de dados");

            var viewData = new ViewDataDictionary(_metadata, context.ModelState)
            {
                ["Mensagem"] = erro.Message
            };

            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/BancoIndisponivel.cshtml",
                ViewData = viewData,
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };

            context.ExceptionHandled = true;
        }
    }
}
