using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Observer
{
    internal class LoggingListenerTXT : IObserver
    {
        public void Update(string operation, object criteria, object result)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"[{DateTime.UtcNow.ToString()}]");
            sb.Append(" Operation: " +operation+"\n");
            sb.Append("Search criteria: " +criteria+"\n");
            sb.Append("--- "+operation+" Details ---\n");
            sb.Append(result);
            sb.Append("\n-----------------------\n");


            using (StreamWriter writer = new StreamWriter("D:\\projects\\C#\\patterns_pr2\\noslq_pr\\Observer\\log.txt", true))
            {
                writer.WriteLine(sb.ToString());
            }

        }
    }
}
