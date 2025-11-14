using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.CompilerServices;

namespace BibliotecaAPI.Utilidades
{
    public static class ModelStateDictionaryExtensions
    {
        public static  BadRequestObjectResult ConstruirProblemaDetail(this ModelStateDictionary modelstate)
        {
            var problemDetails = new ValidationProblemDetails(modelstate)
            {
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest
            };

            return new BadRequestObjectResult(problemDetails);
        }
    }
}
