using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace clientCSharp 
{
    class Compiler
    {
        public string Compilation(string filename, string nameExeFile)
        {
            string source = "";
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        source += line + "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Файл не может быть прочитан:");
                Console.WriteLine(e.Message);
            }
            // Настройки компиляции
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v2.0");

            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.OutputAssembly = nameExeFile;
            compilerParams.GenerateExecutable = true;

            // Компиляция
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);
            return nameExeFile;
        }
    }

    class Tester
    {
        private string filename;

        public Tester(string filename)
        {
            this.filename = filename;
        }

        private Process PrepareStartProcess()
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            return process;
        }

        public bool StartOtherProcess(string input, string result)
        {
            Process process = PrepareStartProcess();
            using (StreamWriter writer = process.StandardInput)
            {
                writer.WriteLine(input);
                writer.Close();
            }

            StreamReader reader = process.StandardOutput;
            string end = reader.ReadLine();

            process.WaitForExit();
            process.Close();

            return end.Equals(result);
        }

        public bool StartOtherProcess(string[] input, string result)
        {
            Process process = PrepareStartProcess();
            using (StreamWriter writer = process.StandardInput)
            {
                foreach (var data in input)
                {
                    writer.WriteLine(data);
                }
                writer.Close();
            }

            StreamReader reader = process.StandardOutput;
            string end = reader.ReadLine();

            process.WaitForExit();
            process.Close();

            return end.Equals(result);
        }

        public bool StartOtherProcess(string[] input, string[] result)
        {
            Process process = PrepareStartProcess();
            using (StreamWriter writer = process.StandardInput)
            {
                foreach (var data in input)
                {
                    writer.WriteLine(data);
                }
                writer.Close();
            }

            StreamReader reader = process.StandardOutput;
            string[] end = new string[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                end[i] = reader.ReadLine();
            }

            process.WaitForExit();
            process.Close();
            return EqualsTest(result, end);
        }

        public bool StartOtherProcess(string input, string[] result)
        {
            Process process = PrepareStartProcess();
            using (StreamWriter writer = process.StandardInput)
            {
                writer.WriteLine(input);
                writer.Close();
            }

            StreamReader reader = process.StandardOutput;

            string[] end = new string[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                end[i] = reader.ReadLine();
            }
            process.WaitForExit();
            process.Close();

            return EqualsTest(result, end);
        }

        private bool EqualsTest(string[] a, string[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler();
            string fileExeName = compiler.Compilation("test.txt", "test.exe");
            Tester tester = new Tester(fileExeName);
            bool result = tester.StartOtherProcess("8", "24");
            Console.WriteLine(result);
            result = tester.StartOtherProcess("2", "6");
            Console.WriteLine(result);
            result = tester.StartOtherProcess("1", "3");
            Console.WriteLine(result);
            result = tester.StartOtherProcess("4", "12");
            Console.WriteLine(result);
            Console.WriteLine("Нажмите любую кнопку для выхода...");
            Console.ReadKey();
        }
    }
}
