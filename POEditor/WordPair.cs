using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEditor
{
    public class WordPair
    {
        public WordPair(string source, string translation)
        {
            Source = source;
            Translation = translation;
        }

        public string Source;
        public string Translation;
    }
}
