using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StiffLibrary;

namespace POEditor
{
    class POFile
    {
        public string myPath;
        //System.IO.FileStream fs;
        public string Language;
        public List<WordPair> wordPairs = new List<WordPair>();
        public POFile(string poFilePath)
        {
            myPath = poFilePath;
            wordPairs.Clear();
            if (System.IO.File.Exists(myPath))
            {
                string[] file = IOManager.GetFile(myPath);
                string source = "";
                string translation = "";
                bool first = true;
                bool lang = true;
                foreach (string line in file)
                {
                    if (lang && line.Contains("Language: "))
                    {
                        string lineTrim = line.Trim();
                        string language = lineTrim.ExtractFromString("\"Language: ", "\\n\"");
                        Language = language;
                        lang = false;
                    }
                    if (line.Contains("msgid"))
                    {
                        string lineTrim = line.Trim();
                        source = lineTrim.ExtractFromString("msgid \"", "\"");
                        //MessageBox.Show(source);
                    }
                    if (line.Contains("msgstr"))
                    {
                        string lineTrim = line.Trim();
                        translation = line.ExtractFromString("msgstr \"", "\"");
                        if (first)
                            first = false;
                        else
                            wordPairs.Add(new WordPair(source, translation));
                    }
                }
                //if (!(fs == null))
                //{
                //    fs.Dispose();
                //    fs.Close();
                //}
                //fs = System.IO.File.OpenWrite(myPath);
                //System.IO.StreamWriter sw = new System.IO.StreamWriter()
            }
        }

        public void SaveFile()
        {
            if (System.IO.File.Exists(myPath))
            {
                //if (!(fs == null))
                //{
                //    fs.Dispose();
                //    fs.Close();
                //}
                int w = 0;
                string[] lines = IOManager.GetFile(myPath);
                bool nextMsgStr = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Contains(wordPairs[w].Source))
                    {
                        nextMsgStr = true;
                    }
                    if (line.Contains("msgstr") && nextMsgStr)
                    {
                        lines[i] = "msgstr \"" + wordPairs[w].Translation + "\"";
                        w++;
                        if (w >= wordPairs.Count) { break; }
                        nextMsgStr = false;
                    }
                }
                
                IOManager.WriteFile(myPath, lines);
                //fs = System.IO.File.OpenWrite(myPath);
            }
        }

        public void MergeFile(POFile otherFile, bool justMissing = false)
        {
            foreach(WordPair wp in wordPairs)
            {
                foreach(WordPair wp2 in otherFile.wordPairs)
                {
                    if(wp.Source == wp2.Source)
                    {
                        if (justMissing)
                        {
                            if (wp.Translation == "" && wp2.Translation != "")
                            {
                                wp.Translation = wp2.Translation;
                            }
                        }
                        else
                        {
                            if (wp2.Translation != "")
                            {
                                wp.Translation = wp2.Translation;
                            }
                        }
                    }
                }
            }
        }

        public void FreeFile()
        {
            //if (!(fs == null))
            //{
            //    fs.Dispose();
            //    fs.Close();
            //}
        }

        ~POFile()
        {
            FreeFile();
        }

    }
}
