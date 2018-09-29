using System.Threading;
using System.IO;

namespace CynetExercise
{
    static class Logger
    {
        static ReaderWriterLock lockObject = new ReaderWriterLock();
        const int MAX_WAIT_TIME = 20000;
        const string FILE_PATH = @"./traffic.txt";

        public static void log(this string text)
        {
            try
            {
                lockObject.AcquireWriterLock(MAX_WAIT_TIME);
                File.AppendAllLines(FILE_PATH, new[] { text + '\n' });
            }
            finally
            {
                lockObject.ReleaseWriterLock();
            }
        }
    }
}
