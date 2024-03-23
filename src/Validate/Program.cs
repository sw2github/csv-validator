﻿
namespace FormatValidator
{
    using System;
    using System.Collections.Generic;
    using CommandLine;
    using Newtonsoft.Json;
    using Validate;

    /// <summary>
    /// Main application entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main application entry point
        /// </summary>
        /// <param name="args">Application aruments</param>
        public static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Run(options));
        }

        internal static void Run(Options options)
        {
            ConsoleUserInterface ui = new ConsoleUserInterface();
            List<RowValidationError> errors = new List<RowValidationError>();

            ui.ShowStart();

            DateTime start = DateTime.Now;
            Validator validator = Validator.FromJson(System.IO.File.ReadAllText(options.With));
            FileSourceReader source = new FileSourceReader(options.File);

            foreach (RowValidationError current in validator.Validate(source))
            {
                errors.Add(current);
                ui.ReportRowError(current);
            }

            DateTime end = DateTime.Now;

            ui.ShowSummary(validator, errors, end.Subtract(start));
        }
    }
}
