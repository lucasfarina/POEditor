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
        string poFilePath;
        POFile currentPOFile = null;

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
                if (_selected > -1 && _selected < currentPOFile.wordPairs.Count)
                {
                    safePassText = true;
                    textBox1.Text = currentPOFile.wordPairs[_selected].Translation;
                    safePassText = false;
                }
            }
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                poFilePath = openFileDialog1.FileName;
                LoadFile();
            }
        }

        private void LoadFile(bool shouldClear = true)
        {
            if(currentPOFile != null) currentPOFile.FreeFile();

            currentPOFile = new POFile(poFilePath);

            if (shouldClear)
            {
                RefreshFile();
            }
        }

        public void RefreshFile()
        {
            safePassText = true;
            safePass = true;
            textBox1.Text = "";
            label2.Text = currentPOFile.Language;
            safePassText = false;
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (WordPair wp in currentPOFile.wordPairs)
            {
                listBox1.Items.Add(wp.Source);
                listBox2.Items.Add(wp.Translation);
            }
            safePass = false;
        }

        private void SaveFile()
        {
            if(currentPOFile != null)
            {
                currentPOFile.SaveFile();
            }
        }

        private void Edit(int id, string newValue)
        {
            currentPOFile.wordPairs[id].Translation = newValue;
            listBox2.Items[id] = currentPOFile.wordPairs[id].Translation;
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        
        }

        private void MergeWithFile(bool justEmpty)
        {
            string oldPath = "";
            if (!String.IsNullOrEmpty(poFilePath))
            {
                oldPath = openFileDialog1.FileName;
            }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string mergeFilePath = openFileDialog1.FileName;

                if (mergeFilePath == oldPath)
                    return;
                if (String.IsNullOrEmpty(mergeFilePath))
                    return;

                POFile mergeFile = new POFile(mergeFilePath);

                currentPOFile.MergeFile(mergeFile, justEmpty);

                RefreshFile();
            }
            if (!String.IsNullOrEmpty(poFilePath))
            {
                openFileDialog1.FileName = oldPath;
            }
        }

        private void mergeWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeWithFile(false);
        }

        private void mergeJustEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeWithFile(true);
        }
    }
}
