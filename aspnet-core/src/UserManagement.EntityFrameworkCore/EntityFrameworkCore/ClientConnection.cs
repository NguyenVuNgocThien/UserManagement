using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UserManagement.EntityFrameworkCore
{
    public static class ClientConnection
    {
        static IConfigurationRoot _appConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        static string clientRequest = _appConfiguration.GetValue<string>("Uam:ClientRequestUrl");
        static string isLocalConnection = _appConfiguration.GetValue<string>("Uam:LocalConnection");
        static string isBypassCert = _appConfiguration.GetValue<string>("Uam:ByPassCert");
        static string _defaultConnection;
        public static string DefaultConnection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultConnection))
                {
                    _defaultConnection = GetConnectionString("ConnectionStrings:Default", "Uam:DefaultConnection");
                }
                return _defaultConnection;
            }
        }
        static string GetConnectionString(string localConnection,string secKey)
        {
            if (isLocalConnection == "false")
            {
                var logger = IocManager.Instance.Resolve<ILogger>();
                logger.Info($"Request GetConnection String from UAM:{localConnection}");
                try
                {
                    using(var client =new HttpClient())
                    {
                        if (isBypassCert == "true")
                        {
                            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        }

                        var secContent = System.Configuration.ConfigurationManager.AppSettings[secKey];
                        logger.Info($"Request:{localConnection} - {secContent}");
                        IocManager.Instance.Release(logger);
                        var content = new StringContent(secContent, Encoding.UTF8, "application/x-ww-form-urlencoded");
                        var result = client.PostAsync(clientRequest, content).Result;
                        var readTask = result.Content.ReadAsStringAsync();
                        var resultContent = readTask.Result;
                        var xdoc = new XmlDocument();
                        xdoc.LoadXml(resultContent);
                        var resultNode = xdoc.SelectSingleNode("//response//result");
                        var contentNode = xdoc.SelectSingleNode("//response//content");
                        if (resultNode == null || contentNode == null)
                        {
                            return resultContent;
                        }
                        if (resultNode.InnerText == "000")
                            return contentNode.InnerText;
                        return $"Get Connection String From UAM Failed:{localConnection} - {secContent}";
                    }
                }
                catch(Exception e)
                {
                    logger.Error("Get Connection String from UAM Exception:" + e.Message);
                    IocManager.Instance.Release(logger);
                    throw e;
                }
            }
            var strValue = _appConfiguration.GetValue<string>(localConnection);
            return strValue;
        }
    }
}
