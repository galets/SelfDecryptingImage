using System;
using System.Text;
using System.IO;

namespace makesdi
{
    public static class Resources
    {
        static string GetResource(string resName)
        {
        
            var name = "makesdi.Resources." + resName;
            var assembly = typeof(MainClass).Assembly;
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static string template { get { return GetResource("template.html"); } }
        #if DEBUG
        public static string aes { get { return GetResource("aes.js"); } }
        public static string b64 { get { return GetResource("b64.js"); } }
        public static string init { get { return GetResource("init.js"); } }
        #else
        public static string aes { get { return GetResource("aes.min.js"); } }
        public static string b64 { get { return GetResource("b64.min.js"); } }
        public static string init { get { return GetResource("init.min.js"); } }
        #endif
        public static string makesdiusage { get { return GetResource("makesdiusage.txt"); } }

    }
}

