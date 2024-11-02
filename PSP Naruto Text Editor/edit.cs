using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NT_EDITOR
{
    public partial class edit : Form
    {
        private List<string> textList;
        private TreeView treeView;
        private int currentIndex;

        public edit(List<string> textList, int initialIndex, TreeView treeView)
        {
            InitializeComponent();
            this.textList = textList;
            this.currentIndex = initialIndex;
            this.treeView = treeView;

            UpdateTextBox();
        }

        private void UpdateTextBox()
        {
            if (currentIndex >= 0 && currentIndex < textList.Count)
            {
              
                textBox1.Text = textList[currentIndex].Replace("\n", "/n");
            }
        }

        private void SaveCurrentText()
        {
            if (currentIndex >= 0 && currentIndex < textList.Count)
            {
               
                string modifiedText = textBox1.Text.Replace("/n", "\n");
                textList[currentIndex] = modifiedText;
                treeView.Nodes[currentIndex].Text = modifiedText.Replace("\n", "/n"); 
            }
        }

        private void edit_Load(object sender, EventArgs e)
        {
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; 
                InsertNewLine();
            }
        }

        private void InsertNewLine()
        {
           
            SaveCurrentText();
            textList[currentIndex] += "\n"; 
            UpdateTextBox();
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            SaveCurrentText();
            if (currentIndex < textList.Count - 1)
            {
                currentIndex++;
                UpdateTextBox();
            }
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            SaveCurrentText();
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateTextBox();
            }
        }
    }
}
