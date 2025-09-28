namespace OpenFlowWebServer.Enums
{
    public enum SecurityMethods
    {
        Secret,
        LoginPassword,
        IPWhitelist,   //future implementation
        SSLClientCert, //future implementation
        None
    }
}
