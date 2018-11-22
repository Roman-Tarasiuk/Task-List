using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProgramArgs;

namespace TaskList
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.Run(args);
        }

        private static void Run(string[] args)
        {
            var argsObj = Args.ParseArgs(args);
            
            if (argsObj.Help == true)
            {
                ShowUsage();
                return;
            }
        
            try
            {
                Console.WriteLine("* Gathering info...");
                Program.LaunchTheSystemTaskList(argsObj);
                // Process.Start("cmd", " /c chcp 1251 & tasklist /v >tasks.txt").WaitForExit();

                Console.WriteLine("* Processing the task list...");
                ProcessTaskList(argsObj);

                Console.WriteLine("* Successfully finished.");
                Console.WriteLine("\n(Memory size is in kilobytes.)");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Run Error:");
                Console.WriteLine(exception);
            }
        }

        // https://stackoverflow.com/questions/9679375/run-an-exe-from-c-sharp-code
        private static void LaunchTheSystemTaskList(Args argsObj)
        {
            var encoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.GetEncoding(1251);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            
            if (argsObj.CreateNoWindow != null) startInfo.CreateNoWindow = (bool)argsObj.CreateNoWindow;
            if (argsObj.UseShellExecute != null) startInfo.UseShellExecute = (bool)argsObj.UseShellExecute;
            
            Console.WriteLine("ProcessStartInfo startInfo:\n  CreateNoWindow: {0}, UseShellExecute: {1}",
                                startInfo.CreateNoWindow,
                                startInfo.UseShellExecute);

            startInfo.FileName = "cmd";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = " /c chcp 1251 & tasklist "
                                + (argsObj.V == true ? "/v " : "")
                                + ">tasks.txt";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                    
                    if (exeProcess.ExitCode != 0)
                    {
                        throw new Exception("** Command run failed. (Maybe cannot create/write to the 'tasks.txt' file.)");
                    }
                }
            }
            catch
            {
                throw;
            }

            Console.OutputEncoding = encoding;
        }

        private static void ProcessTaskList(Args argsObj)
        {
            string fileContent = null;
            using (StreamReader reader = new StreamReader(Path.Combine("tasks.txt"),
                   Encoding.GetEncoding(1251)))
            {
                fileContent = reader.ReadToEnd();
            }

            if ((fileContent != null) && (fileContent != String.Empty))
            {
                var rows = fileContent.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                var rowRe = new Regex("(.{64})(.{12})(.*)");
                
                var list = new List<Tuple<string, int>>();

                var header = "";
                
                for (var i = 0; i < rows.Length; i++)
                {
                    if (i == 1) // The Divide String: ========================= ======== ================ ===========
                    {
                        Console.WriteLine(rows[i]);
                        continue;
                    }
                
                    var match = rowRe.Match(rows[i]);
                    var size = match.Groups[2].ToString()
                                    .Replace(" КБ", String.Empty)
                                    .Replace(" K", String.Empty)
                                    .Replace(" ", String.Empty)
                                    .Replace(",", String.Empty);
                    size = string.Format("{0,23}", size);
                    
                    if (i == 0)
                    {
                        header = match.Groups[1] + size + match.Groups[3];
                    }
                    else
                    {
                        list.Add(Tuple.Create(match.Groups[1] + size + match.Groups[3], int.Parse(size)));
                    }
                }

                if (argsObj.Sort == Sort.Name)
                {
                    list.Sort((x, y) => {
                        return x.Item1.CompareTo(y.Item1);
                    });
                }
                else if (argsObj.Sort == Sort.MemorySize)
                {
                    list.Sort((x, y) => {
                        return x.Item2.CompareTo(y.Item2);
                    });
                }
                else if (argsObj.Sort == Sort.MemorySizeDescending)
                {
                    list.Sort((x, y) => {
                        return -x.Item2.CompareTo(y.Item2);
                    });
                }

                using (StreamWriter writer = new StreamWriter(Path.Combine("tasks.txt")))
                {
                    writer.WriteLine(header);

                    for (var i = 0; i < list.Count; i++)
                    {                        
                        writer.WriteLine(list[i].Item1);
                    }

                    writer.WriteLine();
                    writer.WriteLine("========================= ======== ================ =========== =SUBTOTAL(9;R2C:R[-2]C)");
                    writer.WriteLine("========================= ======== ================ =========== =R[-1]C/1024");
                    writer.WriteLine("========================= ======== ================ =========== =R[-1]C/1024");
                }            
            }
        }
        
        private static void ShowUsage()
        {
            Console.WriteLine("** TaskList Usage:");
            Console.WriteLine("   TaskList_.exe [--V] [--CreateNoWindow true|false] [--UseShellExecute true|false]");
            Console.WriteLine("                 [--OrderBy Name|MemorySize [Descending]]");
            Console.WriteLine("   TaskList_.exe ?|/?|-h|-help|--help");
            Console.WriteLine("       Displays usage.");
        }
    }
}