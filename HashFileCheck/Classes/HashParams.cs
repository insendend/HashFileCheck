using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashCheck.Classes
{
    class HashParams
    {
        public enum EncMethod
        {
            CRC,
            MD5,
            SHA1,
            SHA256,
            SHA512
        }

        // type of encrypt method
        public EncMethod Name { set; get; }

        // hash in hex
        public string Value { set; get; }

        // choosen or not in settings
        public bool IsChecked { get; set; }
    }
}
