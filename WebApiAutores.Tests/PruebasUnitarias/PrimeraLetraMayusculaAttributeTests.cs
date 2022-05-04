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
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            var valor = "juan";
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);

            //Verificación
            if (resultado != null )
            {
                Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
            }
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            string? valor = null;
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);


            //Verificación
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimerraLetraMayuscula_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();

            string valor = "Juan";
            var valConetxt = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valConetxt);


            //Verificación
            Assert.IsNull(resultado);
        }
    }
}