using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardAndSuggester
{
    public partial class frmApplication : Form
    {
        public frmApplication()
        {
            InitializeComponent();
        }

        int screenHeight;
        int screenWidth;
        float fontSize;
        float font_inicial;
        float font_side;
        double ratio_width;
        double ratio_height;
        double ratio;
        double size_font_big;
        float screen_ratio;
        double size_font_small;
        bool nextIsUpper = true;                      
        string firstPosition;
        string composition;
        bool isCancelled;
        string charToPrint;
        bool firstKeyboard;
        int blockType;
        bool isBlocked;
        TextBox txtCentre;
        Panel[] SensitiveArea = new Panel[9];
        MyLabel[] SymbolChar = new MyLabel[8];
        CharacterDecoder Decoder;
        CharacterGuide [] guide;

        //Miniaturas!
        MyLabel[] labelA = new MyLabel[3];
        MyLabel labelB = new MyLabel();
        MyLabel[] labelC = new MyLabel[3];
        MyLabel[] labelD = new MyLabel[3];
        MyLabel[] labelE = new MyLabel[3];
        MyLabel labelF = new MyLabel();
        MyLabel[] labelG = new MyLabel[3];
        MyLabel[] labelH = new MyLabel[3];
        
        
        //**********************************************
        //COISAS A FAZER
        //PORTAR TODAS AS MINIATURAS PRA CÁ!
        //fazer uma imagem com as linhas que seja adaptável (talvez duas, uma pra tela 4:3 e outra pra 16:9
        //Criar as labels referentes a cada canto
        //ver se acha a tal da fonte betsy
        //**********************************************

        protected override void OnPaint(PaintEventArgs e)
        {
            //FAZER AS LINHAS AQUI DE ACORDO COM AS LOCALIZAÇÕES RELATIVAS DO PROJETO JAVASCRIPT!
            // If there is an image and it has a location, 
            // paint it when the Form is repainted.
            base.OnPaint(e);
            Pen pen = new Pen(Color.FromArgb(50, 50, 50),4);
            e.Graphics.DrawLine(pen, 150, 150, 1216, 618);    
         }


        public void txtCentre_Click(object sender, EventArgs e)
        {
            txtCentre.ForeColor = Color.Yellow;
        }

        private void frmApplication_Load(object sender, EventArgs e)
        {
            Decoder = new CharacterDecoder();
            firstPosition = " ";
            composition = "-";
            isCancelled = false;
            charToPrint = "";
            firstKeyboard = true;
            blockType = 0;
            isBlocked = false;

            this.label1.ForeColor = Color.White;
            this.label1.Font = new Font("KEYS", this.label1.Font.Size);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;

            txtCentre = new TextBox();
            txtCentre.Click += new EventHandler(txtCentre_Click);

            screenHeight = Screen.PrimaryScreen.Bounds.Height;
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            fontSize = (float)screenHeight / 20; //5%

            font_inicial = 30;
            font_side = font_inicial/(float)1.4142;
            ratio_width = screenWidth/font_side;
            ratio_height = screenHeight/font_side;
            ratio = Math.Sqrt((ratio_width*ratio_width) + (ratio_height*ratio_height));
            ratio = ratio/(float)69.23;
            size_font_big = (float)(font_inicial*ratio);
            screen_ratio = ((screenWidth/screenHeight)*(float)0.5)/(float)1.778;
            size_font_small = size_font_big*screen_ratio;

            //Desenhos de miniaturas
            guide = new CharacterGuide[8];
            DefineDrawingPositions();

            



            //FOR PARA CRIACAO DAS AREAS E DE SUAS LABELS (APENAS ASPECTOS GERAIS)
            for (int i = 0; i < 9; i++)
            {
                SensitiveArea[i] = new Panel();
                if (i%2==0)
                    SensitiveArea[i].BackColor = Color.FromArgb(i * 13+50, i * 8, i * 3);
                else
                    SensitiveArea[i].BackColor = Color.FromArgb(i * 3, i * 7, i * 12);

                SensitiveArea[i].MouseEnter += new EventHandler(SensitiveArea_MouseEnter);//pega quando entra na area
                SensitiveArea[i].Click += new EventHandler(GlobalMouseClickEventHandler);//pega o click pra mostrar ou não as areas de contato
                if (i != 8)
                {
                    SymbolChar[i] = new MyLabel();
                    SensitiveArea[i].MouseLeave += new EventHandler(SensitiveArea_MouseLeave);
                    SensitiveArea[i].Controls.Add(SymbolChar[i]);
                    SymbolChar[i].Font = new Font("Arial", fontSize);
                    SymbolChar[i].Width *= 6;
                    SymbolChar[i].TextAlign = ContentAlignment.BottomRight;
                    
                }
                SensitiveArea[i].BackColor = Color.Transparent;
                SensitiveArea[i].Name = "Area" + i.ToString();
                //SensitiveArea[i].Visible = false; //DESATIVAR ISSO AQUI DEPOIS
                this.Controls.Add(SensitiveArea[i]);
                
            }
            //
            
            //Definicao de tamanhos de cada panel
            SensitiveArea[0].Size = new Size((int)(screenWidth * 0.35), (int)(screenHeight * 0.35));
            SensitiveArea[1].Size = new Size((int)(screenWidth * 0.3), (int)(screenHeight * 0.2));
            SensitiveArea[2].Size = new Size((int)(screenWidth * 0.4), (int)(screenHeight * 0.35));
            SensitiveArea[3].Size = new Size((int)(screenWidth * 0.3), (int)(screenHeight * 0.3));
            SensitiveArea[4].Size = new Size((int)(screenWidth * 0.4), (int)(screenHeight * 0.4));
            SensitiveArea[5].Size = new Size((int)(screenWidth * 0.4), (int)(screenHeight * 0.3));
            SensitiveArea[6].Size = new Size((int)(screenWidth * 0.4), (int)(screenHeight * 0.4));
            SensitiveArea[7].Size = new Size((int)(screenWidth * 0.2), (int)(screenHeight * 0.3));
            //Definicoes dos Labels de cada panel
            for (int i = 0; i < 8; i++ )
            {
                //SymbolChar[i].Text = "a"; PARA ALINHAR
                SymbolChar[i].BringToFront();
                SymbolChar[i].Height = (int)fontSize*2;
                SymbolChar[i].Enabled = false;
                SymbolChar[i].BackColor = Color.Transparent;
                SymbolChar[i].ForeColor = Color.White;
            }
            //Isso ta uma zona mas funciona
            SymbolChar[0].Location = new Point(SensitiveArea[0].Width / 5, SensitiveArea[1].Height / 4);
            SymbolChar[1].Location = new Point(SensitiveArea[1].Width * 2 / 5, SensitiveArea[1].Height / 4);
            SymbolChar[2].Location = new Point(SensitiveArea[2].Width *3/ 5, SensitiveArea[1].Height / 4);
            SymbolChar[3].Location = new Point(SensitiveArea[2].Width / 5, SensitiveArea[3].Height / 4);
            SymbolChar[5].Location = new Point(SensitiveArea[1].Width * 2 / 5, (int)SymbolChar[5].Font.Size*3/2 - SensitiveArea[1].Height / 4);
            SymbolChar[6].Location = new Point(SensitiveArea[0].Width / 5, SymbolChar[5].Location.Y *7);
            SymbolChar[4].Location = new Point(SensitiveArea[2].Width * 3 / 5, SymbolChar[6].Location.Y);
            SymbolChar[7].Location = new Point(SensitiveArea[0].Width / 5, SensitiveArea[7].Height / 4);
            
                //Definicao das localizacoes de cada panel
            SensitiveArea[0].Location = new Point(0, 0);
            SensitiveArea[1].Location = new Point((int)(screenWidth * 0.35), 0);
            SensitiveArea[2].Location = new Point((int)(screenWidth * 0.65), 0);
            SensitiveArea[3].Location = new Point((int)(screenWidth * 0.80), (int)(screenHeight * 0.35));
            SensitiveArea[4].Location = new Point((int)(screenWidth * 0.65), (int)(screenHeight * 0.65));
            SensitiveArea[5].Location = new Point((int)(screenWidth * 0.35), (int)(screenHeight * 0.8));
            SensitiveArea[6].Location = new Point(0, (int)(screenHeight * 0.65));
            SensitiveArea[7].Location = new Point(0, (int)(screenHeight * 0.35));

            //Definicao do tamanho da central:
            SensitiveArea[8].Size = new Size((int)(screenWidth * 0.6), (int)(screenHeight * 0.6));
            SensitiveArea[8].Location = new Point((int)(screenWidth * 0.2), (int)(screenHeight * 0.2));
            SensitiveArea[8].Visible = true;
            SensitiveArea[8].BackColor = Color.Transparent;
            SensitiveArea[8].Click += new EventHandler(GlobalMouseClickEventHandler);//pega o click pra mostrar ou não as areas de contato
            SensitiveArea[8].BringToFront();   

            //Definicoes da textBox
            txtCentre.Name = "txtCentre";
            txtCentre.Multiline = true;
            txtCentre.Font = new Font("Arial", fontSize);
            txtCentre.ForeColor = Color.FromArgb(120, 120, 120);
            txtCentre.Width = (int)(screenWidth * 0.2); //20% da largura da tela
            txtCentre.Height = (int)(screenHeight *0.18); //20% da altura da tela
            txtCentre.Location = new Point(SensitiveArea[8].Width/3, SensitiveArea[8].Height/3);
            txtCentre.BackColor = Color.FromArgb(20, 20, 20);
            txtCentre.Click += new EventHandler(GlobalMouseClickEventHandler);
            txtCentre.Visible = true;
            txtCentre.WordWrap = true;
            txtCentre.ScrollBars = ScrollBars.Vertical;
            SensitiveArea[8].Controls.Add(txtCentre);

            
        }

        private void CentreProcessing()
        {
            string finalChar;

            if (!isBlocked)
            {
                if (blockType == 1)
                {
                    isBlocked = true;
                    blockType = 0;
                    //codigo de bloqueio
                }
                else
                    if (blockType == 2)
                    {
                        isBlocked = true;
                        blockType = 0;
                        //codigo de bloqueio 2
                    }
                    else
                        if (!isCancelled)
                        {
                            if (composition != "-")
                            {
                                try
                                {
                                    charToPrint = composition.Substring(1, 2);
                                }
                                catch
                                {
                                    charToPrint = composition.Substring(1, 1);
                                }


                                finalChar = Decoder.generateCharacter(charToPrint, blockType, firstKeyboard);

                                if (finalChar == "ESPAÇO")
                                {
                                    finalChar = " ";
                                    txtCentre.AppendText(finalChar);
                                }
                                else
                                {
                                    if (finalChar == ".")
                                    {
                                        nextIsUpper = true;
                                        txtCentre.AppendText(finalChar + " ");
                                    }
                                    else
                                    {
                                        if (finalChar == "DEL")
                                        {
                                            try
                                            {
                                                if (txtCentre.Text.Substring(txtCentre.Text.Length) == txtCentre.Text.Substring(txtCentre.Text.Length).ToUpper())
                                                    nextIsUpper = true;
                                                string aux = txtCentre.Text.Substring(0, txtCentre.Text.Length - 1);
                                                txtCentre.Text = "";
                                                txtCentre.AppendText(aux);
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (!nextIsUpper)
                                                    txtCentre.AppendText(finalChar.ToLower());
                                                else
                                                    txtCentre.AppendText(finalChar);
                                            }
                                            catch
                                            {
                                                txtCentre.AppendText(finalChar);
                                            }
                                            nextIsUpper = false;
                                        }
                                    }
                                }
                            }
                                
                            /*txtCentre.Text += composition;
                            txtCentre.SelectionStart = txtCentre.Text.Length;
                            txtCentre.ScrollToCaret();*/
                           
                            
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
        }

        private void BorderProcessing(string index)
        {
            try
            {
                if (index == composition.Substring(1, 2))
                {
                    blockType = 0;
                    isCancelled = true;
                    composition = "ii"; //letra que não será escrita
                }
            }
            catch
            {
                //index = "";
            }
            composition += index;
            string print;
            try
            {
                print = composition.Substring(1, 2);
            }
            catch
            {
                print = composition.Substring(1, 1);
            }
            if (composition == "12345")
            {
                blockType = 1; //primeiro bloqueio
            }
            if (!isBlocked)
                charToPrint = Decoder.generateCharacter(print,blockType,firstKeyboard);
            else
                charToPrint = "-";
            SymbolChar[Convert.ToInt32(index)].Text = charToPrint;
                
        }

        private void SensitiveArea_MouseEnter(object sender, EventArgs e)
        {

            //assim que entrar tem que contar tempo pra alternar entre teclados primario e secundário!!!!!!!!!!
            
            Panel panel = ((Panel)sender);
            string name = panel.Name;
            string index = name.Substring(4, 1);

            int x = Convert.ToInt32(index.Substring(0, 1));
            if (x!=8)
                SensitiveArea[x].BackColor = Color.FromArgb(20,20,20);

            if (composition == "-")
                firstPosition = index;

            if (index == "8")//centro
            {
                CentreProcessing();
            }
            else //se não for centro
            {
                BorderProcessing(index);
            }
        }        

        private void SensitiveArea_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = ((Panel)sender);
            string name = panel.Name;
            string index = name.Substring(4,1);
            int x = Convert.ToInt32(index);
            SensitiveArea[x].BackColor = Color.Transparent;
            SymbolChar[x].Text = "";
        }

        private void GlobalMouseClickEventHandler(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                SensitiveArea[i].BackColor = Color.Transparent;
            }
        }

        private void DefineDrawingPositions()
        {
            
  /* A */ guide[0] = new CharacterGuide{keyboard=false,
                             width1A=screenWidth*0.02,
                             width1C=screenWidth*0.385-size_font_small,width2C=screenWidth*0.385,width3C=screenWidth*0.385-size_font_small,
                             width1D=screenWidth*0.385,width2D=screenWidth*0.385+size_font_small,width3D=screenWidth*0.385,
                             height1A=size_font_big*2 ,
                             height1C=screenHeight*0.37, height2C=screenHeight*0.37,height3C=screenHeight*0.37+size_font_small,
                             height1D=screenHeight*0.37+size_font_small, height2D=screenHeight*0.37+size_font_small,height3D=screenHeight*0.37+size_font_small*2,
                             text1A="A", text1B="B", text1C="C",text2A="0", text2B="Z",text2C="Y"
                            };
  /*  E */ guide[1] = new CharacterGuide{ keyboard= false,
                             width1A=screenWidth*0.02,
                             width1C=screenWidth*0.368,width2C=screenWidth*0.368,width3C=screenWidth*0.368,
                             width1D=screenWidth*0.385,width2D=screenWidth*0.385,width3D=screenWidth*0.385,
                             height1A=screenHeight*0.52 ,
                             height1C=screenHeight*0.485, height2C=screenHeight*0.485+size_font_small,height3C=screenHeight*0.485+size_font_small*2 ,
                             height1D=screenHeight*0.485, height2D=screenHeight*0.485+size_font_small,height3D=screenHeight*0.485+size_font_small*2 ,
                             text1A="T", text1B="U", text1C="V",text2A="K", text2B="W",text2C="X"
                           };
  /* H */ guide[2] = new CharacterGuide{ keyboard=false,width1A=screenWidth*0.01,
                            width1B=screenWidth*0.01+size_font_big,width2B=screenWidth*0.01+size_font_big,width3B=9999,
                            width1C=screenWidth*0.387,width2C=screenWidth*0.387,width3C=9999,
                            height1A=screenHeight*0.95,
                            height1B=screenHeight*0.95-size_font_small, height2B=screenHeight*0.95,height3B=9999,
                            height1C=screenHeight*0.60, height2C=screenHeight*0.60+size_font_small,height3C=9999,
                            text1A="Q", text1B="S R", text1C="",text2A="", text2B="",text2C=""
                            };
  /* F */ guide[3] = new CharacterGuide{ keyboard=false,
                             width1A=screenWidth*0.965,
                             width1C=screenWidth*0.59+size_font_small,width2C=screenWidth*0.59+size_font_small*2,width3C=screenWidth*0.59+size_font_small*2,
                             width1D=screenWidth*0.59,width2D=screenWidth*0.59+size_font_small,width3D=screenWidth*0.59+size_font_small,
                             height1A=size_font_big*2,
                             height1C=screenHeight*0.37, height2C=screenHeight*0.37,height3C=screenHeight*0.37+size_font_small ,
                             height1D=screenHeight*0.37+size_font_small, height2D=screenHeight*0.37+size_font_small,height3D=screenHeight*0.37+size_font_small*2 ,
                             text1A=" G", text1B=" I", text1C="H",text2A="4", text2B="5",text2C="6"
                           };
  /* G */ guide[4] = new CharacterGuide{ keyboard=false,
                            width1A=screenWidth*0.95+size_font_big ,
                            width1C=screenWidth*0.59+size_font_small,width2C=screenWidth*0.59,width3C=0, 
                            height1A=screenHeight*0.95, 
                            height1C=screenHeight*0.60,height2C=screenHeight*0.60+size_font_small,height3C=screenHeight*0.60+size_font_small, 
                            text1A="N", text1B="P O", text1C="",text2A="", text2B="", text2C=""
                            };
  /*  D */ guide[5] = new CharacterGuide{ keyboard=false,
                             width1A=screenWidth*0.94+size_font_big ,
                             width1C=screenWidth*0.59+size_font_small*2,width2C=screenWidth*0.59+size_font_small*2,width3C=screenWidth*0.59+size_font_small*2,
                             width1D=screenWidth*0.59+size_font_small,width2D=screenWidth*0.59+size_font_small,width3D=screenWidth*0.59+size_font_small,
                             height1A=screenHeight*0.51,
                             height1C=screenHeight*0.485,height2C=screenHeight*0.485+size_font_small,height3C=screenHeight*0.485+size_font_small*2,
                             height1D=screenHeight*0.485,height2D=screenHeight*0.485+size_font_small,height3D=screenHeight*0.485+size_font_small*2,
                             text1A=" J", text1B=" L", text1C=" M",text2A=" 7", text2B=" 8", text2C=" 9"
                           };
  /*  C */ guide[6] = new CharacterGuide{ keyboard=false,
                             width1A=screenWidth*0.49,
                             width1C=screenWidth*0.50-size_font_small*1.3,width2C=screenWidth*0.50-size_font_small*0.3,width3C=screenWidth*0.50+size_font_small*0.7,
                             width1D=screenWidth*0.50-size_font_small*1.3,width2D=screenWidth*0.50-size_font_small*0.3,width3D=screenWidth*0.50+size_font_small*0.7,
                             height1A=size_font_big*2, 
                             height1C=screenHeight*0.37,height2C=screenHeight*0.37,height3C=screenHeight*0.37,
                             height1D=screenHeight*0.37+size_font_small,height2D=screenHeight*0.37+size_font_small,height3D=screenHeight*0.37+size_font_small,
                             text1A="D", text1B="E", text1C="F",text2A="1", text2B="2", text2C="3"
                           };
  /*  B */ guide[7] = new CharacterGuide{ keyboard=false,
                             width1A=screenWidth*0.49,
                             width1C=screenWidth*0.50-size_font_small*4,width2C=0,width3C=0,
                             height1A=screenHeight*0.95, 
                             height1C=screenHeight*0.63+size_font_small,height2C=0,height3C=0,
                             text1A="V D A", text1B="", text1C="",text2A="", text2B="", text2C=""
  };

        }
        
    }
}
