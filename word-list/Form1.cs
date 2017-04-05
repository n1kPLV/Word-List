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
using System.Net.Http;
using System.Net;


namespace Word_list
{
    public partial class Form_main : Form
    {
        private string[] aviableLang = new string[] 
        {
            "none",
            "latin",
            "latin infinitives"
        };

        public Form_main()
        {
            InitializeComponent();

            cbxLanguageSelect.Items.AddRange(aviableLang);
            cbxLanguageSelect.SelectedIndex = 0;
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string[] text = txtText.Text.Split(new char[] { ' ', '.', ',', ';', ':' ,'\r','\n'},StringSplitOptions.RemoveEmptyEntries);
            //MessageBox.Show(text[(int)numDebWord.Value]);
            List<string> Words = new List<string>();
            foreach(string s in text)
            {
                if (!Words.Contains(s.ToLower()))
                {
					Words.Add(s.ToLower());
                }
            }
			//string[,] word = new string[Words.Count, 2];
			//for (int i = 0; i<Words.LongCount();i++)
			//{
			//	word[i, 0] = Words[i];
			//	word[i, 1] = "";
			//}
			//for(int i = 0; i < word.GetLength(0); i++)
			//{
			//	word[i, 1] = getTrans(word[i, 0]);
			//}
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                StreamWriter write = new StreamWriter(saveFileDialog1.OpenFile(),Encoding.UTF8);
                
                //write.Encoding = Encoding.Default;
                pbrProgress.Maximum = Words.Count;
                foreach (string s in Words)
                {
					write.WriteLine(s+'\t'+getTrans(s,cbxLanguageSelect.SelectedIndex));
					write.Flush();
                    pbrProgress.Value++;
                }
                write.Flush();
                write.Close();
                MessageBox.Show("Sucess!");
                pbrProgress.Value = 0;
            }
        }

        private string getTrans(string voc, int langindex)
        {
            switch (aviableLang[langindex])
            {
                case "latin":
                    return getLatTrans(voc, false);

                case "latin infinitives":
                    return getLatTrans(voc, true);

                default:
                    return "";
            }
        }

		private string getLatTrans(string voc, bool inf)
		{
			
			HttpWebRequest request = WebRequest.CreateHttp("https://www.latein.me/latein/" + voc);
			request.KeepAlive = false;
			request.ContentType = "text/html";
			request.Date = DateTime.UtcNow;
			WebResponse rawresponse;
			try
			{
				rawresponse = request.GetResponse();
			}
			catch (WebException ex)
			{
				return ex.Message;  
			}
			//if(rawresponse
			StreamReader response = new StreamReader(rawresponse.GetResponseStream());

			string VocLine = "";
			bool hasntVocLine = true;
			while (hasntVocLine)
			{
				string tmp = response.ReadLine();
				if (tmp.Contains("<div class=\"contentBox\">"))
				{
					hasntVocLine = false;
					VocLine = tmp;
				}
			}
			response.Close();
            //VocLinePrep
            //VocLine = VocLine.Replace("","");

            if (inf)
            {
                string[] infinitives = VocLine.Split(new string[] { "<dt class=\"translationEntryBox\">" }, StringSplitOptions.RemoveEmptyEntries);
                infinitives[0] = "";
                for (int i = 0; i < infinitives.Length; i++)
                {
                    string[] tmp = infinitives[i].Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length > 1)
                    {
                        infinitives[i] = tmp[1];
                    }
                }
                //TODO!!
                StringBuilder output = new StringBuilder();
                foreach (string s in infinitives)
                {
                    if (s != "")
                    {
                        output.Append(s);
                        output.Append(" oder ");
                    }
                }
                if (output.Length > 6)
                    output.Remove(output.Length - 6, 6);
                return output.ToString();
            }
            else
            {
                //VocLineParser
                string[] translation = VocLine.Split(new string[] { "<dd class=\"translationEntry\">" }, StringSplitOptions.RemoveEmptyEntries);
                string[] formAnalysis = VocLine.Split(new string[] { "<dd class=\"formAnalysisEntry\">" }, StringSplitOptions.RemoveEmptyEntries);
                Console.Write("H");
                translation[0] = "";
                formAnalysis[0] = "";
                for (int i = 0; i < translation.Length; i++)
                {
                    string[] tmp = translation[i].Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length > 1)
                    {
                        translation[i] = tmp[1];
                    }
                }
                for (int i = 0; i < formAnalysis.Length; i++)
                {
                    string[] tmp = formAnalysis[i].Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length > 0)
                    {
                        formAnalysis[i] = tmp[0];
                    }
                }
                StringBuilder output = new StringBuilder();

                foreach (string s in translation)
                {
                    if (s != "" & output.Length < 100)
                    {
                        output.Append(s);
                        output.Append(" oder ");
                    }
                }
                output.Remove(output.Length - 6, 6);
                //output.Append("; Formbestimmung: ");
                //foreach (string s in formAnalysis)
                //{
                //	if (s != "")
                //	{
                //		output.Append(s);
                //		output.Append(" oder ");
                //	}
                //}
                //output.Remove(output.Length - 6, 6);

                return output.ToString(); 
            }
		}

        private void cbxLanguageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            object s = sender;
        }
    }
}
