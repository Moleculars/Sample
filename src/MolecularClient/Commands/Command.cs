using Bb.Option;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MolecularClient.Commands
{

    public static partial class Command
    {

        static Command()
        {

            //Command._access = "('" + string.Join("','", Enum.GetNames(typeof(AccessModuleEnum))) + "')";

        }


        /// <summary>
        /// Initializes the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static CommandLineApplication Initialize(this CommandLineApplication app)
        {

            Helper.Load();

            AnsiConsole.GetError(true);

            app.HelpOption(HelpFlag);
            app.VersionOption(VersionFlag, Constants.ShortVersion, Constants.LongVersion);

            app.Name = Constants.Name;
            app.Description = Constants.ProgramHelpDescription;
            app.ExtendedHelpText = Constants.ExtendedHelpText;

            return app;

        }

        //private static AccessModuleEnum Read(CommandOption cmd)
        //{
        //    return (AccessModuleEnum)Enum.Parse(typeof(AccessModuleEnum),
        //        cmd.Value()
        //        .Trim()
        //        .Trim('\'', '"')
        //        .Trim()
        //    );
        //}

        public static BbClientHttp Client => new BbClientHttp(new Uri(Helper.Parameters.ServerUrl));

        public static object Result { get; internal set; }

        public const string HelpFlag = "-? |-h |--help";
        public const string VersionFlag = "-v |--version";
        public static string _access;


    }

}
