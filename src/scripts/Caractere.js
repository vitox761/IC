// Funcao para encontrar o caracter desejado a partir da combinaçao entrada
function Letra(pos,flag)
{
  var caractere = ';';
  var length = editor.getLength();

  if(pos == "A")
		if (!posicao[pos].teclado)
			caractere = 'a';
		else
			caractere = '0';
  if(pos == "AC")
		if (!posicao["A"].teclado)
			caractere = 'b';
		else
			caractere = 'z';
  if(pos == "AE")
		if (!posicao["A"].teclado)
			caractere = 'c';
		else
			caractere = 'y';

  if(pos == "EA")
		if (!posicao["E"].teclado)
			caractere = 't';
		else
			caractere = 'k';
  if(pos == "E")
		if (!posicao[pos].teclado)
    {
      caractere = 'u';
      if (texto[texto.length-1] == 'U' || texto[texto.length-1] == 'u')
			   caractere = "\n";
    }
		else
			caractere = 'w';
  if(pos == "EH")
		if (!posicao["E"].teclado)
			caractere = 'v';
		else
			caractere = 'x';

  if(pos == "HE")
		caractere = 'q';
  if(pos == "H")
		caractere = 's';
  if(pos == "HB")
		caractere = 'r';

  if(pos == "FC")
		if(!posicao["F"].teclado)
			caractere = 'g';
		else
			caractere = '4';
  if(pos == "F")
		if (!posicao[pos].teclado)
			caractere = 'i';
		else
			caractere = '5';
  if(pos == "FD")
		if (!posicao["F"].teclado)
			caractere = 'h';
		else
			caractere = '6';
  if(pos == "DF")
		if (!posicao["D"].teclado)
    {
      caractere = 'j';
    }
		else
			caractere = '7';
  if(pos == "D")
     if (!posicao[pos].teclado)
			caractere = 'l';
     else
		  caractere = '8';
  if(pos == "DG")
     if (!posicao["D"].teclado)
			caractere = 'm';
		else
			caractere = '9';
  if(pos == "GD")
     caractere = 'n';
  if(pos == "GB")
     caractere = 'p';
  if(pos == "G")
     caractere = 'o';
  
  if(pos == "CA")
		if (!posicao["C"].teclado)
			caractere = 'd';
		else
			caractere = '1';
  if(pos == "C")
		if (!posicao[pos].teclado)
			caractere = 'e';
	  else
			caractere = '2';

  if(pos == "CF")
		if (!posicao["C"].teclado)
			caractere = 'f';
		else
			caractere = '3';
  if(pos == "B")
     if(!posicao["B"].teclado)
        caractere = ' ';

  if(pos == "BG")
    if(!posicao["B"].teclado)
      caractere = ". ";

 if(caractere!=';' && flag)
  {
     if(status == 0)
     {
       // Verifica se a letra a ser inserida a seguir é maiuscula
       if (texto.substring(texto.length-2,texto.length) == ". " || texto[texto.length-1] == '.' || texto == "")
          // Letra maiuscula
          caractere = caractere.toUpperCase();
       // Insere letra no editor
       if (caractere == 'j' && ((texto.substring(texto.length-2,texto.length) == "Jj" || texto.substring(texto.length-2,texto.length) == "jj"))) 
          document.getElementById('loudAlarm').play();
       else
          if (caractere == 'j' && (texto[texto.length-1] == 'j' || texto[texto.length-1] == 'J')) 
            document.getElementById('alarm').play();

       if (caractere == '\n')
       {
          texto = texto.substring(0,texto.length-1);
          editor.deleteText(length-2,length);
          texto = texto.concat(',');
       }
       else
          texto = texto.concat(caractere);
       
       

       editor.insertText(length-1,caractere,{'color': cores[coresCont],'size':'30px'});
       // Manter sempre a caixa de texto atualizada ( auto scroll )
       objeto = document.getElementById('editor');
       objeto.scrollTop = objeto.scrollHeight;
       // Posicionando cursor
       if(length == 0)
         length++;
       // Mudando cor do texto
       if(caractere == ". ")
       {
          if(coresCont == 0)
            coresCont = 1;
          else
            coresCont = 0;
          length++;
       }
       document.getElementById('beep03').play();
     }
     editor.setSelection(length,length);
  }
  if(pos == "BH")
  {
     caractere = 'del';
     if(!posicao["B"].teclado && flag)
        if(status == 0)
        {
          document.getElementById('beep03').play();
          if(length > 0)
          {
            var lastChar = editor.getText(length-2,length-1);
            // Verifica se um ponto foi removido e muda a cor no caso positivo
            if(lastChar == '.')
       	      if(coresCont == 0)
       	   		   coresCont = 1;
       	   	  else
       	   		   coresCont = 0;
            editor.deleteText(length-2,length);
            texto = texto.substring(0,texto.length-1);
            editor.setSelection(length-1,length-1);
          }
        }
  }
	if(!flag)
		return caractere.toUpperCase();

    // Combinacao para bloquei de tela
  if(pos == "CFDGB" && flag)
  {
    document.getElementById('lock').play();
    if(status == 0)
      // Bloqueia
      ocioso(0);
    else
      // Desbloqueia
      ocioso(2);
  }
  if(pos == "CAEHB" && flag)
  {
    document.getElementById('lock').play();
    if (status == 0)
      // Entra na tela de visualização de texto
      ocioso(3);
    else
      // Desbloqueia
      ocioso(2);
  }
}