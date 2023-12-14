using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weew
{
    public class Rule
    {
        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value.ToUpper(); }
        }
        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value.ToLower(); }
        }

        public Rule(string key, string value) 
        {
            Key = key;
            Value = value;
        }
    }
}
