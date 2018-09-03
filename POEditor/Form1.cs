using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StiffLibrary;

namespace POEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string poFile;
        List<WordPair> wordPairs = new List<WordPair>();
        int _selected = -1;

        public int Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                if (_selected > -1 && _selected < wordPairs.Count)
                {
                    safePassText = true;
                    textBox1.Text = wordPairs[_selected].Translation;
                    safePassText = false;
                }
            }
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                poFile = openFileDialog1.FileName;
                LoadFile();
            }
        }

        private void LoadFile()
        {
            wordPairs.Clear();
            if (System.IO.File.Exists(poFile))
            {
                string[] file = IOManager.GetFile(poFile);
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
                        label2.Text = language;
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
            }

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (WordPair wp in wordPairs)
            {
                listBox1.Items.Add(wp.Source);
                listBox2.Items.Add(wp.Translation);
            }
        }

        private void SaveFile()
        {
            if (System.IO.File.Exists(poFile))
            {
                int w = 0;
                string[] lines = IOManager.GetFile(poFile);
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
                IOManager.WriteFile(poFile, lines);
            }
        }

        private void Edit(int id, string newValue)
        {
            wordPairs[id].Translation = newValue;
            listBox2.Items[id] = wordPairs[id].Translation;
        }

        bool safePassText = false;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!safePassText)
            {
                Edit(Selected, textBox1.Text);
            }
        }

        bool safePass = false;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!safePass)
            {
                safePass = true;
                Selected = listBox1.SelectedIndex;
                listBox1.SelectedIndex = 0;
                listBox2.SelectedIndex = 0;
                listBox1.SelectedIndex = Selected;
                listBox2.SelectedIndex = Selected;
                safePass = false;
            }

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!safePass)
            {
                safePass = true;
                Selected = listBox2.SelectedIndex;
                listBox1.SelectedIndex = 0;
                listBox2.SelectedIndex = 0;
                listBox1.SelectedIndex = Selected;
                listBox2.SelectedIndex = Selected;
                safePass = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("This program ")
        }

        private void fQAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you are experiencing incorrect or corrupt special characters, please re-save your file with UTF-8 encoding in your notepad before opening.", "FQA");
        }
    }
}
