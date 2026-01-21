using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace MiniSectorsUpdater
{
    class Program
    {
        private const int MaxWaitSeconds = 30;
        private const int PollIntervalMs = 500;

        static int Main(string[] args)
        {
            try
            {
                var options = ParseArgs(args);
                if (options == null)
                {
                    PrintUsage();
                    return 1;
                }

                Console.WriteLine("MiniSectors Updater");
                Console.WriteLine("===================");
                Console.WriteLine();

                // Step 1: Wait for SimHub to exit
                if (!WaitForSimHubExit(options.SimHubPath))
                {
                    return 1;
                }

                // Step 2: Verify downloaded DLL hash
                if (!string.IsNullOrEmpty(options.ExpectedHash))
                {
                    Console.WriteLine("Verifying download integrity...");
                    var actualHash = ComputeSHA256(options.DllPath);
                    if (!actualHash.Equals(options.ExpectedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"ERROR: Hash mismatch!");
                        Console.WriteLine($"  Expected: {options.ExpectedHash}");
                        Console.WriteLine($"  Actual:   {actualHash}");
                        Console.WriteLine("Update aborted. The downloaded file may be corrupted.");
                        WaitForKeyPress();
                        return 1;
                    }
                    Console.WriteLine("Hash verified OK.");
                }

                // Step 3: Backup existing DLL
                var backupPath = options.TargetPath + ".bak";
                if (File.Exists(options.TargetPath))
                {
                    Console.WriteLine($"Backing up existing DLL to {Path.GetFileName(backupPath)}...");
                    try
                    {
                        if (File.Exists(backupPath))
                        {
                            File.Delete(backupPath);
                        }
                        File.Move(options.TargetPath, backupPath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Access denied. Attempting with elevation...");
                        return RelaunchElevated(args);
                    }
                }

                // Step 4: Copy new DLL
                Console.WriteLine("Installing new version...");
                try
                {
                    File.Copy(options.DllPath, options.TargetPath, overwrite: true);

                    // Also copy PDB if present
                    var sourcePdb = Path.ChangeExtension(options.DllPath, ".pdb");
                    var targetPdb = Path.ChangeExtension(options.TargetPath, ".pdb");
                    if (File.Exists(sourcePdb))
                    {
                        File.Copy(sourcePdb, targetPdb, overwrite: true);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Access denied. Attempting with elevation...");
                    // Restore backup before relaunching
                    if (File.Exists(backupPath) && !File.Exists(options.TargetPath))
                    {
                        File.Move(backupPath, options.TargetPath);
                    }
                    return RelaunchElevated(args);
                }

                Console.WriteLine("Update installed successfully.");

                // Step 5: Clean up temp files
                Console.WriteLine("Cleaning up...");
                try
                {
                    File.Delete(options.DllPath);
                    var sourcePdb = Path.ChangeExtension(options.DllPath, ".pdb");
                    if (File.Exists(sourcePdb))
                    {
                        File.Delete(sourcePdb);
                    }
                }
                catch
                {
                    // Ignore cleanup failures
                }

                // Step 6: Restart SimHub
                Console.WriteLine("Starting SimHub...");
                Process.Start(options.SimHubPath);

                Console.WriteLine();
                Console.WriteLine("Update complete!");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                WaitForKeyPress();
                return 1;
            }
        }

        private static bool WaitForSimHubExit(string simhubPath)
        {
            var processName = Path.GetFileNameWithoutExtension(simhubPath);
            var processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine("SimHub is not running.");
                return true;
            }

            Console.WriteLine("Waiting for SimHub to close...");

            // Request graceful close via WM_CLOSE
            foreach (var proc in processes)
            {
                try
                {
                    proc.CloseMainWindow();
                }
                catch
                {
                    // Ignore errors
                }
            }

            // Wait for process to exit
            var waited = 0;
            while (waited < MaxWaitSeconds * 1000)
            {
                Thread.Sleep(PollIntervalMs);
                waited += PollIntervalMs;

                processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    Console.WriteLine("SimHub closed.");
                    return true;
                }

                // Show progress
                Console.Write($"\rWaiting... ({waited / 1000}s)   ");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SimHub hasn't closed after 30 seconds.");
            Console.Write("Force close SimHub? (Y/N): ");

            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("Forcing SimHub to close...");
                processes = Process.GetProcessesByName(processName);
                foreach (var proc in processes)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch
                    {
                        // Ignore errors
                    }
                }

                // Give it a moment
                Thread.Sleep(1000);
                return true;
            }
            else
            {
                Console.WriteLine("Update cancelled.");
                return false;
            }
        }

        private static string ComputeSHA256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static int RelaunchElevated(string[] args)
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var arguments = string.Join(" ", args.Select(a => a.Contains(" ") ? $"\"{a}\"" : a));

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                var process = Process.Start(startInfo);
                process?.WaitForExit();
                return process?.ExitCode ?? 1;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Console.WriteLine("Elevation cancelled. Update aborted.");
                WaitForKeyPress();
                return 1;
            }
        }

        private static void WaitForKeyPress()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static UpdateOptions ParseArgs(string[] args)
        {
            var options = new UpdateOptions();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--simhub":
                        if (i + 1 < args.Length)
                            options.SimHubPath = args[++i];
                        break;
                    case "--dll":
                        if (i + 1 < args.Length)
                            options.DllPath = args[++i];
                        break;
                    case "--target":
                        if (i + 1 < args.Length)
                            options.TargetPath = args[++i];
                        break;
                    case "--hash":
                        if (i + 1 < args.Length)
                            options.ExpectedHash = args[++i];
                        break;
                }
            }

            if (string.IsNullOrEmpty(options.SimHubPath) ||
                string.IsNullOrEmpty(options.DllPath) ||
                string.IsNullOrEmpty(options.TargetPath))
            {
                return null;
            }

            return options;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("MiniSectors Updater");
            Console.WriteLine();
            Console.WriteLine("Usage: MiniSectorsUpdater.exe --simhub <path> --dll <path> --target <path> [--hash <sha256>]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  --simhub  Path to SimHubWPF.exe");
            Console.WriteLine("  --dll     Path to the new DLL file");
            Console.WriteLine("  --target  Path where the DLL should be installed");
            Console.WriteLine("  --hash    (Optional) SHA256 hash of the new DLL to verify");
            WaitForKeyPress();
        }
    }

    class UpdateOptions
    {
        public string SimHubPath { get; set; }
        public string DllPath { get; set; }
        public string TargetPath { get; set; }
        public string ExpectedHash { get; set; }
    }
}
