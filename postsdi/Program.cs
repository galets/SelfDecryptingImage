using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using makesdi;
using System.Diagnostics;
using System.Net;

namespace postsdi
{
    class MainClass
    {
        static void Usage()
        {
            Console.Error.WriteLine("postsdi - Post self-decrypting image to google drive");
            Console.Error.WriteLine("");
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("   postsdi <source-path>");
            Console.Error.WriteLine("");
            Console.Error.WriteLine("source-path   Path on local file system, where image file located. Jpeg of png files are supported");
            Console.Error.WriteLine("");
        }

        static void Play(string sound)
        {
            var name = "postsdi.Resources." + sound + ".wav";
            var assembly = typeof(MainClass).Assembly;
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                try 
                {
                    var psi = new ProcessStartInfo()
                    {
                        FileName = "aplay",
                        Arguments = "- --quiet",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                    };

                    var aplay = Process.Start(psi);
                    aplay.OutputDataReceived += (object sender, DataReceivedEventArgs e) => { };
                    aplay.BeginOutputReadLine();
                    stream.CopyTo(aplay.StandardInput.BaseStream);
                    aplay.StandardInput.Close();
                    aplay.WaitForExit();
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine("Could not play {0}.wav: {1}", sound, ex.Message);
                }
            }
        }

        static string FindClientSecretPath()
        {
            var clientSecretPath = "client_secret.json";

            if (!System.IO.File.Exists(clientSecretPath))
            {
                var exePath = typeof(MainClass).Assembly.Location;
                clientSecretPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "client_secret.json");
            }

            if (!System.IO.File.Exists(clientSecretPath))
            {
                clientSecretPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GDriveSiteDeployCli", "client_secret.json");
            }

            if (!System.IO.File.Exists(clientSecretPath))
            {
                clientSecretPath = System.IO.Path.Combine(System.IO.Path.DirectorySeparatorChar.ToString(), "etc", "GDriveSiteDeployCli", "client_secret.json");
            }

            if (!System.IO.File.Exists(clientSecretPath))
            {
                throw new Exception("Cannot find file: 'client_secret.json'. You need to create this file using instructions on https://developers.google.com/drive/web/about-sdk");
            }

            return clientSecretPath;
        }


        public static int Main(string[] args)
        {
            var webFolderName = "posted-self-decrypting-images";
            var helpOptions = "--help,-help,/?,-h,-?";
            if (args.Length == 0 || args.Length > 1 || helpOptions.Split(',').Contains(args[0].ToLowerInvariant()))
            {
                Usage();
                return 1;
            }

            try
            {
                var makeSdi = new MakeSdi();

                var localFileName = args[0];
                if (!File.Exists(localFileName))
                {
                    throw new Exception("Folder does not exist: " + localFileName);
                }

                var ext = Path.GetExtension(localFileName);
                switch(ext.ToLowerInvariant())
                {
                    case ".jpg":
                    case ".jpeg": makeSdi.imageType = "jpeg"; break;
                    case ".png": makeSdi.imageType = "png"; break;
                }

                var ms = new MemoryStream();

                var hash = "";
                using (makeSdi.output = ms)
                using (makeSdi.input = File.OpenRead(localFileName))
                {
                    hash = makeSdi.Process();
                }

                var gsi = new GoogleServiceInitializer(FindClientSecretPath());
                var service = gsi.CreateDriveService().GetAwaiter().GetResult();

                var pf = new PublicFolder(service);
                var folderId = pf.Setup(webFolderName).GetAwaiter().GetResult();

                var targetFileName = string.Format("{0:N}.html", Guid.NewGuid());
                pf.UploadFile(folderId, targetFileName, new MemoryStream(ms.ToArray()));

                var f = service.Files.Get(folderId).Execute();
                Console.WriteLine("{0}{1}#{2}", f.WebViewLink, targetFileName, hash);

                Play("drip");

                return 0;
            }
            catch(Exception ex)
            {
                #if DEBUG
                Console.Error.WriteLine("Error: {0}", ex);
                #else
                Console.Error.WriteLine("Error: {0}", ex.Message);
                #endif

                var webEx = ex as WebException;
                if (webEx != null && webEx.Status == WebExceptionStatus.SendFailure)
                {
                    Console.Error.WriteLine("This error may be fixable by running 'mozroots --import'");
                }

                Play("sonar");

                return 2;
            }
        }
    }
}
