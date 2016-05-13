using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardAndSuggester
{
    class CharacterDecoder
    {
        public string generateCharacter(string print, int blockType, bool firstKeyboard)
        {
            if (blockType != 0)
                return "ɮ";
            if (firstKeyboard) //teclado primario
            {
                switch (print)
                {
                    case "ii":
                        return "Ø"; //letra cancelada
                    case "0":
                        return "A";
                    case "01":
                        return "B";
                    case "07":
                        return "C";
                    case "1":
                        return "E";
                    case "10":
                        return "D";
                    case "12":
                        return "F";
                    case "2":
                        return "I";
                    case "21":
                        return "G";
                    case "23":
                        return "H";
                    case "3":
                        return "L";
                    case "32":
                        return "J";
                    case "34":
                        return "M";
                    case "4":
                        return "O";
                    case "43":
                        return "N";
                    case "45":
                        return "P";
                    case "5":
                        return "ESPAÇO";
                    case "54":
                        return ".";
                    case "56":
                        return "DEL";
                    case "6":
                        return "S";
                    case "65":
                        return "R";
                    case "67":
                        return "Q";
                    case "7":
                        return "U";
                    case "76":
                        return "V";
                    case "70":
                        return "T";
                    default:
                        return "&";
                }
            }
            else
            {
                switch (print)
                {
                    case "ii":
                        return "Ø"; //letra cancelada
                    case "0":
                        return "0";
                    case "01":
                        return "Z";
                    case "07":
                        return "Y";
                    case "1":
                        return "2";
                    case "10":
                        return "1";
                    case "12":
                        return "3";
                    case "2":
                        return "5";
                    case "21":
                        return "4";
                    case "23":
                        return "6";
                    case "3":
                        return "8";
                    case "32":
                        return "7";
                    case "34":
                        return "9";
                    case "4":
                        return "O";
                    case "43":
                        return "N";
                    case "45":
                        return "P";
                    case "5":
                        return "ESPAÇO";
                    case "54":
                        return ".";
                    case "56":
                        return "DEL";
                    case "6":
                        return "S";
                    case "65":
                        return "R";
                    case "67":
                        return "Q";
                    case "7":
                        return "W";
                    case "76":
                        return "X";
                    case "70":
                        return "K";
                    default:
                        return "&";
                }

            }
        }
    }
}
