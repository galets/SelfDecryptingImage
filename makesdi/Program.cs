using System;
using System.IO;

namespace makesdi
{
    class MainClass
    {

        static void Usage()
        {
            string usageText = Resources.makesdiusage;
            Console.Error.WriteLine(usageText);
        }

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return 1;
            }

            var makeSdi = new MakeSdi();
            Uri outFileUri = null;
            try
            {
                int pos = 0;
                bool typeSpeficied = false;
                var argsEnum = args.GetEnumerator();
                while (argsEnum.MoveNext())
                {
                    switch (argsEnum.Current.ToString().ToLowerInvariant())
                    {
                        case "--help":
                        case "-help":
                        case "/?":
                        case "-?":
                        case "-h":
                            Usage();
                            return 1;

                        case "--type":
                            if (!argsEnum.MoveNext()) 
                            {
                                throw new Exception();
                            }

                            if (argsEnum.Current as string == "jpeg") makeSdi.imageType = "jpeg";
                            else if (argsEnum.Current as string == "png") makeSdi.imageType = "png";
                            else throw new Exception();
                            typeSpeficied = true;
                            break;

                        case "--title":
                            if (!argsEnum.MoveNext()) 
                            {
                                throw new Exception();
                            }

                            makeSdi.title = argsEnum.Current as string;
                            break;

                        default:
                            if (pos == 0)
                            {
                                ++pos;
                                makeSdi.input = File.OpenRead(argsEnum.Current as string);

                                if (!typeSpeficied)
                                {
                                    var ext = Path.GetExtension(argsEnum.Current as string);
                                    switch(ext.ToLowerInvariant())
                                    {
                                        case ".jpg":
                                        case ".jpeg": makeSdi.imageType = "jpeg"; break;
                                        case ".png": makeSdi.imageType = "png"; break;
                                    }
                                }
                            }
                            else if (pos == 1)
                            {
                                ++pos;
                                makeSdi.output = File.Create(argsEnum.Current as string);
                                outFileUri = new Uri(Path.GetFullPath(argsEnum.Current as string));
                            }
                            else
                            {
                                throw new Exception();
                            }
                            break;
                    }
                }
            }
            catch {
                Usage();
                return 1;
            }

            using (makeSdi.input)
            using (makeSdi.output)
            {
                var hash = makeSdi.Process();
                Console.Error.WriteLine("Access file via {0}{1}", outFileUri, hash);
            }

            return 0;
        }
    }
}
