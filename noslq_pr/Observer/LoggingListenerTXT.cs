using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Observer
{
    public class LoggingListenerTXT : IObserver
    {
        public string FileName { get; set; }
        public LoggingListenerTXT(string fileName) {
            FileName = fileName;
        }
        public void Update(string operation, object criteria, object result)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"[{DateTime.UtcNow.ToString()}]");
            sb.Append(" Operation: " +operation+"\n");
            sb.Append("Search criteria:\n" +criteria+"\n");
            sb.Append("--- "+operation+" Details ---\n");
            sb.Append(result);
            sb.Append("\n-----------------------\n");

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Log", FileName + ".txt");
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(sb.ToString());
            }

        }
    }
}
