using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class KeyMessage
    {
        private string procName = null;
        private int nKeys = 0;
        private int[] keys = { 0, 0, 0, 0 };

        public KeyMessage(string procName, int nofKeys, int[] keys)
        {
            this.procName = procName;
            nKeys = nofKeys;
            for (int i = 0; i < nKeys; i++)
                this.keys[i] = keys[i];
        }
    }
}
