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
        public void RemoveRandomAction1(object o)
        {
            List<Status> oldDay;
            List<Status> newDay;
            int findDay, removedIndex;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(6);
                oldDay = oldState.status1[findDay];
                newDay = oldDay;
                removedIndex = r.Next(oldDay.Count);
                // Remove a random action
                newDay.RemoveAt(removedIndex);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay, removedIndex);
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
        public void RemoveRandomAction2(object o)
        {
            List<Status> oldDay;
            List<Status> newDay;
            int ord;
            int findDay, removedIndex;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(6);
                oldDay = oldState.status2[findDay];
                newDay = oldDay;
                removedIndex = r.Next(oldDay.Count);
                // Remove a random action
                ord = newDay[removedIndex].ordnr;
                newDay.RemoveAt(removedIndex);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay, removedIndex);
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
            List<Status> oldDay;
            List<Status> newDay;
            int findDay, addedTime, timeIndex, listIndex;
            Order ord;
            Status newStat, oldStat;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(6);
                oldDay = oldState.status1[findDay];
                newDay = oldDay;
                addedTime = r.Next(21600, 64620);
                timeIndex = listIndex = 0;
                // Add a random available action in between two other actions
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
                // Throw that random order on the given point in time. It can overlap with actions that are already in place, but we ignore that for now
                for (int i = 0; i < oldDay.Count; i++)
                {
                    timeIndex = oldDay[i].beginTime;
                    if (addedTime > timeIndex)
                    {
                        listIndex = i; break;
                    }
                }
                oldStat = oldDay[listIndex];
                newStat = new Status(findDay, addedTime, DTS.companyList[ord.matrixID], oldStat.truck.FillTruck(ord), ord.orderNumber);
                newDay.Insert(listIndex + 1, newStat);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay, listIndex + 2);
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
            int findDay, addedTime, timeIndex, listIndex;
            Order ord;
            Status newStat, oldStat;
            while (!foundSucc)
            {
                // pick a random day of the week
                findDay = r.Next(6);
                oldDay = oldState.status2[findDay];
                newDay = oldDay;
                addedTime = r.Next(21600, 64620);
                timeIndex = listIndex = 0;
                // Add a random available action in between two other actions
                ord = DTS.availableOrders.ElementAt(r.Next(DTS.availableOrders.Count)).Value;
                // Throw that random order on the given point in time. It can overlap with actions that are already in place, but we ignore that for now
                for (int i = 0; i < oldDay.Count; i++)
                {
                    timeIndex = oldDay[i].beginTime;
                    if (addedTime > timeIndex)
                    {
                        listIndex = i; break;
                    }
                }
                oldStat = oldDay[listIndex];
                newStat = new Status(findDay, addedTime, DTS.companyList[ord.matrixID], oldStat.truck.FillTruck(ord), ord.orderNumber);
                newDay.Insert(listIndex + 1, newStat);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay, listIndex + 2);
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
                day1 = r.Next(6);
                day2 = r.Next(6);
                oldDay1 = newDay1 = status1[day1];
                oldDay2 = newDay2 = status1[day2];
                // pick two random actions			
                actionIndex1 = r.Next(status1[day1].Count);
                actionIndex2 = r.Next(status1[day2].Count);
                stat1 = status1[day1][actionIndex1];
                stat2 = status1[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status1[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, prevstat1.truck.FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, new GarbageTruck(1, status1[day1][actionIndex1-1].truck.currentCapacity).FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status1[day1][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, prevstat2.truck.FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, new GarbageTruck(1, status1[day2][actionIndex2 - 1].truck.currentCapacity).FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay1, actionIndex1 + 1);
                MoveAction(newDay2, actionIndex2 + 1);
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
                day1 = r.Next(6);
                day2 = r.Next(6);
                oldDay1 = newDay1 = status2[day1];
                oldDay2 = newDay2 = status2[day2];
                // pick two random actions			
                actionIndex1 = r.Next(status2[day1].Count);
                actionIndex2 = r.Next(status2[day2].Count);
                stat1 = status2[day1][actionIndex1];
                stat2 = status2[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status2[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, prevstat1.truck.FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, new GarbageTruck(2, status2[day1][actionIndex1 - 1].truck.currentCapacity).FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status2[day1][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, prevstat2.truck.FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, new GarbageTruck(2, status2[day2][actionIndex2 - 1].truck.currentCapacity).FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay1, actionIndex1 + 1);
                MoveAction(newDay2, actionIndex2 + 1);
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
                day1 = r.Next(6);
                day2 = r.Next(6);
                oldDay1 = newDay1 = status1[day1];
                oldDay2 = newDay2 = status2[day2];
                // pick two random actions			
                actionIndex1 = r.Next(status1[day1].Count);
                actionIndex2 = r.Next(status2[day2].Count);
                stat1 = status1[day1][actionIndex1];
                stat2 = status2[day2][actionIndex2];
                // Change times so that they are correct, if there was a different action before
                if (actionIndex1 != 0)
                {
                    prevstat1 = status2[day1][actionIndex1 - 1];
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, prevstat1.truck.FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                else
                {
                    tempstat2 = new Status(day1, stat1.beginTime, stat2.company, new GarbageTruck(1, status1[day1][actionIndex1 - 1].truck.currentCapacity).FillTruck(DTS.orders[stat2.ordnr]), stat2.ordnr);
                }
                if (actionIndex2 != 0)
                {
                    prevstat2 = status1[day1][actionIndex2 - 1];
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, prevstat2.truck.FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                else
                {
                    tempstat1 = new Status(day2, stat2.beginTime, stat1.company, new GarbageTruck(2, status2[day2][actionIndex2 - 1].truck.currentCapacity).FillTruck(DTS.orders[stat1.ordnr]), stat1.ordnr);
                }
                // Swap the actions
                newDay1.Insert(actionIndex1, tempstat2);
                newDay2.Insert(actionIndex2, tempstat1);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay1, actionIndex1 + 1);
                MoveAction(newDay2, actionIndex2 + 1);
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

        public int EvalDay(List<Status> day)
        {
            int score = 0;
            // More orders on a day is generally better
            score += day.Count * 100;
            int previousEnd = 21600;
            // Iterate over all actions
            foreach (Status action in day)
            {
                // See if the two events overlap. If yes, deduct points
                if (action.beginTime < previousEnd) score -= ((previousEnd - action.beginTime) * 10);
                // Reward "free" time in between orders
                else if (action.beginTime > previousEnd) score += ((previousEnd - action.beginTime) / 5);
                // Check if there's a moment when the truck is full. deduct a lot of score for that
                if (action.truck.CheckIfOverloaded()) score -= 1000;
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            return score;
        }
        
        public List<Status> MoveAction(List<Status> list, int index)
        {
            Company comp = DTS.maarheeze;
            if (index > 0)
            {
                comp = list[index - 1].company;
            }
            Status toSwap = list[index];
            list[index] = new Status(toSwap.day, toSwap.beginTime, toSwap.company, toSwap.truck.FillTruck(DTS.orders[toSwap.ordnr]), toSwap.ordnr);
            return list;
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
