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
                
                switch (arg)
                {
                    case "createnowindowtrue":
                        result.CreateNoWindow = true;
                        break;
                    case "createnowindowfalse":
                        result.CreateNoWindow = false;
                        break;
                    case "useshellexecutetrue":
                        result.UseShellExecute = true;
                        break;
                    case "useshellexecutefalse":
                        result.UseShellExecute = false;
                        break;
                    case "v":
                        result.V = true;
                        break;
                    case "orderbyname":
                        result.Sort = Sort.Name;
                        break;
                    case "orderbymemorysize":
                        result.Sort = Sort.MemorySize;
                        break;
                    case "orderbymemorysizedescending":
                        result.Sort = Sort.MemorySizeDescending;
                        break;
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