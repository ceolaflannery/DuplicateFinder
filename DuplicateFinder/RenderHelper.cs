using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DuplicateFinder
{
    public static class RenderHelper
    {
        internal static string RenderData(ConcurrentDictionary<string, List<string>> filesToBeCompared, StringBuilder errorsProcessingFiles)
        {
            var sb = new StringBuilder();
            foreach (var dups in filesToBeCompared)
            {
                if (dups.Value.Count == 1)
                    continue;

                sb.AppendLine("Multiples found:");
                foreach (var file in dups.Value)
                {
                    sb.AppendLine(file);
                }
                sb.AppendLine();
            }

            if (errorsProcessingFiles.Length > 0)
            {
                sb.AppendLine("The following errors were found when processing files:");
                sb.Append(errorsProcessingFiles);
            }
            return sb.ToString();
        }

    }
}
