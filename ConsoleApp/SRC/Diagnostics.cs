using System.Diagnostics;

namespace OptimaliserenPracticum
{
    static class Diagnostics
    {
        // Variables
        public static Stopwatch initWatch;                    // A stopwatch that keeps track of the initializationtime
        public static Stopwatch runtimeWatch;                 // A stopwatch that keeps track of the runtime
        public static int AcceptationsPerSecond;              // The number of Acceptations per second, on a given moment
        public static int second;                             // The second that the SE is currently in
        public static int IterationsPerMinute;                // The number of Iterations per minute, on a given moment
        public static bool Printed;                           // Bool that shows if the iterations are printed
    }
}
