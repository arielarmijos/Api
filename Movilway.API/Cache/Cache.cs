using Movilway.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movilway.Cache
{
    public static class CacheFactory
    {
        private const string NameCache = "GENERAL";
        private static volatile Cache _sinCacheSession =null;
        private static volatile Cache _sinCacheSaldo = null;


        static CacheFactory()
        {
              _sinCacheSession = new Cache("SESSIONID");
              _sinCacheSaldo = new Cache("SALDO");
        }

        public static  ICache GetSingleInstaceCacheSession
        {
            get
            {
                return _sinCacheSession;
            }
        }


        public static  ICache GetSingleInstaceCacheSaldo
        {
            get
            {
                return _sinCacheSaldo;
            }
        }



        public static ICache FactoryCache(String name = NameCache)
        {

            return new Cache(name);
         
        }
    }


    /// <summary>
    /// Indica los typos de cache posibles Enum o calse static
    /// 
    /// </summary>
    //public static class CacheType
    //{
    //    public const int NORMAL = 0;
    //    public const int SLIDING_EXPIRATION = 1;
    //    public const int SLIDING_EXPIRATION_MAX_TIME = 2;
    //}

    internal interface ICacheContext
    {
        /// <summary>
        /// Tiempo por Default de vida en cache
        /// </summary>
        TimeSpan DefaultTime { get; }
        /// <summary>
        /// Tiempo maximo de vida permitido
        /// </summary>
        TimeSpan MaxTime { get; }

        /// <summary>
        /// Indica si el cache esta condigurado para validar un tiempo maximo de vida
        /// </summary>
        bool ValidMaxTime { get; }

        //futuro multiple tipos
        //int GetGacheType { get; }

    }

    public class Cache : ICache, ICacheContext
    {



        //TODO QUITAR
        public static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Cache));

        //TODO QUITAR
        public static volatile bool DEBUG_FIND = false;

        public const int NORMAL_STATE = 0;

        public const int ORDERLY_STATE = 1;


        public const int BUSY_STATE = 2;

        /// <summary>

        /// Initial capacity cache
        /// </summary>
        private const string POSFIX_INITIAL_CAPACITY = "_INITIAL_CAPACITY";

        /// <summary>

        /// constante para el posfijo de la configuracion del tiempo
        /// </summary>
        private const string POSFIX_CNF_TIME = "_TIME_CACHE";

        /// <summary>
        /// constante para el posfijo de la configuracion habilitado
        /// </summary>
        private const string POSFIX_CNF_AVIABLE = "_AVAILABLE";

        /// <summary>
        /// constante para el posfijo de la configuracion habilitado
        /// </summary>
        private const string POSFIX_CNF_MAXTIME = "_MAXTIME";



        /// <summary>
        /// Prefijo del cache para acceder a la configuracion del cache
        /// </summary>
        private readonly string PREFIX_CNF;

        /// <summary>
        /// Diccionario de datos 
        /// </summary>
        /// ConcurrentDictionaryDecorator
        private ConcurrentDictionary<CacheKey, CachedItem> diccionario; // <Object, CachedItem> diccionario;


        // ConcurrentDictionaryDecorator<CacheKey, CachedItem> diccionario2;


        /// <summary>
        /// numero de peticiones 
        /// </summary>
        private int _peticiones = 0;



        //private void AddPeticion()
        //{
        //    Interlocked.Increment(ref this._peticiones);
        //}

        //private void QuitPeticion()
        //{
        //    Interlocked.Decrement(ref this._peticiones);
        //}

        /// <summary>
        /// Indica si el diccionario necesita organizarse
        /// </summary>
        private int _state = NORMAL_STATE;

        //private void SetState(int newState )
        //{
        //    Interlocked.Exchange(ref _state, newState);
        //}


        public int GetState()
        {
            return _state;
        }

        private int GetPeticiones()
        {
            return _peticiones;
        }



        private TimeSpan _defaultTime;

        public TimeSpan DefaultTime
        {
            get
            {
                return _defaultTime;
                //return TimeSpan.FromMinutes(2);
                //string llave = ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_CNF_TIME)];//"_TIME_SESSION_ID")];
                //if (!string.IsNullOrEmpty(llave))
                //    return TimeSpan.Parse(llave);
                //else
                //    return TimeSpan.Parse("0:01:00");



            }
        }


        private TimeSpan _maxTime;

        public TimeSpan MaxTime
        {
            get
            {
                return _maxTime;
                ////return TimeSpan.FromMinutes(2);
                //string llave = ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_CNF_MAXTIME)];//"_TIME_SESSION_ID")];
                //if (!string.IsNullOrEmpty(llave))
                //    return TimeSpan.Parse(llave);
                //else
                //    return TimeSpan.Parse("0:00:00");
            }
        }


        private volatile bool _validMaxTime;

        public bool ValidMaxTime
        {
            get
            {
                return _validMaxTime;
            }
        }

        private volatile bool _isActiveCache;

        public Boolean IsActiveCache()
        {

            //if (_state == NORMAL_STATE)
            return _isActiveCache;//ISAVAIABLECACHE;
            // else
            //  return false;
        }



        public Cache(string prefix)
        {
            PREFIX_CNF = prefix;
            
            //TODO FASE DE PRUEBAS
            //try
            //{
            //    int concurrency = 0, capacity = 0;
            //    Int32.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, "_CONCURRENCY_LEVEL")], out concurrency);
            //    Int32.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, "_INITIAL_CAPACITY")], out capacity);

            //    //
            //    diccionario = new ConcurrentDictionary<CacheKey, CachedItem>();
            //    if (concurrency > 0 && capacity > 0)
            //        diccionario = new ConcurrentDictionary<CacheKey, CachedItem>(concurrency, capacity);
            //}
            //catch (Exception ex)
            //{

            //}


            string _prefixLog = String.Concat("CACHE [", PREFIX_CNF, "]");
            diccionario = new ConcurrentDictionary<CacheKey, CachedItem>();



            //TODO FASE DE PRUEBAS
            try
            {
                int concurrency = 4, capacity = 0;
                //Int32.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, "_CONCURRENCY_LEVEL")], out concurrency);
                Int32.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_INITIAL_CAPACITY)], out capacity);
                //

                if (concurrency > 0 && capacity > 0)
                {
                    diccionario = new ConcurrentDictionary<CacheKey, CachedItem>(concurrency, capacity);
                }
                else
                    diccionario = new ConcurrentDictionary<CacheKey, CachedItem>();

                logger.InfoHigh(String.Concat(_prefixLog, " ", POSFIX_INITIAL_CAPACITY, " ", capacity.ToString()));
            }
            catch (Exception ex)
            {
                logger.InfoHigh(String.Concat(_prefixLog, " ERROR VARIABLE ", POSFIX_CNF_TIME, " ", ex.Message, " ", ex.StackTrace));
          
            }


            try
            {

                _defaultTime = TimeSpan.Zero;
                TimeSpan.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_CNF_TIME)], out _defaultTime);


                logger.InfoHigh(String.Concat(_prefixLog, " ", POSFIX_CNF_TIME, " ", _defaultTime.ToString()));
            }
            catch (Exception ex)
            {
                logger.InfoHigh(String.Concat(_prefixLog, " ERROR VARIABLE ", POSFIX_CNF_TIME, " ", ex.Message," ",ex.StackTrace));
            }


            try
            {

                //return TimeSpan.FromMinutes(2);
                _maxTime = TimeSpan.Zero;

                TimeSpan.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_CNF_MAXTIME)], out _maxTime);
                _validMaxTime = _maxTime > TimeSpan.Zero;


                logger.InfoHigh(String.Concat(_prefixLog, " ", POSFIX_CNF_MAXTIME, " ", _maxTime.ToString()));
            }
            catch (Exception ex)
            {
                logger.InfoHigh(String.Concat(_prefixLog, " ERROR VARIABLE ", POSFIX_CNF_MAXTIME, " ", ex.Message, " ", ex.StackTrace));
            }

            try
            {

                bool active = false;
                Boolean.TryParse(ConfigurationManager.AppSettings[string.Concat(PREFIX_CNF, POSFIX_CNF_AVIABLE)], out  active);

                _isActiveCache = active && _defaultTime > TimeSpan.Zero;

                logger.InfoHigh(String.Concat(_prefixLog, " ", POSFIX_CNF_AVIABLE, " ", _isActiveCache));
            }
            catch (Exception ex)
            {
                logger.InfoHigh(String.Concat(_prefixLog, " ERROR VARIABLE ", POSFIX_CNF_AVIABLE, " ", ex.Message, " ", ex.StackTrace));
            }




        }

        public T GetValue<T>(Object key, Func<T> func)
        {
            return GetValue<T>(key, func, DefaultTime, null);
        }

        /// <summary>
        /// Obtengo un objeto en cache si el objeto es nuevo 
        /// o el tiempo de cache se cumplio se ejecuta el delegado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func">delegado para ejecutar </param>
        /// <param name="time"></param>
        /// <returns></returns>
        public T GetValue<T>(object key, Func<T> func, TimeSpan time)
        {
            return GetValue<T>(key, func, time, null);
        }

        /// <summary>
        /// Obtengo un objeto en cache si el objeto es nuevo 
        /// o el tiempo de cache se cumplio se ejecuta el delegado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func">delegado para ejecutar </param>
        /// <param name="oncache">Call back a ejecutar si el objeto esta en cache</param>
        /// <returns></returns>
        public T GetValue<T>(Object key, Func<T> func, Action<Object, Object> oncache)
        {
            return GetValue<T>(key, func, DefaultTime, oncache);
        }

        /// <summary>
        /// Obtengo un objeto en cache si el objeto es nuevo 
        /// o el tiempo de cache se cumplio se ejecuta el delegado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func">delegado para ejecutar </param>
        /// <param name="time"></param>
        /// <param name="oncache">Call back a ejecutar si el objeto esta en cache</param>
        /// <returns></returns>
        public T GetValue<T>(object key, Func<T> func, TimeSpan time, Action<Object,Object> oncache)
        {
            //atiendo peticion
            bool activeCache = IsActiveCache();

            //TODO TEST
            //if (DEBUG_FIND)
            //    logger.InfoHigh(String.Concat("cache activo ", activeCache, " peticiones ", _peticiones));

            if (activeCache)
            {
                //TODO FASE DE PRUEBAS
                //_peticiones++;
                //AddPeticion();
                try
                {
                    #region Implementacion concreta para obtener un valor del cache


                    var fecha = DateTime.Now;


                    CachedItem it = diccionario.GetOrAdd(new CacheKey(key), (copyOfMyKey) =>
                    {
                        //handler se ejecuta por que la llave no existe
                        //se llama al constructor por defecto
                        //objeto es nuevo 
                        return new CachedItem(this);
                    });

                    //si el objeto es nuevo o la fecha expiro
                    int state = it.State(time);
                    if (state != CachedItem.CacheValido)//it.Value == null || (fecha - it.Fecha) > time)
                    {

                        //logger.InfoHigh(String.Concat(key, " VALUE FROM HANDLER ", state));

                        T result = func();
                        it.Fecha = fecha;
                        it.Value = result;
                        // se agrega o se actualiza
                        diccionario.AddOrUpdate(new CacheKey(key), it, (k, existingVal) =>
                        {
                            //
                            existingVal.Fecha = it.Fecha;
                            existingVal.Value = it.Value;
                            return existingVal;
                        });

                        return result;


                    }
                    else
                    {
                        //logger.InfoHigh(String.Concat(key, " VALUE FROM CACHE ", state));
                        //oncache

                        T result = (T)it.Value;

                        if (oncache != null)
                            oncache(key, result);

                  

                        return result;
                    }


                    #endregion
                }
                catch (Exception ex)
                {
      
                    throw ;
                }
                finally
                {
                    //TODO FASE DE PRUEBAS
                    //QuitPeticion();


                }
            }
            else
                return func();
        }


        public S GetValue<T, S>(object key, T param, Func<T, S> func, TimeSpan time)
        {
            //TODO ESTADO ACTIVO
            //atendiendo peticion
            //atiendo peticion
            bool activeCache = IsActiveCache();
            // if(DEBUG_FIND)
            // logger.InfoHigh(String.Concat("cache activo ", activeCache, " peticiones ", _peticiones));



            if (activeCache)
            {


                try
                {
                    #region Implementacion concreta para obtener un valor del cache
                    var fecha = DateTime.Now;
                    CachedItem it = diccionario.GetOrAdd(new CacheKey(key), (copyOfMyKey) =>
                    {
                        //handler se ejecuta por que la llave no existe
                        //se llama al constructor por defecto
                        //objeto es nuevo 
                        return new CachedItem(this);
                    });

                    //si el objeto es nuevo o la fecha expiro
                    int state = it.State(time);
                    if (state != CachedItem.CacheValido)//(it.Value == null || (fecha - it.Fecha) > time)
                    {
                        //logger.InfoHigh(String.Concat(key, " VALUE FROM HANDLER", state));
                        S result = func(param);
                        it.Fecha = fecha;
                        it.Value = result;
                        // se agrega o se actualiza
                        diccionario.AddOrUpdate(new CacheKey(key), it, (k, existingVal) =>
                        {
                            //
                            existingVal.Fecha = it.Fecha;
                            existingVal.Value = it.Value;
                            return existingVal;
                        });
                        return result;


                    }
                    else
                    {

                        //logger.InfoHigh(String.Concat(key, " VALUE FROM CACHE", state));
                        S result = (S)it.Value;

                        return result;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    //_peticiones--;
                    //if (_peticiones < 0)
                    //    logger.ErrorHigh("[FATAL] ESTADO INCOHERENTE -1");
                }
            }
            else
                return func(param);
        }


        public bool Add<T>(object key, T valor)
        {
            try
            {

                // _peticiones++;
                //AddPeticion();

                CachedItem cache = new CachedItem(this);
                cache.Fecha = DateTime.Now;
                cache.Value = valor;

                return diccionario.TryAdd(new CacheKey(key), cache);
            }
            finally
            {
                // _peticiones--;
                // QuitPeticion();
            }
        }

        public void AddOrUpdate<T>(object key, T valor)
        {
            //TODO ESTADO ACTIVO
            if (IsActiveCache())
            {
                //_peticiones++;
                // AddPeticion();

                try
                {

                    //CachedItem it = diccionario.GetOrAdd(new CacheKey(key), (copyOfMyKey) =>
                    //{
                    //    //handler se ejecuta por que la llave no existe
                    //    //se llama al constructor por defecto
                    //    //objeto es nuevo 
                    //    return new CachedItem(this);
                    //});
                    CachedItem it = new CachedItem(this);
                    it.Value = valor;
                    it.Fecha = DateTime.Now;

                    //opcion hacerlo paralelo  no generar bloqueo
                  //  Parallel.Invoke(() =>
                    //{
                        //
                        diccionario.AddOrUpdate(new CacheKey(key), it, (k, existingVal) =>
                        {
                            //
                            existingVal.Fecha = it.Fecha;
                            existingVal.Value = it.Value;
                            return existingVal;
                        });
                        //

                    //  });
                    //

                 
                }
                catch (Exception ex)
                {
                   
                    throw ;
                }
                finally
                {
                    //_peticiones--;
                    //QuitPeticion();
                    //if (_peticiones < 0)
                    //    logger.ErrorHigh("[FATAL] ESTADO INCOHERENTE -1");

                }
            }
        }

        public void ResetAllCache()
        {

            diccionario.Clear();


        }

        //TODO TEST
        public void Sort()
        {
            try
            {
                //logger.InfoHigh(String.Concat("SORT DICTIONARY STATE", _state));
                // if (GetState() == NORMAL_STATE)
                //{

                //SetState(ORDERLY_STATE);
                //_state = ORDERLY_STATE;

                //logger.InfoHigh(String.Concat("SORT WAIT "));
                //

                TimeSpan tMax = TimeSpan.FromMinutes(2);
                DateTime timeInicio = DateTime.Now;

                //while (GetPeticiones() > 0 || IsActiveCache())
                //{
                //    System.Threading.Thread.Sleep(5);
                //    ValidateState(ORDERLY_STATE);
                //}

                // ValidateState(ORDERLY_STATE);
                //logger.InfoHigh(String.Concat("SORT WAIT END"));


                //logger.InfoHigh(String.Concat("SORT START"));
                //
                //Cache.DEBUG_FIND = false;
                SortedDictionary<CacheKey, CachedItem> dic = new SortedDictionary<CacheKey, CachedItem>(diccionario);
                diccionario = new ConcurrentDictionary<CacheKey, CachedItem>(dic);
                //Cache.DEBUG_FIND = true;
                //


                //logger.InfoHigh(String.Concat("SORT END ",_state));
                // }
                // else
                // {
                //TODO QUITAR
                //logger.InfoHigh(String.Concat("CAN'T ORDER " , _state));
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX " + ex.Message);
                throw ex;
            }
            finally
            {
                //SetState(NORMAL_STATE);
                //   _state = NORMAL_STATE;
            }

        }


        /// <summary>
        /// Retorna el numero de elementos en cache
        /// </summary>
        // TODO TEST
        public int Count
        {
            get
            {
                int result = -1;
                //if (GetState() == NORMAL_STATE)
                {
                    //_state = BUSY_STATE;
                    //SetState(BUSY_STATE);
                    //logger.InfoHigh(String.Concat("COUNT  WAIT "));



                    //TODO COLOCAR LIMITE DE TIEMPO
                    //while (GetPeticiones() > 0 || IsActiveCache())
                    //{
                    //    System.Threading.Thread.Sleep(10);
                    //}

                    result = diccionario.Count;
                    //  _state = NORMAL_STATE;

                    //SetState(NORMAL_STATE);
                }

                return result;
            }
        }



        // TODO TEST
        public void ValidateState(int state)
        {
            switch (state)
            {

                case NORMAL_STATE:
                    if (GetPeticiones() > 0)
                        throw new Exception(string.Concat("NORMAL_STATE EL NUMERO DE PETICIONES ES INCORRECTO ", _peticiones));


                    if (_state != NORMAL_STATE)
                        throw new Exception(string.Concat("NORMAL_STATE EL ESTADO DEL CACHE ES INCORRECTO ", _state));
                    break;
                case ORDERLY_STATE:



                    if (GetState() != ORDERLY_STATE)
                        throw new Exception(string.Concat("ORDERLY_STATE EL ESTADO DEL CACHE ES INCORRECTO ", _state));
                    break;
            }

        }


        public override string ToString()
        {
            return PREFIX_CNF+" Cantidad " + diccionario.Count;
        }
    }





 
    class CacheKey : IComparable<CacheKey>, IEquatable<CacheKey>
    {



        private int ID;

        private int hash;

        //TIPPE
        private String Val;


        //constructor por defecto
        public CacheKey(Object param)
        {


            ID = param.GetHashCode();
            //metodo para calcular una vez el hash dado que es inmutable
            CreateHasCode();

            //ID += CONSTAN + Val.GetHashCode();
        }


        private void CreateHasCode()
        {
            //DIRECT HAS
            // hash = ID;
            //TODO MI HAS CODE
            hash = 104723;
            int secondPrime = 104729;
            hash = hash * secondPrime + ID;
        }


        public override bool Equals(object obj)
        {
         
            bool val = false;
            CacheKey objkey = obj as CacheKey;
            if (objkey != null)
            {
              
                val = Equals(objkey);
            }
            return val;
        }

        public bool Equals(CacheKey other)
        {
            if (other == null)
                return false;

            return ID.Equals(other.ID);
        }


        public static bool operator ==(CacheKey emp1, CacheKey emp2)
        {
            if (object.ReferenceEquals(emp1, emp2)) return true;
            if (object.ReferenceEquals(emp1, null)) return false;
            if (object.ReferenceEquals(emp2, null)) return false;

            return emp1.Equals(emp2);
        }

        public static bool operator !=(CacheKey emp1, CacheKey emp2)
        {
            if (object.ReferenceEquals(emp1, emp2)) return false;
            if (object.ReferenceEquals(emp1, null)) return true;
            if (object.ReferenceEquals(emp2, null)) return true;

            return !emp1.Equals(emp2);
        }

        public override int GetHashCode()
        {
           

            return hash;
        }


        public override string ToString()
        {

            return string.Concat("CacheKey:{ID:", ID, "}");
        }



        public int CompareTo(CacheKey other)
        {
           

            int val = ID.CompareTo(other.ID);

            return val;
        }


    }

    class CachedItem //: IEquatable<CachedItem> ,IEqualityComparer<CachedItem>
    {
        /// <summary>
        /// Contexto en el que se encuentra el Item En cache
        /// </summary>
        private ICacheContext _cacheContex;
        /// <summary>
        /// Indica que el cache se creo
        /// </summary>
        public const int CacheCreado = 1;
        /// <summary>
        /// Indica que el cache se vencio
        /// </summary>
        public const int CacheVencido = 2;
        /// <summary>
        /// El tiempo de cache es valido
        /// </summary>
        public const int CacheValido = 3;
        //Fecha de creaciom
        private DateTime _createDate;

        //Fecha de ultimo acceso al objeto
        private DateTime _lastAccess;

        private DateTime _maxTimeLive;

        public DateTime Fecha
        {
            get { return _lastAccess; }
            set
            {
                _createDate = value;
                _lastAccess = value;
                _maxTimeLive = value + _cacheContex.MaxTime;
            }
        }



        /// <summary>
        /// Indica el tiempo maximo de vida de un Item en cache
        /// </summary>
        private DateTime MaxTimeLive
        {
            get
            {
                return _maxTimeLive;
            }

        }

        /// <summary>
        /// Este valor es nulo si y solo si el objeto se creo por primera vez
        /// </summary>
        public Object Value { get; set; }

        //constructor por defecto
        public CachedItem(ICacheContext cacheContex)
        {
            _cacheContex = cacheContex;
            Fecha = DateTime.Now;
        }
        /// <summary>
        /// Verificacion Estado
        /// CacheCreado  1: Objeto es nuevo
        /// CacheVencido 2: Si el objeto se vencio el cache
        /// CacheValido  3: Si el objeto es valido
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int State(TimeSpan time)
        {
           
            DateTime TimeToValidate = DateTime.Now;

            int state = CacheCreado;

                //REALIZAR SLIDENG EXPIRATION 
                if (_cacheContex.ValidMaxTime)
                {
                    if (Value != null)
                    {
                        bool _validMaxTime =  TimeToValidate < MaxTimeLive; //(TimeToValidate - MaxTimeLive) < TimeSpan.FromSeconds(1);


                        // si el tiempo en cache es valido
                        if (_validMaxTime) //if (TimeToValidate < MaxTimeLive) 
                        {

                            //Fecha actual mas rango maximo


                            if ((TimeToValidate - Fecha) > time)
                            {
                                state = CacheVencido;
                            }
                            else
                            {
                                state = CacheValido;

                                //SLIDING EXPIRATION
                                //si y solo si no se supero el rango permitido
                                _lastAccess = DateTime.Now;
                            }
                        }
                        else
                        {

                            state = CacheVencido;
                        }

                    }

                }
                else
                {
                    state = Value == null ? CacheCreado : (TimeToValidate - Fecha) > time ? CacheVencido : CacheValido;//-1;//
                }

                // EL CACHE  MANEJA SLIDING EXPIRATION  _MAX_TIME
            
            // int state = Value == null ? CacheCreado : (TimeToValidate - Fecha) > time ? CacheVencido : CacheValido;//-1;//
            return state;
        }

        //for debug
        //public override bool Equals(Object other)
        //{

        //  return   base.Equals(other);
        //}

        //public int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}


        //public bool Equals(CachedItem other)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Equals(CachedItem x, CachedItem y)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetHashCode(CachedItem obj)
        //{
        //    throw new NotImplementedException();
        //}
    }


}
