using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movilway.Cache
{
    public interface ICache
    {
        /// <summary>
        /// Agrega el Objeto en cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        bool Add<T>(Object key, T valor);



        /// <summary>
        /// Agrega un valor al cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        void AddOrUpdate<T>(Object key, T valor);


        /// <summary>
        /// Retorna en una variable entera el estado del diccionario que se esta implementando
        /// </summary>
        /// <returns></returns>
        int GetState();

        /// <summary>
        /// Indica si el cache esta activo por configuracion
        /// </summary>
        /// <returns></returns>
        bool IsActiveCache();

        /// <summary>
        /// retorna el objeto en cache teniendo encuenta el tiempo de expiracion por defecto
        /// </summary>
        /// <typeparam name="T">Tipo de dato a recuperar</typeparam>
        /// <param name="key">Parametro llave, objeto inmutable</param>
        /// <param name="func">call  back que renueva el valor, si no existe o si el tiempo de cache se cumplio</param>
        /// <returns>Tipo de dato recuperado del cache </returns>
        T GetValue<T>(Object key, Func<T> func);


        /// <summary>
        /// retorna el objeto en cache teniendo encuenta el tiempo de expiracion que se envia 
        /// como parametro
        /// </summary>
        /// <typeparam name="T">Tipo de dato a recuperar</typeparam>
        /// <param name="key">Parametro llave, objeto inmutable</param>
        /// <param name="func">call  back que renueva el valor, si no existe o si el tiempo de cache se cumplio</param>
        /// <param name="oncache">Accion a ejecutar si el valor esta en cache</param>
        /// <returns>Tipo de dato recuperado del cache </returns>
        T GetValue<T>(Object key, Func<T> func, Action<Object,Object> oncache);

        /// <summary>
        /// retorna el objeto en cache teniendo encuenta el tiempo de expiracion que se envia 
        /// como parametro
        /// </summary>
        /// <typeparam name="T">Tipo de dato a recuperar</typeparam>
        /// <param name="key">Parametro llave, objeto inmutable</param>
        /// <param name="func">call  back que renueva el valor, si no existe o si el tiempo de cache se cumplio</param>
        /// <param name="time">Tiempo de expiracion</param>
        /// <returns>Tipo de dato recuperado del cache </returns>
        T GetValue<T>(Object key, Func<T> func, TimeSpan time);


        /// <summary>
        /// retorna el objeto en cache teniendo encuenta el tiempo de expiracion que se envia 
        /// como parametro
        /// </summary>
        /// <typeparam name="T">Tipo de dato a recuperar</typeparam>
        /// <param name="key">Parametro llave, objeto inmutable</param>
        /// <param name="func">call  back que renueva el valor, si no existe o si el tiempo de cache se cumplio</param>
        /// <param name="oncache">Accion a ejecutar si el valor esta en cache</param>
        /// <param name="time">Tiempo de expiracion</param>
        /// <returns>Tipo de dato recuperado del cache </returns>
        T GetValue<T>(Object key, Func<T> func, TimeSpan time, Action<Object, Object> oncache);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S">Tipo de Parametro del delegado</typeparam>
        /// <typeparam name="T">resultado del delegado</typeparam>
        /// <param name="key">llave del cache</param>
        /// <param name="param">Valor del parametro de tipo S</param>
        /// <param name="func">Callback</param>
        /// <param name="time">Tiempo de expiracion</param>
        /// <returns></returns>
        T GetValue<S, T>(Object key, S param, Func<S, T> func, TimeSpan time);


        /// <summary>
        /// Reseta todos los valores del cache
        /// </summary>
        void ResetAllCache();

    }
}
