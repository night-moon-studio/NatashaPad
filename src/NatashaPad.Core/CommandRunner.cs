using System;
using System.Diagnostics;

namespace NatashaPad
{
    public class CommandRunner
    {
        public static int Execute(string commandPath, string arguments = null, string workingDirectory = null)
        {
            var startInformation = CreateProcessStartInfo(commandPath, arguments, workingDirectory);
            using var process = CreateProcess(startInformation);
            RunAndWait(process);
            return process.ExitCode;
        }

        public static CommandResult Capture(string commandPath, string arguments, string workingDirectory = null)
        {
            var startInformation = CreateProcessStartInfo(commandPath, arguments, workingDirectory);
            using var process = CreateProcess(startInformation);
            process.Start();
            var standardOut = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return new CommandResult(process.ExitCode, standardOut, standardError);
        }

        private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments, string workingDirectory)
        {
            var startInformation = new ProcessStartInfo($"{commandPath}")
            {
                CreateNoWindow = true,
                Arguments = arguments ?? "",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
            };

            return startInformation;
        }

        private static void RunAndWait(Process process)
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        private static Process CreateProcess(ProcessStartInfo startInformation)
        {
            var process = new Process
            {
                StartInfo = startInformation
            };
            return process;
        }
    }

    public sealed class CommandResult
    {
        public CommandResult(int exitCode, string standardOut, string standardError)
        {
            ExitCode = exitCode;
            StandardOut = standardOut;
            StandardError = standardError;
        }

        public string StandardOut { get; }
        public string StandardError { get; }
        public int ExitCode { get; }

        public CommandResult EnsureSuccessfulExitCode(int success = 0)
        {
            if (ExitCode != success)
            {
                throw new InvalidOperationException(StandardError);
            }
            return this;
        }
    }
}