function getCookie(cname) 
{
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for(var i=0; i<ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0)==' ') c = c.substring(1);
          if (c.indexOf(name) == 0) return c.substring(name.length,c.length);
    }
    return "";
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

function enter(str)
{
  r = "";
    for(i = 0; i < str.length; i++){
      if(str.charAt(i) != ',')
        r += str.charAt(i);
      else
        r += '\n'
   }
  return r;
}