
namespace MyDownloader.Extension.Protocols
{
    public class ProxyParameters: IHttpFtpProtocolParameters
    {
        public string ProxyAddress { get; set; }

        public string ProxyUserName { get; set; }

        public string ProxyPassword { get; set; }

        public string ProxyDomain { get; set; }

        public bool UseProxy { get; set; }

        public bool ProxyByPassOnLocal { get; set; }

        public int ProxyPort { get; set; }

    }
}
