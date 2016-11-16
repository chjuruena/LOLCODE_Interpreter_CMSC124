using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace LOLCODE_INTERPRETERS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ArrayList lexemes = new ArrayList();
        ArrayList classification = new ArrayList();
        Dictionary<string, string> symbols = new Dictionary<string, string>();
        ArrayList errorList = new ArrayList();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            int size = 1;
            DialogResult result = openFileDialog1.ShowDialog();
            //chooses and opens file
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    //reads text file
                    string text = File.ReadAllText(file);
                    textBox1.Text = text;
                    size = text.Length;
                }
                catch (Exception)
                {

                }
            }
            //cancels open file dialog
            else
            {

            }

        }

        private void print() {
            //lexemes
            for (int j = 0; j < lexemes.Count; j++)
            {
                textBox3.AppendText(lexemes[j] + Environment.NewLine);
            }

            for (int j = 0; j < classification.Count; j++)
            {
                textBox4.AppendText(classification[j] + Environment.NewLine);
            }
            //symbols
            foreach (KeyValuePair<string, string> pair in symbols)
            {
                textBox5.AppendText(pair.Key + Environment.NewLine);
                textBox6.AppendText(pair.Value + Environment.NewLine);
            }
    
        }

        private void printError() {
            for (int j = 0; j < errorList.Count; j++)
            {
                textBox3.AppendText(lexemes[j] + Environment.NewLine);
            }
        }

        private void clearAll() {
            //textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            lexemes.Clear();
            classification.Clear();
            symbols.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearAll();
            Regex r;
            Match m;
            string[] lines;

            lines = textBox1.Text.Split('\n');

            int hai = 0, lineNumber = 0;

            if(textBox1.Text.Length > 0){
                foreach (string s in lines)
                {
                    ++lineNumber;
                    string line = string.Copy(s);
                    line = line.Trim();

                    r = new Regex(@"(?<start>HAI)");
                    m = r.Match(line);
                    if(m.Success){
                        lexemes.Add("HAI");
                        classification.Add("Program Initializer");
                        hai = 1;
                     //   continue;
                    }

                    if (hai == 0)
                    {
                        textBox2.AppendText("LINE " + lineNumber + ": Error: No Program Initializer" + Environment.NewLine);
                        break;
                    }

                    r = new Regex(@"(?<userinput>GIMMEH)\s+(?<identifier>.+)");
                    m = r.Match(line);
                    if(m.Success){
                        if (lexemes.Contains(m.Groups["identifier"].Value))
                        {
                            lexemes.Add(m.Groups["userinput"].Value);
                            classification.Add("User Input");
                            lexemes.Add(m.Groups["identifier"].Value);
                            classification.Add("Variable Identifier");
                        }
                        else {
                            errorList.Add("LINE " + lineNumber + ": Variable is not yet declared" + Environment.NewLine);
                        }
                        continue;
                    }

                    r = new Regex(@"(?<useroutput>VISIBLE)\s+(?<identifier>.+)");
                    m = r.Match(line);
                    Regex l = new Regex(@"([\\""].+[\\""]) | (TRUE|FALSE) | [0-9]+ | [0-9]+\.[0-9]+");
                    Match n = l.Match(m.Groups["identifier"].Value);
                    if(m.Success){
                        if (symbols.ContainsKey(m.Groups["identifier"].Value))
                        {
                            lexemes.Add("VISIBLE");
                            classification.Add("User Output");
                            lexemes.Add(m.Groups["identifier"].Value);
                            classification.Add("Variable Identifier");
                            textBox2.AppendText(symbols[m.Groups["identifier"].Value] + Environment.NewLine);
                        }else if(n.Success){
                            lexemes.Add("VISIBLE");
                            classification.Add("User Output");
                            lexemes.Add(m.Groups["identifier"].Value);
                            classification.Add("Literal");
                            textBox2.AppendText(m.Groups["identifier"].Value + Environment.NewLine);
                        }else{
                            errorList.Add("LINE " + lineNumber + ": Variable is not yet declared" + Environment.NewLine);
                        }
                        /*else for expressions*/
                        continue;
                    }
/*
                    r = new Regex(@"(?<vardec>I\s+HAS\s+A)\s+(?<ident>[a-zA-Z][a-zA-Z0-9_]*)");
                    m = r.Match(line);
                    if(m.Success){
                        if (!lexemes.ContainsKey(m.Groups["ident"].Value))
                        {
                            lexemes.Add(m.Groups["vardec"].Value, "Variable Declaration");
                            lexemes.Add(m.Groups["ident"].Value, "Variable Identifier");
                            symbols.Add(m.Groups["ident"].Value, "NOOB");
                        }
                        continue;
                    }
 */
                    r = new Regex(@"(?<vardec>I\s+HAS\s+A)\s+(?<ident>[a-zA-Z][a-zA-Z0-9_]*)\s+(?<determine>ITZ)\s+(?<val>.+)");
                    m = r.Match(line);
                    if(m.Success){
                        if (!lexemes.Contains(m.Groups["ident"].Value))
                        {
                            lexemes.Add(m.Groups["vardec"].Value);
                            classification.Add("Variable Declaration");
                            lexemes.Add(m.Groups["ident"].Value);
                            classification.Add("Variable Identifier");
                            lexemes.Add(m.Groups["determine"].Value);
                            classification.Add("Determiner");
                            lexemes.Add(m.Groups["val"].Value);
                            classification.Add("Variable Value");
                            symbols.Add(m.Groups["ident"].Value, m.Groups["val"].Value);
                        }
                        continue;
                    }

                    r = new Regex(@"(?<ident>[a-zA-Z][a-zA-Z0-9]*)\s+(?<assign>R)\s+(?<val>.+)");
                    m = r.Match(line);
                    if(m.Success){
                        if (lexemes.Contains(m.Groups["ident"].Value)) {
                            lexemes.Add(m.Groups["ident"].Value);
                            classification.Add("Variable Classification");
                            lexemes.Add(m.Groups["assign"].Value);
                            classification.Add("Assignment");
                            lexemes.Add(m.Groups["val"].Value);
                            classification.Add("Variable Value");
                            symbols[m.Groups["ident"].Value] = m.Groups["val"].Value;
                        }

                    }




                    /************/

                    /************/

                    r = new Regex(@"(?<start>KTHXBYE)");
                    m = r.Match(line);
                    if (m.Success)
                    {
                        lexemes.Add("KTHXBYE");
                        classification.Add("Program Terminator");

                        continue;
                    }
                }

            }
            else{
                //if no code input was made
                MessageBox.Show(
                    "Invalid input! Please enter code.",
                    "Alert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                    );
            }

            if(errorList.Count == 0){
                printError();
            }else{
                print();
            }
        }
    }
}
