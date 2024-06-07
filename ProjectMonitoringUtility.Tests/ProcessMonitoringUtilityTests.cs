using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using System.IO;
using NUnit.Framework.Legacy;

[TestFixture]
public class ProcessMonitorIntegrationTests
{
    [Test]
    public void Test_MonitorCantFindProcess()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe",
            Arguments = "prc 1 1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        // Act
        using (var process = Process.Start(startInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            // Assert
            ClassicAssert.IsTrue(output.Contains("No processes named prc found."));
        }
    }

    [Test]
    public void Test_MonitorKillsProcess_NotEnoughPermissions()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe",
            Arguments = "csrss 1 1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        // Act
        using (var process = Process.Start(startInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            // Assert
            ClassicAssert.IsTrue(output.Contains("Exception occurred: Access is denied."));
        }
    }

    [Test]
    public void Test_MonitorKillsProcess_WhenExceedingMaxLifetime()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe",
            Arguments = "notepad 1 1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        var dummyProcess = Process.Start("notepad");

        // Act
        using (var process = Process.Start(startInfo))
        {
            Thread.Sleep(70 * 1000);
            // Assert
            ClassicAssert.IsTrue(dummyProcess.HasExited, "The monitored process should have been killed but was not.");
        }
    }

    [Test]
    public void Test_InvalidNumberOfArguments()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe", 
            Arguments = "notepad 1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // Act
        using (var process = Process.Start(startInfo))
        {
            if (process != null)
            {
                process.WaitForExit(1000);

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Assert
                ClassicAssert.IsTrue(output.Contains("Invalid arguments count. Usage: ProcessMonitoringUtility.exe <process_name> <max_lifetime_in_minutes> <monitor_frequency_in_minutes>"));
                ClassicAssert.IsTrue(process.ExitCode != 0, "Process should exit with an error code due to invalid arguments.");
            }
            else
            {
                Assert.Fail("Process did not start.");
            }
        }
    }

    [Test]
    public void Test_Frequency_InvalidType()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe",
            Arguments = "notepad one one",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // Act
        using (var process = Process.Start(startInfo))
        {
            if (process != null)
            {
                process.WaitForExit(1000);

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Assert
                ClassicAssert.IsTrue(output.Contains("Invalid arguments. Please ensure that lifetime and frequency are integers."));
                ClassicAssert.IsTrue(process.ExitCode != 0, "Process should exit with an error code due to invalid arguments.");
            }
            else
            {
                Assert.Fail("Process did not start.");
            }
        }
    }

    [Test]
    public void Test_MaxRuntime_InvalidType()
    {
        // Arrange
        var startInfo = new ProcessStartInfo
        {
            FileName = "ProcessMonitoringUtility.exe",
            Arguments = "notepad one one",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // Act
        using (var process = Process.Start(startInfo))
        {
            if (process != null)
            {
                process.WaitForExit(1000);

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Assert
                ClassicAssert.IsTrue(output.Contains("Invalid arguments. Please ensure that lifetime and frequency are integers."));
                ClassicAssert.IsTrue(process.ExitCode != 0, "Process should exit with an error code due to invalid arguments.");
            }
            else
            {
                Assert.Fail("Process did not start.");
            }
        }
    }

    [TearDown]
    public void Cleanup()
    {
        foreach (var proc in Process.GetProcessesByName("notepad"))
        {
            proc.Kill();
        }
    }
}