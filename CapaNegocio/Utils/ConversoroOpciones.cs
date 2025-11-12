using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Utils
{
    public static class ConversoroOpciones
    {



        public static string ConvertirEnteroARespuesta(string valor)
        {
            return valor switch
            {
                "1" => "Sí",     // Si el valor es 1, devuelve "Sí".
                "0" => "No",     // Si el valor es 0, devuelve "No".
                "-1" => string.Empty,
                "" => string.Empty,// Si el valor es -1, devuelve una cadena vacía.
                _ => ""//anza una excepción para valores no válidos.
            };
        }

        public static string DevuelveClienteProveedor(string valor)
        {
            return valor switch
            {
                "1" => "C",     // Si el valor es 1, devuelve "Sí".
                "2" => "V",     // Si el valor es 0, devuelve "No".
                _ => "" // Lanza una excepción para valores no válidos.
            };



        }

        public class DireccionDividida
        {
            public string Direccion1 { get; set; }
            public string Direccion2 { get; set; }
        }

        public static DireccionDividida DividirDireccion(string direccion)
        {
            DireccionDividida resultado = new DireccionDividida();

            if (direccion.Length <= 40)
            {
                resultado.Direccion1 = direccion;
                resultado.Direccion2 = string.Empty;
            }
            else if (direccion.Length <= 80)
            {
                resultado.Direccion1 = direccion.Substring(0, 40);
                resultado.Direccion2 = direccion.Substring(40);
            }
            else
            {
                resultado.Direccion1 = direccion.Substring(0, 40);
                resultado.Direccion2 = direccion.Substring(40, 40); // Toma los siguientes 40 caracteres
            }

            return resultado;
        }



        public static string DevuelveTipoDocumentoERP(string valor)
        {
            return valor switch
            {
                "3" => "A",     // Si el valor es 1, devuelve "Sí".
                "1" => "C",     // Si el valor es 0, devuelve "No".
                "2" => "E",     // Si el valor es 1, devuelve "Sí".
                _ => "" // Lanza una excepción para valores no válidos.
            };
        }



        public static string DevuelveTipoDocumento(string valor)
        {
            return valor switch
            {
                "1" => "Cédula de Ciudadania",     // Si el valor es 1, devuelve "Sí".
                "2" => "Cédula de Extranjeria",     // Si el valor es 0, devuelve "No".
                "3" => "NIT",
                "4" => "Otro",
                "5" => "Pasaporte",
                _ => "" // Lanza una excepción para valores no válidos.
            };
        }


        public static string DevuelvePais(string valor)
        {
            return valor switch
            {
                "1" => "AFGANISTAN",
                "2" => "ALBANIA",
                "3" => "ALEMANIA",
                "4" => "ANDORRA",
                "5" => "ANGOLA",
                "6" => "ANGUILLA",
                "7" => "ANTIGUA Y BARBUDA",
                "8" => "ANTILLAS HOLANDESAS",
                "9" => "ARABIA SAUDI",
                "10" => "ARGELIA",
                "11" => "ARGENTINA",
                "12" => "ARMENIA",
                "13" => "ARUBA",
                "14" => "AUSTRALIA",
                "15" => "AUSTRIA",
                "16" => "AZERBAIYAN",
                "17" => "BAHAMAS",
                "18" => "BAHREIN",
                "19" => "BANGLADESH",
                "20" => "BARBADOS",
                "21" => "BELARUS",
                "22" => "BELGICA",
                "23" => "BELICE",
                "24" => "BENIN",
                "25" => "BERMUDAS",
                "26" => "BHUTÁN",
                "27" => "BOLIVIA",
                "28" => "BOSNIA Y HERZEGOVINA",
                "29" => "BOTSWANA",
                "30" => "BRASIL",
                "31" => "BRUNEI",
                "32" => "BULGARIA",
                "33" => "BURKINA FASO",
                "34" => "BURUNDI",
                "35" => "CABO VERDE",
                "36" => "CAMBOYA",
                "37" => "CAMERUN",
                "38" => "CANADA",
                "39" => "CHAD",
                "40" => "CHILE",
                "41" => "CHINA",
                "42" => "CHIPRE",
                "43" => "COLOMBIA",
                "44" => "COMORES",
                "45" => "CONGO",
                "46" => "COREA",
                "47" => "COREA DEL NORTE ",
                "48" => "COSTA DE MARFIL",
                "49" => "COSTA RICA",
                "50" => "CROACIA",
                "51" => "CUBA",
                "52" => "DINAMARCA",
                "53" => "DJIBOUTI",
                "54" => "DOMINICA",
                "55" => "ECUADOR",
                "56" => "EGIPTO",
                "57" => "EL SALVADOR",
                "58" => "EMIRATOS ARABES UNIDOS",
                "59" => "ERITREA",
                "60" => "ESLOVENIA",
                "61" => "ESPAÑA",
                "62" => "ESTADOS UNIDOS DE AMERICA",
                "63" => "ESTONIA",
                "64" => "ETIOPIA",
                "65" => "FIJI",
                "66" => "FILIPINAS",
                "67" => "FINLANDIA",
                "68" => "FRANCIA",
                "69" => "GABON",
                "70" => "GAMBIA",
                "71" => "GEORGIA",
                "72" => "GHANA",
                "73" => "GIBRALTAR",
                "74" => "GRANADA",
                "75" => "GRECIA",
                "76" => "GROENLANDIA",
                "77" => "GUADALUPE",
                "78" => "GUAM",
                "79" => "GUATEMALA",
                "80" => "GUAYANA FRANCESA",
                "81" => "GUERNESEY",
                "82" => "GUINEA",
                "83" => "GUINEA ECUATORIAL",
                "84" => "GUINEA-BISSAU",
                "85" => "GUYANA",
                "86" => "HAITI",
                "87" => "HONDURAS",
                "88" => "HONG KONG",
                "89" => "HUNGRIA",
                "90" => "INDIA",
                "91" => "INDONESIA",
                "92" => "IRAN",
                "93" => "IRAQ",
                "94" => "IRLANDA",
                "95" => "ISLA DE MAN",
                "96" => "ISLA NORFOLK",
                "97" => "ISLANDIA",
                "98" => "ISLAS ALAND",
                "99" => "ISLAS CAIMÁN",
                "100" => "ISLAS COOK",
                "101" => "ISLAS DEL CANAL",
                "102" => "ISLAS FEROE",
                "103" => "ISLAS MALVINAS",
                "104" => "ISLAS MARIANAS DEL NORTE",
                "105" => "ISLAS MARSHALL",
                "106" => "ISLAS PITCAIRN",
                "107" => "ISLAS SALOMON",
                "108" => "ISLAS TURCAS Y CAICOS",
                "109" => "ISLAS VIRGENES BRITANICAS",
                "110" => "ISLAS VÍRGENES DE LOS ESTADOS UNIDOS",
                "111" => "ISRAEL",
                "112" => "ITALIA",
                "113" => "JAMAICA",
                "114" => "JAPON",
                "115" => "JERSEY",
                "116" => "JORDANIA",
                "117" => "KAZAJSTAN",
                "118" => "KENIA",
                "119" => "KIRGUISTAN",
                "120" => "KIRIBATI",
                "121" => "KUWAIT",
                "122" => "LAOS",
                "123" => "LESOTHO",
                "124" => "LETONIA",
                "125" => "LIBANO",
                "126" => "LIBERIA",
                "127" => "LIBIA",
                "128" => "LIECHTENSTEIN",
                "129" => "LITUANIA",
                "130" => "LUXEMBURGO",
                "131" => "MACAO",
                "132" => "MACEDONIA ",
                "133" => "MADAGASCAR",
                "134" => "MALASIA",
                "135" => "MALAWI",
                "136" => "MALDIVAS",
                "137" => "MALI",
                "138" => "MALTA",
                "139" => "MARRUECOS",
                "140" => "MARTINICA",
                "141" => "MAURICIO",
                "142" => "MAURITANIA",
                "143" => "MAYOTTE",
                "144" => "MEXICO",
                "145" => "MICRONESIA",
                "146" => "MOLDAVIA",
                "147" => "MONACO",
                "148" => "MONGOLIA",
                "149" => "MONTENEGRO",
                "150" => "MONTSERRAT",
                "151" => "MOZAMBIQUE",
                "152" => "MYANMAR",
                "153" => "NAMIBIA",
                "154" => "NAURU",
                "155" => "NEPAL",
                "156" => "NICARAGUA",
                "157" => "NIGER",
                "158" => "NIGERIA",
                "159" => "NIUE",
                "160" => "NORUEGA",
                "161" => "NUEVA CALEDONIA",
                "162" => "NUEVA ZELANDA",
                "163" => "OMAN",
                "164" => "OTROS PAISES  O TERRITORIOS DE AMERICA DEL NORTE",
                "165" => "OTROS PAISES O TERRITORIOS  DE SUDAMERICA",
                "166" => "OTROS PAISES O TERRITORIOS DE AFRICA",
                "167" => "OTROS PAISES O TERRITORIOS DE ASIA",
                "168" => "OTROS PAISES O TERRITORIOS DE LA UNION EUROPEA",
                "169" => "OTROS PAISES O TERRITORIOS DE OCEANIA",
                "171" => "OTROS PAISES O TERRITORIOS DEL RESTO DE EUROPA",
                "172" => "PAISES BAJOS",
                "173" => "PAKISTAN",
                "174" => "PALAOS",
                "175" => "PALESTINA",
                "176" => "PANAMA",
                "177" => "PAPUA NUEVA GUINEA",
                "178" => "PARAGUAY",
                "179" => "PERU",
                "180" => "POLINESIA FRANCESA",
                "181" => "POLONIA",
                "182" => "PORTUGAL",
                "183" => "PUERTO RICO",
                "184" => "QATAR",
                "185" => "REINO UNIDO",
                "186" => "REP.DEMOCRATICA DEL CONGO",
                "187" => "REPUBLICA CENTROAFRICANA",
                "188" => "REPUBLICA CHECA",
                "189" => "REPUBLICA DOMINICANA",
                "190" => "REPUBLICA ESLOVACA",
                "191" => "REUNION",
                "192" => "RUANDA",
                "193" => "RUMANIA",
                "194" => "RUSIA",
                "195" => "SAHARA OCCIDENTAL",
                "196" => "SAMOA",
                "197" => "SAMOA AMERICANA",
                "198" => "SAN BARTOLOME",
                "199" => "SAN CRISTOBAL Y NIEVES",
                "200" => "SAN MARINO",
                "201" => "SAN MARTIN (PARTE FRANCESA)",
                "202" => "SAN PEDRO Y MIQUELON ",
                "203" => "SAN VICENTE Y LAS GRANADINAS",
                "204" => "SANTA HELENA",
                "205" => "SANTA LUCIA",
                "206" => "SANTA SEDE",
                "207" => "SANTO TOME Y PRINCIPE",
                "208" => "SENEGAL",
                "209" => "SERBIA",
                "210" => "SEYCHELLES",
                "211" => "SIERRA LEONA",
                "212" => "SINGAPUR",
                "213" => "SIRIA",
                "214" => "SOMALIA",
                "215" => "SRI LANKA",
                "216" => "SUDAFRICA",
                "217" => "SUDAN",
                "218" => "SUECIA",
                "219" => "SUIZA",
                "220" => "SURINAM",
                "221" => "SVALBARD Y JAN MAYEN",
                "222" => "SWAZILANDIA",
                "223" => "TADYIKISTAN",
                "224" => "TAILANDIA",
                "225" => "TANZANIA",
                "226" => "TIMOR ORIENTAL",
                "227" => "TOGO",
                "228" => "TOKELAU",
                "229" => "TONGA",
                "230" => "TRINIDAD Y TOBAGO",
                "231" => "TUNEZ",
                "232" => "TURKMENISTAN",
                "233" => "TURQUIA",
                "234" => "TUVALU",
                "235" => "UCRANIA",
                "236" => "UGANDA",
                "237" => "URUGUAY",
                "238" => "UZBEKISTAN",
                "239" => "VANUATU",
                "240" => "VENEZUELA",
                "241" => "VIETNAM",
                "242" => "WALLIS Y FORTUNA",
                "243" => "YEMEN",
                "244" => "ZAMBIA",
                "245" => "ZIMBABWE",
                _ => "" // Lanza una excepción para valores no válidos.
            };
        }

        public static List<KeyValuePair<string, string>> ObtenerPaisesProhibidos()
        {
            return new List<KeyValuePair<string, string>>
        {
 new KeyValuePair<string, string>("2","ALBANIA"),
 new KeyValuePair<string, string>("20","BARBADOS"),
 new KeyValuePair<string, string>("33","BURKINA FASO"),
 new KeyValuePair<string, string>("36","CAMBOYA"),
 new KeyValuePair<string, string>("47","COREA DEL NORTE"),
 new KeyValuePair<string, string>("66","FILIPINAS"),
 new KeyValuePair<string, string>("86","HAITI"),
 new KeyValuePair<string, string>("92","IRAN"),
 new KeyValuePair<string, string>("99","ISLAS CAIMÁN"),
 new KeyValuePair<string, string>("113","JAMAICA"),
 new KeyValuePair<string, string>("116","JORDANIA"),
 new KeyValuePair<string, string>("137","MALI"),
 new KeyValuePair<string, string>("138","MALTA"),
 new KeyValuePair<string, string>("139","MARRUECOS"),
 new KeyValuePair<string, string>("152","MYANMAR"),
 new KeyValuePair<string, string>("156","NICARAGUA"),
 new KeyValuePair<string, string>("173","PAKISTAN"),
 new KeyValuePair<string, string>("176","PANAMA"),
 new KeyValuePair<string, string>("208","SENEGAL"),
 new KeyValuePair<string, string>("213","SIRIA"),
 new KeyValuePair<string, string>("217","SUDAN"),
 new KeyValuePair<string, string>("233","TURQUIA"),
 new KeyValuePair<string, string>("236","UGANDA"),
 new KeyValuePair<string, string>("243","YEMEN"),
 new KeyValuePair<string, string>("245","ZIMBABWE"),


        };
        }

        public static bool ValidarPaisesProhibidos(string paisesEnTexto)
        {
            // Obtener la lista de países prohibidos
            var paisesProhibidos = ObtenerPaisesProhibidos();

            // Dividir la cadena en una lista de países
            var listaPaises = paisesEnTexto.Split(',').Select(p => p.Trim()).ToList();

            // Extraer los nombres y los códigos de los países prohibidos
            var nombresPaisesProhibidos = paisesProhibidos.Select(p => p.Value).ToList();
            var codigosPaisesProhibidos = paisesProhibidos.Select(p => p.Key).ToList();

            // Comprobar si algún país prohibido está presente en la lista, ya sea por nombre o por código
            return listaPaises.Any(pais => nombresPaisesProhibidos.Contains(pais) || codigosPaisesProhibidos.Contains(pais));
        }

        public static string ConvertirListaAString(List<string> listaPaises)
        {
            return string.Join(",", listaPaises);
        }

    }
}
