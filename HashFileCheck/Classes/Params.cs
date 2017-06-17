using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashCheck.Classes
{
    // class for transporting settings between settings-window and main-window
    static class Params
    {
        public static List<HashParams> lstPrms = new List<HashParams>();

        // init default values
        static Params()
        {
            HashParams[] tc = new HashParams[]
            {
                new HashParams { Name = HashParams.EncMethod.CRC, IsChecked = true },
                new HashParams { Name = HashParams.EncMethod.MD5, IsChecked = true },
                new HashParams { Name = HashParams.EncMethod.SHA1, IsChecked = true }
            };

            // add crc, md5 and sha1 methods
            lstPrms.AddRange(tc);
        }
    }
}
