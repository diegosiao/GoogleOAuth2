using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using CommandLine;
using System.Collections.Generic;
using System;
using System.IO;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;
using System.Text;

namespace GoogleOAuth2
{
    internal class Options
    {
        [Option('u', "user",
            Required = true,
            HelpText = "Email account provided by Google Developers Console to access")]
        public string ServiceAccount { get; set; }

        [Option('c', "certificate-p12",
            Required = true,
            HelpText = "Email account provided by Google Developers Console to API access")]
        public string CertificateFile { get; set; }

        [Option('s', "scopes",
            Required = true,
            HelpText = "The comma separated values describing the scopes associated with the authorization")]
        public string Scopes { get; set; }

        [Option('o', "output",
            Default = "access-token.json",
            Required = false,
            HelpText = "Name of the output file")]
        public string OutputFile { get; set; }

        [Option('f', "format",
            Default = "json",
            Required = false,
            HelpText = "Output file content format ['json' or 'keyvalue']")]
        public string OutputFormat { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => Run(opts))
                .WithNotParsed(errs => HandleParseError(errs));
        }

        private static int Run(Options options)
        {
            try
            {
                var certificate = new X509Certificate2(options.CertificateFile, "notasecret", X509KeyStorageFlags.Exportable);

                var scopes = options.Scopes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(options.ServiceAccount)
                {
                    Scopes = scopes
                }.FromCertificate(certificate));

                if (credential.RequestAccessTokenAsync(CancellationToken.None).Result == false)
                    return -1;
                
                switch (options.OutputFormat)
                {
                    case "json":
                        return SaveJson(options.OutputFile, credential.Token);
                    case "keyvalue":
                        return SaveKeyValue(options.OutputFile, credential.Token);
                    default:
                        Console.Error.WriteLine("The output format is not valid. Inform 'json' or 'keyvalue' with --o option.");
                        return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error requesting authorization: {ex.Message}");
                return -1;
            }
        }

        private static int SaveKeyValue(string outputFile, TokenResponse accessToken)
        {
            try
            {
                var contents = new StringBuilder();
                foreach (var prop in accessToken.GetType().GetProperties())
                {
                    if (prop.GetValue(accessToken) == null)
                    {
                        contents.AppendLine($"{ prop.Name }=");
                        continue;
                    }

                    if(prop.PropertyType == typeof(DateTime))
                        contents.AppendLine($"{prop.Name}={((DateTime)prop.GetValue(accessToken)).ToString("yyyy-MM-dd HH:mm:ss")}");
                    else
                        contents.AppendLine($"{prop.Name}={prop.GetValue(accessToken)}");
                }

                File.WriteAllText(outputFile, contents.ToString());
                return -1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while saving the output file:\r\n { ex.Message }");
                return -1;
            }
        }

        private static int SaveJson(string outputFile, TokenResponse accessToken)
        {
            try
            {
                File.WriteAllText(outputFile, JsonConvert.SerializeObject(accessToken));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while saving the output file:\r\n { ex.Message }");
                return -1;
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            foreach(var error in errors)
            {
                Console.Error.WriteLine($"Error parsing options: { error.ToString() }");
            }

            Console.Error.WriteLine("Type --help for instructions");
        }
    }
}