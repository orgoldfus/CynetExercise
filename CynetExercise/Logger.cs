using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace CynetExercise
{
    static class Logger
    {
        private static SemaphoreSlim _sync = new SemaphoreSlim(1);
        const string FILE_PATH = @"./traffic.txt";

        public static void Log(this string message)
        {
            try
            {
                _sync.Wait();
                File.AppendAllLines(FILE_PATH, new[] { message });
            }
            finally
            {
                _sync.Release();
            }
        }

        public static async Task LogAsync(this string message)
        {
            StreamWriter _stream = null;
            await _sync.WaitAsync();

            try
            {
                _stream = new StreamWriter(FILE_PATH, true);
                await _stream.WriteLineAsync(message);
            }
            finally
            {
                _stream.Close();
                _sync.Release();
            }
        }
    }
}
