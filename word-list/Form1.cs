using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Word_list
{
    public partial class Form_main : Form
    {
        public Form_main()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string[] text = txtText.Text.Split(new char[] { ' ', '.', ',', ';', ':' },StringSplitOptions.RemoveEmptyEntries);
            //MessageBox.Show(text[(int)numDebWord.Value]);
            List<string> Words = new List<string>();
            foreach(string s in text)
            {
                if (!Words.Contains(s.ToLower()))
                {
                    Words.Add(s.ToLower());
                }
            }
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                StreamWriter write = new StreamWriter(saveFileDialog1.OpenFile());
                foreach (string s in Words)
                {
                    write.WriteLine(s);
                }
                write.Flush();
                write.Close();
            }
        }

    }
}
