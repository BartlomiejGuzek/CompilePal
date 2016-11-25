﻿using CompilePalX.Compiling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CompilePalX.Compilers
{
    class NavProcess : CompileProcess
    {
        public NavProcess() : base("Parameters\\BuiltIn\\nav.meta") { }

        static string mapname;
        static string mapnav;
        static string mapcfg;
        static string mapCFGBackup;

        bool hidden;

        public override void Run(CompileContext context)
        {
            try
            {

                CompilePalLogger.LogLine("\nCompilePal - Nav Generator");

                if (!File.Exists(context.CopyLocation))
                {
                    throw new FileNotFoundException();
                }

                mapname = System.IO.Path.GetFileName(context.CopyLocation).Replace(".bsp", "");
                mapnav = context.CopyLocation.Replace(".bsp", ".nav");
                mapcfg = context.Configuration.GameFolder + "/cfg/" + mapname + ".cfg";
                mapCFGBackup = context.Configuration.GameFolder + "/cfg/" + mapname + "_cpalbackup.cfg";

                deleteNav(mapname, context.Configuration.GameFolder);

                hidden = GetParameterString().Contains("-hidden");

                string args = "-game \"" + context.Configuration.GameFolder + "\" -windowed -novid -nosound +sv_cheats 1 +map " + mapname;

                if (hidden)
                    args += " -noborder -x 4000 -y 2000";

                var startInfo = new ProcessStartInfo(context.Configuration.GameEXE, args);
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = false;

                CompilePalLogger.LogLine("Generating...");
                if (File.Exists(mapcfg))
                {
                    if (File.Exists(mapCFGBackup))
                        System.IO.File.Delete(mapCFGBackup);
                    System.IO.File.Move(mapcfg, mapCFGBackup);
                }

                System.IO.File.Create(mapcfg).Dispose();
                TextWriter tw = new StreamWriter(mapcfg);
                tw.WriteLine("nav_generate");
                tw.Close();

                Process = new Process { StartInfo = startInfo };
                Process.Start();

                FileSystemWatcher fw = new FileSystemWatcher();
                fw.Path = System.IO.Path.GetDirectoryName(mapnav);
                fw.Filter = "*.nav";
                fw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fw.Changed += new FileSystemEventHandler(fileSystemWatcher_NavCreated);
                fw.Created += new FileSystemEventHandler(fileSystemWatcher_NavCreated);
                fw.EnableRaisingEvents = true;

                Process.WaitForExit();
                fw.Dispose();

                cleanUp();
                CompilePalLogger.LogLine("nav file complete!");
            }
            catch (FileNotFoundException)
            {
                CompilePalLogger.LogLine("FAILED - Could not find " + context.CopyLocation);
            }
            catch (Exception exception)
            {
                CompilePalLogger.LogLine("Something broke:");
                CompilePalLogger.LogLine(exception.ToString());
            }
        }

        private void deleteNav(string mapname, string gamefolder)
        {
            List<string> navdirs = BSPPack.BSPPack.GetSourceDirectories(gamefolder, false);
            foreach (string source in navdirs)
            {
                string externalPath = source + "/maps/" + mapname + ".nav";

                if (File.Exists(externalPath))
                {
                    CompilePalLogger.LogLine("Deleting existing nav file.");
                    File.Delete(externalPath);
                }
            }
        }

        private void exitClient()
        {
            if (Process != null)
                try
                {
                    this.Process.Kill();
                }
                catch (Win32Exception) { }
        }
        private void cleanUp()
        {
            if (File.Exists(mapcfg))
                File.Delete(mapcfg);
            if (File.Exists(mapCFGBackup))
                System.IO.File.Move(mapCFGBackup, mapcfg);
        }

        public override void Cancel()
        {
            cleanUp();
        }

        void fileSystemWatcher_NavCreated(object sender, FileSystemEventArgs e)
        {
            exitClient();
        }
    }
}
