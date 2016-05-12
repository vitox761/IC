function keyboardTrans(pos) //Funcao para inverter teclado principal com secundario
{
    // Inverte teclado principal
    if(pos != "H" && pos != "G" && pos != "B") //tirar ultima condição para habilitar shift
    {
      // Inverte o valor do gatilho
      posicao[pos].teclado = !posicao[pos].teclado;
      //Trocando os valores de text da posicao desejada
      // Toca o som ao inverter teclado
      document.getElementById('flip').play();
    }
    // Chama a funcao highlight
    highlight(posicao[pos].highlightW,posicao[pos].highlightH,0);
}

function mOver(pos)
{
  if (pos == "centro") //se entrou no centro
  {
    var aux = 0;
    var comb = "";
    while (indice > aux)
    {
    		if ((combination[0] == combination[aux]) && aux != 0)
    		{
    			comb = ';'
    			break;
    		}
        comb =  comb + combination[aux];
        aux++;
    }

	  document.getElementById("editor").style.backgroundColor = '#333333';
	  if (comb != "CFDGB" && comb != "CAEHB")
		  Letra(comb.substring(0,2),1);
	  else
		  Letra(comb,1); //para os bloqueios
    if (status != 2) //se a tela estiver desbloqueada
    {
      // Volta o teclado para a posicao principal
      if(posicao[combination[0]].teclado != shiftaux && combination[0] != "B")
        keyboardTrans(combination[0]);
      if(shiftaux && posicao["B"].teclado == shiftaux)
        keyboardTrans("B");
    }
	  indice = 0;
    while(indice <= 7)
    {
      highlight(vetorDados[indice].highlightW,vetorDados[indice].highlightH,1);
      indice++;
    }
    indice = 0;
    pos = "N";
    comb = "";
  }
  else //se entrou em algum canto
  {
    if (indice == 0 && status != 2) //se é a primeira letra após sair do centro
    {
      highlight(posicao[pos].highlightW,posicao[pos].highlightH,0);
      timerId = setInterval(function(){keyboardTrans(pos);drawChar(comb,pos);},3000);
      // Caso a tela esteja desbloqueada tocar beep
      if(status == 0)
        document.getElementById('beep01').play();
      // Insere na comb
      combination[indice] = pos;
      indice++;
    }
    else
    {
      // Caso a tela esteja bloqueada , é possivel destacar 2 ou mais pontos na tela
      if(status!=2)
        highlight(posicao[pos].highlightW,posicao[pos].highlightH,0);

      if(indice == 1 && combination[indice-1] == pos)
        timerId = setInterval(function(){keyboardTrans(pos);drawChar(comb,pos);},3000);
    }

    if (indice > 0 && combination[indice-1]!=pos)
    {
      combination[indice] = pos;
      indice++;
      if(status == 0)
        document.getElementById('beep02').play();
    }
	  if(status == 0)
	  {
	    var aux = 0;
      var comb = "";
      while (indice > aux)
      {
    		if ((combination[0] == combination[aux]) && aux != 0)
    		{
    			comb = ';'
    			break;
    		}
        comb =  comb + combination[aux];
        aux++;
      }
      drawChar(comb,pos);
	  }
  }
}

function mOut(pos)
{
  if (pos == "centro")//ao sair do centro
  {
    indice = 0;
	  document.getElementById("editor").style.backgroundColor = 'transparent';
  }
  else//ao sair de qualquer canto
  {
    if (indice == 1)
      clearInterval(timerId);
	  document.getElementById(pos).innerHTML = '';
  }
}

function ocioso(controle)
{
    ctx.fillStyle = 'white';
    // Limpa tela
    ctx.clearRect ( 0 , 0 , c.width, c.height );
    // Calculando posicao do spinner
    var topRatio = ((c.width/2)+64*3)*(100/c.width)+'%';
    // Escondendo o editor o log e o botao
    document.getElementById("editor").style.visibility="hidden";
    // Recebendo imagem e mostrando na tela
    var img = new Image();
    
    // Caso a tela tenha sido bloqueada
    if(controle == 0)
    {
       // Carregando imagens
       img.src = "./data/img/locked.png";
       img.onload = function(){ctx.drawImage(img,c.width/2-64,c.height/2-64);}
       document.getElementById("editor").style.visibility="visible";
       document.getElementById("editor").style.border="0px";
       // Declarando aplicao como travado
       status = 1;
       // Timer para apagar texto
       apagarEditor = setTimeout(function(){document.getElementById("editor").style.visibility="hidden";},60000);
    }

    // Caso o programa esteja sendo inicializado
    if(controle == 1)
    {
      // Carregando imagens
        img.src = "./data/img/splash.png";
        img.onload = function(){ctx.drawImage(img,c.width/2-64,c.height/2-64);}
        // Alinhando e escrevendo texto
        ctx.textAlign="center";
        ctx.font = size_font_big+"px Arial";
        ctx.fillText("Carregando",c.width/2,((c.height/2)-64*2));
        // Definindo velocidade do spinner
        spinner_speed = 3;
        // Definindo status como inicializando
        status = 2;
    }

    // Desbloqueando a tela
    if(controle == 2)
    {
      if(status == 1 && document.getElementById("centro").style.height == "20%")
         clearTimeout(apagarEditor);
      
      // Definindo status como livre
      status = 0;
      // Limpando tela
      highlight(0,0,1);
      // Gerando borda do editor
      document.getElementById("editor").style.visibility="visible";
      document.getElementById("editor").style.border="2px solid white";
      document.getElementById("centro").style.height  = "20%";
      document.getElementById("centro").style.width = "20%"; 
      document.getElementById("centro").style.left = "40%"; 
      document.getElementById("centro").style.top = "40%"; 

      // scroll
      Letra("B",1)
      Letra("BH",1)
    }

    // Acessando o texto expandido
    if(controle == 3)
    {
      status = 1;
      document.getElementById("editor").style.visibility="visible";
      document.getElementById("editor").style.border="2px solid white";
      document.getElementById("centro").style.height  = "60%";
      document.getElementById("centro").style.width = "60%"; 
      document.getElementById("centro").style.left = "20%"; 
      document.getElementById("centro").style.top = "20%"; 
    }

    //  Definindo suas propriedades
    var opts =
    {
      lines: 7, // Numero de linhas no spinner
      length: 5, // Tamanho de cada linha
      width: 5, // Espessura de cada linha
      radius: 10, // Raio da circunferencia interior
      corners: 0, // Formato dos cantos 0 = quadrado 1 = circular  (0..1)
      rotate: 0, // offset de rotacao
      direction: 1, // 1: sentido horario, -1: sentido anti-horario
      color: '#fff', // #rgb ou #rrggbb ou vetor de cores
      speed: spinner_speed, // Rotacoes por segundo
      trail: 30, // % de trilha deixada nas posicoes anteriores
      shadow: true, // Sombra
      hwaccel: true, // Usar hardware acceleration
      className: 'spinner', // Classe CSS do spinner , se existir
      zIndex: 2e9, // O z-index (defaults to 2000000000)
      top: topRatio, // Posicao y -Relativo ao div pai-
      left: '50%' // Posicao x -Relativo ao div pai-
    };
    if(controle == 1)
    {
      // Desenhando spinner no div
      target = document.getElementById("spinnerDiv");
      spinner = new Spinner(opts).spin(target);
    }
    else
      spinner.stop(target);
}