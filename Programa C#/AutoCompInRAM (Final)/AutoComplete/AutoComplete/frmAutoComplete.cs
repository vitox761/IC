using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
using AutoComplete.Classes;
using Lucene.Net.QueryParsers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;

namespace AutoComplete
{
    public partial class frm_autoComplete : Form
    {
        public frm_autoComplete()
        {
            InitializeComponent();
        }
        //global variables declarations
        private bool _isNormalizing;
        private int typedLetterCount;
        private int totalLetterCount;
        private string textToBeAnalysed;
        private string textToBeIndexed;
        private LuceneSearch luceneSearch;
        private int overWeight;
        private ProgressBar pBar;
        private volatile bool _stopNow;
        char[] keys = { (char)Keys.D1, (char)Keys.D2, (char)Keys.D3, (char)Keys.D4 };
        private void frmAutoComplete_Load(object sender, EventArgs e)
        {
            lbl_total.Text = "";
            lbl_typed.Text = "";
            typedLetterCount = 0;
            totalLetterCount = 0;
            textToBeIndexed = "";
            label1.Visible = false;
            _isNormalizing = false;
            luceneSearch = new LuceneSearch("lucene_index");
            btnStopIndexing.Enabled = false;
        }

        private void indexInThread(string[] text, bool isUserEntry)
        {            
            int numberOfWordsIndexed = 0;
            string InsertedWord = "";
            foreach (string word in text)
            {
                if (_stopNow)
                    break;
                numberOfWordsIndexed++;
                this.Invoke((MethodInvoker)delegate 
                { 
                    pBar.PerformStep(); 
                    lblProgress.Text = Convert.ToInt32((numberOfWordsIndexed * 100 / text.Length)).ToString() + "%"; 
                });
                if (word.Length > 3)
                {
                    string finalWord = RemoveDiacritics(word);
                    InsertedWord = finalWord.ToLower();
                    overWeight = luceneSearch.AddUpdateLuceneIndex(finalWord, isUserEntry);

                    //Normalize indexes after reaching the determined boundary
                    if (overWeight != 0 && !_isNormalizing)
                    {
                        //Normalization
                        //creates a new searcher with a different path, reducing the weights porportionally
                        _isNormalizing = true;
                        IEnumerable<DataType> wordsToBeCopied = luceneSearch.GetAllIndexRecords();
                        Thread normalizeThread = new Thread(() => Normalize(wordsToBeCopied));
                        normalizeThread.Start();
                    }
                }    
            }

            //execute some functions on form thread ("this" represents the form)
            this.Invoke((MethodInvoker)delegate 
            {
                lblProgress.Text = "";
                pBar.Dispose();
                label1.Visible = false;
                _stopNow = false;
                txtInput.Enabled = true;
                ltbSuggestions.Enabled = true;
                btnClearIndexes.Enabled = true;
                btnTxtProcessing.Enabled = true;
                btnShowFullText.Enabled = true;
                btnSaveIndexesToDisk.Enabled = true;
                btnShowAllSuggestions.Enabled = true;
                txtInput.Focus();
            });
            
        }
        
        private void Normalize(IEnumerable<DataType> words)
        {            
            LuceneSearch temporarySearch = new LuceneSearch();
            foreach (DataType data in words)
            {
                data.Weight /= 2;
                temporarySearch.AddLuceneIndex(data);
            }
            this.Invoke((MethodInvoker)delegate
            {
                luceneSearch.changeDirectory(temporarySearch.CurrentDirectory);
                _isNormalizing = false;
            });
        }

        private void IndexTextAndSaveToLucene(string text, bool isUserEntry)
        {
            
            label1.Visible = true;
            txtInput.Enabled = false;
            ltbSuggestions.Enabled = false;
            btnClearIndexes.Enabled = false;
            btnTxtProcessing.Enabled = false;
            btnShowFullText.Enabled = false;
            btnShowAllSuggestions.Enabled = false;
            btnSaveIndexesToDisk.Enabled = false;
            pBar = new ProgressBar();
            pBar.Location = new Point(54, 482);
            pBar.Size = new Size(407, 23);
            this.Controls.Add(pBar);
            pBar.Visible = true;
            pBar.Minimum = 0;            
            pBar.Value = 1;
            pBar.Step = 1;
            btnStopIndexing.Enabled = true;   
            string[] wordsToBeIndexed = Regex.Split(text, @"\W+");
            pBar.Maximum = wordsToBeIndexed.Length;
            Thread indexingThread = new Thread(() => indexInThread(wordsToBeIndexed, isUserEntry));
            indexingThread.Priority = ThreadPriority.Highest;
            indexingThread.Start();
            
        }

        private string RemoveDiacritics(string value)
        {            
            int j;
            string r = "";

            if (value == null)
            {
                return null;
            }

            value = value.ToLower();
            for (j = 0; j < value.Length; j++)
            {
                if ((value[j] == 'á') ||
                    (value[j] == 'â') ||
                    (value[j] == 'ã'))
                {
                    r = r + "a"; continue;
                }
                if ((value[j] == 'é') ||
                    (value[j] == 'ê'))
                {
                    r = r + "e"; continue;
                }
                if (value[j] == 'í')
                {
                    r = r + "i"; continue;
                }
                if ((value[j] == 'ó') ||
                    (value[j] == 'ô') ||
                    (value[j] == 'õ'))
                {
                    r = r + "o"; continue;
                }
                if ((value[j] == 'ú') ||
                    (value[j] == 'ü'))
                {
                    r = r + "u"; continue;
                }
                if (value[j] == 'ç')
                {
                    r = r + "c"; continue;
                }
                if (value[j] == 'ñ')
                {
                    r = r + "n"; continue;
                }

                r = r + value[j];
            }

            return r;
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber;
            e.Handled = int.TryParse(e.KeyChar.ToString(), out isNumber); // doesn't allow number typing
            isNumber--;
            int numberDifference = 0;
            string originalText = txtInput.Text;
            if (keys.Contains(e.KeyChar))
            {
                try
                {
                    try
                    {
                        txtInput.Text = txtInput.Text.Remove(originalText.LastIndexOf(' ') + 1);
                    }
                    catch
                    {
                        txtInput.Text = txtInput.Text.Remove(originalText.Length - 1);
                    }                    
                    string word = ltbSuggestions.Items[isNumber].ToString().Substring(0,ltbSuggestions.Items[isNumber].ToString().IndexOf(' '));
                    txtInput.Text += word + " ";
                    numberDifference = txtInput.Text.Trim().Length - originalText.Trim().Length;
                    totalLetterCount+= numberDifference;
                    ltbSuggestions.Items.Clear();
                    txtInput.SelectionStart = txtInput.Text.Length;
                    if (!_isNormalizing)
                        IndexTextAndSaveToLucene(word, true);
                    textToBeAnalysed = "";
                    return;
                }
                catch
                {                    
                    txtInput.Text = originalText;
                    txtInput.SelectionStart = txtInput.Text.Length;
                    ltbSuggestions.Items.Clear();
                    return;
                }
            }
            if ((e.KeyChar == (char)Keys.Space))
            {
                textToBeIndexed = textToBeAnalysed;
                if (!_isNormalizing)
                    IndexTextAndSaveToLucene(textToBeIndexed, true);
                textToBeIndexed = "";
                ltbSuggestions.Items.Clear();
                txtInput.Focus();
                textToBeAnalysed = ""; //as spacebar was pressed, the typed word must be added to the index
            }
            else
            {
                if (e.KeyChar == (char)Keys.Back) //when backspace is pressed, the prefix must be processed again
                {
                    try
                    {
                        textToBeAnalysed = txtInput.Text.Substring(txtInput.Text.LastIndexOf(" "), txtInput.Text.Length - txtInput.Text.LastIndexOf(" ") - 1);
                    }
                    catch
                    {
                        try
                        {
                            textToBeAnalysed = txtInput.Text.Remove(txtInput.Text.Length - 1);
                        }
                        catch { }
                    }
                }
                else
                {
                    totalLetterCount++;
                    typedLetterCount++;
                    textToBeAnalysed += (char)e.KeyChar;
                }
                ltbSuggestions.Items.Clear();
                IEnumerable<DataType> searchResults = new List<DataType>();
                try
                {
                    searchResults = luceneSearch.Search(textToBeAnalysed);
                }
                catch { }
                int count = 0;
                foreach (DataType word in searchResults.Reverse<DataType>())
                {
                    count++;
                    ltbSuggestions.Items.Add(word.Word + " - " + word.Weight.ToString());
                    if (count == 4)
                        break;
                }                
            }
        }

        private void btnShowFullText_Click(object sender, EventArgs e) //shows everything that's written in a textbox
        {
            MessageBox.Show(txtInput.Text, "Texto Completo");
        }

        private void btnClearIndexes_Click(object sender, EventArgs e) //clears all indexes! (FOR GOOD!)
        {
            if (MessageBox.Show("Tem certeza que deseja excluir os indices?","Atenção!",MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {                
                luceneSearch.ClearLuceneIndex();
                MessageBox.Show("Indices deletados com sucesso!");
            }
            else
            {
                MessageBox.Show("Indices mantidos!");
            }
        }

        private void btnTxtProcessing_Click(object sender, EventArgs e) //Processes all the words written in a .txt file
        {
            StreamReader strReader;
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            strReader = new StreamReader(myStream);
                            IndexTextAndSaveToLucene(strReader.ReadToEnd(), false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro! O arquivo não pode ser lido do disco! Original error: " + ex.Message);
                }
            }
        }

        private void btnShowAllSuggestions_Click(object sender, EventArgs e)
        {
            ltbSuggestions.Items.Clear();
            IEnumerable<DataType> searchResults = new List<DataType>();
            searchResults = luceneSearch.GetAllIndexRecords();
            IEnumerable<DataType> query = searchResults.OrderBy(word => word.Weight);
            foreach (DataType word in query.Reverse<DataType>())
            {
                ltbSuggestions.Items.Add(word.Word + " - " + word.Weight.ToString());
            }    
            
        }

        private void btnStopIndexing_Click(object sender, EventArgs e)
        {
            _stopNow = true;
        }

        private void btnSaveIndexesToDisk_Click(object sender, EventArgs e)
        {
            try
            {
                luceneSearch.saveCurrentIndexesToDisk();
                MessageBox.Show("Sugestões salvas com sucesso!");
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar as sugestões");
            }
        }

        private void frm_autoComplete_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                luceneSearch.saveCurrentIndexesToDisk();
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar as sugestões");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            lbl_typed.Text = "Número de caracteres digitados: " + typedLetterCount.ToString();
            lbl_total.Text = "Número total de caracteres: " + totalLetterCount.ToString();
        }

       
    }
}
