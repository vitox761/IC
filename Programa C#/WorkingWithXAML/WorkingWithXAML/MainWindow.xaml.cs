using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using AutoComplete.Classes;
using System.IO;
using System.Text;
using System.Diagnostics;



namespace WorkingWithXAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Global Variables
        bool nextIsUpper = true; //the first character is upper 
        string composition; 
        bool isCancelled; 
        bool centreProcessed = false; //centre has not yet been processed at the beginning
        string charToPrint;
        bool primaryKeyboard;
        int blockType;
        bool isBlocked;
        const int initialHeight = 720; //initial height and width determined to create the design pattern
        const int initialWidth = 1280;
        string lastPanel = ""; //last accessed grid name
        string currentGrid;
        string[] panelNames;
        string[] labelNames;
        string[] suggestionNames;
        Label[] suggestionLabel;
        string currentWord;
        List<string> suggestionsList;
        Timer keyboardTimer = new Timer(); //timer reference for secondary keyboard
        bool isBetsy;
        Label lbl_aux;
        string path = @"data.txt";
        bool colectorOn = false;
        int erros = 0;
        int caracteresTotal = 0;
        string[] arquivo = new string[4];
        Stopwatch timer = new Stopwatch();
        #endregion

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //initial system values
            composition = "-";
            isCancelled = false;
            charToPrint = "";
            primaryKeyboard = true;
            blockType = 0;
            isBlocked = false;
            suggestionLabel = new Label[4];
            for (int i = 1; i < 5; i++) //References the suggestion labels with an array and clear their "Content" property
            {
                suggestionLabel[i - 1] = (Label)(FindName("sug" + i));
                suggestionLabel[i - 1].Content = "";
            }
            changeObjectsSize();
            //initialize the Suggester
            Suggester.initializeSuggester();
            suggestionsList = new List<string>();
            currentWord = "";
            //every 2 seconds after the keyboardTimer has started, the changeKeyboard function will be called 
            keyboardTimer.Interval = 2000; 
            keyboardTimer.Elapsed += new ElapsedEventHandler(changeKeyboard); 
            
        }

        private void borderMouseLeave(object sender, EventArgs e)
        {
            keyboardTimer.Stop(); //stop timer since you left a specific grid
            Grid g = (Grid)sender;
            string name = g.Name;
            string index = name.Substring(1, 1);
            Label obj = (Label)FindName("min" + index);
            obj.Content = ""; //clears the grid that has just be disposed
        }

        private void changeKeyboard(object sender, ElapsedEventArgs e)
        {
            primaryKeyboard = !primaryKeyboard; //change keyboard
            composition = composition.Substring(0, composition.Length - 1);
            Dispatcher.Invoke(new Action(() =>
            {
                //Reprints the character inside the current grid
                //Invoke a new action because the Timer thread cannot access directly the MainWindow thread
                panelMouseEnterF((Grid)FindName(currentGrid));
            }));
        }

        private void panelMouseEnter(object sender, MouseEventArgs e)
        {
            panelMouseEnterF(sender); //This event handler must be "simulated" for timed operations, and
                                      //that's the reason it was wrapped in a function that does not 
                                      //need MouseEventArgs and can be, therefore, called from inside the code easily
        }

        private void panelMouseEnterF(object sender) 
        {
            keyboardTimer.Stop(); //Restart the timer
            keyboardTimer.Start();
            Grid panel = ((Grid)sender);
            currentGrid = panel.Name;
            string index = currentGrid.Substring(1, 1); //gets the current grid name
            
            centreProcessed = false;
            Grid x = (Grid)sender;
            BorderProcessing(index);
            lastPanel = panel.Name;
        }

        private void centreMouseEnter(object sender, MouseEventArgs e)
        {
            centreProcessing();            
        }

        #endregion

        #region Processing

        /// <summary>
        /// Adapts all the controls to the resolution of the user's screen
        /// </summary>
        private void changeObjectsSize()
        {
            isBetsy = false;   
            double hProportion = this.ActualHeight / initialHeight;
            double wProportion = this.ActualWidth / initialWidth;
            panelNames = new string[11] {"txtGrid", "pMain", "p0", "p1", "p2", "p3", "p4", "p5", "p6", "p7", "p8" };
            labelNames = new string [35] { "min0","min1", "min2", "min3", "min4", "min5", "min6", "min7", "min00", "min01", "min02", "min03", "min10", "min11", "min20", "min21", "min22", "min23", "min30", "min31", "min32", "min33", "min34", "min35", "min40", "min41", "min42", "min43", "min44", "min45", "min50", "min51", "min60", "min70", "min71" };
            suggestionNames = new string[4] { "sug1", "sug2", "sug3", "sug4" };
            foreach (string name in panelNames) //Grids adaptation
            {
                var obj = (Grid)FindName(name);
                obj.Height *= hProportion;
                obj.Width *= wProportion;
                obj.Margin = new Thickness(obj.Margin.Left * wProportion, obj.Margin.Top * hProportion, obj.Margin.Right * wProportion, obj.Margin.Bottom * hProportion);
            }
            foreach (string name in labelNames) //Miniatures adaptation
            {
                var obj = (Label)FindName(name);
                obj.Height *= hProportion;
                obj.Width *= wProportion;
                obj.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
                obj.Margin = new Thickness(obj.Margin.Left * wProportion, obj.Margin.Top * hProportion, obj.Margin.Right * wProportion, obj.Margin.Bottom * hProportion);
            }
            for (int i = 0; i < 4; i++) //Suggestions adaptation
            {
                //var obj = (Label)FindName(name);
                suggestionLabel[i].Height *= hProportion;
                suggestionLabel[i].Width *= wProportion;
                suggestionLabel[i].FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
                suggestionLabel[i].Margin = new Thickness(suggestionLabel[i].Margin.Left * wProportion, suggestionLabel[i].Margin.Top * hProportion, suggestionLabel[i].Margin.Right * wProportion, suggestionLabel[i].Margin.Bottom * hProportion);
            }
            /*
            for (int i = 0; i < 4; i++)
            {                
                suggestionLabel[i].Height *= hProportion;
                suggestionLabel[i].Width *= wProportion;
                suggestionLabel[i].FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
                suggestionLabel[i].Margin = new Thickness(suggestionLabel[i].Margin.Left * wProportion, suggestionLabel[i].Margin.Top * hProportion, suggestionLabel[i].Margin.Right * wProportion, suggestionLabel[i].Margin.Bottom * hProportion);
            }
            */
            txtInput.Height *= hProportion;
            txtInput.Width *= wProportion;
            txtInput.Margin = new Thickness(txtInput.Margin.Left * wProportion, txtInput.Margin.Top * hProportion, txtInput.Margin.Right * wProportion, txtInput.Margin.Bottom * hProportion);
            //Fonts adaptation
            txtInput.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2)); 
        }
                    
        /// <summary>
        /// Processes the Grid that the user is currently looking at
        /// </summary>
        /// <param name="index">The index of the current Grid</param>
        private void BorderProcessing(string index)
        {
            if (composition[composition.Length - 1] != index[0])
                composition += index;
            if (isBetsy) //if it's DEL, SPACE or . and the font is already changed
            {
                if (composition.Length == 3)
                {
                    if (index == "6") //if it's a Delete Op
                    {
                        charToPrint = "V";
                        lbl_aux = (Label)FindName("min" + index);
                        lbl_aux.Content = charToPrint;
                    }
                    else //if it's a '.' Op
                    {
                        charToPrint = "A";
                        lbl_aux = (Label)FindName("min" + index);
                        lbl_aux.Content = charToPrint;
                    }
                    return;
                }
                else //mantain proper charToPrint for each element DEL SPACE . (V D A in Betsy)
                {
                    lbl_aux = (Label)FindName("min" + index);
                    lbl_aux.Content = charToPrint;
                }
                return;
            }
            if (composition.Length == 2 && index == "5") //if the first movement is \/ so the font has to change to Betsy
            {
                for (int i = 0; i <= 7; i++)
                {
                    var obj = (Label)FindName("min" + i.ToString());
                    obj.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Betsy Flanagan 2"); 
                }
                isBetsy = true;
                charToPrint = "D"; //space is set as character to be printed (D in betsy format)
                Label lbl = (Label)FindName("min" + index);
                lbl.Content = charToPrint;
                return;
            }
            try
            {               
                if (composition.Length > 2 && index == composition.Substring(1, 1))
                {
                    blockType = 0;
                    isCancelled = true;
                    composition = "ii"; //character won't be written
                }
            }
            catch
            {  }      
               
            try
            {   //Prepare to print a character composed of 2 movements
                charToPrint = composition.Substring(1, 2); 
            }
            catch
            {   //Prepare to print a character composed of a single movement
                charToPrint = composition.Substring(1, 1);
            }
            
            if (!isBlocked) //if the environment is currently not blocked, the character for the current composition is generated
                charToPrint = CharacterDecoder.generateCharacter(charToPrint, blockType, primaryKeyboard);
            else
                charToPrint = "-";  //if it's blocked, the character - is print           
            
            //Prints the processed character if not blocked or '-' if blocked
            lbl_aux = (Label)FindName("min"+index);
            lbl_aux.Content = charToPrint;
        }


        /// <summary>
        /// Processes the sequence of movements the user performed since the last centre focus
        /// </summary>
        private void centreProcessing()
        {            
            keyboardTimer.Stop();
            
            if (isBetsy) //changing back to the original font
            {
                for (int i = 0; i <= 7; i++)
                {
                    var obj = (Label)FindName("min" + i.ToString());
                    obj.FontFamily = new FontFamily("Segoe UI");
                }
                isBetsy = false;
            }
            string finalChar;
            lastPanel = "";
            if (centreProcessed) //if the centre was already processed (no movement to the borders so far), no operation needed
                return;
            
            switch (blockType)
            {
                case 1: //Blocked to rest
                    isBlocked = false;
                    if (composition == "-12345" || composition == "-10765")
                    {
                        blockType = 0; 
                        //Restoring default UI
                        locklogo.Visibility = System.Windows.Visibility.Hidden;
                        txtInput.Visibility = System.Windows.Visibility.Visible;
                        for (int i = 8; i < 35; i++)
                        {
                            var obj = (Label)FindName(labelNames[i]);
                            obj.Visibility = System.Windows.Visibility.Visible;
                        }                
                    }
                    break;                        

                case 2: //Blocked to read full text
                    isBlocked = false;
                    if (composition == "-12345" || composition == "-10765")
                    {
                        blockType = 0;
                        // Restoring default UI
                        txtInput.Width = txtInput.Width / 3;
                        txtInput.Height = txtInput.Height / 3;
                        for (int i = 8; i < 35; i++)
                        {
                            var obj = (Label)FindName(labelNames[i]);
                            obj.Visibility = System.Windows.Visibility.Visible;
                        }                                        
                    }
                    break;

                default: //No Block
                    if (!isCancelled)
                    {
                        if (composition == "-") //if there was no movement to the extremities
                        {
                            goto didntLeaveCentre; //jumps all the character processing
                        }
                        if (composition == "-12345")
                        {
                            isBlocked = true;
                            blockType = 1;
                            // Blocking Screen to rest mode
                            locklogo.Visibility = System.Windows.Visibility.Visible;
                            txtInput.Visibility = System.Windows.Visibility.Hidden;
                            for (int i = 8; i < 35; i++)
                            {
                                var obj = (Label)FindName(labelNames[i]);
                                obj.Visibility = System.Windows.Visibility.Hidden;
                            }
                            goto hasJustBeenBlocked; //jumps all the character processing
                        }
                        if (composition == "-10765")
                        {
                            isBlocked = true;
                            blockType = 2;
                            // Block Screen to read mode
                            txtInput.Width = txtInput.Width * 3;
                            txtInput.Height = txtInput.Height * 3;
                            for (int i = 8; i < 35; i++)
                            {
                                var obj = (Label)FindName(labelNames[i]);
                                obj.Visibility = System.Windows.Visibility.Hidden;
                            }
                            goto hasJustBeenBlocked; //jumps all the character processing
                        }

                        //suggestions' acceptance
                        if (composition == "-701" || composition == "-107")
                        {
                            acceptSuggestion(0);
                            goto acceptedSuggestion;
                        }
                        if (composition == "-123" || composition == "-321")
                        {
                            acceptSuggestion(1);
                            goto acceptedSuggestion;
                        }
                        if (composition == "-345" || composition == "-543")
                        {
                            acceptSuggestion(2);
                            goto acceptedSuggestion;
                        }
                        if (composition == "-567" || composition == "-765")
                        {
                            acceptSuggestion(3);
                            goto acceptedSuggestion;
                        }
                        for (int i = 0; i < 4; i++) //Clear all the suggestion labels
                        {
                            suggestionLabel[i].Content = "";
                        }
                        try
                        {   //Prepare to print a character composed of 2 movements
                            charToPrint = composition.Substring(1, 2);
                        }
                        catch
                        {   //Prepare to print a character composed of a single movement
                            charToPrint = composition.Substring(1, 1);
                        }

                        //Process the "character to be concatenated" / "operation to be done" in the TextBox
                        finalChar = CharacterDecoder.generateCharacter(charToPrint, blockType, primaryKeyboard);
                     
                        
                        //Data colector implementation below
                        if (!File.Exists(path))
                        {
                            using (StreamWriter writer = new StreamWriter(path))
                            {
                                writer.WriteLine("0"); // Tempo Total
                                writer.WriteLine("0"); // Caracteres Total
                                writer.WriteLine("0"); // Erros Total
                                writer.WriteLine(" "); // Vetor de erros
                            }
                        }
                        using(StreamReader readtext = new StreamReader(path))
                        {
                           arquivo[0] = readtext.ReadLine();
                           arquivo[1] = readtext.ReadLine();
                           arquivo[2] = readtext.ReadLine();
                           arquivo[3] = readtext.ReadLine();
                        }

                        if (colectorOn == true && finalChar != "ESPAÇO" && finalChar != "." && finalChar != "DEL")
                        {
                            caracteresTotal++;
                        }

                        if (colectorOn == true && finalChar == "DEL")
                        {
                            erros++;
                            arquivo[3] = arquivo[3] + ";" + txtInput.Text.Substring(txtInput.Text.Length - 1);
                        }

                        if (colectorOn == true && finalChar == "ESPAÇO" || finalChar == ".")
                        {
                            timer.Stop();
                            colectorOn = false;

                            int aux;
                            long aux2;
                            aux2 = Int32.Parse(arquivo[0]);
                            aux2 += timer.ElapsedMilliseconds;
                            arquivo[0] = aux2.ToString();

                            aux = Int32.Parse(arquivo[1]);
                            aux += caracteresTotal;
                            arquivo[1] = aux.ToString();

                            aux = Int32.Parse(arquivo[2]);
                            aux += erros;
                            arquivo[2] = aux.ToString();

                            caracteresTotal = 0;
                            erros = 0;

                        }

                        if (colectorOn == false && finalChar != "ESPAÇO" && finalChar != "." && finalChar != "DEL")
                        {
                            colectorOn = true;
                            timer.Start();
                            caracteresTotal++;
                        }

                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            writer.WriteLine(arquivo[0]); // Tempo Total
                            writer.WriteLine(arquivo[1]); // Caracteres Total
                            writer.WriteLine(arquivo[2]); // Erros Total
                            writer.WriteLine(arquivo[3]); // Vetor de erros
                        }


                        switch (finalChar)
                        {
                            case "ESPAÇO": 
                                finalChar = " ";
                                txtInput.AppendText(finalChar); //append a space
                                Suggester.indexWord(currentWord);
                                currentWord = "";
                                break;
                            case ".":
                                nextIsUpper = true;
                                txtInput.AppendText(finalChar + " ");
                                break; //append a point and a space
                            case "DEL":
                                try
                                {   //delete a character. If it was in upper case, the next character to be written must be written in upper case too
                                    if (txtInput.Text.Substring(txtInput.Text.Length - 1) == ".")
                                        nextIsUpper = false;
                                    else
                                    {
                                        if (txtInput.Text.Substring(txtInput.Text.Length - 1) == txtInput.Text.Substring(txtInput.Text.Length - 1).ToUpper())
                                            nextIsUpper = true;
                                    }
                                    string aux = txtInput.Text.Substring(0, txtInput.Text.Length - 1);

                                    if (currentWord.Length > 0) 
                                        currentWord = currentWord.Substring(0, currentWord.Length - 1);
                                    //TODO: take last word entered if the entire currentWord has been deleted

                                    addSuggestionsToLabels();

                                    txtInput.Text = "";
                                    txtInput.AppendText(aux);
                                }
                                catch { }
                                break;
                            default: //if it's not space, '.' or delete operations, so the character must be simply added to the TextBox

                                if (!nextIsUpper)
                                    txtInput.AppendText(finalChar.ToLower());
                                else
                                    txtInput.AppendText(finalChar);
                                currentWord += finalChar; //currentWord receives the last char

                                addSuggestionsToLabels();

                                nextIsUpper = false; //after the upper case letter has been already entered, the next one will be lower for sure
                                break;
                        }
                    }                   
                    else
                        isCancelled = !isCancelled; //the letter that was cancelled wasn't processed and the next may not be cancelled
                    break; //breaking the default case
            }
acceptedSuggestion:
didntLeaveCentre:
hasJustBeenBlocked:
            currentWord = "";            
            primaryKeyboard = true;
            composition = "-";
            centreProcessed = true;
        }

        //accepts one of the suggestions
        public void acceptSuggestion(int which)
        {
            if (suggestionLabel[which].Content != "")
            {
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - currentWord.Length - 1);
                txtInput.Text += suggestionLabel[which].Content + " ";
                for (int i = 0; i < 4; i++) //Clear all the suggestion labels
                {
                    suggestionLabel[i].Content = "";
                }
            }
            
        }

        public void addSuggestionsToLabels()
        {
            suggestionsList = Suggester.getSuggestions(currentWord);
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    suggestionLabel[i].Content = suggestionsList[i];
                }
                catch
                {
                    break;
                }
            }
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            Suggester.saveInDisk();
        }
    }
}
