using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebApiAutores.Tests.Mocks;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public  class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtener4Links()
        {
            //Preparación
            var autorizationService = new AuthorizationServiceMock();
            autorizationService.Resultado = AuthorizationResult.Success();
            var rootControlle = new RootController(autorizationService);
            rootControlle.Url = new URLHelperMock();

            //Ejecución
            var resultado = await rootControlle.Get();

            //Verificación
            #pragma warning disable CS8604 // Posible argumento de referencia nulo
            Assert.AreEqual(4, resultado.Value.Count());
            #pragma warning restore CS8604 // Posible argumento de referencia nulo

        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtener2Links()
        {
            //Preparación
            var autorizationService = new AuthorizationServiceMock();
            autorizationService.Resultado = AuthorizationResult.Failed();
            var rootControlle = new RootController(autorizationService);
            rootControlle.Url = new URLHelperMock();


            //Ejecución
            var resultado = await rootControlle.Get();

            //Verificación
            #pragma warning disable CS8604 // Posible argumento de referencia nulo
            Assert.AreEqual(2, resultado.Value.Count());
            #pragma warning restore CS8604 // Posible argumento de referencia nulo

        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtener2Links_UsandoMoq()
        {
            //Preparación
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));



            var mockURlHelper = new Mock<IUrlHelper>();
            mockURlHelper.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
                )).Returns(string.Empty);

            var rootControlle = new RootController(mockAuthorizationService.Object);
            rootControlle.Url = new URLHelperMock();

            //Ejecución
            var resultado = await rootControlle.Get();

            //Verificación
            Assert.AreEqual(2, resultado.Value.Count());

        }
    } 
}
