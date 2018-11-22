using System;

namespace ProgramArgs
{
    public enum Sort
    {
        None,
        Name,
        MemorySize,
        MemorySizeDescending
    }

    public class Args
    {
        public bool? CreateNoWindow { get; protected set; }
        public bool? UseShellExecute { get; protected set; }
        public bool? V { get; protected set; }
        public bool? Help { get; protected set; }
        public Sort Sort { get; set; }

        protected Args()
        {
            this.Sort = Sort.None;
        }
        
        public static Args ParseArgs(string[] args)
        {
            Args result = new Args();
            
            if (args.Length == 1)
            {
                if (args[0] == "?"
                 || args[0] == "/?"
                 || args[0] == "-h"
                 || args[0] == "-help"
                 || args[0] == "--help")
                 {
                    result.Help = true;
                    return result;
                 }
            }
            
            var joined = string.Join(" ", args).Trim().ToLower();
            joined = Clean(joined);
            
            var programArgs = joined.Split(new string[] {"--"}, StringSplitOptions.RemoveEmptyEntries);
            
            for (var i = 0; i < programArgs.Length; i++)
            {
                var arg = programArgs[i];
                arg = arg.Replace(" ", "").ToLower();
                
                if (arg == "createnowindowtrue")
                {
                    result.CreateNoWindow = true;
                }
                else if (arg == "createnowindowfalse")
                {
                    result.CreateNoWindow = false;
                }
                else if (arg == "useshellexecutetrue")
                {
                    result.UseShellExecute = true;
                }
                else if (arg == "useshellexecutefalse")
                {
                    result.UseShellExecute = false;
                }
                else if (arg == "v")
                {
                    result.V = true;
                }
                else if (arg == "orderbyname")
                {
                    result.Sort = Sort.Name;
                }
                else if (arg == "orderbymemorysize")
                {
                    result.Sort = Sort.MemorySize;
                }
                else if (arg == "orderbymemorysizedescending")
                {
                    result.Sort = Sort.MemorySizeDescending;
                }
            }
            
            return result;
        }
        
        private static string Clean(string str)
        {
            str = str.Replace("\t", " ");
            
            var currentLength = 0;
            
            do
            {
                currentLength = str.Length;
                str = str.Replace("  ", " ");
            }
            while (str.Length != currentLength);
            
            return str;
        }
    }
}