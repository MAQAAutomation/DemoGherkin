using System;
using System.Xml;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.RestServiceFramework.Security;

namespace Demo.RestServiceFramework.Helpers
{
    public class XmlHelper : BaseXmlHelper
    {
        /// <summary>
        /// Loads the authentication configuration.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="auth">The authentication.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">The authorization selected (" + authNode.Attributes["name"].Value + ") is not supported! " 
        ///                         + " Please configure a supported one: " + Enum.GetNames(typeof(SecurityManager.EAuthorization))</exception>
        public static SecurityManager.EAuthorization LoadAuthenticationConfig(string path, ref object auth)
        {
            XmlDocument xml = GetXMLDocumentFromFile(path);
            SecurityManager.EAuthorization authentication = SecurityManager.EAuthorization.None;

            XmlNode authNode = xml.SelectSingleNode(@"//*[local-name()='Authorization']");
            if (authNode != null)
            {
                authentication = (SecurityManager.EAuthorization)Enum.Parse(typeof(SecurityManager.EAuthorization), authNode.Attributes["name"].Value);
            }

            switch (authentication)
            {
                case SecurityManager.EAuthorization.None:
                    auth = null;
                    break;
                case SecurityManager.EAuthorization.Basic:
                    auth = new BasicAuthConfig(GetStringElementValueByName(xml, "user")
                                            , GetStringElementValueByName(xml, "password"));
                    break;
                case SecurityManager.EAuthorization.Login:
                case SecurityManager.EAuthorization.LoginSPA:
                    auth = new LoginConfig(RunSettings.RestServiceUrl
                                            , RunSettings.SecurityServerURL
                                            , GetStringElementValueByName(xml, "user")
                                            , GetStringElementValueByName(xml, "password")
                                            , GetStringElementValueByName(xml, "mainService")
                                            );
                    break;
                case SecurityManager.EAuthorization.RefreshToken:
                    auth = new RefreshTokenConfig(RunSettings.RestServiceUrl
                                            , GetStringElementValueByName(xml, "refreshtoken")
                                            , GetStringElementValueByName(xml, "environment"));
                    break;
                case SecurityManager.EAuthorization.OAuth2:
                    auth = new OAuth2Config(RunSettings.SecurityServerURL + RunSettings.AuthServerEndpoint
                                            , GetStringElementValueByName(xml, "clientId")
                                            , GetStringElementValueByName(xml, "clientKey")
                                            , GetListStringElementValueByName(xml, "scopes", "scope"));
                    break;
                default:
                    throw new FrameworkException("The authorization selected (" + authNode.Attributes["name"].Value + ") is not supported!"
                        + " Please configure a supported one: " + Enum.GetNames(typeof(SecurityManager.EAuthorization)));
            }

            return authentication;
        }
    }
}
