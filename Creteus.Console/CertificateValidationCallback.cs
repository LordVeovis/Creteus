using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Kveer.Creteus.Console
{
	class CertificateValidationCallback
	{
		protected IDictionary<string, X509Certificate2> _certificates;

		protected string _hostname;

		public CertificateValidationCallback(string hostname, IDictionary<string, X509Certificate2> certificates)
		{
			_hostname = hostname;
			_certificates = certificates;
		}

		public bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			string key = _hostname;

			if (certificate == null || chain == null)
			{
				_certificates.Add(key, null);
				return false;
			}

			_certificates.Add(key, new X509Certificate2(certificate.GetRawCertData()));

			return true;
		}

	}
}
