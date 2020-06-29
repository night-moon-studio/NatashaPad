using System;
using WeihanLi.Common.Helpers;

namespace NatashaPad.ReferenceResolver.Nuget
{
    internal static class NugetHelper
    {
        public static string GetGlobalPackagesFolder()
        {
            using var executor = new ProcessExecutor("dotnet", "nuget locals global-packages -l");
            var folder = string.Empty;
            executor.OnOutputDataReceived += (sender, str) =>
            {
                if (str is null)
                    return;

                Console.WriteLine(str);

                if (str.StartsWith("global-packages:"))
                {
                    folder = str.Substring("global-packages:".Length).Trim();
                }
            };
            executor.Execute();

            return folder;
        }
    }
}