using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Entidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; //Controller.V1
            var versionApi = namespaceControlador.Split('.').Last().ToLower(); //v1
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
