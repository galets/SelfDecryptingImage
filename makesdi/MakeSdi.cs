using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace makesdi
{
    public class MakeSdi
    {
        public Stream input;
        public Stream output;
        public string title;
        public string imageType;

        public MakeSdi()
        {
            input = Console.OpenStandardInput();
            output = Console.OpenStandardOutput();
            title = "Self-Decrypting Image";
            imageType = "png";
        }


        public string Process()
        {
            var returnCode = "";

            using (var templateReader = new StringReader(Resources.template))
            using (var writer = new StreamWriter(output, Encoding.UTF8))
            {
                for (var line = templateReader.ReadLine(); line != null; line = templateReader.ReadLine())
                {
                    switch (line.Trim())
                    {
                        case "/*AES*/":
                            writer.WriteLine(Resources.aes);
                            break;
                        case "/*B64*/":
                            writer.WriteLine(Resources.b64);
                            break;
                        case "/*DATA*/":
                            using (var aes = new RijndaelManaged())
                            using (var rnd = new RNGCryptoServiceProvider())
                            using (var ms = new MemoryStream())
                            {
                                aes.BlockSize = 128;
                                aes.KeySize = 256;
                                aes.Padding   = PaddingMode.PKCS7;
                                aes.Mode = CipherMode.CBC;

                                var key = new byte[aes.KeySize / 8];
                                rnd.GetBytes(key);
                                var iv = new byte[16];
                                rnd.GetBytes(iv);

                                using (var encryptor = aes.CreateEncryptor(key, iv))
                                using (var cryptoStream = new CryptoStream(input, encryptor, CryptoStreamMode.Read))
                                {
                                    cryptoStream.CopyTo(ms);
                                }

                                returnCode = string.Format("{0}", Uri.EscapeUriString(Convert.ToBase64String(key)));
                                writer.WriteLine("var imageType='{0}';", imageType);
                                writer.WriteLine("var iv='{0}';", Convert.ToBase64String(iv));
                                writer.WriteLine("var data='{0}';", Convert.ToBase64String(ms.ToArray()));
                            }
                            break;
                        case "<title/>":
                            writer.WriteLine("\t<title>{0}</title>", Utility.HtmlEncode(title));
                            break;
                        default:
                            writer.WriteLine(line);
                            break;
                    }
                }
            }

            return returnCode;
        }
    }
}

