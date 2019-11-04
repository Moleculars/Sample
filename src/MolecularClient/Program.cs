using Microsoft.Extensions.CommandLineUtils;
using MolecularClient.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;

namespace MolecularClient
{


    public partial class Program
    {
        static void Main(string[] args)
        {

            new Program(args);
            Environment.Exit(0);

        }

        private Program(string[] args)
        {
            CommandLineApplication app = null;
            try
            {

                app = new CommandLineApplication()
                    .Initialize()
                    .CommandPushworkflow()
                ;

                if (Output.DebugModeActivated)
                    Output.WriteLine(string.Join(' ', args));

                int result = app.Execute(args);

                if (result == 0)
                    Helper.Save();

                else if (result == 1)
                    app.ShowHelp();

                Output.Flush();

                Environment.ExitCode = Program.ExitCode = result;

            }
            catch (System.FormatException e2)
            {
                FormatException(app, e2);
            }
            catch (AuthenticationException)
            {
                AuthorizeException();
            }
            catch (ExpiratedTokenException)
            {
                ExpirationException();
            }
            catch (AggregateException e4)
            {
                foreach (var item in e4.InnerExceptions)
                {
                    if (item is AuthenticationException)
                        AuthorizeException();
                    else
                        Exception(item);
                }
            }
            catch (Exception e)
            {
                Exception(e);
            }
        }


        public static int ExitCode { get; private set; }


        private static void Exception(Exception e)
        {
            Output.ErrorWriteLine(e.Message);
            Output.ErrorWriteLine(e.StackTrace);
            Output.Flush();

            if (e.HResult > 0)
                Environment.ExitCode = Program.ExitCode = e.HResult;

            Environment.ExitCode = Program.ExitCode = 1;
        }


        private static void ExpirationException()
        {
            Output.ErrorWriteLine("token expirated. please considere must be authenticated for access on the server.");
            Output.Flush();
            Environment.ExitCode = Program.ExitCode = 1;
        }


        private static void AuthorizeException()
        {
            Output.ErrorWriteLine("Not authenticated or not enough right to access this feature");
            Output.Flush();
            Environment.ExitCode = Program.ExitCode = 1;
        }


        private static void FormatException(CommandLineApplication app, FormatException e2)
        {
            Output.ErrorWriteLine(e2.Message);
            Output.Flush();
            app.ShowHelp();
            Environment.ExitCode = Program.ExitCode = 2;
        }

    }

    public static class Helper
    {

        static Helper()
        {
            var _path = new FileInfo(typeof(Helper).Assembly.Location).Directory.FullName;
            filename = Path.Combine(_path, "mem.json");
        }

        public static void Load()
        {

            if (File.Exists(filename))
            {
                var txt = File.ReadAllText(filename);
                Parameters = JsonConvert.DeserializeObject<Parameters>(txt);
            }
            else
                Parameters = new Parameters();

            if (!ValidatorExtension.CheckToken())
                Helper.Parameters.Token = null;

        }

        public static void Save()
        {
            string txt = JsonConvert.SerializeObject(Parameters);
            File.WriteAllLines(filename, new List<string>() { txt }, Encoding.UTF8);
        }

        public static Parameters Parameters { get; set; }


        public static string LoadContentFromFile(string _path)
        {

            string fileContents = string.Empty;
            System.Text.Encoding encoding = null;
            FileInfo _file = new FileInfo(_path);

            using (FileStream fs = _file.OpenRead())
            {

                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();
                if (cdet.Charset != null)
                    encoding = System.Text.Encoding.GetEncoding(cdet.Charset);
                else
                    encoding = System.Text.Encoding.UTF8;

                fs.Position = 0;

                byte[] ar = new byte[_file.Length];
                fs.Read(ar, 0, ar.Length);
                fileContents = encoding.GetString(ar);
            }

            if (fileContents.StartsWith("ï»¿"))
                fileContents = fileContents.Substring(3);

            if (encoding != System.Text.Encoding.UTF8)
            {
                var datas = System.Text.Encoding.UTF8.GetBytes(fileContents);
                fileContents = System.Text.Encoding.UTF8.GetString(datas);
            }

            return fileContents;

        }


        public static string LoadContentFromText(byte[] text)
        {

            string textContents = string.Empty;
            System.Text.Encoding encoding = null;

            using (MemoryStream fs = new MemoryStream(text))
            {

                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();
                if (cdet.Charset != null)
                    encoding = System.Text.Encoding.GetEncoding(cdet.Charset);
                else
                    encoding = System.Text.Encoding.UTF8;

                fs.Position = 0;

                byte[] ar = new byte[text.Length];
                fs.Read(ar, 0, ar.Length);
                textContents = encoding.GetString(ar);
            }

            if (textContents.StartsWith("ï»¿"))
                textContents = textContents.Substring(3);

            if (encoding != System.Text.Encoding.UTF8)
            {
                var datas = System.Text.Encoding.UTF8.GetBytes(textContents);
                textContents = System.Text.Encoding.UTF8.GetString(datas);
            }

            return textContents;

        }


        public static string SerializeContract(this string self)
        {

            if (string.IsNullOrEmpty(self))
                return string.Empty;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(self);
            var result = Convert.ToBase64String(bytes);
            return result;
        }


        public static string DeserializeContract(this string self)
        {

            if (string.IsNullOrEmpty(self))
                return string.Empty;

            byte[] bytes = Convert.FromBase64String(self); ;
            string result = System.Text.Encoding.UTF8.GetString(bytes);

            return result;

        }


        private static readonly string filename;

    }

}
