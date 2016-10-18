using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using AutoComplete.Classes;
using System.IO;
using System.Diagnostics;
using System.Media;
using WorkingWithXAML.Classes;
//using System.Net.Sockets;
//using Newtonsoft.Json.Linq;


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
        TextBlock[] suggestionLabel;
        StackPanel[] suggestionStackPanel;
        double suggestionFontCoefficient = 155;
        //required adjustments for the second blocking mode (reading mode)
        double textBoxMarginCoefH = 0.308;
        double textBoxMarginCoefW = 0.367;
        bool[] naturalSugFontSize = new bool[4] { true, true, true, true };
        double[] naturalSugWidth;
        string currentWord;
        List<string> suggestionsList;
        Timer keyboardTimer = new Timer(); //timer reference for secondary keyboard
        bool isBetsy;
        Label lbl_aux;       
        bool colectorOn = false;
        int erros = 0;
        int caracteresTotal = 0;
        string[] arquivo = new string[4];
        //Stopwatch timer = new Stopwatch();

        //for user tests!
        string path = @"data"+"-"+DateTime.Now.ToShortDateString().Replace("/","_")+"-"+DateTime.Now.ToShortTimeString().Replace(":", ".") + ".txt";
        Stopwatch dataTimer = new Stopwatch(); //gets the time elapsed since the program was open
        string characterTimestampRecord;
        // enteredCharacter1|number1(milliseconds)-enteredCharacter2|number2(milliseconds)
         
        SoundPlayer alarm, loudAlarm, stateTransition;
        GazePoint gp;


        #endregion

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            alarm = new SoundPlayer(@"..\..\Alarms\alarm.wav");
            //alarm.Play();
            loudAlarm = new SoundPlayer(@"..\..\Alarms\loudAlarm.wav");
            //loudAlarm.Play();
            stateTransition = new SoundPlayer(@"..\..\Alarms\stateTransition.wav");
            //stateTransition.Play();
            //stateTransition.PlayLooping();

            //Mouse.OverrideCursor = Cursors.None; //remove mouse            

            //initial system values
            composition = "-";
            isCancelled = false;
            charToPrint = "";
            primaryKeyboard = true;
            blockType = 0;
            isBlocked = false;
            //suggestionLabel = new Label[4];
            suggestionLabel = new TextBlock[4];
            for (int i = 1; i < 5; i++) //References the suggestion labels with an array and clear their "Content" property
            {
                    suggestionLabel[i - 1] = (TextBlock)(FindName("sug" + i));
                    suggestionLabel[i - 1].Text = "";
               
            }
            suggestionStackPanel = new StackPanel[4];
            for (int i = 1; i < 5; i++) //References the suggestion labels with an array and clear their "Content" property
            {                
                suggestionStackPanel[i - 1] = (StackPanel)(FindName("view" + i));

            }
            changeObjectsSize();
            //initialize the Suggester
            Suggester.initializeSuggester();
            suggestionsList = new List<string>();
            currentWord = "";
            //every 2 seconds after the keyboardTimer has started, the changeKeyboard function will be called 
            keyboardTimer.Interval = 2000; 
            keyboardTimer.Elapsed += new ElapsedEventHandler(changeKeyboard);

            dataTimer.Start();
            //EyeTracker data retrieval
            try
            {
                //gp = new GazePoint();
            }
            catch
            {
                Console.WriteLine("No camera found! Mouse mode! :)");
            }
            
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Suggester.saveInDisk();
            try
            {
                gp.OnExit();
            }
            catch
            {
                Console.WriteLine("Didn't close camera (probably in mouse mode, if not, something's wrong!");
            }
            if (!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(characterTimestampRecord);
                }
            }
        }

        #endregion

        #region Procedures and Functions

        /// <summary>
        /// Adjust the textblock and stackpanel size and orientation according to the suggestion's size
        /// </summary>
        /// <param name="which">which suggestion needs adaptation</param>
        /// <param name="rollback">true = the suggestion is short so the size and orientation must 
        /// rollback to the default; false otherwise</param>
        private void adjustSuggestionSize(int which, bool rollback)
        {
            //The margin can only be modified through a Thickness object
            Thickness newMargin = suggestionStackPanel[which].Margin;
            if (which%2==0)
            {                
                if (rollback)
                {
                    suggestionLabel[which].HorizontalAlignment = HorizontalAlignment.Left;
                    newMargin.Left += suggestionFontCoefficient*1.1;
                    suggestionStackPanel[which].Width = naturalSugWidth[which];
                }
                else
                {
                    suggestionLabel[which].HorizontalAlignment = HorizontalAlignment.Right;
                    newMargin.Left -= suggestionFontCoefficient*1.1;
                    suggestionStackPanel[which].Width = naturalSugWidth[which] + suggestionFontCoefficient;
                }
                suggestionStackPanel[which].Margin = newMargin;               
            }
            else
            {         
                if (rollback)
                {
                    suggestionLabel[which].HorizontalAlignment = HorizontalAlignment.Right;
                    suggestionStackPanel[which].Width = naturalSugWidth[which];
                }
                else
                {
                    suggestionLabel[which].HorizontalAlignment = HorizontalAlignment.Left;
                    suggestionStackPanel[which].Width = naturalSugWidth[which] + suggestionFontCoefficient;
                }
            }
        }


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
            naturalSugWidth = new double[4];
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
                suggestionStackPanel[i].Height *= hProportion;
                suggestionStackPanel[i].Width *= wProportion;
                naturalSugWidth[i] = suggestionStackPanel[i].Width; //finds the width after the proportion applied
                suggestionStackPanel[i].Margin = new Thickness(suggestionStackPanel[i].Margin.Left * wProportion, 
                    suggestionStackPanel[i].Margin.Top * hProportion, suggestionStackPanel[i].Margin.Right * wProportion, 
                    suggestionStackPanel[i].Margin.Bottom * hProportion);
                suggestionLabel[i].MaxHeight *= hProportion;
                suggestionLabel[i].MaxWidth *= wProportion;
                suggestionLabel[i].FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
                suggestionLabel[i].Margin = new Thickness(suggestionLabel[i].Margin.Left * wProportion, suggestionLabel[i].Margin.Top * hProportion, suggestionLabel[i].Margin.Right * wProportion, suggestionLabel[i].Margin.Bottom * hProportion);
            }

            //textbox adaptation
            txtInput.Height *= hProportion;
            txtInput.Width *= wProportion;
            txtInput.Margin = new Thickness(txtInput.Margin.Left * wProportion, txtInput.Margin.Top * hProportion, txtInput.Margin.Right * wProportion, txtInput.Margin.Bottom * hProportion);
            //Fonts adaptation
            txtInput.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));

            //suggestion label coefficient
            //155 is the coefficient a margin which left value is 275 (155/275=0.563)(changes with resolution)
            suggestionFontCoefficient = suggestionStackPanel[0].Margin.Left * 0.563;

              
        }
                    
        /// <summary>
        /// Processes the Grid that the user is currently looking at
        /// </summary>
        /// <param name="index">The index of the current Grid</param>
        private void BorderProcessing(string index)
        {
            if (composition[composition.Length - 1] != index[0])
                composition += index;
            if (!isBlocked) //if the environment is currently not blocked, the character for the current composition is generated
            {                
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
                if (composition.Length > 2 && index == composition.Substring(1, 1))
                {
                    blockType = 0;
                    isCancelled = true;
                    composition = "ii"; //character won't be written
                }
                try
                {   //Prepare to print a character composed of 2 movements
                    charToPrint = composition.Substring(1, 2);
                }
                catch
                {   //Prepare to print a character composed of a single movement
                    charToPrint = composition.Substring(1, 1);
                }

                charToPrint = CharacterDecoder.generateCharacter(charToPrint, blockType, primaryKeyboard);
            }
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
                    if (composition == "-12345" || composition == "-10765")
                    {
                        stateTransition.Play();
                        isBlocked = false;
                        blockType = 0; 
                        //Restoring default UI
                        locklogo.Visibility = System.Windows.Visibility.Hidden;
                        txtInput.Visibility = System.Windows.Visibility.Visible;
                        for (int i = 8; i < 35; i++)
                        {
                            var obj = (Label)FindName(labelNames[i]);
                            obj.Visibility = System.Windows.Visibility.Visible;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            suggestionStackPanel[i].Visibility = Visibility.Visible;
                        }
                    }
                    break;                        

                case 2: //Blocked to read full text
                    if (composition == "-12345" || composition == "-10765")
                    {
                        stateTransition.Play();
                        isBlocked = false;
                        blockType = 0;
                        // Restoring default UI
                        txtInput.Margin = new Thickness(txtInput.Margin.Left / textBoxMarginCoefW, 
                            txtInput.Margin.Top / textBoxMarginCoefH, txtInput.Margin.Right / textBoxMarginCoefW,
                            txtInput.Margin.Bottom / textBoxMarginCoefH);
                        for (int i = 8; i < 35; i++)
                        {
                            var obj = (Label)FindName(labelNames[i]);
                            obj.Visibility = System.Windows.Visibility.Visible;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            suggestionStackPanel[i].Visibility = Visibility.Visible;
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
                            stateTransition.Play();
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

                            for (int i = 0; i < 4; i++)
                            {
                                suggestionStackPanel[i].Visibility = Visibility.Hidden;
                            }
                            goto hasJustBeenBlocked; //jumps all the character processing
                        }
                        if (composition == "-10765")
                        {
                            stateTransition.Play();
                            isBlocked = true;
                            blockType = 2;
                            // Block Screen to read mode
                            txtInput.Margin = new Thickness(txtInput.Margin.Left * textBoxMarginCoefW,
                            txtInput.Margin.Top * textBoxMarginCoefH, txtInput.Margin.Right * textBoxMarginCoefW,
                            txtInput.Margin.Bottom * textBoxMarginCoefH);

                            for (int i = 8; i < 35; i++)
                            {
                                var obj = (Label)FindName(labelNames[i]);
                                obj.Visibility = Visibility.Hidden;
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                suggestionStackPanel[i].Visibility = Visibility.Hidden;
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
                        if (composition == "-567" || composition == "-765")
                        {
                            acceptSuggestion(2);
                            goto acceptedSuggestion;
                        }
                        if (composition == "-345" || composition == "-543")
                        {
                            acceptSuggestion(3);
                            goto acceptedSuggestion;
                        }
                       

                        //No suggestion accepted, so go on...
                        for (int i = 0; i < 4; i++) //Clear all the suggestion labels
                        {
                            //suggestionLabel[i].Content = "";
                            suggestionLabel[i].Text = "";
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

                        #region codigoDoColetor
                        /*
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

                        //Erro no coletor aqui!

                        //if (colectorOn == true && finalChar == "DEL")
                        //{
                        //    erros++;
                        //    arquivo[3] = arquivo[3] + ";" + txtInput.Text.Substring(txtInput.Text.Length - 1);
                        //}

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
                        */
                        #endregion

                        
                        // enteredCharacter1|number1(milliseconds)-enteredCharacter2|number2(milliseconds)
                        characterTimestampRecord += finalChar + "|" + dataTimer.ElapsedMilliseconds.ToString() + "-";


                        switch (finalChar)
                        {
                            case "&":
                                goto cancelledCharacter;
                            case "ESPAÇO": 
                                finalChar = " ";
                                txtInput.AppendText(finalChar); //append a space
                                Suggester.indexWord(currentWord);
                                currentWord = "";
                                break;
                            case ".":
                                nextIsUpper = true;
                                if (txtInput.Text.Substring(txtInput.Text.Length - 1) == " ")
                                    txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
                                txtInput.AppendText(finalChar + " ");
                                Suggester.indexWord(currentWord);
                                currentWord = "";
                                break; //append a point and a space
                            case "DEL":
                                string inputContent = txtInput.Text;
                                string aux;
                                try
                                {
                                    aux = inputContent.Substring(0, inputContent.Length - 1);
                                    //delete a character. If it was in upper case, the next character to be written must be written in upper case too
                                    if (inputContent.Substring(inputContent.Length - 1) == ".")
                                        nextIsUpper = false;
                                    else
                                    {   //if the last erased character was a space, do not uppercase the next letter to be inserted                            

                                        if (inputContent.LastIndexOf(" ") != inputContent.Length - 1)
                                        {
                                            if (inputContent.Substring(inputContent.Length - 1) == inputContent.Substring(inputContent.Length - 1).ToUpper())
                                                nextIsUpper = true;

                                            if (currentWord.Length > 0)
                                                currentWord = currentWord.Substring(0, currentWord.Length - 1);
                                        }
                                        else //if it was a space, so the current word is the whole last word
                                        {
                                            //aux is the current text written without the last character (space in this case)
                                            
                                            try
                                            {
                                                //takes the last written word as current word
                                                currentWord = aux.Substring(aux.LastIndexOf(" ") + 1, aux.Length - aux.LastIndexOf(" ") - 1);
                                            }
                                            catch
                                            {
                                                currentWord = aux.Substring(0, aux.Length);
                                            }

                                            //if '.' is the last character, it must be removed too.
                                            if (aux[aux.Length - 1] == '.')
                                            {
                                                currentWord = currentWord.Substring(0, currentWord.Length - 1);
                                                aux = aux.Substring(0, aux.Length - 1);
                                                nextIsUpper = false;
                                            }
                                        }
                                    }
                                    addSuggestionsToLabels();

                                    txtInput.Text = "";
                                    txtInput.AppendText(aux);
                                }
                                catch
                                {
                                    Console.WriteLine("txtInput is empty!");
                                }
                                break;
                            default: //if it's not space, '.' or delete operations, so the character must be simply added to the TextBox
                                
                                if (!nextIsUpper)
                                    finalChar = finalChar.ToLower();

                                txtInput.AppendText(finalChar);
                                currentWord += finalChar; //currentWord receives the last char

                                //TODO: fazer sons tocarem quando digitar jj e jjj em loop e parar quando digitar outro simbolo que não j.

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
cancelledCharacter:
            txtInput.ScrollToEnd(); //always focus the end of the txtBox
            primaryKeyboard = true;
            composition = "-";
            centreProcessed = true;
        }

        //accepts one of the suggestions
        public void acceptSuggestion(int which)
        {
            if (suggestionLabel[which].Text != "")
            {
                string chosenWord = suggestionLabel[which].Text;
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - currentWord.Length);
                if (currentWord[0].ToString().ToUpper() == currentWord[0].ToString())
                {
                    chosenWord = chosenWord.Substring(0, 1).ToUpper() + chosenWord.Substring(1, chosenWord.Length - 1);
                }
                txtInput.Text +=  chosenWord + " ";
                chosenWord.ToLower();
                characterTimestampRecord += chosenWord + "|" + dataTimer.ElapsedMilliseconds.ToString() + "-";
                Suggester.indexWord(chosenWord);
                for (int i = 0; i < 4; i++) //Clear all the suggestion labels
                {
                    suggestionLabel[i].Text = "";
                }
                currentWord = "";
            }
            
        }

        public void addSuggestionsToLabels()
        {
            suggestionsList = Suggester.getSuggestions(currentWord);
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if (suggestionsList[i].Length < 9 && !naturalSugFontSize[i])
                    {
                        naturalSugFontSize[i] = true;
                        adjustSuggestionSize(i, true);
                    }
                    if (suggestionsList[i].Length > 9 && naturalSugFontSize[i])
                    {
                        naturalSugFontSize[i] = false;
                        adjustSuggestionSize(i, false);
                    }
                    suggestionLabel[i].Text = suggestionsList[i];
                }
                catch
                {
                    if (!naturalSugFontSize[i])
                    {
                        naturalSugFontSize[i] = true;
                        adjustSuggestionSize(i, true);
                    }
                    suggestionLabel[i].Text = "";
                }
            }
        }


        #endregion

        //Not using
        #region EyeTrackerManagement

        ////For the Eye Tribe Tracker
        //private TcpClient socket;
        //private System.Threading.Thread incomingThread;
        //private Timer timerHeartbeat;
        //private bool isRunning;

        //public bool ConnectEyeTracker(string host, int port)
        //{
        //    try
        //    {
        //        socket = new TcpClient("LocalHost", 6555);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Out.WriteLine("Error connecting: " + ex.Message);
        //        return false;
        //    }

        //    // Send the obligatory connect request message
        //    string REQ_CONNECT = "{\"values\":{\"push\":true,\"version\":1},\"category\":\"tracker\",\"request\":\"set\"}";
        //    SendTrackerMessage(REQ_CONNECT);

        //    // Lauch a seperate thread to parse incoming data
        //    incomingThread = new System.Threading.Thread(ListenerLoop);
        //    incomingThread.Start();

        //    // Start a timer that sends a heartbeat every 250ms.
        //    // The minimum interval required by the server can be read out 
        //    // in the response to the initial connect request.   

        //    string REQ_HEATBEAT = "{\"category\":\"heartbeat\",\"request\":null}";
        //    timerHeartbeat = new System.Timers.Timer(250);
        //    timerHeartbeat.Elapsed += delegate { SendTrackerMessage(REQ_HEATBEAT); };
        //    timerHeartbeat.Start();

        //    return true;
        //}

        //private void SendTrackerMessage(string message)
        //{
        //    if (socket != null && socket.Connected)
        //    {
        //        StreamWriter writer = new StreamWriter(socket.GetStream());
        //        writer.WriteLine(message);
        //        writer.Flush();
        //    }
        //}

        //public event EventHandler<ReceivedDataEventArgs> OnData;

        //private void ListenerLoop()
        //{
        //    StreamReader reader = new StreamReader(socket.GetStream());
        //    isRunning = true;

        //    while (isRunning)
        //    {
        //        string response = string.Empty;

        //        try
        //        {
        //            response = reader.ReadLine();

        //            JObject jObject = JObject.Parse(response);

        //            Packet p = new Packet();
        //            //p.RawData = jObject["json"].ToString(); not sure

        //            p.Category = (string)jObject["category"];
        //            p.Request = (string)jObject["request"];
        //            p.StatusCode = (string)jObject["statuscode"];

        //            JToken values = jObject.GetValue("values");

        //            if (values != null)
        //            {
        //                /* 
        //                  We can further parse the Key-Value pairs from the values here.
        //                  For example using a switch on the Category and/or Request 
        //                  to create Gaze Data or CalibrationResult objects and pass these 
        //                  via separate events.

        //                  To get the estimated gaze coordinate (on-screen pixels):
        //                  JObject gaze = JObject.Parse(jFrame.SelectToken("avg").ToString());
        //                  double gazeX = (double) gaze.Property("x").Value;
        //                  double gazeY = (double) gaze.Property("y").Value;
        //                */

        //            }

        //            // Raise event with the data
        //            if (OnData != null)
        //                OnData(this, new ReceivedDataEventArgs(p));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.Out.WriteLine("Error while reading response: " + ex.Message);
        //        }
        //    }
        //}


        #endregion





    }
}
