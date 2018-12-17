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
        private bool foundSucc;                                        // A bool that checks whether a successor has been found
        private Random r;                                              // Random number generator that is used sometimes

        public StateGenerator()
        {
            // Initialize the thread array
            successorfunctions = new Thread[7];
            r = new Random();
            successorfunctions[0] = new Thread(RemoveRandomAction1);
            successorfunctions[1] = new Thread(RemoveRandomAction2);
            successorfunctions[2] = new Thread(AddRandomAction1);
            successorfunctions[3] = new Thread(AddRandomAction2);
            successorfunctions[4] = new Thread(SwapRandomActionsWithin1);
            successorfunctions[5] = new Thread(SwapRandomActionsWithin2);
            successorfunctions[6] = new Thread(SwapRandomActionsBetween);
        }

        public State GetNextState(State old)
        {
            oldState = old;
            status1 = old.status1;
            status2 = old.status2;
            // Start all the threads
            /*
            successorfunctions[0].Start();
            successorfunctions[1].Start();
            successorfunctions[2].Start();
            successorfunctions[3].Start();
            successorfunctions[4].Start();
            successorfunctions[5].Start();
            successorfunctions[6].Start();
            // Wait untill all threads finish
            for (int i = 0; i < successorfunctions.Length; i++)
            {
                successorfunctions[i].Join();
            }
            */
            int function = 2;// r.Next(7);
            switch (function)
            {
                case 0: RemoveRandomAction1(); break;
                case 1: RemoveRandomAction2(); break;
                case 2: AddRandomAction1(); break;
                case 3: AddRandomAction2(); break;
                case 4: SwapRandomActionsWithin1(); break;
                case 5: SwapRandomActionsWithin2(); break;
                case 6: SwapRandomActionsBetween(); break;
            }
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
            List<Status> oldDay;
            List<Status> newDay;
            int findDay, removedIndex;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(5);
                if (oldState.status1[findDay].Count == 0) continue;
                oldDay = oldState.status1[findDay];
                newDay = new List<Status>(oldDay);
                removedIndex = r.Next(1, oldDay.Count - 1);
                // Remove a random action
                newDay.RemoveAt(removedIndex);
                // Give ratings to the old and new day, and evaluate them
                if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay)))
                {
                    foundSucc = true;
                    newState = oldState;
                    newState.status1[findDay] = newDay;
                }
            }
        }

        // Remove a random action on a random day of the schedule of a truck
        public void RemoveRandomAction2()
        {
            List<Status> oldDay;
            List<Status> newDay;
            int ord;
            int findDay, removedIndex;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(5);
                oldDay = oldState.status2[findDay];
                newDay = new List<Status>(oldDay);
                removedIndex = r.Next(1, oldDay.Count - 1);
                // Remove a random action
                ord = newDay[removedIndex].ordnr;
                newDay.RemoveAt(removedIndex);
                // Give ratings to the old and new day, and evaluate them
                if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay)))
                {
                    foundSucc = true;
                    DTS.availableOrders.Remove(ord);
                    newState = oldState;
                    newState.status2[findDay] = newDay;
                }
            }
        }
        #endregion
        #region Adders
        // Add a random action at a random time, ignoring whether it is possible or not
        public void AddRandomAction1()
        {
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            Order ord;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(5);
                oldDay = oldState.status1[findDay];
                newDay = new List<Status>(oldDay);
                addedIndex = r.Next(oldDay.Count);
                // Add a random available action in between two other actions
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
                // Add a random available action in between two other actions
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
                newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
                // Give ratings to the old and new day, and evaluate them
                if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay)))
                {
                    foundSucc = true;
                    DTS.availableOrders.Remove(ord.orderNumber);
                    newState = oldState;
                    newState.status1[findDay] = newDay;
                }
            }
        }

        public void AddRandomAction2()
        {
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            Order ord;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(5);
                oldDay = oldState.status2[findDay];
                newDay = new List<Status>(oldDay);
                addedIndex = r.Next(oldDay.Count);
                // Add a random available action in between two other actions
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
                newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
                // Give ratings to the old and new day, and evaluate them
                if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay)))
                {
                    foundSucc = true;
                    DTS.availableOrders.Remove(ord.orderNumber);
                    newState = oldState;
                    newState.status2[findDay] = newDay;
                }
            }
        }
        #endregion
        #region Swappers
        // Swap two random actions within a truck
        public void SwapRandomActionsWithin1()
        {
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2;
            Status stat1, stat2, tempstat1, tempstat2, prevstat1, prevstat2;
            int actionIndex1, actionIndex2;
            while (!foundSucc)
            {
                day1 = r.Next(5);
                day2 = r.Next(5);
                oldDay1 = status1[day1];
                oldDay2 = status1[day2];
                newDay1 = new List<Status>(oldDay1);
                newDay2 = new List<Status>(oldDay2);

                // pick two random actions			
                actionIndex1 = r.Next(1, status1[day1].Count - 1);
                actionIndex2 = r.Next(1, status1[day2].Count - 1);
                stat1 = status1[day1][actionIndex1];
                stat2 = status1[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status1[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status1[day2][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat1.company, stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2, stat1.company, stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                if (AcceptNewDay(EvalDay(oldDay1) + EvalDay(oldDay2), EvalDay(newDay1) + EvalDay(newDay2)))
                {
                    foundSucc = true;
                    newState = oldState;
                    newState.status1[day1] = newDay1;
                    newState.status1[day2] = newDay2;
                }
            }
        }

        // Swap two random actions within a truck
        public void SwapRandomActionsWithin2()
        {
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2;
            Status stat1, stat2, tempstat1, tempstat2, prevstat1, prevstat2;
            int actionIndex1, actionIndex2;
            while (!foundSucc)
            {
                day1 = r.Next(5);
                day2 = r.Next(5);
                oldDay1 = status2[day1];
                oldDay2 = status2[day2];
                newDay1 = new List<Status>(oldDay1);
                newDay2 = new List<Status>(oldDay2);
                // pick two random actions			
                actionIndex1 = r.Next(1, status2[day1].Count - 1);
                actionIndex2 = r.Next(1, status2[day2].Count - 1);
                stat1 = status2[day1][actionIndex1];
                stat2 = status2[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status2[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status2[day1][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat1.company, stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2, stat1.company, stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                if (AcceptNewDay(EvalDay(oldDay1) + EvalDay(oldDay2), EvalDay(newDay1) + EvalDay(newDay2)))
                {
                    foundSucc = true;
                    newState = oldState;
                    newState.status2[day1] = newDay1;
                    newState.status2[day2] = newDay2;
                }
            }
        }
        public void SwapRandomActionsBetween()
        {
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2;
            Status stat1, stat2, tempstat1, tempstat2, prevstat1, prevstat2;
            int actionIndex1, actionIndex2;
            while (!foundSucc)
            {
                day1 = r.Next(5);
                day2 = r.Next(5);
                oldDay1 = status1[day1];
                oldDay2 = status2[day2];
                newDay1 = new List<Status>(oldDay1);
                newDay2 = new List<Status>(oldDay2);
                // pick two random actions			
                actionIndex1 = r.Next(1,status1[day1].Count - 1);
                actionIndex2 = r.Next(1,status2[day2].Count - 1);
                stat1 = status1[day1][actionIndex1];
                stat2 = status2[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status2[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat2.company, stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status1[day2][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat1.company, stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2,stat1.company, stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                if (AcceptNewDay(EvalDay(oldDay1) + EvalDay(oldDay2), EvalDay(newDay1) + EvalDay(newDay2)))
                {
                    foundSucc = true;
                    newState = oldState;
                    newState.status1[day1] = newDay1;
                    newState.status2[day2] = newDay2;
                }
            }
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
        public bool AcceptNewDay(int oldrating, int newrating)
        {
            return (newrating > oldrating) || PCheck(oldrating, newrating);
        }


        // x Swap actions of 2 cars
        // x Swap actions within a car
        // Add action (wat voor actie? Ergens pakken uit een lijst?)
        // x Remove action
        // x Change day of action

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(int fx, int fy)
        {
            return Math.Pow(Math.E, (fx - fy) / DTS.temperature) < r.Next(0, 1);
        }




    }
}
