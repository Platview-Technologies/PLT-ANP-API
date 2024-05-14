using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.SystemModel
{
    public class SMTPSettings
    {
        private string _fromEmail;
        private string _fromEmailPassword;
        private int _hostPort;
        private string _hostServer;
        private bool _sslStatus;

        public string Section { get; set; } = "SMTPSettings";

        public string FromEmail
        {
            get => _fromEmail;
            set
            {
                if (value != null)
                    _fromEmail = value;
            }
        }

        public string FromEmailPassword
        {
            get => _fromEmailPassword;
            set
            {
                if (value != null)
                    _fromEmailPassword = value;
            }
        }

        public int HostPort
        {
            get => _hostPort;
            set
            {
                // Assuming a valid port range (0 to 65535)
                if (value >= 0 && value <= 65535)
                    _hostPort = value;
            }
        }

        public string HostServer
        {
            get => _hostServer;
            set
            {
                if (value != null)
                    _hostServer = value;
            }
        }

        public bool SSLStatus
        {
            get => _sslStatus;
            set => _sslStatus = value;
        }
    }

}
