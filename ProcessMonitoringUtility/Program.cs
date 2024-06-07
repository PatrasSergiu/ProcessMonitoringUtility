using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

class Program
{
    private static string processName { get; set; }
    private static int maxLifetime;
    private static int frequency;

    static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/monitorLogs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        if (args.Length != 3)
        {
            Console.WriteLine("Invalid arguments count. Usage: ProcessMonitoringUtility.exe <process_name> <max_lifetime_in_minutes> <monitor_frequency_in_minutes>");
            return 1;
        }
        processName = args[0];
        if (!int.TryParse(args[1], out maxLifetime) || !int.TryParse(args[2], out frequency))
        {
            Console.WriteLine("Invalid arguments. Please ensure that lifetime and frequency are integers.");
            return 1;
        }
        Log.Information($"Monitoring {processName} every {frequency} minutes. Will kill if running longer than {maxLifetime} minutes.");
        using (Timer timer = new Timer(Check, null, TimeSpan.Zero, TimeSpan.FromMinutes(frequency)))
        {
            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey(true).KeyChar != 'q')
            {
                Thread.Sleep(100);
            }
            Log.Information("Utility stopped from keyboard");
        }
        Log.CloseAndFlush();
        return 0;
    }
    static void Check(object state)
    {
        try
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Log.Error($"No processes named {processName} found.");
                Console.WriteLine($"No processes named {processName} found.");
                return;
            }
            foreach (var proc in processes)
            {
                var runtime = DateTime.Now - proc.StartTime;
                Log.Information($"Checking process {proc.ProcessName} with ID {proc.Id}, runtime: {runtime.TotalMinutes} minutes.");
                if (runtime.TotalMinutes > maxLifetime)
                {
                    try
                    {
                        Log.Information($"Killing process {proc.ProcessName} with ID {proc.Id}, running for {runtime.TotalMinutes} minutes.");
                        Kill(proc);
                        Log.Information($"Process {proc.ProcessName} with ID {proc.Id} killed after {runtime.TotalMinutes} minutes");
                    }
                    catch (Win32Exception ex)
                    {
                        Log.Error($"Insufficient permissions to kill process {proc.ProcessName} with ID {proc.Id}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            Log.Fatal($"Exception occurred: {ex.Message}");
        }
    }

    static void Kill(Process process)
    {
        process.Kill();
    }
}