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
        private volatile bool foundSucc;                                        // A bool that checks whether a successor has been found on multiple threads (multithreading)
        private object orderLock;
        private int check;
        private Random r;

        public StateGenerator(State initial)
        {
            foundSucc = true;  
            orderLock = new object();
            oldState = initial;
            r = new Random();
            // Initialize the thread array
            /*
            successorfunctions = new Thread[7];
            successorfunctions[0] = new Thread(RemoveRandomAction);
            successorfunctions[1] = new Thread(RemoveRandomAction);
            successorfunctions[2] = new Thread(AddRandomAction);
            successorfunctions[3] = new Thread(AddRandomAction);
            successorfunctions[4] = new Thread(SwapRandomActionsWithin);
            successorfunctions[5] = new Thread(SwapRandomActionsWithin);
            successorfunctions[6] = new Thread(SwapRandomActionsBetween);
            // Start the threads
            successorfunctions[0].Start(0);
            successorfunctions[1].Start(1);
            successorfunctions[2].Start(0);
            successorfunctions[3].Start(1);
            successorfunctions[4].Start(0);
            successorfunctions[5].Start(1);
            successorfunctions[6].Start();
            */
        }

        public State GetNextState(State old)
        {
            check = 0;
            oldState = old;
            State newwState = null;
            foundSucc = false;
            while (newwState == null)
            {
                //todo terug zetten naar 7
                int i = r.Next(7);
                switch (i)
                {
                    case 0: newwState = RemoveRandomAction(0); break;
                    case 1: newwState = RemoveRandomAction(1); break;
                    case 2: newwState = AddRandomAction(0); break;
                    case 3: newwState = AddRandomAction(1); break;
                    case 4: newwState = SwapRandomActionsWithin(0); break;
                    case 5: newwState = SwapRandomActionsWithin(1); break;
                    case 6: newwState = SwapRandomActionsBetween(); break;
                    default: break;
                }
            }
            return newwState;
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
        public State RemoveRandomAction(object i)
        {
            int x = (int)i;
            Random r = new Random();
            List<Status>[] oldStatus;
            List<Status> oldDay;
            List<Status> newDay;
            int orda;
            int findDay, removedIndex;
            while (foundSucc) { }
            oldStatus = oldState.status[x];
            // pick a random day of the week
            findDay = r.Next(5);
            if (oldStatus[findDay].Count == 1) return null;
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            removedIndex = r.Next(oldDay.Count - 2);
            // Remove a random action
            orda = newDay[removedIndex].ordnr;
            if (orda == 0) return null;
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
                       // Gotta catch 'em all!
                    }
                }
                newState = oldState;
                newState.status[x][findDay] = newDay;
                foundSucc = true;
                return newState;
            }
            return null;
        }
        #endregion
        #region Adders
        // Add a random action at a random time
        public State AddRandomAction(object i)
        {
            int x = (int)i;
            List<Status>[] oldStatus;
            Random r = new Random();
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            Order ord;

            //WTF IS DIT???? behalve crash programma.
            while (foundSucc) { }

            oldStatus = oldState.status[x];
            // pick a random day of the week
            findDay = r.Next(5);
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            addedIndex = r.Next(oldDay.Count - 1);
            if (DTS.availableOrders.Count == 0) return null;
            // Add a random available action in between two other actions
            lock (orderLock)
            {
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
            }
            newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
            // Give ratings to the old and new day, and evaluate them
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
                lock (orderLock)
                {
                    DTS.availableOrders.Remove(ord.orderNumber);
                }
                newState = oldState;
                newState.status[x][findDay] = newDay;
                foundSucc = true;
                return newState;
            }
            return null;
        }
        #endregion
        #region Swappers
        // Swap two random actions within a truck
        public State SwapRandomActionsWithin(object i)
        {
            int x = (int) i;
            Random r = new Random();
            List<Status>[] oldStatus;
            List<Status> oldDaya1, oldDaya2, newDaya1, newDaya2;
            int daya1, daya2;
            Status stata1, stata2, tempstata1, tempstata2;
            int actionIndexa1, actionIndexa2;
            while (foundSucc) { }
            oldStatus = oldState.status[x];
            daya1 = r.Next(5);
            daya2 = r.Next(5);
            oldDaya1 = oldStatus[daya1];
            oldDaya2 = oldStatus[daya2];
            newDaya1 = new List<Status>(oldDaya1);
            newDaya2 = new List<Status>(oldDaya2);
            if (newDaya1.Count < 2 || newDaya2.Count < 2) return null;
            // pick two random actions			
            actionIndexa1 = r.Next(oldDaya1.Count - 2);
            actionIndexa2 = r.Next(oldDaya2.Count - 2);
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
                newState.status[x][daya1] = newDaya1;
                newState.status[x][daya2] = newDaya2;
                foundSucc = true;
                return newState;
            }
            return null;
        }

        public State SwapRandomActionsBetween()
        {
            Random r = new Random();
            List<Status> oldDayc1, oldDayc2, newDayc1, newDayc2;
            int dayc1, dayc2;
            Status statc1, statc2, tempstatc1, tempstatc2;
            int actionIndexc1, actionIndexc2;
            while (foundSucc) { }
            dayc1 = r.Next(5);
            dayc2 = r.Next(5);
            oldDayc1 = oldState.status[0][dayc1];
            oldDayc2 = oldState.status[1][dayc2];
            newDayc1 = new List<Status>(oldDayc1);
            newDayc2 = new List<Status>(oldDayc2);
            if (newDayc1.Count < 2 || newDayc2.Count < 2) return null;
            // pick two random actions			
            actionIndexc1 = r.Next(oldDayc1.Count - 2);
            actionIndexc2 = r.Next(oldDayc2.Count - 2);
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
            if (AcceptNewDay(EvalDay(oldDayc1) + EvalDay(oldDayc2), EvalDay(newDayc1) + EvalDay(newDayc2), r) && Interlocked.Exchange(ref check, 1) == 0)
            {
                newState = oldState;
                newState.status[0][dayc1] = newDayc1;
                newState.status[1][dayc2] = newDayc2;
                foundSucc = true;
                return newState;
            }
            return null;
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

                    //DIT IS ECHT TOTALEN CANCER.... Score die je krijgt is PER ACTIE * 100 dus 31 * 100 voor de 31ste actie en 3200 bonus als je er een actie BIJ GOOIT. er kan maar 1000 af en wat tijd. DUS ja..... Dit is jammer.
                    //bonus geven is kut dus doen we niet anders die regel uit de FOREACH HALEN.
                    //score += day.Count * 100;

                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + (int)ord.emptyingTime;
                    truck.FillTruck(ord);
                    // Check if there's a moment when the truck is full. deduct a lot of score for that
                    if (truck.CheckIfOverloaded()) score -= 1000;
                }
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            //score + de tijd die de checke
            

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
            if (Math.Sign(fx) == -1 || Math.Sign(fy) == -1) return false;
            double quickmaffs = Math.Pow(Math.E, (fy - fx) / DTS.temperature);
            return quickmaffs >= r.NextDouble();
        }




    }
}
