using System;
using System.Collections.Generic;
using System.IO;
using CompilePalX.Compiling;

namespace CompilePalX
{
    class GameConfigurationParser
    {
        public static List<GameConfiguration> Parse(string filename)
        {
            CompilePalLogger.LogLine("Parsing game configuration file {0}",filename);

            var lines = File.ReadAllLines(filename);
	        var gameInfoDir = Path.GetDirectoryName(filename);
            //not as lazy parsing! woo!

            var gameInfos = new List<GameConfiguration>();
            for (int i = 4; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line == "	}" || line == "        }")
                    break;

                var game = new GameConfiguration { Name = GetKey(line) };

                game.BinFolder = Path.GetDirectoryName(filename);

                CompilePalLogger.LogLine("Loading new game configuration:");
                CompilePalLogger.LogLine(GetKey(line));

                i++;
                for (; i < lines.Length; i++)
                {
                    line = lines[i];
                    if (IsValid(line))
                    {
                        switch (GetKey(line))
                        {
                            case "GameDir":
                                game.GameFolder = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "GameExe":
                                game.GameEXE = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "MapDir":
                                game.SDKMapFolder = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "BSP":
                                game.VBSP = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "Vis":
                                game.VVIS = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "Light":
                                game.VRAD = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                            case "BSPDir":
                                game.MapFolder = GetFullPath(GetValue(line), gameInfoDir);
                                CompilePalLogger.LogLine(GetKey(line) + ":" + GetValue(line));
                                break;
                        }
                    }

                    if (line == "		}" || line == "                }")
                    {
                        gameInfos.Add(game);
                        break;
                    }
                }
            }

            return gameInfos;
        }

        static private bool IsValid(string line)
        {
            return line.Contains("\"");
        }

        static private string GetValue(string line)
        {
            return line.Split('"')[3];
        }

        static private string GetKey(string line)
        {
            return line.Split('"')[1];
        }

	    static private string GetFullPath(string line, string gameInfoDir)
	    {
		    if (!line.StartsWith("..") || !line.StartsWith(""))
			    return line;

		    string fullPath = Path.GetFullPath(Path.Combine(gameInfoDir, line));
		    return fullPath;
	    }
    }
}