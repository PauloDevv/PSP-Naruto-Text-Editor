using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NT_EDITOR
{
    public partial class Form1 : Form
    {

        private byte[] fileContent;
        private List<int> pointerList = new List<int>();
        private List<string> textList = new List<string>();
        private bool isUpdatingTextBox = false;
       

        public Form1()
        {
            InitializeComponent();
            treeView1.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = true;
            AllowDrop = true; 
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                string filePath = files[0]; 
                fileContent = File.ReadAllBytes(filePath);
                ReadFile(fileContent);
                ShowControls();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
           
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; 
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ShowControls()
        {
            treeView1.Visible = true;
            button1.Visible = true;
            pictureBox1.Visible = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.bin)|*.bin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                fileContent = File.ReadAllBytes(filePath);
                ReadFile(fileContent);
                ShowControls();
            }
        }

        private void ReadFile(byte[] content)
        {
            pointerList.Clear();
            textList.Clear();

            for (int i = 0; i < content.Length - 4; i += 4)
            {
                int pointer = BitConverter.ToInt32(content, i);

                if (pointer >= 0 && pointer < content.Length)
                {
                    pointerList.Add(pointer);
                    string text = ReadTextAtOffset(content, pointer);
                    textList.Add(text);
                }
            }

            UpdateTreeView();
        }

        private string ReadTextAtOffset(byte[] content, int offset)
        {
            List<byte> textBytes = new List<byte>();

            while (offset < content.Length)
            {
                byte currentByte = content[offset];

                if (currentByte == 0x00)
                {
                    break;
                }

                textBytes.Add(currentByte);
                offset++;
            }

            return Encoding.UTF8.GetString(textBytes.ToArray());
        }



      




        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int selectedIndex = treeView1.SelectedNode.Index;
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Binary Files (*.bin)|*.bin";
                saveFileDialog.Title = "Salvar arquivo modificado";


                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.Create)))
                    {
                        int offset = pointerList.Count * 4;
                        List<int> newPointers = new List<int>();

                        for (int i = 0; i < textList.Count; i++)
                        {
                            newPointers.Add(offset);

                            byte[] textBytes = Encoding.UTF8.GetBytes(textList[i]);
                            writer.Seek(offset, SeekOrigin.Begin);
                            writer.Write(textBytes);
                            writer.Write((byte)0x00);
                            offset += textBytes.Length + 1;
                        }

                        writer.Seek(0, SeekOrigin.Begin);
                        foreach (int pointer in newPointers)
                        {
                            writer.Write(pointer);
                        }
                    }

                    MessageBox.Show("Alterações salvas com sucesso!", "Salvar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                int selectedIndex = treeView1.SelectedNode.Index;

                using (edit editForm = new edit(textList, selectedIndex, treeView1))
                {
                    editForm.ShowDialog(); 
                }
            }
        }

        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            int selectedIndex = treeView1.SelectedNode.Index;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

     


        private void UpdateTreeView()
        {
            treeView1.Nodes.Clear();
            foreach (string text in textList)
            {
                treeView1.Nodes.Add(text);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string[] importedTexts = File.ReadAllLines(filePath);

                    pointerList.Clear();
                    textList.Clear();

                    foreach (string text in importedTexts)
                    {
       
                        string formattedText = text.Replace("/n", "\n");
                        textList.Add(formattedText);

                     
                        pointerList.Add(textList.Count - 1); 
                    }

  
                    UpdateTreeView();
                    MessageBox.Show("Textos importados com sucesso!", "Importar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }



        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.Title = "Salvar textos como";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {

                        foreach (var pointer in pointerList)
                        {
                     
                            string text = ReadTextAtOffset(fileContent, pointer);

                            string formattedText = text.Replace("\n", "/n");

                           
                            writer.WriteLine(formattedText);
                        }
                    }

                    MessageBox.Show("Textos exportados com sucesso!", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }





        private void ccdatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
