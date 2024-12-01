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
    public class LoggingListenerJSON : IObserver
    {

        public string FileName { get; set; }
        public LoggingListenerJSON(string fileName)
        {
            FileName = fileName;
        }

        public void Update(string operation, object criteria, object result)
        {
            try
            {
                var logEntry = new JObject
                {
                    ["Timestamp"] = DateTime.UtcNow.ToString("o"),
                    ["Operation"] = operation,
                    ["Criteria"] = JToken.FromObject(criteria),
                    ["Result"] = JToken.FromObject(result)
                };


                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", FileName + ".jsonl");

                /*List <JObject> combinedJsonObjects = new List<JObject>();
                if (File.Exists(logFilePath))
                {

                    string existingContent = File.ReadAllText(logFilePath);
                    if (!string.IsNullOrWhiteSpace(existingContent))
                    {
                        JObject[] jsonObject = JsonConvert.DeserializeObject<JObject[]>(existingContent);
                        combinedJsonObjects.AddRange(jsonObject);
                    }



                }
                combinedJsonObjects.Add(logEntry);

                string combinedJson = JsonConvert.SerializeObject(combinedJsonObjects, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(logFilePath, combinedJson);*/

                using (var writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.WriteLine(logEntry.ToString(Newtonsoft.Json.Formatting.None));
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Error logging operation: {ex.Message}");
            }
        }
    }
}
