/*
 
using System.Security.Principal;

var identity = WindowsIdentity.GetCurrent();
Console.WriteLine($"Running as: {identity.Name}");
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        //  Seriously? How did we not see this? Not only was the think missing the "users" portion,
        //  we were using '/' in place of '\'! No wonder it kept failing at run time! What is this,
        //  amature hour? Remains untested as of 08Aug2025.
        string sourceFolder = @"C:\Users\jhamptonsa\Documents\Startup";
        string[] appNames = GetAppPaths(sourceFolder);
        string[] lnkNames = GetShortcutPaths(sourceFolder);

        // Run all apps in target folder.
        runApps(appNames);
    }

    // Stops the current instances of an app (if applicable)
    // TODO: When this gets transfered to Dullahan, we need to use the process ID instead of just its name.
    // Right now, LinchPin needs to just run once, and we're not recording the IDs anywhere, so there is no way
    // for us to have a new instance kill the processes. Especially if they were manually launched.
    static void killApp(string appName)
    {
        string processName = appName; // Without extension

        // Make sure the provided name or path is valid.
        if (string.IsNullOrEmpty(appName))
        {
            return;
        }

        // Get just the processes name.
        processName = Path.GetFileNameWithoutExtension(appName);

        foreach (Process process in Process.GetProcessesByName(processName))
        {
            try { process.Kill(); } catch { }
            process.WaitForExit(); // [Technically] Optional: wait for the process to exit
        }
    }

    // Run all apps from list given
    static void runApps(string[] appNames)
    {
        foreach (string appName in appNames)
        {
            // Make sure the provided name or path is valid.
            if (string.IsNullOrEmpty(appName))
            {
                // <Future log statement>
                continue;
            }
            else if (!appName.Contains("."))
            {
                // <Future log statement>
                continue;
            }

            // TODO: See above, we need to stop using the process name. Really, this
            // should be replaced with a call to killApp, with a possible overload for dealing
            // with a list of names.

            // Make sure teh app isn't already running.
            string processName = Path.GetFileNameWithoutExtension(appName);
            killApp(processName);

            ProcessStartInfo startInfo = new ProcessStartInfo(appName)
            {
                UseShellExecute = true // Important for launching .lnk files
            };

            try { Process.Start(startInfo); } catch { }
        }
    }

    // Overload for single string/name.
    static void runApp(string appName)
    {
        // Make sure the provided name or path is valid.
        if (string.IsNullOrEmpty(appName))
        {
            // <Future log statement>
            return;
        }
        else if (!appName.Contains("."))
        {
            // <Future log statement>
            return;
        }

        // TODO: See above, we need to stop using the process name. Really, this
        // should be replaced with a call to killApp, with a possible overload for dealing
        // with a list of names.

        // Make sure the app isn't already running.
        string processName = Path.GetFileNameWithoutExtension(appName);
        killApp(processName);

        ProcessStartInfo startInfo = new ProcessStartInfo(appName)
        {
            UseShellExecute = true // Important for launching .lnk files
        };

        Process.Start(startInfo);
    }

    // TODO: These could probably get replaced with a single function
    // that takes a second input of the desired extention.

    // Returns the full path to all files ending in .exe in sourceFolder
    static string[] GetAppPaths(string sourceFolder)
    {
        //If folder doesn't exist, return empty string
        if (!Directory.Exists(sourceFolder))
            return Array.Empty<string>();

        //get all .exe files in the folder
        string[] appNames = Directory.GetFiles(sourceFolder, "*.exe");

        return appNames;
    }

    // Returns the full path to all files ending in .lnk in sourceFolder
    static string[] GetShortcutPaths(string sourceFolder)
    {
        //If folder doesn't exist, return empty string
        if (!Directory.Exists(sourceFolder))
            return Array.Empty<string>();

        //get all .exe files in the folder
        string[] appNames = Directory.GetFiles(sourceFolder, "*.lnk");

        return appNames;
    }
}