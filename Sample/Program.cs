using System;
using System.IO;
using Utils;

namespace Sample
{
    class Program
    {
        private static void WriteSample(MemoryStream ms)
        {
            using var rs = new ProgressReportingStream(ms);
            rs.WriteProgress += Rs_WriteProgress;

            using var sw = new StreamWriter(rs, leaveOpen: true) { AutoFlush = true };
            sw.WriteLine("Hello");
            sw.WriteLine("ProgressStreaming");
            sw.WriteLine("World!");
        }
        static void ReadSample(Stream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            using var rs = new ProgressReportingStream(ms);
            rs.ReadProgress += Rs_ReadProgress;
            using var sr = new StreamReader(rs, leaveOpen: true);
            Console.WriteLine(sr.ReadLine());
            Console.WriteLine(sr.ReadLine());
            Console.WriteLine(sr.ReadLine());
        }

        private static void CopySample(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            using var reportingStream = new ProgressReportingStream(ms);
            reportingStream.ReadProgress += Rs_ReadProgress;
            using MemoryStream os = new MemoryStream();
            using var pros = new ProgressReportingStream(os);
            pros.WriteProgress += Rs_WriteProgress;
            reportingStream.CopyTo(pros);
        }

        static void Main(string[] args)
        {
            using var ms = new MemoryStream();

            DisplaySample(()=>WriteSample(ms), "Write sample");
            DisplaySample(()=>ReadSample(ms), "Read sample");
            DisplaySample(()=>CopySample(ms), "Copy sample");
        }

        private static void Rs_WriteProgress(Stream sender, long progress, long totalProgress)
        {
            Console.WriteLine($"Status: {progress} bytes written ({totalProgress} total)");
        }

        private static void Rs_ReadProgress(Stream sender, long progress, long totalProgress)
        {
            Console.WriteLine($"Status: {progress} bytes read ({totalProgress} total)");
        }
        static void DisplaySample(Action sample, string heading)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"[[{heading}]]");
            sample();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine();
        }
    }
}
