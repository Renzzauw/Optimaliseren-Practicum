using System.Diagnostics;

namespace OptimaliserenPracticum
{
    static class Diagnostics
    {
        // Variables
        public static Stopwatch initWatch;                    // A stopwatch that keeps track of the initializationtime
        public static Stopwatch runtimeWatch;                 // A stopwatch that keeps track of the runtime
        public static int IterationsPerSecond;                // The number of iterations per second, on a given moment
        public static int second;                             // The second that the SE is currently in
    }
}
