# Process Monitoring Utility

## A simple tool for monitoring and automatically terminating Windows processes exceeding specified maximum runtime.

The Process Monitoring Utility manages and monitors Windows processes based on user-defined criteria. It is designed to automatically terminate processes that exceed a specified maximum runtime, ensuring system stability and performance. This utility is particularly useful in environments where processes may hang or consume excessive resources without supervision.

### How to Run the Utility
To use the Process Monitoring Utility, follow these steps:

1. **Clone the repository**
2. **Prepare Your Environment**
   - Ensure you have .NET Framework or .NET Core installed on your system.
   - Run with administrator rights if you wish to monitor processes from other users.
3. **Run the Utility**
   - Open a command prompt or open the solution in Visual Studio.
   - Usage:
     ```
     ProcessMonitoringUtility.exe <process_name> <max_lifetime_in_minutes> <monitor_frequency_in_minutes>
     ```
   - Example usage:
     ```
     ProcessMonitoringUtility.exe notepad 5 1
     ```
4. **Terminate the Utility**
   - To stop the utility, press 'q' in the command prompt where it is running.

The development of the Process Monitoring Utility includes only end-to-end testing with NUnit, as unit testing would rely heavily on testing methods from System.
