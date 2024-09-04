using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Linq;
using System;

public class CountryDropDown : DropDownBase
{ 
    public Action OnListGenerated;
    private TMP_Dropdown countryDropdown;
	
    private string listOfContriesString = "Afghanistan, Åland Islands, Albania, Algeria, American Samoa, Andorra, Angola, Anguilla, Antarctica, Antigua and Barbuda, Argentina, Armenia, Australia, Austria, Azerbaijan, Bahamas, Bahrain, Bangladesh, Barbados, Belarus, Belgium, Belize, Benin, Bermuda, Bhutan, Bolivia, Bosnia and Herzegovina, Botswana, Bouvet Island, Brazil, British Indian Ocean Territory, Brunei Darussalam, Bulgaria, Burkina Faso, Burundi, Cambodia, Cameroon, Canada, Cape Verde, Cayman Islands, Central African Republic, Chad, Chile, China, Christmas Island, Cocos (Keeling) Islands, Colombia, Comoros, Congo, Congo; The Democratic Republic of the, Cook Islands, Costa Rica, Cote Dlvoire, Croatia, Cuba, Cyprus, Czech Republic, Denmark, Djibouti, Dominica, Dominican Republic, Ecuador, Egypt, El Salvador, Equatorial Guinea, Eritrea, Estonia, Ethiopia, Falkland Islands (Malvinas), Faroe Islands, Fiji, Finland, France, French Guiana, French Polynesia, French Southern Territories, Gabon, Gambia, Georgia, Germany, Ghana, Gibraltar, Greece, Greenland, Grenada, Guadeloupe, Guam, Guatemala, Guernsey, Guinea, Guinea-Bissau, Guyana, Haiti, Heard Island and McDonald Islands, Holy See (Vatican City State), Honduras, Hong Kong, Hungary, Iceland, India, Indonesia, Iran; Islam Republic of, Iraq, Ireland, Israel, Italy, Jamaica, Japan, Jersey, Jordan, Kazakhstan, Kenya, Kiribati, Korea; Democratic People Republic of, Korea; Republic of, Kuwait, Kyrgyzstan, Lao People Democratic Republic, Latvia, Lebanon, Lesotho, Liberia, Libya Arab Jamahiriya, Liechtenstein, Lithuania, Luxembourg, Macao, Macedonia; The Former Yugoslav Republic, Madagascar, Malawi, Malaysia, Maldives, Mali, Malta, Marshall Islands, Martinique, Mauritania, Mauritius, Mayotte, Mexico, Micronesia; Federal States of, Moldova; Republic of, Monaco, Mongolia, Montserrat, Morocco, Mozambique, Myanmar, Namibia, Nauru, Nepal, Netherlands, Netherlands Antilles, New Caledonia, New Zealand, Nicaragua, Niger, Nigeria, Niue, Norfolk Island, Northern Mariana Islands, Norway, Oman, Pakistan, Palau, Palestine Territory; Occupied, Panama, Papua New Guinea, Paraguay, Peru, Philippines, Pitcairn, Poland, Portugal, Puerto Rico, Qatar, Romania, Russian Federation, RWANDA, Saint Helena, Saint Kitts and Nevis, Saint Lucia, Saint Pierre and Miquelon, Saint Vincent and the Grenadines, Samoa, San Marino, Sao Tome and Principe, Saudi Arabia, Senegal, Serbia and Montenegro, Seychelles, Sierra Leone, Singapore, Slovakia, Slovenia, Solomon Islands, Somalia, South Africa, South Georgia and the South Sandwich Islands, Spain, Sri Lanka, Sudan, Suriname, Svalbard and Jan Mayen, Swaziland, Sweden, Switzerland, Syria Arab Republic, Taiwan; Province of China, Tajikistan, Tanzania; United Republic of, Thailand, Timor-Leste, Togo, Tokelau, Tonga, Trinidad and Tobago, Tunisia, Turkey, Turkmenistan, Turks and Caicos Islands, Tuvalu, Uganda, Ukraine, United Arab Emirates, United Kingdom, United States, United States Minor Outlying Islands, Uruguay, Uzbekistan, Vanuatu, Venezuela, Viet nam, Virgin Islands; British, Virgin Islands; U.S., Wallis and Futuna, Western Sahara, Yemen, Zambia, Zimbabwe";

    private static string[] countryList;

    protected override void Awake()
    {
        base.Awake();

        listOfContriesString = listOfContriesString.Replace(", ", ",");

        countryList = listOfContriesString.Split(',');

       /* foreach (string country in countryList) 
        {
            country.Replace(";", ",");
        }*/

        countryDropdown.AddOptions(countryList.ToList());

        countryDropdown.value = 226;
        OnListGenerated?.Invoke();
    }

    protected override void OnValueChanged(int value)
    {
        base.Awake();

    }

    public static string GetCountryFromIndex(int index) 
    {
        return countryList[index];

       // for (int i = 1; i < countryList.Length; i++)
       // {
       // if (dropdown.itemText.text == cou)
       //{

        // }
        //}
    }

    public static string GetAvbreviationFromCountry(string abv)
    {
        switch (abv)
        {
            case "Afghanistan":
                return "AF";
            case "Aland Islands":
                return "AX";
            case "Albania":
                return "AL";
            case "Algeria":
                return "DZ";
            case "American Samoa":
                return "AS";
            case "AndorrA":
                return "AD";
            case "Angola":
                return "AO";
            case "Anguilla":
                return "AI";
            case "Antarctica":
                return "AQ";
            case "Antigua and Barbuda":
                return "AG";
            case "Argentina":
                return "AR";
            case "Armenia":
                return "AM";
            case "Australia":
                return "AU";
            case "Austria":
                return "AT";
            case "Azerbaijan":
                return "AZ";
            case "Bahamas":
                return "BS";
            case "Bahrain":
                return "BH";
            case "Bangladesh":
                return "BD";
            case "Barbados":
                return "BB";
            case "Belarus":
                return "BY";
            case "Belgium":
                return "BE";
            case "Belize":
                return "BZ";
            case "Benin":
                return "BJ";
            case "Bermuda":
                return "BM";
            case "Bhutan":
                return "BT";
            case "Bolivia":
                return "BO";
            case "Bosnia and Herzegovina":
                return "BA";
            case "Botswana":
                return "BW";
            case "Bouvet Island":
                return "BV";
            case "Brazil":
                return "BR";
            case "British Indian Ocean Territory":
                return "IO";
            case "Brunei Darussalam":
                return "BN";
            case "Bulgaria":
                return "BG";
            case "Burkina Faso":
                return "BF";
            case "Burundi":
                return "BI";
            case "Cambodia":
                return "KH";
            case "Cameroon":
                return "CM";
            case "Canada":
                return "CA";
            case "Cape Verde":
                return "CV";
            case "Cayman Islands":
                return "KY";
            case "Central African Republic":
                return "CF";
            case "Chad":
                return "TD";
            case "Chile":
                return "CL";
            case "China":
                return "CN";
            case "Christmas Island":
                return "CX";
            case "Cocos(Keeling) Islands":
                return "CC";
            case "Colombia":
                return "CO";
            case "Comoros":
                return "KM";
            case "Congo":
                return "CG";
            case "Congo, The Democratic Republic of the":
                return "CD";
            case "Cook Islands":
                return "CK";
            case "Costa Rica":
                return "CR";
            case "Cote Dlvoire":
                return "CI";
            case "Croatia":
                return "HR";
            case "Cuba":
                return "CU";
            case "Cyprus":
                return "CY";
            case "Czech Republic":
                return "CZ";
            case "Denmark":
                return "DK";
            case "Djibouti":
                return "DJ";
            case "Dominica":
                return "DM";
            case "Dominican Republic":
                return "DO";
            case "Ecuador":
                return "EC";
            case "Egypt":
                return "EG";
            case "El Salvador":
                return "SV";
            case "Equatorial Guinea":
                return "GQ";
            case "Eritrea":
                return "ER";
            case "Estonia":
                return "EE";
            case "Ethiopia":
                return "ET";
            case "Falkland Islands(Malvinas)":
                return "FK";
            case "Faroe Islands":
                return "FO";
            case "Fiji":
                return "FJ";
            case "Finland":
                return "FI";
            case "France":
                return "FR";
            case "French Guiana":
                return "GF";
            case "French Polynesia":
                return "PF";
            case "French Southern Territories":
                return "TF";
            case "Gabon":
                return "GA";
            case "Gambia":
                return "GM";
            case "Georgia":
                return "GE";
            case "Germany":
                return "DE";
            case "Ghana":
                return "GH";
            case "Gibraltar":
                return "GI";
            case "Greece":
                return "GR";
            case "Greenland":
                return "GL";
            case "Grenada":
                return "GD";
            case "Guadeloupe":
                return "GP";
            case "Guam":
                return "GU";
            case "Guatemala":
                return "GT";
            case "Guernsey":
                return "GG";
            case "Guinea":
                return "GN";
            case "Guinea - Bissau":
                return "GW";
            case "Guyana":
                return "GY";
            case "Haiti":
                return "HT";
            case "Heard Island and McDonald Islands":
                return "HM";
            case "Holy See(Vatican City State)":
                return "VA";
            case "Honduras":
                return "HN";
            case "Hong Kong":
                return "HK";
            case "Hungary":
                return "HU";
            case "Iceland":
                return "IS";
            case "India":
                return "IN";
            case "Indonesia":
                return "ID";
            case "Iran, Islam Republic of":
                return "IR";
            case "Iraq":
                return "IQ";
            case "Ireland":
                return "IE";
            case "Israel":
                return "IL";
            case "Italy":
                return "IT";
            case "Jamaica":
                return "JM";
            case "Japan":
                return "JP";
            case "Jersey":
                return "JE";
            case "Jordan":
                return "JO";
            case "Kazakhstan":
                return "KZ";
            case "Kenya":
                return "KE";
            case "Kiribati":
                return "KI";
            case "Korea, Democratic People Republic of":
                return "KP";
            case "Korea, Republic of":
                return "KR";
            case "Kuwait":
                return "KW";
            case "Kyrgyzstan":
                return "KG";
            case "Lao People Democratic Republic":
                return "LA";
            case "Latvia":
                return "LV";
            case "Lebanon":
                return "LB";
            case "Lesotho":
                return "LS";
            case "Liberia":
                return "LR";
            case "Libya Arab Jamahiriya":
                return "LY";
            case "Liechtenstein":
                return "LI";
            case "Lithuania":
                return "LT";
            case "Luxembourg":
                return "LU";
            case "Macao":
                return "MO";
            case "Macedonia, The Former Yugoslav Republic":
                return "MK";
            case "Madagascar":
                return "MG";
            case "Malawi":
                return "MW";
            case "Malaysia":
                return "MY";
            case "Maldives":
                return "MV";
            case "Mali":
                return "ML";
            case "Malta":
                return "MT";
            case "Marshall Islands":
                return "MH";
            case "Martinique":
                return "MQ";
            case "Mauritania":
                return "MR";
            case "Mauritius":
                return "MU";
            case "Mayotte":
                return "YT";
            case "Mexico":
                return "MX";
            case "Micronesia, Federal States of":
                return "FM";
            case "Moldova, Republic of":
                return "MD";
            case "Monaco":
                return "MC";
            case "Mongolia":
                return "MN";
            case "Montserrat":
                return "MS";
            case "Morocco":
                return "MA";
            case "Mozambique":
                return "MZ";
            case "Myanmar":
                return "MM";
            case "Namibia":
                return "NA";
            case "Nauru":
                return "NR";
            case "Nepal":
                return "NP";
            case "Netherlands":
                return "NL";
            case "Netherlands Antilles":
                return "AN";
            case "New Caledonia":
                return "NC";
            case "New Zealand":
                return "NZ";
            case "Nicaragua":
                return "NI";
            case "Niger":
                return "NE";
            case "Nigeria":
                return "NG";
            case "Niue":
                return "NU";
            case "Norfolk Island":
                return "NF";
            case "Northern Mariana Islands":
                return "MP";
            case "Norway":
                return "NO";
            case "Oman":
                return "OM";
            case "Pakistan":
                return "PK";
            case "Palau":
                return "PW";
            case "Palestine Territory, Occupied":
                return "PS";
            case "Panama":
                return "PA";
            case "Papua New Guinea":
                return "PG";
            case "Paraguay":
                return "PY";
            case "Peru":
                return "PE";
            case "Philippines":
                return "PH";
            case "Pitcairn":
                return "PN";
            case "Poland":
                return "PL";
            case "Portugal":
                return "PT";
            case "Puerto Rico":
                return "PR";
            case "Qatar":
                return "QA";
            case "Romania":
                return "RO";
            case "Russian Federation":
                return "RU";
            case "RWANDA":
                return "RW";
            case "Saint Helena":
                return "SH";
            case "Saint Kitts and Nevis":
                return "KN";
            case "Saint Lucia":
                return "LC";
            case "Saint Pierre and Miquelon":
                return "PM";
            case "Saint Vincent and the Grenadines":
                return "VC";
            case "Samoa":
                return "WS";
            case "San Marino":
                return "SM";
            case "Sao Tome and Principe":
                return "ST";
            case "Saudi Arabia":
                return "SA";
            case "Senegal":
                return "SN";
            case "Serbia and Montenegro":
                return "CS";
            case "Seychelles":
                return "SC";
            case "Sierra Leone":
                return "SL";
            case "Singapore":
                return "SG";
            case "Slovakia":
                return "SK";
            case "Slovenia":
                return "SI";
            case "Solomon Islands":
                return "SB";
            case "Somalia":
                return "SO";
            case "South Africa":
                return "ZA";
            case "South Georgia and the South Sandwich Islands":
                return "GS";
            case "Spain":
                return "ES";
            case "Sri Lanka":
                return "LK";
            case "Sudan":
                return "SD";
            case "Suriname":
                return "SR";
            case "Svalbard and Jan Mayen":
                return "SJ";
            case "Swaziland":
                return "SZ";
            case "Sweden":
                return "SE";
            case "Switzerland":
                return "CH";
            case "Syria Arab Republic":
                return "SY";
            case "Taiwan; Province of China":
                return "TW";
            case "Tajikistan":
                return "TJ";
            case "Tanzania, United Republic of":
                return "TZ";
            case "Thailand":
                return "TH";
            case "Timor - Leste":
                return "TL";
            case "Togo":
                return "TG";
            case "Tokelau":
                return "TK";
            case "Tonga":
                return "TO";
            case "Trinidad and Tobago":
                return "TT";
            case "Tunisia":
                return "TN";
            case "Turkey":
                return "TR";
            case "Turkmenistan":
                return "TM";
            case "Turks and Caicos Islands":
                return "TC";
            case "Tuvalu":
                return "TV";
            case "Uganda":
                return "UG";
            case "Ukraine":
                return "UA";
            case "United Arab Emirates":
                return "AE";
            case "United Kingdom":
                return "GB";
            case "United States":
                return "US";
            case "United States Minor Outlying Islands":
                return "UM";
            case "Uruguay":
                return "UY";
            case "Uzbekistan":
                return "UZ";
            case "Vanuatu":
                return "VU";
            case "Venezuela":
                return "VE";
            case "Viet nam":
                return "VN";
            case "Virgin Islands, British":
                return "VG";
            case "Virgin Islands, U.S.":
                return "VI";
            case "Wallis and Futuna":
                return "WF";
            case "Western Sahara":
                return "EH";
            case "Yemen":
                return "YE";
            case "Zambia":
                return "ZM";
            case "Zimbabwe":
                return "ZW";
            default:
                Debug.LogError("CountryDropDown//GetAvbreviationFromCountry// not found");
                return "";
        }
    }

    public static string GetCountryFromAvbreviation(string abv)
    {
        switch (abv)
        {
            case "AF":
                return "Afghanistan";
            case "AX":
                return "Aland Islands";
            case "AL":
                return "Albania";
            case "DZ":
                return "Algeria";
            case "AS":
                return "American Samoa";
            case "AD":
                return "AndorrA";
            case "AO":
                return "Angola";
            case "AI":
                return "Anguilla";
            case "AQ":
                return "Antarctica";
            case "AG":
                return "Antigua and Barbuda";
            case "AR":
                return "Argentina";
            case "AM":
                return "Armenia";
            case "AU":
                return "Australia";
            case "AT":
                return "Austria";
            case "AZ":
                return "Azerbaijan";
            case "BS":
                return "Bahamas";
            case "BH":
                return "Bahrain";
            case "BD":
                return "Bangladesh";
            case "BB":
                return "Barbados";
            case "BY":
                return "Belarus";
            case "BE":
                return "Belgium";
            case "BZ":
                return "Belize";
            case "BJ":
                return "Benin";
            case "BM":
                return "Bermuda";
            case "BT":
                return "Bhutan";
            case "BO":
                return "Bolivia";
            case "BA":
                return "Bosnia and Herzegovina";
            case "BW":
                return "Botswana";
            case "BV":
                return "Bouvet Island";
            case "BR":
                return "Brazil";
            case "IO":
                return "British Indian Ocean Territory";
            case "BN":
                return "Brunei Darussalam";
            case "BG":
                return "Bulgaria";
            case "BF":
                return "Burkina Faso";
            case "BI":
                return "Burundi";
            case "KH":
                return "Cambodia";
            case "CM":
                return "Cameroon";
            case "CA":
                return "Canada";
            case "CV":
                return "Cape Verde";
            case "KY":
                return "Cayman Islands";
            case "CF":
                return "Central African Republic";
            case "TD":
                return "Chad";
            case "CL":
                return "Chile";
            case "CN":
                return "China";
            case "CX":
                return "Christmas Island";
            case "CC":
                return "Cocos(Keeling) Islands";
            case "CO":
                return "Colombia";
            case "KM":
                return "Comoros";
            case "CG":
                return "Congo";
            case "CD":
                return "Congo, The Democratic Republic of the";
            case "CK":
                return "Cook Islands";
            case "CR":
                return "Costa Rica";
            case "CI":
                return "Cote Dlvoire";
            case "HR":
                return "Croatia";
            case "CU":
                return "Cuba";
            case "CY":
                return "Cyprus";
            case "CZ":
                return "Czech Republic";
            case "DK":
                return "Denmark";
            case "DJ":
                return "Djibouti";
            case "DM":
                return "Dominica";
            case "DO":
                return "Dominican Republic";
            case "EC":
                return "Ecuador";
            case "EG":
                return "Egypt";
            case "SV":
                return "El Salvador";
            case "GQ":
                return "Equatorial Guinea";
            case "ER":
                return "Eritrea";
            case "EE":
                return "Estonia";
            case "ET":
                return "Ethiopia";
            case "FK":
                return "Falkland Islands(Malvinas)";
            case "FO":
                return "Faroe Islands";
            case "FJ":
                return "Fiji";
            case "FI":
                return "Finland";
            case "FR":
                return "France";
            case "GF":
                return "French Guiana";
            case "PF":
                return "French Polynesia";
            case "TF":
                return "French Southern Territories";
            case "GA":
                return "Gabon";
            case "GM":
                return "Gambia";
            case "GE":
                return "Georgia";
            case "DE":
                return "Germany";
            case "GH":
                return "Ghana";
            case "GI":
                return "Gibraltar";
            case "GR":
                return "Greece";
            case "GL":
                return "Greenland";
            case "GD":
                return "Grenada";
            case "GP":
                return "Guadeloupe";
            case "GU":
                return "Guam";
            case "GT":
                return "Guatemala";
            case "GG":
                return "Guernsey";
            case "GN":
                return "Guinea";
            case "GW":
                return "Guinea - Bissau";
            case "GY":
                return "Guyana";
            case "HT":
                return "Haiti";
            case "HM":
                return "Heard Island and McDonald Islands";
            case "VA":
                return "Holy See(Vatican City State)";
            case "HN":
                return "Honduras";
            case "HK":
                return "Hong Kong";
            case "HU":
                return "Hungary";
            case "IS":
                return "Iceland";
            case "IN":
                return "India";
            case "ID":
                return "Indonesia";
            case "IR":
                return "Iran, Islam Republic of";
            case "IQ":
                return "Iraq";
            case "IE":
                return "Ireland";
            case "IL":
                return "Israel";
            case "IT":
                return "Italy";
            case "JM":
                return "Jamaica";
            case "JP":
                return "Japan";
            case "JE":
                return "Jersey";
            case "JO":
                return "Jordan";
            case "KZ":
                return "Kazakhstan";
            case "KE":
                return "Kenya";
            case "KI":
                return "Kiribati";
            case "KP":
                return "Korea, Democratic People Republic of";
            case "KR":
                return "Korea, Republic of";
            case "KW":
                return "Kuwait";
            case "KG":
                return "Kyrgyzstan";
            case "LA":
                return "Lao People Democratic Republic";
            case "LV":
                return "Latvia";
            case "LB":
                return "Lebanon";
            case "LS":
                return "Lesotho";
            case "LR":
                return "Liberia";
            case "LY":
                return "Libya Arab Jamahiriya";
            case "LI":
                return "Liechtenstein";
            case "LT":
                return "Lithuania";
            case "LU":
                return "Luxembourg";
            case "MO":
                return "Macao";
            case "MK":
                return "Macedonia, The Former Yugoslav Republic";
            case "MG":
                return "Madagascar";
            case "MW":
                return "Malawi";
            case "MY":
                return "Malaysia";
            case "MV":
                return "Maldives";
            case "ML":
                return "Mali";
            case "MT":
                return "Malta";
            case "MH":
                return "Marshall Islands";
            case "MQ":
                return "Martinique";
            case "MR":
                return "Mauritania";
            case "MU":
                return "Mauritius";
            case "YT":
                return "Mayotte";
            case "MX":
                return "Mexico";
            case "FM":
                return "Micronesia, Federal States of";
            case "MD":
                return "Moldova, Republic of";
            case "MC":
                return "Monaco";
            case "MN":
                return "Mongolia";
            case "MS":
                return "Montserrat";
            case "MA":
                return "Morocco";
            case "MZ":
                return "Mozambique";
            case "MM":
                return "Myanmar";
            case "NA":
                return "Namibia";
            case "NR":
                return "Nauru";
            case "NP":
                return "Nepal";
            case "NL":
                return "Netherlands";
            case "AN":
                return "Netherlands Antilles";
            case "NC":
                return "New Caledonia";
            case "NZ":
                return "New Zealand";
            case "NI":
                return "Nicaragua";
            case "NE":
                return "Niger";
            case "NG":
                return "Nigeria";
            case "NU":
                return "Niue";
            case "NF":
                return "Norfolk Island";
            case "MP":
                return "Northern Mariana Islands";
            case "NO":
                return "Norway";
            case "OM":
                return "Oman";
            case "PK":
                return "Pakistan";
            case "PW":
                return "Palau";
            case "PS":
                return "Palestine Territory, Occupied";
            case "PA":
                return "Panama";
            case "PG":
                return "Papua New Guinea";
            case "PY":
                return "Paraguay";
            case "PE":
                return "Peru";
            case "PH":
                return "Philippines";
            case "PN":
                return "Pitcairn";
            case "PL":
                return "Poland";
            case "PT":
                return "Portugal";
            case "PR":
                return "Puerto Rico";
            case "QA":
                return "Qatar";
            case "RO":
                return "Romania";
            case "RU":
                return "Russian Federation";
            case "RW":
                return "RWANDA";
            case "SH":
                return "Saint Helena";
            case "KN":
                return "Saint Kitts and Nevis";
            case "LC":
                return "Saint Lucia";
            case "PM":
                return "Saint Pierre and Miquelon";
            case "VC":
                return "Saint Vincent and the Grenadines";
            case "WS":
                return "Samoa";
            case "SM":
                return "San Marino";
            case "ST":
                return "Sao Tome and Principe";
            case "SA":
                return "Saudi Arabia";
            case "SN":
                return "Senegal";
            case "CS":
                return "Serbia and Montenegro";
            case "SC":
                return "Seychelles";
            case "SL":
                return "Sierra Leone";
            case "SG":
                return "Singapore";
            case "SK":
                return "Slovakia";
            case "SI":
                return "Slovenia";
            case "SB":
                return "Solomon Islands";
            case "SO":
                return "Somalia";
            case "ZA":
                return "South Africa";
            case "GS":
                return "South Georgia and the South Sandwich Islands";
            case "ES":
                return "Spain";
            case "LK":
                return "Sri Lanka";
            case "SD":
                return "Sudan";
            case "SR":
                return "Suriname";
            case "SJ":
                return "Svalbard and Jan Mayen";
            case "SZ":
                return "Swaziland";
            case "SE":
                return "Sweden";
            case "CH":
                return "Switzerland";
            case "SY":
                return "Syria Arab Republic";
            case "TW":
                return "Taiwan; Province of China";
            case "TJ":
                return "Tajikistan";
            case "TZ":
                return "Tanzania, United Republic of";
            case "TH":
                return "Thailand";
            case "TL":
                return "Timor - Leste";
            case "TG":
                return "Togo";
            case "TK":
                return "Tokelau";
            case "TO":
                return "Tonga";
            case "TT":
                return "Trinidad and Tobago";
            case "TN":
                return "Tunisia";
            case "TR":
                return "Turkey";
            case "TM":
                return "Turkmenistan";
            case "TC":
                return "Turks and Caicos Islands";
            case "TV":
                return "Tuvalu";
            case "UG":
                return "Uganda";
            case "UA":
                return "Ukraine";
            case "AE":
                return "United Arab Emirates";
            case "GB":
                return "United Kingdom";
            case "US":
                return "United States";
            case "UM":
                return "United States Minor Outlying Islands";
            case "UY":
                return "Uruguay";
            case "UZ":
                return "Uzbekistan";
            case "VU":
                return "Vanuatu";
            case "VE":
                return "Venezuela";
            case "VN":
                return "Viet nam";
            case "VG":
                return "Virgin Islands, British";
            case "VI":
                return "Virgin Islands, U.S.";
            case "WF":
                return "Wallis and Futuna";
            case "EH":
                return "Western Sahara";
            case "YE":
                return "Yemen";
            case "ZM":
                return "Zambia";
            case "ZW":
                return "Zimbabwe";
            default:
                Debug.LogError("CountryDropDown//GetCountryFromAvbreviation// not found");
                return "";
        }
    }

}
