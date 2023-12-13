/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;

namespace iText.IO.Font.Otf {
    /// <summary>Constants corresponding to language tags in the OTF specification.</summary>
    /// <remarks>
    /// Constants corresponding to language tags in the OTF specification.
    /// Extracted from the specification, as published by Microsoft
    /// <a href="https://docs.microsoft.com/en-us/typography/opentype/spec/languagetags">here</a>.
    /// Note that tags in OTF always consist of exactly 4 bytes. Shorter
    /// identifiers are padded with spaces as necessary.
    /// </remarks>
    /// <author><a href="mailto:matthias.valvekens@itextpdf.com">Matthias Valvekens</a></author>
    public sealed class LanguageTags {
        public const String ABAZA = "ABA ";

        public const String ABKHAZIAN = "ABK ";

        public const String ACHOLI = "ACH ";

        public const String ACHI = "ACR ";

        public const String ADYGHE = "ADY ";

        public const String AFRIKAANS = "AFK ";

        public const String AFAR = "AFR ";

        public const String AGAW = "AGW ";

        public const String AITON = "AIO ";

        public const String AKAN = "AKA ";

        public const String ALSATIAN = "ALS ";

        public const String ALTAI = "ALT ";

        public const String AMHARIC = "AMH ";

        public const String ANGLO_SAXON = "ANG ";

        public const String PHONETIC_AMERICANIST = "APPH";

        public const String ARABIC = "ARA ";

        public const String ARAGONESE = "ARG ";

        public const String AARI = "ARI ";

        public const String RAKHINE = "ARK ";

        public const String ASSAMESE = "ASM ";

        public const String ASTURIAN = "AST ";

        public const String ATHAPASKAN = "ATH ";

        public const String AVAR = "AVR ";

        public const String AWADHI = "AWA ";

        public const String AYMARA = "AYM ";

        public const String TORKI = "AZB ";

        public const String AZERBAIJANI = "AZE ";

        public const String BADAGA = "BAD ";

        public const String BANDA = "BAD0";

        public const String BAGHELKHANDI = "BAG ";

        public const String BALKAR = "BAL ";

        public const String BALINESE = "BAN ";

        public const String BAVARIAN = "BAR ";

        public const String BAULE = "BAU ";

        public const String BATAK_TOBA = "BBC ";

        public const String BERBER = "BBR ";

        public const String BENCH = "BCH ";

        public const String BIBLE_CREE = "BCR ";

        public const String BANDJALANG = "BDY ";

        public const String BELARUSSIAN = "BEL ";

        public const String BEMBA = "BEM ";

        public const String BENGALI = "BEN ";

        public const String HARYANVI = "BGC ";

        public const String BAGRI = "BGQ ";

        public const String BULGARIAN = "BGR ";

        public const String BHILI = "BHI ";

        public const String BHOJPURI = "BHO ";

        public const String BIKOL = "BIK ";

        public const String BILEN = "BIL ";

        public const String BISLAMA = "BIS ";

        public const String KANAUJI = "BJJ ";

        public const String BLACKFOOT = "BKF ";

        public const String BALUCHI = "BLI ";

        public const String PAO_KAREN = "BLK ";

        public const String BALANTE = "BLN ";

        public const String BALTI = "BLT ";

        public const String BAMBARA = "BMB ";

        public const String BAMILEKE = "BML ";

        public const String BOSNIAN = "BOS ";

        public const String BISHNUPRIYA_MANIPURI = "BPY ";

        public const String BRETON = "BRE ";

        public const String BRAHUI = "BRH ";

        public const String BRAJ_BHASHA = "BRI ";

        public const String BURMESE = "BRM ";

        public const String BODO = "BRX ";

        public const String BASHKIR = "BSH ";

        public const String BURUSHASKI = "BSK ";

        public const String BETI = "BTI ";

        public const String BATAK_SIMALUNGUN = "BTS ";

        public const String BUGIS = "BUG ";

        public const String MEDUMBA = "BYV ";

        public const String KAQCHIKEL = "CAK ";

        public const String CATALAN = "CAT ";

        public const String ZAMBOANGA_CHAVACANO = "CBK ";

        public const String CHINANTEC = "CCHN";

        public const String CEBUANO = "CEB ";

        public const String CHECHEN = "CHE ";

        public const String CHAHA_GURAGE = "CHG ";

        public const String CHATTISGARHI = "CHH ";

        public const String CHICHEWA = "CHI ";

        public const String CHUKCHI = "CHK ";

        public const String CHUUKESE = "CHK0";

        public const String CHOCTAW = "CHO ";

        public const String CHIPEWYAN = "CHP ";

        public const String CHEROKEE = "CHR ";

        public const String CHAMORRO = "CHA ";

        public const String CHUVASH = "CHU ";

        public const String CHEYENNE = "CHY ";

        public const String CHIGA = "CGG ";

        public const String WESTERN_CHAM = "CJA ";

        public const String EASTERN_CHAM = "CJM ";

        public const String COMORIAN = "CMR ";

        public const String COPTIC = "COP ";

        public const String CORNISH = "COR ";

        public const String CORSICAN = "COS ";

        public const String CREOLES = "CPP ";

        public const String CREE = "CRE ";

        public const String CARRIER = "CRR ";

        public const String CRIMEAN_TATAR = "CRT ";

        public const String KASHUBIAN = "CSB ";

        public const String CHURCH_SLAVONIC = "CSL ";

        public const String CZECH = "CSY ";

        public const String CHITTAGONIAN = "CTG ";

        public const String SAN_BLAS_KUNA = "CUK ";

        public const String DANISH = "DAN ";

        public const String DARGWA = "DAR ";

        public const String DAYI = "DAX ";

        public const String WOODS_CREE = "DCR ";

        public const String GERMAN = "DEU ";

        public const String DOGRI = "DGO ";

        public const String DOGRI2 = "DGR ";

        public const String DHANGU = "DHG ";

        public const String DHIVEHI = "DHV ";

        public const String DIMLI = "DIQ ";

        public const String DIVEHI = "DIV ";

        public const String ZARMA = "DJR ";

        public const String DJAMBARRPUYNGU = "DJR0";

        public const String DANGME = "DNG ";

        public const String DAN = "DNJ ";

        public const String DINKA = "DNK ";

        public const String DARI = "DRI ";

        public const String DHUWAL = "DUJ ";

        public const String DUNGAN = "DUN ";

        public const String DZONGKHA = "DZN ";

        public const String EBIRA = "EBI ";

        public const String EASTERN_CREE = "ECR ";

        public const String EDO = "EDO ";

        public const String EFIK = "EFI ";

        public const String GREEK = "ELL ";

        public const String EASTERN_MANINKAKAN = "EMK ";

        public const String ENGLISH = "ENG ";

        public const String ERZYA = "ERZ ";

        public const String SPANISH = "ESP ";

        public const String CENTRAL_YUPIK = "ESU ";

        public const String ESTONIAN = "ETI ";

        public const String BASQUE = "EUQ ";

        public const String EVENKI = "EVK ";

        public const String EVEN = "EVN ";

        public const String EWE = "EWE ";

        public const String FRENCH_ANTILLEAN = "FAN ";

        public const String FANG = "FAN0";

        public const String PERSIAN = "FAR ";

        public const String FANTI = "FAT ";

        public const String FINNISH = "FIN ";

        public const String FIJIAN = "FJI ";

        public const String DUTCH_FLEMISH = "FLE ";

        public const String FEFE = "FMP ";

        public const String FOREST_NENETS = "FNE ";

        public const String FON = "FON ";

        public const String FAROESE = "FOS ";

        public const String FRENCH = "FRA ";

        public const String CAJUN_FRENCH = "FRC ";

        public const String FRISIAN = "FRI ";

        public const String FRIULIAN = "FRL ";

        public const String ARPITAN = "FRP ";

        public const String FUTA = "FTA ";

        public const String FULAH = "FUL ";

        public const String NIGERIAN_FULFULDE = "FUV ";

        public const String GA = "GAD ";

        public const String SCOTTISH_GAELIC = "GAE ";

        public const String GAGAUZ = "GAG ";

        public const String GALICIAN = "GAL ";

        public const String GARSHUNI = "GAR ";

        public const String GARHWALI = "GAW ";

        public const String GEEZ = "GEZ ";

        public const String GITHABUL = "GIH ";

        public const String GILYAK = "GIL ";

        public const String KIRIBATI_GILBERTESE = "GIL0";

        public const String KPELLE_GUINEA = "GKP ";

        public const String GILAKI = "GLK ";

        public const String GUMUZ = "GMZ ";

        public const String GUMATJ = "GNN ";

        public const String GOGO = "GOG ";

        public const String GONDI = "GON ";

        public const String GREENLANDIC = "GRN ";

        public const String GARO = "GRO ";

        public const String GUARANI = "GUA ";

        public const String WAYUU = "GUC ";

        public const String GUPAPUYNGU = "GUF ";

        public const String GUJARATI = "GUJ ";

        public const String GUSII = "GUZ ";

        public const String HAITIAN_CREOLE = "HAI ";

        public const String HALAM_FALAM_CHIN = "HAL ";

        public const String HARAUTI = "HAR ";

        public const String HAUSA = "HAU ";

        public const String HAWAIIAN = "HAW ";

        public const String HAYA = "HAY ";

        public const String HAZARAGI = "HAZ ";

        public const String HAMMER_BANNA = "HBN ";

        public const String HERERO = "HER ";

        public const String HILIGAYNON = "HIL ";

        public const String HINDI = "HIN ";

        public const String HIGH_MARI = "HMA ";

        public const String HMONG = "HMN ";

        public const String HIRI_MOTU = "HMO ";

        public const String HINDKO = "HND ";

        public const String HO = "HO  ";

        public const String HARARI = "HRI ";

        public const String CROATIAN = "HRV ";

        public const String HUNGARIAN = "HUN ";

        public const String ARMENIAN = "HYE ";

        public const String ARMENIAN_EAST = "HYE0";

        public const String IBAN = "IBA ";

        public const String IBIBIO = "IBB ";

        public const String IGBO = "IBO ";

        public const String IJO_LANGUAGES = "IJO ";

        public const String IDO = "IDO ";

        public const String INTERLINGUE = "ILE ";

        public const String ILOKANO = "ILO ";

        public const String INTERLINGUA = "INA ";

        public const String INDONESIAN = "IND ";

        public const String INGUSH = "ING ";

        public const String INUKTITUT = "INU ";

        public const String INUPIAT = "IPK ";

        public const String PHONETIC_TRANSCRIPTION_IPA = "IPPH";

        public const String IRISH = "IRI ";

        public const String IRISH_TRADITIONAL = "IRT ";

        public const String ICELANDIC = "ISL ";

        public const String INARI_SAMI = "ISM ";

        public const String ITALIAN = "ITA ";

        public const String HEBREW = "IWR ";

        public const String JAMAICAN_CREOLE = "JAM ";

        public const String JAPANESE = "JAN ";

        public const String JAVANESE = "JAV ";

        public const String LOJBAN = "JBO ";

        public const String KRYMCHAK = "JCT ";

        public const String YIDDISH = "JII ";

        public const String LADINO = "JUD ";

        public const String JULA = "JUL ";

        public const String KABARDIAN = "KAB ";

        public const String KABYLE = "KAB0";

        public const String KACHCHI = "KAC ";

        public const String KALENJIN = "KAL ";

        public const String KANNADA = "KAN ";

        public const String KARACHAY = "KAR ";

        public const String GEORGIAN = "KAT ";

        public const String KAZAKH = "KAZ ";

        public const String MAKONDE = "KDE ";

        public const String KABUVERDIANU_CRIOULO = "KEA ";

        public const String KEBENA = "KEB ";

        public const String KEKCHI = "KEK ";

        public const String KHUTSURI_GEORGIAN = "KGE ";

        public const String KHAKASS = "KHA ";

        public const String KHANTY_KAZIM = "KHK ";

        public const String KHMER = "KHM ";

        public const String KHANTY_SHURISHKAR = "KHS ";

        public const String KHAMTI_SHAN = "KHT ";

        public const String KHANTY_VAKHI = "KHV ";

        public const String KHOWAR = "KHW ";

        public const String KIKUYU = "KIK ";

        public const String KIRGHIZ = "KIR ";

        public const String KISII = "KIS ";

        public const String KIRMANJKI = "KIU ";

        public const String SOUTHERN_KIWAI = "KJD ";

        public const String EASTERN_PWO_KAREN = "KJP ";

        public const String BUMTHANGKHA = "KJZ ";

        public const String KOKNI = "KKN ";

        public const String KALMYK = "KLM ";

        public const String KAMBA = "KMB ";

        public const String KUMAONI = "KMN ";

        public const String KOMO = "KMO ";

        public const String KOMSO = "KMS ";

        public const String KHORASANI_TURKIC = "KMZ ";

        public const String KANURI = "KNR ";

        public const String KODAGU = "KOD ";

        public const String KOREAN_OLD_HANGUL = "KOH ";

        public const String KONKANI = "KOK ";

        public const String KIKONGO = "KON ";

        public const String KOMI = "KOM ";

        public const String KONGO = "KON0";

        public const String KOMI_PERMYAK = "KOP ";

        public const String KOREAN = "KOR ";

        public const String KOSRAEAN = "KOS ";

        public const String KOMI_ZYRIAN = "KOZ ";

        public const String KPELLE = "KPL ";

        public const String KRIO = "KRI ";

        public const String KARAKALPAK = "KRK ";

        public const String KARELIAN = "KRL ";

        public const String KARAIM = "KRM ";

        public const String KAREN = "KRN ";

        public const String KOORETE = "KRT ";

        public const String KASHMIRI = "KSH ";

        public const String RIPUARIAN = "KSH0";

        public const String KHASI = "KSI ";

        public const String KILDIN_SAMI = "KSM ";

        public const String SGAW_KAREN = "KSW ";

        public const String KUANYAMA = "KUA ";

        public const String KUI = "KUI ";

        public const String KULVI = "KUL ";

        public const String KUMYK = "KUM ";

        public const String KURDISH = "KUR ";

        public const String KURUKH = "KUU ";

        public const String KUY = "KUY ";

        public const String KORYAK = "KYK ";

        public const String WESTERN_KAYAH = "KYU ";

        public const String LADIN = "LAD ";

        public const String LAHULI = "LAH ";

        public const String LAK = "LAK ";

        public const String LAMBANI = "LAM ";

        public const String LAO = "LAO ";

        public const String LATIN = "LAT ";

        public const String LAZ = "LAZ ";

        public const String L_CREE = "LCR ";

        public const String LADAKHI = "LDK ";

        public const String LEZGI = "LEZ ";

        public const String LIGURIAN = "LIJ ";

        public const String LIMBURGISH = "LIM ";

        public const String LINGALA = "LIN ";

        public const String LISU = "LIS ";

        public const String LAMPUNG = "LJP ";

        public const String LAKI = "LKI ";

        public const String LOW_MARI = "LMA ";

        public const String LIMBU = "LMB ";

        public const String LOMBARD = "LMO ";

        public const String LOMWE = "LMW ";

        public const String LOMA = "LOM ";

        public const String LURI = "LRC ";

        public const String LOWER_SORBIAN = "LSB ";

        public const String LULE_SAMI = "LSM ";

        public const String LITHUANIAN = "LTH ";

        public const String LUXEMBOURGISH = "LTZ ";

        public const String LUBA_LULUA = "LUA ";

        public const String LUBA_KATANGA = "LUB ";

        public const String GANDA = "LUG ";

        public const String LUYIA = "LUH ";

        public const String LUO = "LUO ";

        public const String LATVIAN = "LVI ";

        public const String MADURA = "MAD ";

        public const String MAGAHI = "MAG ";

        public const String MARSHALLESE = "MAH ";

        public const String MAJANG = "MAJ ";

        public const String MAKHUWA = "MAK ";

        public const String MALAYALAM = "MAL ";

        public const String MAM = "MAM ";

        public const String MANSI = "MAN ";

        public const String MAPUDUNGUN = "MAP ";

        public const String MARATHI = "MAR ";

        public const String MARWARI = "MAW ";

        public const String MBUNDU = "MBN ";

        public const String MBO = "MBO ";

        public const String MANCHU = "MCH ";

        public const String MOOSE_CREE = "MCR ";

        public const String MENDE = "MDE ";

        public const String MANDAR = "MDR ";

        public const String MEEN = "MEN ";

        public const String MERU = "MER ";

        public const String PATTANI_MALAY = "MFA ";

        public const String MORISYEN = "MFE ";

        public const String MINANGKABAU = "MIN ";

        public const String MIZO = "MIZ ";

        public const String MACEDONIAN = "MKD ";

        public const String MAKASAR = "MKR ";

        public const String KITUBA = "MKW ";

        public const String MALE = "MLE ";

        public const String MALAGASY = "MLG ";

        public const String MALINKE = "MLN ";

        public const String MALAYALAM_REFORMED = "MLR ";

        public const String MALAY = "MLY ";

        public const String MANDINKA = "MND ";

        public const String MONGOLIAN = "MNG ";

        public const String MANIPURI = "MNI ";

        public const String MANINKA = "MNK ";

        public const String MANX = "MNX ";

        public const String MOHAWK = "MOH ";

        public const String MOKSHA = "MOK ";

        public const String MOLDAVIAN = "MOL ";

        public const String MON = "MON ";

        public const String MOROCCAN = "MOR ";

        public const String MOSSI = "MOS ";

        public const String MAORI = "MRI ";

        public const String MAITHILI = "MTH ";

        public const String MALTESE = "MTS ";

        public const String MUNDARI = "MUN ";

        public const String MUSCOGEE = "MUS ";

        public const String MIRANDESE = "MWL ";

        public const String HMONG_DAW = "MWW ";

        public const String MAYAN = "MYN ";

        public const String MAZANDERANI = "MZN ";

        public const String NAGA_ASSAMESE = "NAG ";

        public const String NAHUATL = "NAH ";

        public const String NANAI = "NAN ";

        public const String NEAPOLITAN = "NAP ";

        public const String NASKAPI = "NAS ";

        public const String NAURUAN = "NAU ";

        public const String NAVAJO = "NAV ";

        public const String N_CREE = "NCR ";

        public const String NDEBELE = "NDB ";

        public const String NDAU = "NDC ";

        public const String NDONGA = "NDG ";

        public const String LOW_SAXON = "NDS ";

        public const String NEPALI = "NEP ";

        public const String NEWARI = "NEW ";

        public const String NGBAKA = "NGA ";

        public const String NAGARI = "NGR ";

        public const String NORWAY_HOUSE_CREE = "NHC ";

        public const String NISI = "NIS ";

        public const String NIUEAN = "NIU ";

        public const String NYANKOLE = "NKL ";

        public const String NKO = "NKO ";

        public const String DUTCH = "NLD ";

        public const String NIMADI = "NOE ";

        public const String NOGAI = "NOG ";

        public const String NORWEGIAN = "NOR ";

        public const String NOVIAL = "NOV ";

        public const String NORTHERN_SAMI = "NSM ";

        public const String SOTHO_NORTHERN = "NSO ";

        public const String NORTHERN_TAI = "NTA ";

        public const String ESPERANTO = "NTO ";

        public const String NYAMWEZI = "NYM ";

        public const String NORWEGIAN_NYNORSK = "NYN ";

        public const String MBEMBE_TIGON = "NZA ";

        public const String OCCITAN = "OCI ";

        public const String OJI_CREE = "OCR ";

        public const String OJIBWAY = "OJB ";

        public const String ODIA_ORIYA = "ORI ";

        public const String OROMO = "ORO ";

        public const String OSSETIAN = "OSS ";

        public const String PALESTINIAN_ARAMAIC = "PAA ";

        public const String PANGASINAN = "PAG ";

        public const String PALI = "PAL ";

        public const String PAMPANGAN = "PAM ";

        public const String PUNJABI = "PAN ";

        public const String PALPA = "PAP ";

        public const String PAPIAMENTU = "PAP0";

        public const String PASHTO = "PAS ";

        public const String PALAUAN = "PAU ";

        public const String BOUYEI = "PCC ";

        public const String PICARD = "PCD ";

        public const String PENNSYLVANIA_GERMAN = "PDC ";

        public const String POLYTONIC_GREEK = "PGR ";

        public const String PHAKE = "PHK ";

        public const String NORFOLK = "PIH ";

        public const String FILIPINO = "PIL ";

        public const String PALAUNG = "PLG ";

        public const String POLISH = "PLK ";

        public const String PIEMONTESE = "PMS ";

        public const String WESTERN_PANJABI = "PNB ";

        public const String POCOMCHI = "POH ";

        public const String POHNPEIAN = "PON ";

        public const String PROVENCAL = "PRO ";

        public const String PORTUGUESE = "PTG ";

        public const String WESTERN_PWO_KAREN = "PWO ";

        public const String CHIN = "QIN ";

        public const String KICHE = "QUC ";

        public const String QUECHUA_BOLIVIA = "QUH ";

        public const String QUECHUA = "QUZ ";

        public const String QUECHUA_ECUADOR = "QVI ";

        public const String QUECHUA_PERU = "QWH ";

        public const String RAJASTHANI = "RAJ ";

        public const String RAROTONGAN = "RAR ";

        public const String RUSSIAN_BURIAT = "RBU ";

        public const String R_CREE = "RCR ";

        public const String REJANG = "REJ ";

        public const String RIANG = "RIA ";

        public const String TARIFIT = "RIF ";

        public const String RITARUNGO = "RIT ";

        public const String ARAKWAL = "RKW ";

        public const String ROMANSH = "RMS ";

        public const String VLAX_ROMANI = "RMY ";

        public const String ROMANIAN = "ROM ";

        public const String ROMANY = "ROY ";

        public const String RUSYN = "RSY ";

        public const String ROTUMAN = "RTM ";

        public const String KINYARWANDA = "RUA ";

        public const String RUNDI = "RUN ";

        public const String AROMANIAN = "RUP ";

        public const String RUSSIAN = "RUS ";

        public const String SADRI = "SAD ";

        public const String SANSKRIT = "SAN ";

        public const String SASAK = "SAS ";

        public const String SANTALI = "SAT ";

        public const String SAYISI = "SAY ";

        public const String SICILIAN = "SCN ";

        public const String SCOTS = "SCO ";

        public const String SEKOTA = "SEK ";

        public const String SELKUP = "SEL ";

        public const String OLD_IRISH = "SGA ";

        public const String SANGO = "SGO ";

        public const String SAMOGITIAN = "SGS ";

        public const String TACHELHIT = "SHI ";

        public const String SHAN = "SHN ";

        public const String SIBE = "SIB ";

        public const String SIDAMO = "SID ";

        public const String SILTE_GURAGE = "SIG ";

        public const String SKOLT_SAMI = "SKS ";

        public const String SLOVAK = "SKY ";

        public const String NORTH_SLAVEY = "SCS ";

        public const String SLAVEY = "SLA ";

        public const String SLOVENIAN = "SLV ";

        public const String SOMALI = "SML ";

        public const String SAMOAN = "SMO ";

        public const String SENA = "SNA ";

        public const String SHONA = "SNA0";

        public const String SINDHI = "SND ";

        public const String SINHALA = "SNH ";

        public const String SONINKE = "SNK ";

        public const String SODO_GURAGE = "SOG ";

        public const String SONGE = "SOP ";

        public const String SOTHO_SOUTHERN = "SOT ";

        public const String ALBANIAN = "SQI ";

        public const String SERBIAN = "SRB ";

        public const String SARDINIAN = "SRD ";

        public const String SARAIKI = "SRK ";

        public const String SERER = "SRR ";

        public const String SOUTH_SLAVEY = "SSL ";

        public const String SOUTHERN_SAMI = "SSM ";

        public const String SATERLAND_FRISIAN = "STQ ";

        public const String SUKUMA = "SUK ";

        public const String SUNDANESE = "SUN ";

        public const String SURI = "SUR ";

        public const String SVAN = "SVA ";

        public const String SWEDISH = "SVE ";

        public const String SWADAYA_ARAMAIC = "SWA ";

        public const String SWAHILI = "SWK ";

        public const String SWATI = "SWZ ";

        public const String SUTU = "SXT ";

        public const String UPPER_SAXON = "SXU ";

        public const String SYLHETI = "SYL ";

        public const String SYRIAC = "SYR ";

        public const String SYRIAC_ESTRANGELA = "SYRE";

        public const String SYRIAC_WESTERN = "SYRJ";

        public const String SYRIAC_EASTERN = "SYRN";

        public const String SILESIAN = "SZL ";

        public const String TABASARAN = "TAB ";

        public const String TAJIKI = "TAJ ";

        public const String TAMIL = "TAM ";

        public const String TATAR = "TAT ";

        public const String TH_CREE = "TCR ";

        public const String DEHONG_DAI = "TDD ";

        public const String TELUGU = "TEL ";

        public const String TETUM = "TET ";

        public const String TAGALOG = "TGL ";

        public const String TONGAN = "TGN ";

        public const String TIGRE = "TGR ";

        public const String TIGRINYA = "TGY ";

        public const String THAI = "THA ";

        public const String TAHITIAN = "THT ";

        public const String TIBETAN = "TIB ";

        public const String TIV = "TIV ";

        public const String TURKMEN = "TKM ";

        public const String TAMASHEK = "TMH ";

        public const String TEMNE = "TMN ";

        public const String TSWANA = "TNA ";

        public const String TUNDRA_NENETS = "TNE ";

        public const String TONGA = "TNG ";

        public const String TODO = "TOD ";

        public const String TOMA = "TOD0";

        public const String TOK_PISIN = "TPI ";

        public const String TURKISH = "TRK ";

        public const String TSONGA = "TSG ";

        public const String TSHANGLA = "TSJ ";

        public const String TUROYO_ARAMAIC = "TUA ";

        public const String TULU = "TUM ";

        public const String TUMBUKA = "TUL ";

        public const String TUVIN = "TUV ";

        public const String TUVALU = "TVL ";

        public const String TWI = "TWI ";

        public const String TAY = "TYZ ";

        public const String TAMAZIGHT = "TZM ";

        public const String TZOTZIL = "TZO ";

        public const String UDMURT = "UDM ";

        public const String UKRAINIAN = "UKR ";

        public const String UMBUNDU = "UMB ";

        public const String URDU = "URD ";

        public const String UPPER_SORBIAN = "USB ";

        public const String UYGHUR = "UYG ";

        public const String UZBEK = "UZB ";

        public const String VENETIAN = "VEC ";

        public const String VENDA = "VEN ";

        public const String VIETNAMESE = "VIT ";

        public const String VOLAPUK = "VOL ";

        public const String VORO = "VRO ";

        public const String WA = "WA  ";

        public const String WAGDI = "WAG ";

        public const String WARAY_WARAY = "WAR ";

        public const String WEST_CREE = "WCR ";

        public const String WELSH = "WEL ";

        public const String WALLOON = "WLN ";

        public const String WOLOF = "WLF ";

        public const String MEWATI = "WTM ";

        public const String LU = "XBD ";

        public const String KHENGKHA = "XKF ";

        public const String XHOSA = "XHS ";

        public const String MINJANGBAL = "XJB ";

        public const String SOGA = "XOG ";

        public const String KPELLE_LIBERIA = "XPE ";

        public const String SAKHA = "YAK ";

        public const String YAO = "YAO ";

        public const String YAPESE = "YAP ";

        public const String YORUBA = "YBA ";

        public const String Y_CREE = "YCR ";

        public const String YI_CLASSIC = "YIC ";

        public const String YI_MODERN = "YIM ";

        public const String ZEALANDIC = "ZEA ";

        public const String STANDARD_MOROCCAN_TAMAZIGHT = "ZGH ";

        public const String ZHUANG = "ZHA ";

        public const String CHINESE_HONG_KONG = "ZHH ";

        public const String CHINESE_PHONETIC = "ZHP ";

        public const String CHINESE_SIMPLIFIED = "ZHS ";

        public const String CHINESE_TRADITIONAL = "ZHT ";

        public const String ZANDE = "ZND ";

        public const String ZULU = "ZUL ";

        public const String ZAZAKI = "ZZA ";

        private LanguageTags() {
        }
    }
}
