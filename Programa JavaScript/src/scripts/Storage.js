function inicializarStorage()
{
    // Vetor de Storages
  StoragesDeHoje = "";

  clearStorage(localStorage.getItem("historico"));
  // Storages
  texto = "";
  var d = new Date();
  var n = new Date();
  var o = new Date(n.getTime()-30*24*60*60*1000); //30 dias atras


  if (localStorage.getItem("historico") && (localStorage.getItem(trim(n.toString().substring(4,15)))) == "" )
    localStorage.setItem("historico", localStorage.getItem("historico") + trim(n.toString().substring(4,15)) + ";");
  else
    localStorage.setItem("historico", trim(n.toString().substring(4,15)) + ";");

  var indice = 2;
  nomeStorage = getStorage(localStorage.getItem("historico"),1);

  while (nomeStorage != "ERRO")
  {
      if (localStorage.getItem(nomeStorage))
         StoragesDeHoje += localStorage.getItem(nomeStorage)+ " ";
      nomeStorage = getStorage(localStorage.getItem("historico"),indice);
      indice++;
  }

  StoragesDeHoje = virgula(StoragesDeHoje);
  StoragesDeHoje = enter(StoragesDeHoje);
  nomeStorage = trim(n.toString().substring(4,15));

  if(localStorage.getItem(nomeStorage))
     var hist = localStorage.getItem(nomeStorage);
  else
    hist = "";
  timerAutoSave = setInterval(function()
                              {
                                  localStorage.setItem(nomeStorage, hist + texto);
                              }, 5000);
}


function trim(str)
{
    r = "";
    for(i = 0; i < str.length; i++){
      if(str.charAt(i) != ' ')
        r += str.charAt(i);
   }
  return r;
}

function virgula(str)
{
  r = "";
    for(i = 0; i < str.length; i++){
      if(str.charAt(i) != ',')
        r += str.charAt(i);
      else
        r += ' '
   }
  return r;
}

function enter(str)
{
  r = "";
    for(i = 0; i < str.length; i++){
      if(str.charAt(i) != ';')
        r += str.charAt(i);
      else
        r += '\n'
   }
  return r;
}

function getStorage(str, indice)
{
    if(str)
      for(i = 0; i < str.length; i++)
      {
        if(str.charAt(i) == ';')
          indice--;
        if (indice == 0)
        {
           return str.substring(i-9,i); 
        }
      }
      return "ERRO";

}

function clearStorage(str)
{
    if(str)
    {
        indice = 0;
        for(i = 0; i < str.length; i++)
        {
          if(str.charAt(i) == ';')
            indice++;
          if (indice == 30)
          {
             expirado = str.substring(0,9);
             localStorage.removeItem(expirado);
             novoHistorico = str.substring(11,str.length);
             localStorage.setItem("historico",novoHistorico); 
          }
        }
    }
}