using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace noslq_pr.Observer
{
    internal class LoggingListenerJSON : IObserver
    {
        public void Update(string operation, object criteria, object result)
        {
            var logEntry = new JObject
            {
                ["Timestamp"] = DateTime.UtcNow.ToString("o"),
                ["Operation"] = operation,
                ["Criteria"] = JToken.FromObject(criteria),
                ["Result"] = JToken.FromObject(result)
            };


            string jsonLog = System.Text.Json.JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            //string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.json");
            string logFilePath = Path.Combine("D:\\projects\\C#\\patterns_pr2\\noslq_pr\\Observer\\", "log.json");


            List<JObject> combinedJsonObjects = new List<JObject>();
            if (File.Exists(logFilePath))
            {
                
                string existingContent = File.ReadAllText(logFilePath);
                if (!string.IsNullOrWhiteSpace(existingContent))
                {
                    JObject[] jsonObject = JsonConvert.DeserializeObject<JObject[]>(existingContent);
                    combinedJsonObjects.AddRange(jsonObject);
                }

                combinedJsonObjects.Add(logEntry);
                
            }

            string combinedJson = JsonConvert.SerializeObject(combinedJsonObjects, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(logFilePath, combinedJson);
        }
    }
}
