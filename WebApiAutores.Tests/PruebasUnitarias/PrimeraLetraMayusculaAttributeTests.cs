using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;
using Xunit.Sdk;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            var valor = "juan";
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecuci�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);

            //Verificaci�n
            if (resultado != null )
            {
                Assert.AreEqual("La primera letra debe ser may�scula", resultado.ErrorMessage);
            }
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            string? valor = null;
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecuci�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);


            //Verificaci�n
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimerraLetraMayuscula_NoDevuelveError()
        {
            //Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            string valor = "Juan";
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecuci�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);


            //Verificaci�n
            Assert.IsNull(resultado);
        }
    }
}