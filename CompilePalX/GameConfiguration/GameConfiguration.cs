﻿using System.IO;

namespace CompilePalX
{
    class GameConfiguration
    {
        public string GameFolder { get; set; }

        public string VBSP { get; set; }
        public string VVIS { get; set; }
        public string VRAD { get; set; }

        public string BSPZip {
            get { return Path.Combine(BinFolder, "bspzip.exe"); }
        }
        public string VBSPInfo {
            get { return Path.Combine(BinFolder, "vbspinfo.exe"); }
        }
        public string VPK {
            get { return Path.Combine(BinFolder, "vpk.exe"); }
        }

        public string GameEXE { get; set; }

        public string MapFolder { get; set; }
        public string SDKMapFolder { get; set; }

        public string BinFolder { get; set; }

        public string Name { get; set; }
    }
}