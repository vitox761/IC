function keyboardTrans(pos) //Funcao para inverter teclado principal com secundario
{
    // Inverte teclado principal
    if(pos != "H" && pos != "G" && pos != "B") //tirar ultima condição para habilitar shift
    {
      // Inverte o valor do gatilho
      posicao[pos].teclado = !posicao[pos].teclado;
      //Trocando os valores de text da posicao desejada
      posicao[pos].text1A = posicao[pos].text2A + (posicao[pos].text2A = posicao[pos].text1A,'');
      posicao[pos].text1B = posicao[pos].text2B + (posicao[pos].text2B = posicao[pos].text1B,'');
      posicao[pos].text1C = posicao[pos].text2C + (posicao[pos].text2C = posicao[pos].text1C,'');
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
	  if (comb != "CFDGB")
		Letra(comb.substring(0,2),1);
	  else
		Letra(comb,1);
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