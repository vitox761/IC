function canvasDrawLine()
{
  ctx.fillStyle = "white";

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.37,c.height*0.37);
  ctx.lineTo(size_font_big*3,size_font_big*3);
  ctx.stroke();//< /\

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.36,c.height*0.5);
  ctx.lineTo(size_font_big*5,c.height*0.5);
  ctx.stroke();//<

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.38,c.height*0.62);
  ctx.lineTo(size_font_big*3,c.height-size_font_big*3);
  ctx.stroke();//< \/

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.5,c.height*0.67);
  ctx.lineTo(c.width*0.5,c.height-size_font_big*5);
  ctx.stroke();//\/

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.62,c.height*0.62);
  ctx.lineTo(c.width-size_font_big*3,c.height-size_font_big*3);
  ctx.stroke();//> \/

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.63,c.height*0.5);
  ctx.lineTo(c.width-size_font_big*5,c.height*0.5);
  ctx.stroke();//>

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.63,c.height*0.37);
  ctx.lineTo(c.width-size_font_big*3,size_font_big*3);
  ctx.stroke();//> /\

  ctx.beginPath();
  ctx.strokeStyle = "white";
  ctx.lineWidth = 1;
  ctx.moveTo(c.width*0.5,c.height*0.34);
  ctx.lineTo(c.width*0.5,size_font_big*5);
  ctx.stroke();// /\
}

function canvasDraw()
{
  // Desenhando linhas
  canvasDrawLine();
  // Desenhando letras em cada ponto da tela
  ctx.textAlign="start";
  canvasDrawTeclado("A");
  canvasDrawTeclado("E");
  canvasDrawTeclado("H");
  canvasDrawTeclado("F");
  canvasDrawTeclado("G");
  canvasDrawTeclado("D");
  canvasDrawTeclado("C");
  canvasDrawTeclado("B");
}

function highlight(x,y,limpar)
{
  if(status == 0)
    // Limpa tela
    ctx.clearRect ( 0 , 0 , c.width, c.height );

  // Limpa selecao
  if (limpar)
  {
    ctx.beginPath()
    ctx.arc(x,y,c.width*0.1,0,2*Math.PI);
    ctx.fillStyle = "black";
    ctx.fill();
  }
  // Desenha selecao
  else
  {
    var grd = ctx.createRadialGradient(x,y,c.width*0.001,x,y,c.width*0.5);
    grd.addColorStop(0,"black");
    grd.addColorStop(1,"white");
    ctx.beginPath();
    ctx.arc(x,y,c.width*0.1,0,2*Math.PI);
    ctx.fillStyle = grd;
    ctx.fill();
  }
  if(status == 0)
    // Desenha o teclado novamente
    canvasDraw();
}

function canvasDrawTeclado(pos)
{
    ctx.fillStyle = 'white';
    //Teclado Miniatura
    if(pos == "B")
      ctx.font = size_font_small*2+"px "+posicao[pos].font2;
    else
      ctx.font = size_font_small+"px "+posicao[pos].font2;
    // MINIATURA Secundario
    ctx.fillStyle = "rgba(100, 100, 100, 100)";
    ctx.fillText(posicao[pos].text2A,posicao[pos].width1D,posicao[pos].height1D);
    ctx.fillText(posicao[pos].text2B,posicao[pos].width2D,posicao[pos].height2D);
    ctx.fillText(posicao[pos].text2C,posicao[pos].width3D,posicao[pos].height3D);
    // MINIATURA Primario
    if(pos != "B")
      ctx.font = "bold "+size_font_small+"px "+posicao[pos].font2;
    ctx.fillStyle = "white";
    ctx.fillText(posicao[pos].text1A,posicao[pos].width1C,posicao[pos].height1C);
    ctx.fillText(posicao[pos].text1B,posicao[pos].width2C,posicao[pos].height2C);
    ctx.fillText(posicao[pos].text1C,posicao[pos].width3C,posicao[pos].height3C);
}

function drawChar(comb,pos)
{
  if(comb.substring(0,2) == ";")
  {
    ctx.font = size_font_big+"px sign"
    ctx.fillText("^",posicao[pos].width1A,posicao[pos].height1A);
  }
  else
    if(comb.substring(0,1) == "B")
    {
      ctx.font = size_font_big+"px betsy2"
      if(Letra(comb.substring(0,2),0) == " ")
        letra = "D";
      if(Letra(comb.substring(0,2),0) == "DEL")
        letra = "V";
      if(Letra(comb.substring(0,2),0) == ". ")
        letra = "A"

      if(letra == "D")
        ctx.fillText(letra,posicao[pos].width1A-size_font_small,posicao[pos].height1A);
      else
        ctx.fillText(letra,posicao[pos].width1A,posicao[pos].height1A);
    }
    else
    {
      if(comb == "CFDGB")
      {
        ctx.font = size_font_big*2+"px keys"
        ctx.fillText("L",posicao[pos].width1A-size_font_small,posicao[pos].height1A);
      }
      else
      {
        ctx.font = size_font_big+"px Arial"
        ctx.fillText(Letra(comb.substring(0,2),0),posicao[pos].width1A,posicao[pos].height1A);
      }
    }
}

function cursor()
{
  if (cursorVisivel)
  {
    document.body.style.cursor = 'none';
    document.getElementById('editor').style.cursor = 'none';
    document.getElementById('A').style.background = 'none'
    document.getElementById('C').style.background = 'none'
    document.getElementById('F').style.background = 'none'
    document.getElementById('D').style.background = 'none'
    document.getElementById('G').style.background = 'none'
    document.getElementById('B').style.background = 'none'
    document.getElementById('H').style.background = 'none'
    document.getElementById('E').style.background = 'none'
  }
  else
  {
    document.body.style.cursor = 'default';
    document.getElementById('editor').style.cursor = 'default';
    document.getElementById('A').style.background = 'gray'
    document.getElementById('C').style.background = 'yellow'
    document.getElementById('F').style.background = 'blue'
    document.getElementById('D').style.background = 'green'
    document.getElementById('G').style.background = 'red'
    document.getElementById('B').style.background = 'white'
    document.getElementById('H').style.background = 'brown'
    document.getElementById('E').style.background = 'purple'
  }
  cursorVisivel = !cursorVisivel;
}
