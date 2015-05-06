//tem q pegar dos Cookies.js

function inicio(){
  // Criando editor rich text e vetor de cores
  editor = new Quill('#editor');
  cores = ['rgb(255,255,255)','rgb(255,255,0)'];
  coresCont = 0;

  cursorVisivel = 1;
  cursor();

  // Vetor de cookies
  CookiesDeHoje = "";

  // Cookies
  texto = "";
  indiceCookie = 1;
  var d = new Date();
  var n = new Date();
  var o = new Date(n.getTime()-30*24*60*60*1000); //30 dias atras
  d.setTime(d.getTime() + (30*24*60*60*1000));//30 dias de validade de um cookie

  for (var i = 0; i < 31; i++)
  {
    o.setTime(o.getTime()+24*60*60*1000);
    nomeCookie = o.toString().substring(4,15);
    nomeCookie = trim(nomeCookie);
    while (getCookie(nomeCookie+"_"+indiceCookie) != "")
    {
       CookiesDeHoje += getCookie(nomeCookie+"_"+indiceCookie)+ " ";
       indiceCookie++;
    }
  }
  CookiesDeHoje = enter(CookiesDeHoje);

  timerAutoSave = setInterval(function(){document.cookie = nomeCookie+"_"+indiceCookie+"="+texto+"; expires="+d.toUTCString();}, 5000);

  // Vetor para salvar combinacao
  combination = ["",""];
  indice = 0;
  timerAux = 0;

  // Variavel shift
  shiftaux = false;

  // Definindo tamanho do canvas
  c = document.getElementById("Fundo");
  c.width = window.screen.width;
  c.height= window.screen.height;
  ctx = c.getContext("2d");

  // Definindo proporção e tamanho de fonte inicial
  var font_inicial = 30;
  var font_side = font_inicial/1.4142;
  var ratio_width = c.width/font_side;
  var ratio_height = c.height/font_side;
  var ratio = Math.sqrt((ratio_width*ratio_width) + (ratio_height*ratio_height));
  var ratio = ratio/69.23;
  size_font_big = font_inicial*ratio;
  var screen_ratio = ((c.width/c.height)*0.5)/1.778;
  size_font_small = size_font_big*screen_ratio;


  // Criando vetor com todos os dados usados para desenhar o teclado + o highlight
  vetorDados = [];
  /* A */ vetorDados[0] = { font1:"Arial",font2:"Arial",teclado:false,
                             width1A:c.width*0.02,
                             width1C:c.width*0.385-size_font_big*0.7,width2C:c.width*0.385,width3C:c.width*0.385-size_font_big*0.7,
                             width1D:c.width*0.385,width2D:c.width*0.385+size_font_big*0.6,width3D:c.width*0.385,
                             height1A:size_font_big*2 ,
                             height1C:c.height*0.37, height2C:c.height*0.37,height3C:c.height*0.37+size_font_big*0.7,
                             height1D:c.height*0.37+size_font_big*0.6, height2D:c.height*0.37+size_font_big*0.6,height3D:c.height*0.37+size_font_big*1.2,
                             text1A:"A", text1B:"B", text1C:"C",text2A:"0", text2B:"Z",text2C:"Y",highlightW:0, highlightH:0
                            };
  /*  E */ vetorDados[1] = { font1:"Arial",font2:"Arial",teclado:false,
                             width1A:c.width*0.02,
                             width1C:c.width*0.368,width2C:c.width*0.368,width3C:c.width*0.368,
                             width1D:c.width*0.368+size_font_big*0.6,width2D:c.width*0.368+size_font_big*0.6,width3D:c.width*0.368+size_font_big*0.6,
                             height1A:c.height*0.52 ,
                             height1C:c.height*0.485, height2C:c.height*0.485+size_font_big*0.7,height3C:c.height*0.485+size_font_big*1.4 ,
                             height1D:c.height*0.485, height2D:c.height*0.485+size_font_big*0.7,height3D:c.height*0.485+size_font_big*1.4 ,
                             text1A:'T', text1B:'U', text1C:'V',text2A:'K', text2B:'W',text2C:'X',highlightW:0, highlightH:c.height/2
                           };
  /* H */ vetorDados[2] = { teclado:false,width1A:c.width*0.01,
                            width1B:c.width*0.01+size_font_big,width2B:c.width*0.01+size_font_big,width3B:9999,
                            width1C:c.width*0.368+size_font_big*0.6,width2C:c.width*0.368+size_font_big*0.6,width3C:9999,
                            height1A:c.height*0.95,
                            height1B:c.height*0.95-size_font_big*0.7, height2B:c.height*0.95,height3B:9999,
                            height1C:c.height*0.60, height2C:c.height*0.60+size_font_big*0.7,height3C:9999,
                            text1A:'Q', text1B:'S R', text1C:'',text2A:'', text2B:'',text2C:'',
                            highlightW:0, highlightH:c.height};
  /* F */ vetorDados[3] = { font1:"Arial",font2:"Arial",teclado:false,
                             width1A:c.width*0.965,
                             width1C:c.width*0.59+size_font_big*0.6,width2C:c.width*0.59+size_font_big*1.2,width3C:c.width*0.59+size_font_big*1.2,
                             width1D:c.width*0.59,width2D:c.width*0.59+size_font_big*0.6,width3D:c.width*0.59+size_font_big*0.6,
                             height1A:size_font_big*2,
                             height1C:c.height*0.37, height2C:c.height*0.37,height3C:c.height*0.37+size_font_big*0.6 ,
                             height1D:c.height*0.37+size_font_big*0.6, height2D:c.height*0.37+size_font_big*0.6,height3D:c.height*0.37+size_font_big*1.2 ,
                             text1A:' G', text1B:' I', text1C:'H',text2A:'4', text2B:'5',text2C:'6',highlightW:c.width, highlightH:0
                           };
  /* G */ vetorDados[4] = { font1:"Arial",font2:"Arial",teclado:false,
                            width1A:c.width*0.95+size_font_big ,
                            width1C:c.width*0.59+size_font_big*0.6,width2C:c.width*0.59,width3C:0,
                            height1A:c.height*0.95,
                            height1C:c.height*0.60,height2C:c.height*0.60+size_font_big*0.6,height3C:c.height*0.60+size_font_big*0.6,
                            text1A:"N", text1B:"P O", text1C:'',text2A:'', text2B:'', text2C:'',
                            highlightW:c.width, highlightH:c.height};
  /*  D */ vetorDados[5] = { font1:"Arial",font2:"Arial",teclado:false,
                             width1A:c.width*0.94+size_font_big ,
                             width1C:c.width*0.59+size_font_small*2,width2C:c.width*0.59+size_font_small*2,width3C:c.width*0.59+size_font_small*2,
                             width1D:c.width*0.59+size_font_small,width2D:c.width*0.59+size_font_small,width3D:c.width*0.59+size_font_small,
                             height1A:c.height*0.51,
                             height1C:c.height*0.485,height2C:c.height*0.485+size_font_big*0.7,height3C:c.height*0.485+size_font_big*1.4,
                             height1D:c.height*0.485,height2D:c.height*0.485+size_font_big*0.7,height3D:c.height*0.485+size_font_big*1.4,
                             text1A:" J", text1B:" L", text1C:" M",text2A:" 7", text2B:" 8", text2C:" 9", highlightW:c.width, highlightH:c.height/2
                           };
  /*  C */ vetorDados[6] = { font1:"Arial",font2:"Arial",teclado:false,
                             width1A:c.width*0.49,
                             width1C:c.width*0.50-size_font_big*0.7*1.3,width2C:c.width*0.50-size_font_big*0.7*0.3,width3C:c.width*0.50+size_font_big*0.7*0.7,
                             width1D:c.width*0.50-size_font_big*0.7*1.3,width2D:c.width*0.50-size_font_big*0.7*0.3,width3D:c.width*0.50+size_font_big*0.7*0.7,
                             height1A:size_font_big*2,
                             height1C:c.height*0.37,height2C:c.height*0.37,height3C:c.height*0.37,
                             height1D:c.height*0.37+size_font_big*0.6,height2D:c.height*0.37+size_font_big*0.6,height3D:c.height*0.37+size_font_big*0.6,
                             text1A:"D", text1B:'E', text1C:'F',text2A:"1", text2B:'2', text2C:'3', highlightW:c.width/2, highlightH:0
                           };
  /*  B */ vetorDados[7] = { font1:"betsy2",font2:"betsy2",teclado:false,
                             width1A:c.width*0.49,
                             width1C:c.width*0.50-size_font_big*0.7*4,width2C:0,width3C:0,
                             height1A:c.height*0.95,
                             height1C:c.height*0.63+size_font_big*0.7,height2C:0,height3C:0,
                             text1A:"V D A", text1B:'', text1C:'',text2A:'', text2B:'', text2C:'',
                             highlightW:c.width/2, highlightH:c.height};
  posicao = { A:vetorDados[0] , E:vetorDados[1] , H:vetorDados[2],F:vetorDados[3],G:vetorDados[4],D:vetorDados[5],C:vetorDados[6],B:vetorDados[7]};



  // Carregando
  setTimeout(function(){ocioso(2);},3000);

  // Inicializando fontes externas
  ctx.font = size_font_big+"px sign"
  ctx.fillText("^",posicao["C"].width1A,posicao["C"].height1A);
  ctx.font = size_font_big+"px keys"
  ctx.fillText("^",posicao["C"].width1A,posicao["C"].height1A);


  editor.insertText(editor.getLength()-1,CookiesDeHoje,{'color': cores[coresCont],'size':'30px'});

  canvasDraw();

  // Chama a tela de splash
  ocioso(1);
}
