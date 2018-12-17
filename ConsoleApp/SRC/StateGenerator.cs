using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OptimaliserenPracticum
{
    public class StateGenerator : SimulatedAnnealing
    {
        private Thread[] successorfunctions;                       // A list of threads, each containing a successorfunction
        private State oldState, newState;                              // Each iteration is given an old state, and must return a new state
        private List<Status>[] status1, status2;                        // The two seperate days of the old state, written explicitely to make calcuations easier
        private volatile bool foundSucc;                                        // A bool that checks whether a successor has been found
        private object orderLock;

        public StateGenerator(State initial)
        {
            foundSucc = true;
            orderLock = new object();
            oldState = initial;
            // Initialize the thread array
            successorfunctions = new Thread[7];
            successorfunctions[0] = new Thread(RemoveRandomAction1);
            successorfunctions[1] = new Thread(RemoveRandomAction2);
            successorfunctions[2] = new Thread(AddRandomAction1);
            successorfunctions[3] = new Thread(AddRandomAction2);
            successorfunctions[4] = new Thread(SwapRandomActionsWithin1);
            successorfunctions[5] = new Thread(SwapRandomActionsWithin2);
            successorfunctions[6] = new Thread(SwapRandomActionsBetween);
            for(int i = 0; i < 7; i++)
            {
                successorfunctions[i].Start();
            }
        }

        public State GetNextState(State old)
        {
            oldState = old;
            status1 = oldState.status1;
            status2 = oldState.status2;
            foundSucc = false;
            while (!foundSucc) { }
            return newState;
        }

        #region ASCII
        /*
         * yssso+++/+++////////:::////+/////+o++++++++++ooossssoooossssssyyhhyyyyssooooooo+oooo++++/////:////://:----------------.--::--::////:-..--:+ossyyhmNNMNNNyo+//:-.--:::-:/++///::----:::/+oo+//+s+///////+oo++/::::/::///++/::/+/-.....-//:--/:-..:.:+:--..-.-:-.-/++:::/sds:/o/-:oos-////+++soo:...--:-://+s/
        o+ossooo/:::::::::--------:--::::://:::::::::-----:---:::----:://///:::::::------://///+/:://+ooosshss+:...`........```......--------://:::-:/+ossyyhdh+:-----/+/:---:-::::-://::--------:///////:--------:::::-..----:--:------.--.....-:/:://-....::-.....-:-..::/:--/hd-:-+:-/+s-:::///++++:.-...-.:::oy:
        :://///o++oooooo+/:::-----::::::-------------------------.------...-------......-/o:/+so++/++/+++o+so+//-`....--.....--.......`.....``..-://+///+osssso-`   `.-:/++++:-.--------::::-----------/:----:--....-----.....---...-----..--......:///++....---.....-::..-:----oN+-:+/:-:+-:--:::/://-.......--/so-
        .....--..:/o++ooyyyyysso+///+++//:::::::--...--....-------.---...---...--.......:+.::-----::-..-:::`:..::.---..---::--------..-....```.....-:/+osooooso.`    ```..://+o+//:----..--::/::-------------:--:--.....--....`........-......--.....-:+//:.......-...---.......:hd.-:+/.-/.:-.--:o:-:-........-os--
        -:::/+++////+ooo++oooosssssssossyyso+///+os+//://:--.....--------...--::::::...-:++-.-..-::.-//::-.`.`...--........`..--------:-.....`...-----:/+oso+/+.     ````.....-//+oo+/:...-----:::-----.--.--.------............`...````..........`.....-///-......-.....--.....-/d+--/:..:.:-----o/--.........-+s./
        :////+++/://:::::::://+/+ooosyhddmNmdhyyooysyyyssoooo+/::---------..------:..:///-.-......-+/:....----..--.:-....--:----------:::::::------:::/+ooooo++.`    ```......`...-:/+oo+/:--.-------------.....-.....--.....``..`..`````....-----........-::-..........-::----..-ys-.:/:..-------o/-..........://.+
        --:://:/+////:::::::----:/+//++ooossssyhyyyyyyyhhhhyyysso++/::://:::::----::///--......--://o-....`..--.-::-::-.`.`..`...---::::::---------::/+oooooo/+.`    ```..---.......---/+oo+//---------.-:-----..................````````........--......-..--:-.......--.--------+s/--::...-:-.-.//-.........-/+-:o
        ..----..-.-:::::/:::--------://+oooossoo+osyyhhyyssyyyyyyyyyso/::---::--::+/:--.-:--.-:/+/..+/:-.`..`.....::.-/:.`...-..-----------.-.-----::/+oooooo++`      ``.....--..---..----:/oss+::---------.-:::-----.......---...`````..``..``......----......--..........--..--.:o+:.--....:-.../:-.....``..-++-/o
        ``....--.-..---::------::::::::-:--::://++++++++++///::////:::///---...-/o/-::::/o/:./yo-...--:::/::--:::::---:++:-.-::------.......--::---:::/+osoo+++.    ``.........--..---.-------:/oo+/:-..--.--....---..---........```........-....`....-...-......--.``...........-.+/-...-..-:-...--.......`..//::+:
        ````...-:---...-:://///////::////:::::::////::--:::--------.......--..-/sh+----......-:oo-.`.`./oo:--.:::-..--/o/---...`...........`...---::::/+osss++/`     ``....-------------------.---:/+o+/-..----...............-...:://:////+o++++:......................``.........-/-...--.----..............o+://-
        .----::::::--...---::::::::::///////+++++++///:-------..................-/ho------.....-/o/.`./so-..````.....:+:.......`..``.`....`..`..------://+++++/`    ``......-------::::-----------..-:///::--...................-:-----::+:://:::+:....``...........................:.....-`-.--...........`./s+:/-/
        `....----.........------.-----:::::::://////++++///::----.....--..-.......-yo-.----:-...-++///++:.````````..:/-.```..`.......`.`..``````....--://++++//`     `.......------::-:::---::::::::-.--.-://:--...........-....-:-..-.--++/-..`.-::-......................................................`.+/:::/:
        ........---.....-----:::-:+++ooooosssoo++/:::::::--::::--.--.-:-.--...--...-++:.....::--.-///s+-.`........-::....--.`........`.....`.``......--:/++++//`     ``................--::::--:::::::-..------:-.....```.``..-:-`..`.://--:-...`..-/:.................................--...................:/:::::.
        ....----////::---:::::::::++oooooosssyyhyysyyyyyso++++++++//:///:---.-..----://::..:----:/o++ys/::..-::://-.`............-----:-::-....---.--::+++o++/+`      ``............-......-------------...--............`.-/////::::/+-..`.-::......-::....................................................///:::.+
        -..-:///::::------::----------:/+++++++++o++/:/+/::::/+o+o+++osso++::/:----:--/++:/----.-:--/++::::://:-::`.....-:/--```.-.```  .::-...-:-```.-/+++++++.``...`.-----:---------.--------.----....-.....`........`.../+/-//---+s+.`...:o+------:oy-..................................................--://:-/:
        dhhyyhhys+++++++/////::::::----:/++ooooosyysoooo+/:////+////+/os++o:+o+//:-:`.::--//----:-`-//++//++o+/+oo/+-:..:++:-````.-:   ``.--....-.`   ..-::::/o-.``.:...---..```.......://::/:/-:::---------::----.------..-//:..--.-/s/..-++/...-..:oy+...................................................--//:://:
        mNNNNNNNmhhhhhhhhhhhysssso+//:::--:/+ossyoooooyhyy+++////::::-+s++s://:----/::/:-...`-:/+o+oo+/:-:::--/++++/-++ssosso++osyyo--.``-:-....--.   `.-:::--/`   `--`..-:-  ``      ``+/-.-:/-`````-::.-.--.``````..`.-:-:-:+:.`..-:/o+/+/-...`.-:+o:.....................................................++/:://+
        mNNNNmddddddmmmddhhhhyyyhhhhhhdddyyhyyyyy++/::+o+o....````-s/-:o+/+:/+/::::/::/:-.-:-:::::-----..-.-..::--...````...---::://:--::+sooo/////:/://////:-/````.-/-:-:/:``````````` ./:---/:``   `-/---//`  ```    `/:.-:::/:-::/+/+oo/-::-..::oo/--:.............--...................................:o/:///oo
        NNNNMMNNNmdhhyyyyyyhhyyssssyyyhhhhyysooo+/:::///os`` `` ``-s/.:///::/o+:::::/::::--/.---://-:++///-:----....`.``````.-.......```.-:/:/````````.::/++++o....--://////://////////-:::::///::////+++//++-..:---..`./:-::-``.::-:/:++//:-::--::o//:-/--...-......---...................................:::++++oo
        mNMMMMMMNNNmmmmmdyyyyyyyssoo+//:/++++sysso:/:::/oy::::::/:oo:-:/:/:::++/:::::::::::/::.`-o+-/so//+:://///:----------:+:::::::----::://-::-:::-::////:-/` ``````````````....`.:-.``````````....-..---.```.`````.--....` ` -..----.-.```` ```.``.-//:--//:-..:-----....................................--:/+/:
        mNMMNNMMNNMNmddddhhyyyyyyyys+/:-...--/sy++:::://+o+://://///:-:/:::::://:::::::::::::/:-:+/-:++/::--:::::-..-....-----..-----...--..::----:::-:://+oooo-.....---------/:::/::////:--::------::-:------..-:-..----.--:-:-.-.``..--:::---.----.---:--.-:::/:-::----.......................................-:--
        mNNMNNMNNNNmdhyhhhhhhhddhyysso/-...-/+++/::::://++o/:::::::::-:/::::-::::::::::::::::::::/:::////:---:::-.------------::-------------.:::::::///++oo+oo..`...::-------+:---:://:::-----::--::/:/:::::://-::----::-/::://:/:::----::/:::--::--::///o//++o+o/oo:/+/:.........................................-
        mNNNNNNNNNNmdhysoosyyhhhhyyyys+:-../+oo+:/:::://///:::::://:-://:::::::://:::/+/://:/:::/+/-:/::::---------------------------:--:----.-::---:///+oososo-``...---::--:/s+::::://:::::::::::::////////::/::--::---/:---::-----------::----:-.---:-::/:://///++o/+s+-.................................`.......:
        mNNNNNNNMMNNmmmdhyyssyyhhhhyyyso+/::--+o.:/:::/:/:::::::::+/:-//--:::::://:::+:..:+:/+/++/:-::----:-:----------..--------------:::---------:/s//+osssys:.`...-++oo+::/+/:::::::::::::::::::://++/::///:::::-----::------------:::---:::----:::::::-:-::::/+++//o+-........................................--
        mNNNMMNmddmmmmmNmdhyssssssssosssyso+/:/o+::::///+:////://://:.:/:-:::::://::://///::::://::::/::::::-------------------------/:////::-----:/+oooossssso-````-+mmmmh+:::::::://::::::::::/::::///::::/+/:--::--::--------------------------::::---------:::://:/+/-........................................--
        NMMNNNNmmmmmddhhyso++//+ooo+++oooo++++++://////+s//+///+o/++:.-/::::::/+//o://:///::::-:::-////:////-----::--::-----..-------o+++///:+/:://+sh+ooooosso.` ``./hdhhs/:-::::::/+/+so+++/+oo++/::/:::--:/:+/--:-----:-----:----------::::::/::://:------------:::/+:..........................................-
        mNNNNMMNMNNmdhyhhyhysoo+//::::---:/++++o/--ydho+o:+/::--:-+::`-//+:::://:oyyyyys+://::-::::/o:-:/:/+---:::--::-://::---------s///:/+:o+/:/+++s/++oooyyy-....-////+//::::::://o+oso++osoooos+::::::://++ss////:/:/++++/:/:/:------///////o/+/+o+/--:/:::::::::/++-...........................................
        mNMMNNmdhhhhdddddhdddmdhyo+++//:-.-://+o: `dMdo+o:o:/.   `o--`-///.//.``:dNNNNNd+`-/:--::::++.`/o//+---::.` `   .--/-----::--s.```./:oo.``-+os//+oosyoy:.---:.`.../+::::::///+...:+ooo.--ss+::::::///:--y/++/:::s/++/:---o/:-----::--:/os+::+oo/--+/::+o/o-.-+o+-...........................................
        NNNNNmddmmddmNmdhhhhhdmmhhhddhysooos++ss:  hNdo+o/o//-   `Ny:`-//+yMy- .+MNNNhs+---+:/:://:+/.`:so:+.--:-.      `--/---:-----s.   .//o+.  ./+s//+oooo-/` .--:`    :+-::-::////   `+s+/   +s+::::::/:`   o:+-`   +:+:`    o/:--:--/.  `-oo-  :oo/-:+.  -+:o  `-+/-..`........................................
        mmNmdhhhhhhhhdddhhhhhdmmmddddmmNMMMmddho/  yNh+/o:o//.   `Nh/`-+/oNNs--omNNNo:smmmd//o//+/:/o-`/ys/y.--:-`      `--/---:-----s.   `:/+/`  ./+s:/++ooo./` .--:`    -/-::::::///   `+s+/  `os/::::::/:`   o:+-`   +/+:`    o/:--:--/.  `-oo-  -oo:-/+.  -+:+  `-//............................................
        yhhhhhyyyhhhhhhhhhddhyyyyyhyhdmmdmmmmmds+  yNd+/o:+/+.   `my/`-//oNMy+hNNNMM--yNNNh+:/--::::o. :ss/o`--:-`      `.-/---------o`   `:/+/`  `:+o:/+ooo+./` .---`    -/-:::::::::   `+s+/  `os/-:::::/:`   +:+-    +/+-`    o/:--:--/`  `-oo-  -o+--/+`  ./:/  `-+/...`........................................
        hhhhhddddhdmmmNNNNNmmmmmmmmmddmmNmmmmmds+  yNdo/o/+/s:   `dh/`-+/+dNd:sMNNNm-:hMNNd+::---::/o-`:ss/s`--:-`      `.-/--:-::---o`   `::o/`  `:/o:/+ooo+./` .---`    -/-:-:::::::   `+s+/  `oo/-:::::/:`   +/+-    +/+-` `  o::--:--+`  `:os:  /so:::+`  -/:/  `:+/...`````....................................
        /////////+o++oooossyyyyhhddmNNMMMNMNNmyo/` yNds+o/o:s:   `dd/`-/:/:sm+sNNNNmymNNMNso:::::::/s-`:yy+s`--:-`      `--/.:/:::---o`   .::o/`  `:++://+oo+.:` .---     -:-:::::::::   `+s+/  `oo/-:::::+:`   +/+-    +/+-`  ` o/::----+`  .:os.  +yo:/:+`  ./:/  `:+/...```````.............`....................
        yddddddhyyssyyyhhhhyyso//:::/ossssssssso+  yNds+o+o+o-   `hd+.-+/sh+ms+hMMNNMMNNMm++::::::::s-`/ss+s`--:-.      .-:/-:/-:+/--o.   `::+/`  `:++:/+++o+.:` .---     -:-::::::::- ` `+s+/  .ss/:::-:-+/`   +/+-    +/+-`   `o:------/`  ./os`  +y+:/:+`  .///  `:+:..``````..`````.....``............`.........
        osyyyhddddhhhhdhhhyhhhhhdhhhysoo++:///+s/` smms+oy+/h+`  `dm+.-//hMomNo/hNdmNNNNMm/+:-::::::s-`:yy+o`--::.      .::/-:--:+/--o.   `:/++`  `:++:/++ooo.:` .---     -:--::::::/:   `+s+:  .oo:-::::-+o:   +/o-    o:+:` ` `o/::::::/`` ./+s.  +yo:///```-+/:``.:/:`..`````````....`.```.....`......```.......`
        ossssydmddmNNNMMMNMMMMNmhhssdmdhosoo+++oo``odds++y+/dy:.``hd+.-//yM/hds::++sNNNMMm:o//::/::/s:`+hsoo.--::.      .::/.---::---s.`  `:/++`  .//+:/++ooo.:` .--:    `-/::-:::::::  .:os+-  -oo/-::::-+o/-.:s/o:....o/o/--:::o/:::/+/+---/oos:--+so::/+:///++o+o+//:````````````....```......`....```..``......`
        hhhhhhddysooyddddddmmNNNNmdhhhhdhmmNNdsoo:++o+++++//hmmdyymh+.-//yMyhhddddmdmNNNms-++s+++:::s-:yhoos.--:/-  `  `-/:/---------s---.-+/oo..-/oo+-/++oso-+..---/..-://+/:-:::-:/o//+ooooo//+so:-:::::/++++o/://:::://:::/++/::--::/:+//+/://:::-+o:::-:/::://://:/:`` `````````  ```...````.....````...........
        yyyyyhdNmmdhhhysosoossyyyyyyyyyhddddddho:+ysyys++//:ohddy+s//.-//ohyysssoooo+oooo+/+//::::::hoooo+oo.--:++::/:-/+o/:--------:o/ooooo/o+//+o++o::/+osyyy:.``..--:///:----::::/+::::://::::://::::::::::::::--:--:-:-::--:----::::-+::///:/----+o::/::/:::+/-++//:`` ```` ``   `````.`````.`````..............
        yyyyyhdddhyyss+/:::/+++++osshmMMNMMMMMms-:++sso+//-//:/+o++:/.:+-://+::::/::///+o+/:/o::/:::s/:::::s.-----:-----::-----------/::////:o---:::/o//+oossss. ```.:---::::--:-::::+:::::/+::::://::::::::::::/::::-::-:-:::-::::::///-+:://///::--+o::/:////:++:/o+/:.     ``    `````..`````````..`.............
        mNNNMMMMNNNNNmmmmdddddhhdhhhdmNNNNNNMMms-:/+s+///+:/::://:::+.-/--:::::::/:-:::/+//+/o:::::-y/::::/s.------------------------/--:----s------/o:/+oossss`  ``-:::---::--://///+:::::/+::::://::::::::::::/:///:::-:--::-::::::///:+:://///:--:+o:::::::::+o://///`  ````   ````````` ` ```.`....```..........
        mNNNMMNNMNmmmddddhdhhyyyyhhhysshdmmNMMmy-:/++/:///--::::--::/.-/--:-:://///::::://++/+:-/:-:y/:--:/s-------------------------/-------o-----:/o::/oossss.```..:::/::::::://::/+::::://-:::://:::::::::::::::::--:-:--::::/::::////+::////::---+o:::::::::/://::/:   ```  ```   ```  ````....````````.........
        mNNNNNNNMNMmdhhhhhdhhhhddmmddddmmmNNNNmy-/ss+:://+--:/:::--:/.-/----:-.```.-/:-::////+/:::::s/::::/y::::---------------------+-----.-o------/s:/++ossoo``...--/:/:::::--..-://-:::-:/-:::://::::::::::::-:::-``-::--:::://++///+/o///::/::---+o:::://////://:::.  `.`  ```` ```````.....``````...``....```..
        mNNNNNNNNNNNmdmmmmddhysyhdmmdhdNMMMMMMNs-:/+/:://+/+//:::-::/--/--:/-`      -+/::/++os+++:::s/::::/y:------------------------+-----.-o-----:/o:/++ossss.`....-/::----...-..-:/-:::://::::///:::::::::::::::::-.--:::-::/+/o+///+:o//:://:-::/o+:/::///:::::/-.`            ````````.``````````````` ``````..
        mNNNNNNMNNmddhhhhhhhdhyyyhhysydNNMMMNMNy-:://:://++ss++o/++++--/--:+`        ++:://++s+//:::y/::::/y:----------------::------+-----.-o------/o/+++ossyh:..--...-.........--::/-:::://:-::://::/:::::::::/:////::::::/+++/:///:://o//::+o/-:+oo+:/-:///:::::-``          ```` ``````` ```  ``````    ``````..
        mNNNNNNMNMMNNNNNNMMMMNmmmmmmmmmmNNNNNNmy/:://////+/+o/+o++o//-:/::::`       `/o+:::o//--::::y/::::/y::---------------:/:--:--+:------o-----:+o+o++osyyy.```````.......--:::://::::://:::::+//++/:///::/:/:///////::///://:///////+//:///:::::/+oo//+++/::..-.```   `    `` `````  ` `     `      ```````````
        mNNNNNNMNMMMNNNmdNNNNmmmmmdhhhhhhsoooooo+ossooo+/o::::::/::::-:+//:o.`      .+o/:::+//-.-:::y/::::/y::---:----------:/:--:/--+:------o----::++/+++ossoo`   ````.```.---::--::/::-:::::::::///++//++/::/:///:/+++/+//++/o++ooossssyhyyyyyhhhyysss+/s/++:.` `.``` `   ``    ` ````` ```                ``````.
        NNMNNNNNmmNNNmmmddddmddddhhhhhhhhyoyo::+++/o+/://so:--:::++:/-:++//+o/.    .sy+::::+//--::::y/::::/h:-:-:/----::---::///::/--o/////-:s:::///+o:/+ooooo+.    ```......`......--:----::--.--::/++o+sssoosoysssyhyhhmdmmmmmNNmNNmmmmmmmmmNNNNmh+-/so+++++:.   `       ``                    ```      ````````..
        +++/+so+/+oosoooo++//::::.----:----++::/+/-+/:://oso/::::+o:/-:+++///+++//+++/:::::/+/-:/:::s++//++y/::://:-:::::::::/:+/::--+oso+::/:://::::://+ssoo+:`   ````````````.-:///+////::/odhyyyhssdy+hyyhhsyyoo++ooosNmmNNNNNNNNNmdmNmmmNNmmmdy+:--/o+oso///        ```                 `````    `````````......
        ::::/oo+/////////+++oosyysssssssyyyhds//:::+o+++//++o+//+ysoo:-/o+//://+//:::::::::+/::+o:::+/ooos+//////++//:::::///+/+/::/::+++/---::+//:::///+o+/:-`         ``    `::::::-.....-/oddsssysoms:hsoyy+o/:/:://++NNNmmNNNNmNNmmNmmmmmmdyy++/-..-::os+:::      ```               ````````   ```..............
        mmNNNNNNmdhhddddmmmdddmmdhdddhhhhysssoooo++y+///+//:+//+os+://+os+///+++++/:/::::/+:++/oso+/:/ooooo//+o++ooo+///++o+osossosysoyhhhs//oyhhddys/-...``. `        `````  `/:/:..````.::-+oo//osoodo:y+/+/:/:-::::/++dmddhhmdmddddddddhyyso++/+///:::/oy+:-`      ```               ``````````..................
        NNMNNNdddyysssssyyddddddddhhysoo+:::///+:/+o/-:/oydhs/.--.--ymNds++oso++/+o///++++s++sssyhhysssyhhhhhhmddmmmNmmmNNmdhdddhhhhhyddddy:.sdhhdh+-`  ````  `   ``      ``..-:./:.`````.:::+:/::/+++y:.y+://:/::/:://+/yhyysyhhdyso++oo+//+++/++yso+oo:oys/.`                       `   ``````......`.............
        NNNNNNmdhhyyyyyyyyyyhhhhhdhhhhysso+oo/:::/o/-...`-MMd/....-+dNMMd:-++o+++/+::hmNNNNmmNhshMMNMy+hMMNMMdmNMNNNMNNNdhho:+/:/+ssy+soso/.-oo/so-` ``..`    `        ``...``.-.:-``` `-://++:/:::://o..y+/++//://::/++/hhhyyyyhhso++o++/ooo++/sos:.-++`:o/-```                  `` `````    `````````````...``....
        mNNNNNmddhyyyyyhhhyyyyysooo+++///::///+::/+/-.`` .mNy:./osydmmNmh-./////+s/-:dNNNNNNNMy+yMMMN/:yMNNMNNmmdhhdhssyo+ooo+/ydddhyyoos+-///-/+:``````     ``    ````.```````:----` `.--:/+sos//+++/o/+yoossyyssssssssyhhddhssshyyhhdhsoys//+++s:``.:+///-`                ```.````````...------::-.``............
        hdddmmNNNmmdhhhhhhhyyyyso+oooo+++//+o+//+o-:-..``-+//++oo+//+++/:-/+:-:/oddhdNNNmhNNMMo/sNmNy:/hMNNNdss+:::/+:///++oyooddhhysssyy+/ysooo+:..-```     ``         ````..-:.`/o/::++osydddddmmmmddmmmddmmmNmmmmmmmhyhyhmhoydmmmmmmmdmmddhhhhs.   `..`      `````````...........`..----::-----//:-..............
        ddddddmNmmdddhhhyyhhhhyyyyhhyysso/+yyyo/s+`::-.`.-:.-+yyhmdddddhhs-:--:+hMNNMMNMNNMMMM+:smmmo//hMNmo+/oo//+s++o/--+++ydhyssyysy//oo+sy+:-::--`.-.`             ``...-::/+shddhhhhdhhydmdmmmmmNNNmmmdddmNdmmmmmmdhyyyssymmmmdhddddmddmdhhmy-` `` `   `.....`..............-/-..-:-:---..---::--.............-
        syyyyhdNmmddmmmmmNNmmmmmhyyso+++/:ommho+h:../:..-:+:::/+ohydmdmMm++.:-/yNNNMMNMMMMMMMMs/+dmmo/-ohy/oo+soshyssyo+//ooo+yhhhyoo/hsoosyy/-`````` `.` `.` `   `.-:::/+/::::-:+osossoo+osyyhhhyssyyyysssyhdmmdhssooyhhdhyyhhhdhhhhyssyhyssyyyy-.`.....`.......``....`.:.//ooo+/yo+:.....//:------..``..``.......-
        syyhhdmNmddmNNNNmdhhyyyssssssoooosyhddhshy+/+///:/:-..---:oyhshmh/s-::+hNNNMMNNNMNMNMMdo/smdo::::+o++hhoymd+ddysoossoys+oss+//+/+so+-`.`          ``` `.``.---....``` ```......:::/+oo+++/////+++osoossoo//++shhdyoosso+s+oss+/sosoo++//::--.........----:.......:./+oyhsooss/---:-+:--.....................
        ysssyyyhhyyyyyysoo++//++sso+++////:::+o+sdho//++//::::::..:os+shs/yssoohNNNNNNNmmNNmmmdy+:so/.::/o+oo+s/ods:hhyhhy++:/soosoo+:::+/-`                   `   ``                `...------------:://///////+oyhhyo++++//////:::--:::+/:---:+:/------.-----`-/-/---.-.::::+oos::----:---...............-......--
        hyyysso+++/////////////+++++osyyyso++++++syooooo++oo+++:-.-:-:oysy/dhddmNddhddhysyyyhyhyssssooosssym+-o::s+-yddhy/.:-.syysoo//.`````                  `  ``             `       ```....-------:::--://ohdhyo/::-:-:-----.....--....`./o+-+-.--.......-.`.:---...--::-.-/+s+-----.--...----:-.`..........----
        -.``..-:/ooooosyssooosyhhhhysso+++oyyyyss+///:::--::::::-:/:::::+yhyhy+odmyssyyso+++sshddNNMNNNNNNMNy+o/:+y/hhyysoshy+oooo+/..``  ``                  `  ``             `   ```.....----.-----:::/shddhyo/+/:--:-:::-----:-....---.:so:-o.`..........--..-:----::://:-/s+s+/------..-......---...-:---..-...
        //osyhddmmmmmmmmdhhyyhdddhhyysssyysssso+/+++///++++++ooooooooo++oyhdho:/dMNmsyhhyso+/sdmmmNNMNNNNNNMmsoo+/yyyo:-:+os+..`.-...  ```    ``              `            ``````..-----------.-.---:/oyddhso++o//+/////://+/////:------.:sy+:/+-...-//::`:--::--:+///+oo++/:::+hyss:----:-..--....`--....-:///:::::
        NNNMMMNNNMNNNNNmddhhdddhhyssss+/++ooossssssso+oosooooossyhyyhdmdddhys/-:omNNmhsso+sosyddddmNMNNmmmNNmhsoo/+y+:++oso-.``  ``    .:.``                           ``..---..-----.......---:///ohdhyoo+/::/::://:::/::::::-:::-::-.-+ds:/o+-::-:/o+:/-+:://:/-/++oooo///----/+ys++/---:.:---....:--.`..`-://::::
        NNNNNMMNNmmddhyyyyyyysssssyso++++//+ooo+//++oosyhhyyyyyyyoo++///::-.```./syydsssydNNmmNNmmNddmmNNNmmmmdhs/:+o+yyys+-..`````              `          ``-`  ```...---...............---:/ohhdhs++oo/-..---.:------::::--------.-/hm+:oo/:/+/::/+/-::----:::--++ooos-:/-....-/o++//:-::/:-..-:..---.....:///:::
        NNmhyssyysoosyyhyyyyyssoooossooosyyyyyhdddmmdhyyo++/::::-:-::/:----://+ssyhhdhhhhhdhhdddhyyhhhddmmmddhdhysosyysoosyydyo+/-````        ``   ````   `..:+`    ``.........```....-:--:+shmds+/+oo+:------..........`.-.....``.-+yms-/s/-:/-...//:-..:.-..--:/./+so+s.:-+:...:..+++++o---//:-::/-...-:..`.-/++++
        hyyyyhhyyosyssooosssoosyhhhhddddhyhyyysyyhysooo+///////:///+oosyhdmddyyhysyyhysso++ooooooosyssyyyso+/://+sdmmdmNmddhyso/-.````````````-...--...``..--:+`  `````....---...-----:/ohddso/+oso/::/::::::--:-::::::---::---`.:+hmho/yo:--...-...------...---+-:-/ysoo-+/sho:-:..-++/:/y-..-/++/+o+:..::.....-/os
        yhhyyyhyooossssyyyyyhhhhhyyyssoosoo+oossssooo+++///++oshhdddddddhyyyyssooo+/:::::://:/++++ooo+/:/+oshdmmdhhyssoo++++/--...``````..``````..:///-----::./`  ``````.`.-:-::--:+oydhy+//++so+:::::::--:::--:::://:::/::/:-`-+ydds++s+:::-..-......:+/------::/-:/+ss+/-yhmd+-y+.-:/+/:+s:.-/oyoo/++:..-//.....-+
        hhyysoooossyyyhhdhhhhhyssssoo++++oo++++++//++osyhhdmmmdhhyssyyhddhhyso+/::::::----:-/+/////ooyhdmmmmhyo++///+///::-------...```..``..--//:://-.---://:/`    ``..-:/:-::+syhhho//:/++/::-:------.......--...-----..-.../smdy++so////:-...`...-::/o:://-:/-o.::+yy+o.ysdNy-/h::::/++/yy+::/sy+++::::..++-....-
        yyysooo/sssooosyyyyyyyysoooo+//:::/+osyhmmNmmmdhyyyssoo+ssyssssosoooooooooo+++/:-:/+oydmNNNNmhyssoss++///:-----.----.---........:/+++/:-::-.````.:++/-+` `.-:/:---/oyddhyo////s+/-.......`.......................``-+ydmy/+so/:/:-:/-......+++/:+-://:/+/+:::/ymys./hdys+:do:::/+///dms/::sy+++/----.+o/-..-
        osoosssoo+/+ossooooo+//:///+osydmNNNNmmmdhysssso+/:-......---::/++/+ooo+//::/osydmmmmdhhyso+++++/:-........-:..--.........-:/+++////:::-.`..`..---://:o:--..--+syddhs+/:+ooo+:...----------..-.-.--------.....``./shmmy//yy+::---.........`so/://::://++//+/:-+hy:--sdyyy:+++::/++/-ommy//-/o/:::---:-/++/..
        ////::-:+o+/::::::/+osydmNNNNmmmmhyhyhhdddhyso//::-::-----....--:://++osyhmmNNmdhs++++oooo++/::::------:/++//:--...--:-:/+o+//+///:-``````..````..-///s:-:/oyhhyso++oshys+/::::/:::/:::::::-:--:::::::::::--..-/shmds/:sho:-.....`........-ss::o-+-:-://-:+/-:+hd-o./ohdsy:+//:-:+:s:sdds+:::o++//-:.---+so/
        -::::/:--//+oyhdddmmmmmmmmmdhysoo++++++++o++++/////++//////:://+osyhmNNmmdyoo++++o+oo+++++/:::::/+oo+++++:::------:ossoo+////:-.....`.......``....-/shmhyooo//:/+syhhhs+////////+/::///::::::////::::::-:...:+yhmmo::oho-.........`..`..../y+:+o-//+:-::--:+-:+sm//./+ymhyo+/+/-.:/ss/ohyso::-//://::---:ohs
        -//+shhhmmmNmmmdddmmmmmdhys+/----..........--:/+oo+//::/+oooshmmmNmhso++/++//::---------..-:/+ossso++/:------:+ssooo+++++++/://::/::::-.````.-.--+ydhhMy+:..:oyyyso+++//++////:/://///++://++:----.----. .:+ydmNs--syo:`.`..---.........``so:/o:+-oo/:::/..+-+/:y/..:/oyyyd:o/++-.//++/+yyyo//:-.-/s.:/-.:od
        hhddmdhyhhhyyysso++oooooo+++++///+++//::::/+++//:--+ooossyyyyyys+///+++//:::::::::::::::/++soo+/::::-::::/+syho//+oo++++++oo+/////:::::::::--:/sso+++os:::/+++/++++////::://::///:-:/:::///::::/:://::--:+osyhh/:/ys+///:/+oooo+///++oo++/s+/++/o:o+o+//+//+/+//o+::/+++syy++++/+::o+o+/osss++/://:+/:::://+
        ```....````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````.:.o-``...`.``````-./.`````````````````````````````````````````````````````.`````````````````````````````````````````````````````````````````````````````````````
        ...``````.```````````````````````````````````````````......`..........................-....................................................-:-s/::/oyyh--.``./:+--.````.``````````````..`````............`......-....-:.-.........--....-..............----...--------------..---..-----..-.--..--..---.....
        yyyyo++oosoooo++/++////::-..------------------------:/++oooooossysssyysssssyssssyyhmNMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNNNNNNMNMMMMMMMMMMMMNMNNNNNMMh:y+-:-+-osddo------------------...........-..-----.......:-:-::---.-/+:...---.--.:............-/++/----:::/ooooooo+++++/:-..-:-..`...o+:-/+..:/:.-/:`
        yssooo+/+++++++::-::-.``                            `...-:://+++++oo+++++oo+++ossyyhdmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMNNMho/-..-`-ydsso-                               ``` `       `.-````.`.`/+-```.`..``..   ``   `--:-.`````````````.-:://::::::---...---.``.:o/--/-.`::.`-:
        sssoooo+//////:/:-..`                                   ``.:/+++++++++oo+oooooossyyhhhdNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMNNMMMMMhh:.... omd+/++                                           ``.` ``. -`:+:````..``       ``.--.``````````````````             ``` `..`   `:/:.-...`-:.`-
        soooo++++//:::-...                                        .-:://///+++++++ossssssssssydmNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNMMMMMMMMMMMMMMMMMMMMMMNMMMMMMmm+-..``dMNs:/y`               ``                         `  ` ```. `-+/.`````       `...`.``````````....`` `   `.```               `  ``-:-./`.../o-`
        o+++++++++/::--` `                                        `...---:/+//++ooosoo+oossssydNMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNmmNMMMMNMMMMMMMMMMMNNMNNm+-..`-MMNh::y`         `     `- `      ``````````          ` `. ` `/+/.``````    ...`````````````..`````````  `..```                   `--.+``.`./+`
        ++++//////:--.`                                         `   ````--::/+oooooooooossyyhhdmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMMMNNNMMMMNMMMMMMMMMMMMMMMMmm+-..`sdNNh+:::             ``.:`..``````````````````   ` ` ````  .-/+:. ````` `.:-```````  ``.--...```   `           `                  .--:.```.-:/
        +////:::::-..` `                                             ``.-:/++++++++ooosssssyhhdmNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMmm+-.`.dsmMds:-s             `..:```````   ``````````    ` .`````.--/+/-`  ` ``...``````````..--.```.--://++/::/++:::--.`                 `.---`   `:/
        /:://::--...``                                               `.-://++++ooossssssssyyyyhdmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNMMMMNNMMMMMMMMMMMMMMMMmm+-..-NoNMNy:+y.            `.-+.` ````` ```````....`  `````` `.-::::-``  `....````````--.```.-:///::-.`````` `.-..----.``                .--``   `-`
        //:---:--..``                              `                 ``..-://+oossssssssssyyhhdddmNMMMMMMMMMMMMMMMMMMMNNMMMNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmms-.-:NomNMmo-y+ `           ```                 ...```` ` `` `.--/--` `.-..`````` `.-:-`.:+oo+/-.``       `````...`.`         ````       `--     `.`
        /::---.....``                                                  `.-/+ooosssssssssssyyyyhdmNMMMMMMMMMMMMMMMMMMMMMNNMNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmds-.:/N+dNMNd/:d`             `                   `.```. . `````.-.`  `-.`` ```` `.::-.-/yy+.` `.----`       ``.--.``          `.-.       `-`     `.`
        //::--..`                         Multi                     `-:+++ooooooosssyyyyyyyhhdmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNy:--/MdsNMMNd:d`             `                    ```````` ``   `   `..  `````.--.:.-+s+-`  `.-````/o:`      `...``     ` `    ```       .-      `.`
        ++//::-`                                                      `.--://+++ooosssssyyyyyhhhdmmNNMMMMMMMMMMMMMMMMMMNMMMNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMd/`./mNodNMMNhd`  `````````                         `...`.`      ` `.--`````....`-.`oy+`    --.````:o+.      `.-.     .-. .`            `.`      `. 
        +++//::-.                                                      ``.-:/+oooossssssssssyyyyhdmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMs.`.hMdsdMMMMN.`::....----....```                   `...``     `......```.--.` `. :yo.     .::-//:+o/`       ```   `--. `...`     ``  ```       `. 
        ++/:::-..                                                     `.:/+oooooossssssssssyyyhdmmNNNMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMm+ `:MMsymMMMN:oo:````.````...`..``......```...`-::. `.-``     `:-.-.```--.`     `oo-       .-/+o+/:.          `.--.`   `-`-`     ..`           `` 
        ++//::-.`                        Thread               `  `     `-:/++oossssssssssyyyyhhhddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMd/ `yNNosmMMMoh:```..```````.``````````````````-shy/.```       :..-.`-:-`       .o/`          `..`         `.-::-``.-.` ...`      `            `` 
        ++//::--.`                                                     `.-/+++ooossssssssssyyyhhhddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNh: .dNmsomMMys:....--...```````````````````````.+yho:`        .``-::/.         `--`                  ``.-----.`...--`  `-`.`                     
        ++/::--.``                                                   ``.-:/++oossssssssyyyyyyhhhhdddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMNNNd+`:dMdo+mMso:://::-::---.....----...``````.....:oys:`          -:--.`          --`            `..--:-.``````.`  `    `:`-`                     
        ++//::-.``                                                  ``.-:/++oosssssyyyhhhhhhdddddmmNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMNy:omMdo+Nh/::-```  `````.-//:::--:::-....---::::/o+.`         .:::-           `````.---.---::-..`````...`            ..``                     
        ++///::-.```                    Drifting!                    ``-//+ooooosssssyyyyyhhddmmmmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMNhdNMNh/ms-`        `....-:/oo+/////::::--/::.`.`++`         -///.              ``.---...``    ``...`                          `````         
        //////::-..``                                                 `.:/++osyysssyyyyyyyyyyhhhdmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMNNMMNNMMNdoyho:-``    `../yo:...-.``-.`  ``.`.----``o.        `-///-`                          ```````` ```                  ```````           
        +++////:-.`                                                   `.-:/++oossssyyyyyyyyyhhhddmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMNMMNNNMNNhyhdh+`    `:ym/`  :/-`..//.  `.......::`.-`       `-///-.`       ``` ````        ```````` `````````             ````````  `````````
        ooo++//:-.`                      `                           ``-:/++++oooooosssyyyyyhhddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMNMMMMMMNNMMmhhmNy.   -sd+   `/ss//+o+-   ``...```-`.:.       `..--:-...``` ``````...`...```````````````.``````     ``  ````````               
        oo++//:::..`                                                 `.-//++++osssssssssyyyyhhddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMNNNMMMMMMMNNmmMh/`.+y/`   `.+hhyyo-`     `.` ``-`./.       `````.`....```````........````````````.``..`````````.`````     `````````        `
        +//////::.`                                                  `.-:/+oooooosoossyyyyyhdddmmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMMNNMMNMMMMMMNm:`--`      `.::.`         ````.``:.    ..``..``.-...``.`  ```..```..`````````````.```````.```` ``````````                 `
        ++++///:-.``                                               ``.-:://+osssssssoossyyyhhddmNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMNmNNNMMMMNNMMMMMMs-                     `..```.``.-.`   :`.`..`..```...`````.``````````..`````````````````   `````````````  ````````````````
        ++++//::---.`                                              `.://+++++ooosssssssyyyyyhhddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMNMMNMMMMMMMMNMNMNo.        `       ```.-.````.`--`.`   .`-.````````````.````` ` `` ```  ```   ```       ```````````````````````````````````
        o++/////::-.``                                            `.:///++++++oooossssyyyyhhhhmNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMm+      .-.``..-:-...    `.-..-` ..    -`-.  `` ````````                              `        ```````                    
        +oo+//////:-``                                       ``` `.--:/+++++ooooooossssyyyhhdmmmNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMNMy.  `  ./o+/:-.``...`   .--`.   -.    `-...```` ````````                 ..`                  ```````````````````````````
        ++//+/////--..```                                   ``.--..:/+++oooo++ooooossssyyhhdddmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd:      ``..``....`   `.::.`    -.     .-...`.``````````           `` .  ``                   `` ````````````````````````
        /++++++++//:--.```                                 `.--://///+oooosooooooossssssyhddmNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNdMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMms`      ``````  ````.:/-.      ..    ` ````  ``   ``.``           `-```                                        `````````
        oooo+++++//:::-..`                              `.---::/:/+++++oosssoooosooyyyyhhddmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMNNMd/        ```---..::::-`   ``  ..          ``     ``.`             `:`.`                                                
        ossooso++/////:-....``.``                `    ``-::::://///+++++oossssssosyyydNNmmNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd.     `.--::--..`      ``-   -.                ```````            `..`             `````````````````````   ```````````
        dyyssoo++/+++//:/:::-----.``` ``  ``````..`....-://////++++++oo+++oosyyyyyyyyhmMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNo` ` `-::-:-.``       ..`.   --                 ```````                              ``````    ```````````````````````
        hdysso++++o++///////////:::-..--..--.--.::-::::::://///+++++++oo+++oosyyhhhdddmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm:`  -:::-.``         .-`-   ::`              ```````                                                               ``
        hdysooo+oso++++++///////////:::/::::::::////+///:////+++++++o+++oooooosyddmmNNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMs-` `----.           .-`/   ::`              `.``````                                  `````````````````````    
                                 */
        #endregion

        // TODO: waarschijnlijk checken dat hij legen niet gaat swappen of herberekenen wanneer je moet legen, plus de dubbele code verwijderen
        #region Removers
        // Remove a random action on a random day of the schedule of a truck
        public void RemoveRandomAction1()
        {
            Random r = new Random();
            List<Status> oldDay;
            List<Status> newDay;
            int orda;
            int findDay, removedIndex;
        loop1:
            while (foundSucc) { }
            // pick a random day of the week
            findDay = r.Next(5);
            if (oldState.status1[findDay].Count == 0) goto loop1;
            oldDay = oldState.status1[findDay];
            newDay = new List<Status>(oldDay);
            removedIndex = r.Next(1, oldDay.Count - 1);
            // Remove a random action
            orda = newDay[removedIndex].ordnr;
            if (orda == 0) goto loop1;
            newDay.RemoveAt(removedIndex);
            // Give ratings to the old and new day, and evaluate them
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
                lock (orderLock)
                {
                    try
                    {
                        DTS.availableOrders.Add(orda, DTS.orders[orda]);
                    }
                    catch
                    {
                        // Niet nodig om extra toe te veogen
                    }
                }
                newState = oldState;
                newState.status1[findDay] = newDay;
                foundSucc = true;
            }
            goto loop1;
        }

        // Remove a random action on a random day of the schedule of a truck
        public void RemoveRandomAction2()
        {
            Random r = new Random();
            List<Status> oldDay;
            List<Status> newDay;
            int ordb;
            int findDay, removedIndex;
        loop2:
            while (foundSucc) { }
            // pick a random day of the week
            findDay = r.Next(5);
            if (oldState.status1[findDay].Count == 0) goto loop2;
            oldDay = oldState.status2[findDay];
            newDay = new List<Status>(oldDay);
            removedIndex = r.Next(1, oldDay.Count - 1);
            // Remove a random action
            ordb = newDay[removedIndex].ordnr;
            if (ordb == 0) goto loop2;
            newDay.RemoveAt(removedIndex);
            // Give ratings to the old and new day, and evaluate them
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
                lock (orderLock)
                {
                    try
                    {
                        DTS.availableOrders.Add(ordb, DTS.orders[ordb]);
                    }
                    catch
                    {
                        // Niet nodig om extra toe te veogen
                    }
                }
                newState = oldState;
                newState.status2[findDay] = newDay;
                foundSucc = true;
            }
            goto loop2;
        }
        #endregion
        #region Adders
        // Add a random action at a random time, ignoring whether it is possible or not
        public void AddRandomAction1()
        {
            Random r = new Random();
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            Order ord;
        loop3:
            while (foundSucc) { }
            // pick a random day of the week
            findDay = r.Next(5);
            oldDay = oldState.status1[findDay];
            newDay = new List<Status>(oldDay);
            addedIndex = r.Next(oldDay.Count);
            // Add a random available action in between two other actions
            lock (orderLock)
            {
                int rand = r.Next(DTS.availableOrders.Count);
                ord = DTS.availableOrders.ElementAt(rand).Value;
            }
            newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
            // Give ratings to the old and new day, and evaluate them
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
                lock (orderLock) DTS.availableOrders.Remove(ord.orderNumber);
                newState = oldState;
                newState.status1[findDay] = newDay;
                foundSucc = true;
            }
            goto loop3;
        }

        public void AddRandomAction2()
        {
            Random r = new Random();
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            Order ord;
        loop4:
            while (foundSucc) { }
            // pick a random day of the week
            findDay = r.Next(5);
            oldDay = oldState.status2[findDay];
            newDay = new List<Status>(oldDay);
            addedIndex = r.Next(oldDay.Count);
            // Add a random available action in between two other actions
            lock (orderLock)
            {
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
            }
            newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
            int a = EvalDay(oldDay);
            int b = EvalDay(newDay);
            // Give ratings to the old and new day, and evaluate them
            if (AcceptNewDay(a, b, r))
            {
                lock (orderLock) DTS.availableOrders.Remove(ord.orderNumber);
                newState = oldState;
                newState.status2[findDay] = newDay;
                foundSucc = true;
            }
            goto loop4;
        }
        #endregion
        #region Swappers
        // Swap two random actions within a truck
        public void SwapRandomActionsWithin1()
        {
            Random r = new Random();
            List<Status> oldDaya1, oldDaya2, newDaya1, newDaya2;
            int daya1, daya2;
            Status stata1, stata2, tempstata1, tempstata2;
            int actionIndexa1, actionIndexa2;
        loop5:
            while (foundSucc) { }
            daya1 = r.Next(5);
            daya2 = r.Next(5);
            oldDaya1 = status1[daya1];
            oldDaya2 = status1[daya2];
            newDaya1 = new List<Status>(oldDaya1);
            newDaya2 = new List<Status>(oldDaya2);

            // pick two random actions			
            actionIndexa1 = r.Next(1, status1[daya1].Count - 1);
            actionIndexa2 = r.Next(1, status1[daya2].Count - 1);
            stata1 = oldDaya1[actionIndexa1];
            stata2 = oldDaya2[actionIndexa2];
            // Change times so that they are correct, if there was a different action before
            if (actionIndexa1 != 0)
            {
                tempstata2 = new Status(daya1, stata2.company, stata2.ordnr);
            }
            else
            {
                tempstata2 = new Status(daya1, stata2.company, stata2.ordnr);
            }
            if (actionIndexa2 != 0)
            {
                tempstata1 = new Status(daya2, stata1.company, stata1.ordnr);
            }
            else
            {
                tempstata1 = new Status(daya2, stata1.company, stata1.ordnr);
            }
            // Swap the actions
            newDaya1.Remove(stata1);
            newDaya2.Remove(stata2);
            newDaya1.Insert(actionIndexa1, tempstata2);
            newDaya2.Insert(actionIndexa2, tempstata1);
            if (AcceptNewDay(EvalDay(oldDaya1) + EvalDay(oldDaya2), EvalDay(newDaya1) + EvalDay(newDaya2), r))
            {
                newState = oldState;
                newState.status1[daya1] = newDaya1;
                newState.status1[daya2] = newDaya2;
                foundSucc = true;
            }
            goto loop5;
        }

        // Swap two random actions within a truck
        public void SwapRandomActionsWithin2()
        {
            Random r = new Random();
            List<Status> oldDayb1, oldDayb2, newDayb1, newDayb2;
            int dayb1, dayb2;
            Status statb1, statb2, tempstatb1, tempstatb2;
            int actionIndexb1, actionIndexb2;
        loop6:
            while (foundSucc) { }
            dayb1 = r.Next(5);
            dayb2 = r.Next(5);
            oldDayb1 = status2[dayb1];
            oldDayb2 = status2[dayb2];
            newDayb1 = new List<Status>(oldDayb1);
            newDayb2 = new List<Status>(oldDayb2);
            // pick two random actions			
            actionIndexb1 = r.Next(1, status2[dayb1].Count - 1);
            actionIndexb2 = r.Next(1, status2[dayb2].Count - 1);
            statb1 = oldDayb1[actionIndexb1];
            statb2 = oldDayb2[actionIndexb2];
            // Change times so that they are correct, if there was a different action before
            if (actionIndexb1 != 0)
            {
                tempstatb2 = new Status(dayb1, statb2.company, statb2.ordnr);
            }
            else
            {
                tempstatb2 = new Status(dayb1, statb2.company, statb2.ordnr);
            }
            if (actionIndexb2 != 0)
            {
                tempstatb1 = new Status(dayb2, statb1.company, statb1.ordnr);
            }
            else
            {
                tempstatb1 = new Status(dayb2, statb1.company, statb1.ordnr);
            }
            // Swap the actions
            newDayb1.Remove(statb1);
            newDayb2.Remove(statb2);
            newDayb1.Insert(actionIndexb1, tempstatb2);
            newDayb2.Insert(actionIndexb2, tempstatb1);
            if (AcceptNewDay(EvalDay(oldDayb1) + EvalDay(oldDayb2), EvalDay(newDayb1) + EvalDay(newDayb2), r))
            {
                newState = oldState;
                newState.status2[dayb1] = newDayb1;
                newState.status2[dayb2] = newDayb2;
                foundSucc = true;
            }
            goto loop6;
        }

        public void SwapRandomActionsBetween()
        {
            Random r = new Random();
            List<Status> oldDayc1, oldDayc2, newDayc1, newDayc2;
            int dayc1, dayc2;
            Status statc1, statc2, tempstatc1, tempstatc2;
            int actionIndexc1, actionIndexc2;
        loop7:
            while (foundSucc) { }
            dayc1 = r.Next(5);
            dayc2 = r.Next(5);
            oldDayc1 = status1[dayc1];
            oldDayc2 = status2[dayc2];
            newDayc1 = new List<Status>(oldDayc1);
            newDayc2 = new List<Status>(oldDayc2);
            // pick two random actions			
            actionIndexc1 = r.Next(1, status1[dayc1].Count - 1);
            actionIndexc2 = r.Next(1, status2[dayc2].Count - 1);
            statc1 = oldDayc1[actionIndexc1];
            statc2 = oldDayc2[actionIndexc2];
            if (actionIndexc1 != 0)
            {
                tempstatc2 = new Status(dayc1, statc2.company, statc2.ordnr);
            }
            else
            {
                tempstatc2 = new Status(dayc1, statc2.company, statc2.ordnr);
            }
            if (actionIndexc2 != 0)
            {
                tempstatc1 = new Status(dayc2, statc1.company, statc1.ordnr);
            }
            else
            {
                tempstatc1 = new Status(dayc2, statc1.company, statc1.ordnr);
            }
            // Swap the actions
            newDayc1.Remove(statc1);
            newDayc2.Remove(statc2);
            newDayc1.Insert(actionIndexc1, tempstatc2);
            newDayc2.Insert(actionIndexc2, tempstatc1);
            if (AcceptNewDay(EvalDay(oldDayc1) + EvalDay(oldDayc2), EvalDay(newDayc1) + EvalDay(newDayc2), r))
            {
                newState = oldState;
                newState.status1[dayc1] = newDayc1;
                newState.status2[dayc2] = newDayc2;
                foundSucc = true;
            }
            goto loop7;
        }

        #endregion
        // TODO: Compleet herschrijven
        public int EvalDay(List<Status> day)
        {
            int score = 0;
            int newstart = DTS.dayStart;
            Company previousLoc = DTS.maarheeze;
            GarbageTruck truck = new GarbageTruck();
            // Iterate over all actions
            foreach (Status action in day)
            {
                if (action.ordnr == 0)
                {
                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + DTS.emptyingTime;
                    truck.EmptyTruck();
                    previousLoc = DTS.maarheeze;
                }
                else
                {
                    // More orders on a day is generally better
                    Order ord = DTS.orders[action.ordnr];
                    score += day.Count * 100;
                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + (int)ord.emptyingTime;
                    truck.FillTruck(ord);
                    // Check if there's a moment when the truck is full. deduct a lot of score for that
                    if (truck.CheckIfOverloaded()) score -= 1000;
                }
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            // Reward "free" time at the end, deduct heavily for overtime
            if (newstart <= DTS.dayEnd) score += DTS.dayEnd - newstart;
            else score += DTS.dayEnd - newstart * 5;
            return score;
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(int oldrating, int newrating, Random r)
        {
            return (newrating > oldrating) || PCheck(oldrating, newrating, r);
        }


        // x Swap actions of 2 cars
        // x Swap actions within a car
        // Add action (wat voor actie? Ergens pakken uit een lijst?)
        // x Remove action
        // x Change day of action

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(int fx, int fy, Random r)
        {
            return Math.Pow(Math.E, (fx - fy) / DTS.temperature) < r.NextDouble();
        }




    }
}
