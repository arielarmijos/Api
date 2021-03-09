using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Core
{
    /// <summary>
    /// Util para rutinas de felexion de codigo
    /// </summary>
    public class Reflection
    {
        /// <summary>
        /// Crea un objeto invocando al publico por defecto
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a retornar definido por genrecis</typeparam>
        /// <returns>Instancia de objeto </returns>
        public static T FactoryObject<T>()
        {
            Type type = typeof(T);
            T result = default(T);
            Type[] parameters = new Type[] { };
            System.Reflection.ConstructorInfo ctor = type.GetConstructor(parameters);
            result = (T)ctor.Invoke(new object[] { });

            return result;
        }

        /// <summary>
        /// Crea un objeto invocando al publico por defecto
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a retornar definido por genrecis</typeparam>
        /// <returns>Instancia de objeto </returns>
        public static T FactoryObject<T>(Type type)
        {

            T result = default(T);
            Type[] parameters = new Type[] { };
            System.Reflection.ConstructorInfo ctor = type.GetConstructor(parameters);
            result = (T)ctor.Invoke(new object[] { });

            return result;
        }
    }
}