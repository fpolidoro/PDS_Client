using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class KeyMessage
    {
        [JsonProperty(PropertyName = "procName")]
        private string procName = null;
        [JsonProperty(PropertyName = "nKeys")]
        private int nKeys = 0;
        [JsonProperty(PropertyName = "keys")]
        private int[] keys = { 0, 0, 0, 0, 0 };

        public KeyMessage(string procName, int nofKeys, int[] keys)
        {
            if (procName == null)
                this.procName = String.Empty;
            else
                this.procName = procName;
            nKeys = nofKeys;
            for (int i = 0; i < nKeys; i++)
                this.keys[i] = keys[i];

#if (DEBUG)
            Debug.WriteLine("KeyMessage:");
            Debug.WriteLine("procName: " + this.procName == null ? "null" : this.procName);
            Debug.WriteLine("nKeys: " + nKeys);
            for (int i = 0; i < nKeys; i++)
                Debug.Write(keys[i]);
            Debug.WriteLine("");
#endif
        }

#if (DEBUG)
        public void PrintKMsg()
        {
            Debug.WriteLine("KeyMessage:");
            Debug.WriteLine("procName: " + this.procName == null ? "null" : this.procName);
            Debug.WriteLine("nKeys: " + nKeys);
            for (int i = 0; i < nKeys; i++)
                Debug.Write(keys[i]);
            Debug.WriteLine("");
        }
#endif
    }
}
