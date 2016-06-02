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
        bool nextIsUpper = true; //variavel que diz se a proxima letra a ser digitada é maiuscula
        string firstPosition; 
        string composition; //composição de caracteres para formar determinada letra (somente os dois primeiros caracteres serão considerados
        bool isCancelled; //diz se a letra foi cancelada
        bool centreProcessed = false; 
        string charToPrint;
        bool firstKeyboard;
        int blockType;
        bool isBlocked;
        const int initialHeight = 720;
        const int initialWidth = 1280;
        string lastPanel = "";
        string[] names;
        string[] names2;
        bool isBetsy;
        Label lbl_aux;

        /// <summary>
        /// Adapts all the controls to the resolution of the user's screen
        /// </summary>
        private void changeObjectsSize() //função responsável por deixar todas 
        {
            isBetsy = false;   
            double hProportion = this.ActualHeight / initialHeight;
            double wProportion = this.ActualWidth / initialWidth;
            names = new string[10] {"pMain", "p0", "p1", "p2", "p3", "p4", "p5", "p6", "p7", "p8" };
            names2 = new string [35] { "min0","min1", "min2", "min3", "min4", "min5", "min6", "min7", "min00", "min01", "min02", "min03", "min10", "min11", "min20", "min21", "min22", "min23", "min30", "min31", "min32", "min33", "min34", "min35", "min40", "min41", "min42", "min43", "min44", "min45", "min50", "min51", "min60", "min70", "min71" };
            foreach (string name in names) //proporciona os grids para se adaptarem a qualquer tamanho de tela
            {
                var obj = (Grid)FindName(name);
                obj.Height *= hProportion;
                obj.Width *= wProportion;
                obj.Margin = new Thickness(obj.Margin.Left * wProportion, obj.Margin.Top * hProportion, obj.Margin.Right * wProportion, obj.Margin.Bottom * hProportion);
            }
            foreach (string name in names2) //proporciona as miniaturas para se adaptarem a qualquer tamanho de tela
            {
                var obj = (Label)FindName(name);
                obj.Height *= hProportion;
                obj.Width *= wProportion;
                obj.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
                obj.Margin = new Thickness(obj.Margin.Left * wProportion, obj.Margin.Top * hProportion, obj.Margin.Right * wProportion, obj.Margin.Bottom * hProportion);
            }
            txtInput.Height *= hProportion;
            txtInput.Width *= wProportion;
            txtInput.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2)); //faz a proporção da fonte para qualquer tamanho de tela
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //valores iniciais do sistema
            firstPosition = " ";
            composition = "-";
            isCancelled = false;
            charToPrint = "";
            firstKeyboard = true;
            blockType = 0;
            isBlocked = false;
            changeObjectsSize();
        }
        /// <summary>
        /// Processes the Grid the user is currently looking at
        /// </summary>
        /// <param name="index">The index of the current Grid</param>
        private void BorderProcessing(string index)
        {
            composition += index;
            if (isBetsy) //if it's DEL, SPACE or .
            {
                if (composition.Length == 3)
                {
                    if (index == "6")
                    {
                        charToPrint = "V";
                        lbl_aux = (Label)FindName("min" + index);
                        lbl_aux.Content = charToPrint;
                    }
                    else
                    {
                        charToPrint = "A";
                        lbl_aux = (Label)FindName("min" + index);
                        lbl_aux.Content = charToPrint;
                    }
                    return;
                }
                else
                {
                    lbl_aux = (Label)FindName("min" + index);
                    lbl_aux.Content = charToPrint;
                }
                return;
            }
            if (composition.Length == 2 && index == "5") //if it's a movement to the space first (has to change font)
            {
                for (int i = 0; i <= 7; i++)
                {
                    var obj = (Label)FindName("min" + i.ToString());
                    obj.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Betsy Flanagan 2"); 
                }
                isBetsy = true;
                charToPrint = "D";
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
            {
                charToPrint = composition.Substring(1, 2);
            }
            catch
            {
                charToPrint = composition.Substring(1, 1);
            }
            
            if (!isBlocked)
                charToPrint = CharacterDecoder.generateCharacter(charToPrint, blockType, firstKeyboard);
            else
                charToPrint = "-";             
            lbl_aux = (Label)FindName("min"+index);
            if (!isBetsy)
                lbl_aux.Content = charToPrint;
            else
            {
                
            }
        }
        private void centreMouseEnter(object sender, MouseEventArgs e)
        {
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
            if (centreProcessed)
                return;
            if (!isBlocked)
            {
                if (blockType == 1)
                {
                    isBlocked = true;
                    blockType = 0;
                    //BLOCK IMPLEMENTATION HERE
                }
                else
                    if (blockType == 2)
                    {
                        isBlocked = true;
                        blockType = 0;
                        //BLOCK IMPLEMENTATION 2 HERE (full reading experience)
                    }
                    else
                        if (!isCancelled)
                        {
                            if (composition != "-")
                            {
                                if (composition == "-12345")
                                {
                                    blockType = 1; //block 1
                                }
                                try
                                {
                                    charToPrint = composition.Substring(1, 2);
                                }
                                catch
                                {
                                    charToPrint = composition.Substring(1, 1);
                                }


                                finalChar = CharacterDecoder.generateCharacter(charToPrint, blockType, firstKeyboard);

                                

                                switch(finalChar)
                                {
                                    case "ESPAÇO":
                                        finalChar = " ";
                                        txtInput.AppendText(finalChar);
                                        break;
                                    case ".":
                                        nextIsUpper = true;
                                        txtInput.AppendText(finalChar + " ");
                                        break;
                                    case "DEL":
                                        try
                                        {
                                            if (txtInput.Text.Substring(txtInput.Text.Length) == txtInput.Text.Substring(txtInput.Text.Length).ToUpper())
                                                nextIsUpper = true;
                                            string aux = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
                                            txtInput.Text = "";
                                            txtInput.AppendText(aux);
                                        }
                                        catch { }
                                        break;
                                    default:
                                        try
                                        {
                                            if (!nextIsUpper)
                                                txtInput.AppendText(finalChar.ToLower());
                                            else
                                                txtInput.AppendText(finalChar);
                                        }
                                        catch
                                        {
                                            txtInput.AppendText(finalChar);
                                        }
                                        nextIsUpper = false;
                                        break;
                                }
                               
                            }

                            /*txtInput.Text += composition;
                            txtInput.SelectionStart = txtInput.Text.Length;
                            txtInput.ScrollToCaret();*/


                            //tratar o caractere final para dizer o que efetivamente será escrito de acordo com o que foi gerado e com o que tiver antes!
                            //tratar casos como espaço, delete e ponto de maneira diferenciada, bem como letras maiusculas e tal!
                        }
            }
            else
            {
                if (blockType != 0)
                {
                    //codigo de desbloqueio
                }
            }

            composition = "-";
            firstPosition = " ";
            centreProcessed = true;
        }
        private void borderMouseLeave(object sender, EventArgs e)
        {
            Grid g = (Grid)sender;
            string name = g.Name;
            string index = name.Substring(1, 1);
            //int x = Convert.ToInt32(index);
            //SensitiveArea[x].BackColor = Color.Transparent;
            Label obj = (Label)FindName("min" + index);
            obj.Content = "";
        }


        private void panelMouseEnter(object sender, MouseEventArgs e)
        {
            Panel panel = ((Panel)sender);
            string name = panel.Name;
            string index = name.Substring(1, 1);

            //int x = Convert.ToInt32(index.Substring(0, 1));
            //SensitiveArea[x].BackColor = Color.FromArgb(20,20,20);

            if (composition == "-")
                firstPosition = index;

            centreProcessed = false;
            Grid x = (Grid)sender;
            if (lastPanel == x.Name)
                return;
            BorderProcessing(index);
            lastPanel = x.Name;
        }
    }
}
