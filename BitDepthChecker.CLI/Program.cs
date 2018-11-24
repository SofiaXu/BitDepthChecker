using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSCore;
using CSCore.Codecs;

namespace BitDepthChecker.CLI
{
    class Program
    {
        static readonly string enInfomation = "Bit Depth Checker v1.0.0.0 by Aoba\r\n" +
            "License: MIT License\r\n" +
            "Build Date: 2018-11-24";
        static readonly List<string> fileExtensionList = new List<string>()
        {
            ".flac",
            ".wav",
            ".m4a",
            ".ape"
        };
        static void Main(string[] args)
        {
            Console.WriteLine(enInfomation);
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Program needs a audio file or directory's path.");
                HelpInfo();
                return;
            }
            if (args[0].ToLower() == "-d")
            {
                DirectoryInfo directory = new DirectoryInfo(args[1]);
                if (!directory.Exists)
                {
                    Console.WriteLine("Program cannot find the directory.");
                    HelpInfo();
                    return;
                }
                var files = directory.GetFiles();
                List<Task> taskList = new List<Task>();
                foreach (var item in files)
                {
                    Task task = new Task(() =>
                    {
                        try
                        {
                            CheckSingleFile(item);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
                    taskList.Add(task);
                    task.Start();
                }
                Task.WaitAll(taskList.ToArray());
            }
            else
            {
                FileInfo file = new FileInfo(args[0]);
                if (!file.Exists)
                {
                    Console.WriteLine("Program cannot find the file.");
                    HelpInfo();
                    return;
                }
                try
                {
                    CheckSingleFile(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        static void CheckSingleFile(FileInfo file)
        {
            string fileGuid = Guid.NewGuid().ToString();
            int originalBitDepth = 0;
            int frameLength = 0;
            Checker.BitDepth trueBitDepth = Checker.BitDepth.Other;
            if (!fileExtensionList.Any(s => file.Extension.ToLower() == s))
            {
                throw new Exception(file.Name + "\tUnsupported File");
            }
            DateTime start = DateTime.Now;
            using (FileStream fileStream = File.Create(fileGuid + ".pcm"))
            {
                var decode = CodecFactory.Instance.GetCodec(file.FullName);
                originalBitDepth = decode.WaveFormat.BitsPerSample;
                frameLength = decode.WaveFormat.BytesPerSample;
                decode.WriteToStream(fileStream);
                decode.Dispose();
            }
            Checker checker = new Checker(frameLength, File.ReadAllBytes(fileGuid + ".pcm"), originalBitDepth);
            trueBitDepth = checker.FullyCheckBitDepth();
            DateTime stop = DateTime.Now;
            Console.WriteLine(file.Name + "\t" + trueBitDepth.ToString() + "\t" + (stop - start).TotalSeconds.ToString() + "s");
            File.Delete(fileGuid + ".pcm");
        }
        static void HelpInfo()
        {
            Console.WriteLine("[Usage]");
            Console.WriteLine("\tFor Single File: ");
            Console.WriteLine("\t\tBitDepthChecker D:\\1.flac");
            Console.WriteLine("\tFor Directory: ");
            Console.WriteLine("\t\tBitDepthChecker -d D:\\test");
        }
    }
}
