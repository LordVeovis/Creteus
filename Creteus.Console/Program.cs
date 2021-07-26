using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Kveer.Creteus.Console
{
	class JsonResponse
	{
		public IList<string> sites { get; set; }

		public int warnDays { get; set; }
	}

	class Program
	{
		private static IDictionary<string, X509Certificate2> _certificates = new Dictionary<string, X509Certificate2>();

		static void Main(string[] args)
		{
			var json = JsonConvert.DeserializeObject<JsonResponse>(File.ReadAllText("sites.json"));
			var sites = json.sites;

			//ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
			foreach (var s in sites)
			{
				Uri url;
				var urlResult = Uri.TryCreate(s, UriKind.Absolute, out url);

				switch (url.Scheme)
				{
					case "https":
						var request = (HttpWebRequest)WebRequest.Create(s);
						request.ServerCertificateValidationCallback = new CertificateValidationCallback(url.DnsSafeHost, _certificates).ServerCertificateValidationCallback;
						request.AllowAutoRedirect = false;

						try
						{
							using (var response = request.GetResponse())
								response.Close();
						}
						catch (WebException e) when ((e.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Found)
						{

						}
						break;
					case "smtps":
						using (var smtpClient = new SmtpClient(url.DnsSafeHost, url.Port))
						{
							ServicePointManager.ServerCertificateValidationCallback = new CertificateValidationCallback(url.DnsSafeHost, _certificates).ServerCertificateValidationCallback; 
							smtpClient.EnableSsl = true;
							smtpClient.UseDefaultCredentials = false;
							using (var mail = new MailMessage("test@example.nono", "test@example.nono", "Test", "Test"))
								try
								{
									smtpClient.Send(mail);
								}
								catch (SmtpFailedRecipientException e)
								{

								}
						}

						ServicePointManager.ServerCertificateValidationCallback = null;
						break;
					default:
						_certificates.Add(s, null);
						break;
				}

			}

			foreach (var el in _certificates)
			{
				var url = el.Key;
				System.Console.Write($"{url}: ");

				if (el.Value == null)
				{
					ConsoleExtension.Write("ERROR: NO CERTIFICATE", ConsoleColor.DarkRed);
					continue;
				}

				var today = DateTime.Today;
				var daysBeforeExpiration = Math.Floor((el.Value.NotAfter - today).TotalDays);


				if (daysBeforeExpiration < json.warnDays)
					ConsoleExtension.Write("MUST BE REPLACED ", ConsoleColor.Red);
				else
					ConsoleExtension.Write("OK ", ConsoleColor.Green);
				System.Console.WriteLine($" ({daysBeforeExpiration} days remaining)");
			}

			if (!System.Diagnostics.Debugger.IsAttached)
				System.Console.ReadLine();
		}
	}
}
