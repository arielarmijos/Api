using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Net;
using Movilway.Logging;
using Movilway.API.Core.IPAddressExtensions;

namespace Movilway.API.Core.Security
{
    public static class NetWorkSecurity
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(NetWorkSecurity));



        /// <summary>
        /// por defecto el numero de IPS son dos
        /// </summary>
        private const int DEF_IPS = 2;
        /// <summary>
        /// Coleccion unica de ips las dos primeras IPS son localhost e ippropia
        /// </summary>


        private static HashSet<IPNetWork> _IPLISTH2H = null;
        /// <summary>
        /// Si la lista es valida contiene mas de dos IPS
        /// </summary>
        private static bool CanValidateIpsH2h
        {
            get
            {

                return Convert.ToBoolean(ConfigurationManager.AppSettings["VALID_IPS_H2H"]);
                //return _IPLISTH2H.Count > DEF_IPS;
            }
        }



        static NetWorkSecurity()
        {

            _IPLISTH2H = FactoryMethod(ConfigurationManager.AppSettings["IP_TABLE_FILE_H2H"]);

        }

        public static bool IpIsWithinRangeH2H(string ip)
        {
            //if (CanValidateIpsH2h)
           // {
          //  IPNetWorkValidator ips = ;
            return CanValidateIpsH2h ? new IPNetWorkValidator(new List<IPNetWork>(_IPLISTH2H)).IpIsWithinRange(ip) : true;
            //}
            /* else
             {
                 return true;
             }*/
        }



        /// <summary>
        /// Retorna una lista de IPNetwork con local host y la ip del servidor
        /// </summary>
        /// <param name="FILE_IPS_PATH"></param>
        /// <returns></returns>
        private static HashSet<IPNetWork> FactoryMethod(string FILE_IPS_PATH)
        {

            HashSet<IPNetWork> result = new HashSet<IPNetWork>();
            try
            {

                IPNetWork network = IPNetWork.Parse("127.0.0.1");
                result.Add(network);

                IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
                if (ipEntry.AddressList.Length > 0)
                {
                    IPAddress IP = ipEntry.AddressList[ipEntry.AddressList.Length - 1];
                    result.Add(IPNetWork.Parse(IP.ToString()));
                }



                // Read the file and display it line by line.
                using (System.IO.StreamReader file = new System.IO.StreamReader(FILE_IPS_PATH, System.Text.Encoding.Default))
                {
                    string line = null;
                    while ((line = file.ReadLine()) != null)
                    {
                        try
                        {
                            line = line.Trim().Replace(" ", "");
                            if (line[0] != '!' && line[0] != '-')
                            {
                                network = IPNetWork.Parse(line);
                                result.Add(network);
                            }
                        }
                        catch (ArgumentException ex)
                        {

                            logger.InfoHigh(() => TagValue.New().Tag("[ERROR IP ARCHIVO H2H]").Value(line));
                        }
                        catch (Exception ex)
                        {
                            logger.InfoHigh(() => TagValue.New().Tag("[ERROR INESPERADO IP DE ARCHIVO]").Value(line));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MANEJO DE ERROR SI NO SE PUDO CARGAR LA LISTA DE IPS
                logger.InfoHigh(() => TagValue.New().Tag("ERROR INESPERADO CARGANDO IP'S H2H").Exception(ex));
            }

            return result;
        }
    }

    class IPNetWorkValidator
    {
        private List<IPNetWork> _IPLIST;

        public IPNetWorkValidator(List<IPNetWork> list)
        {
            _IPLIST = list;
        }

        public bool IpIsWithinRange(string ip)
        {
            bool result = false;
            IPAddress incomingIp = IPAddress.Parse(ip);
            //IPAddress incomingIp = IPAddress.Parse(ip);
            for (int i = 0; i < _IPLIST.Count && !result; i++)
            {
                var subnet = _IPLIST[i];
                result = incomingIp.IsInSameSubnet(subnet.ip, subnet.netmask);
            }

            return result;
        }


    }

    public class IPNetWork
    {
        public IPAddress ip { get; set; }
        public IPAddress netmask { get; set; }
        private static IPAddress[] submask = { 
        IPAddress.Parse("0.0.0.0"),
        IPAddress.Parse("128.0.0.0"),
        IPAddress.Parse("192.0.0.0"),
        IPAddress.Parse("224.0.0.0"),
        IPAddress.Parse("240.0.0.0"),
        IPAddress.Parse("248.0.0.0"),
        IPAddress.Parse("252.0.0.0"),
        IPAddress.Parse("254.0.0.0"),
        IPAddress.Parse("255.0.0.0"),
        IPAddress.Parse("255.128.0.0"),
        IPAddress.Parse("255.192.0.0"),
        IPAddress.Parse("255.224.0.0"),
        IPAddress.Parse("255.240.0.0"),
        IPAddress.Parse("255.248.0.0"),
        IPAddress.Parse("255.252.0.0"),
        IPAddress.Parse("255.254.0.0"),
        IPAddress.Parse("255.255.0.0"),
        IPAddress.Parse("255.255.128.0"),
        //18
        IPAddress.Parse("255.255.192.0"),
        IPAddress.Parse("255.255.224.0"),
        IPAddress.Parse("255.255.240.0"),
        IPAddress.Parse("255.255.248.0"),
        IPAddress.Parse("255.255.252.0"),
        IPAddress.Parse("255.255.254.0"),
        IPAddress.Parse("255.255.255.0"),
        IPAddress.Parse("255.255.255.128"),
        IPAddress.Parse("255.255.255.192"),
        IPAddress.Parse("255.255.255.224"),
        IPAddress.Parse("255.255.255.240"),
        IPAddress.Parse("255.255.255.248"),
        IPAddress.Parse("255.255.255.252"),
        IPAddress.Parse("255.255.255.254"),
        IPAddress.Parse("255.255.255.255")};


        public IPNetWork(IPAddress _ip)
        {
            ip = _ip;



            //  netmask = null;//IPAddress.Parse(uintNetmask.ToString());
        }

        public IPNetWork(IPAddress _ip, byte cidr)
        {
            ip = _ip;

            //netmask = IPNetwork.ToUint(cidr);
            if (cidr > 32)
                throw new ArgumentException("SUB MASCARA DE RED INVALIDA");
            netmask = submask[cidr];
            //  netmask = null;//IPAddress.Parse(uintNetmask.ToString());
        }






        public override bool Equals(object obj)
        {
            IPNetWork objip = obj as IPNetWork;
            if (objip == null)
                return false;

            bool val = false;
            if (objip.netmask != null)
            {
                val = objip.netmask.Equals(this.netmask) && objip.ip.Equals(this.ip);

            }
            else
            {
                val = objip.ip.Equals(this.ip);
            }



            return val;
        }

        public override int GetHashCode()
        {
            int rand = 0x123222;

            if (this.netmask != null)
            {
                rand += this.ip.GetHashCode() + this.netmask.GetHashCode();
            }
            else
            {
                rand += this.ip.GetHashCode();
            }
            return rand;
        }

        public static IPNetWork Parse(string ip)
        {
            IPNetWork result = null;

            string[] parts = ip.Split('/');
            if (parts.Length > 1)
            {
                result = new IPNetWork(IPAddress.Parse(parts[0]), byte.Parse(parts[1]));
            }
            else
            {
                result = new IPNetWork(IPAddress.Parse(ip));
            }

            return result;
        }



    }

}