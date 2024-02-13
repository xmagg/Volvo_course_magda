using System.Text.RegularExpressions;

namespace homework_3
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputDirectory = "C:\\Magda\\Volvo Course\\homework\\100 books";
            string outputDirectory = "C:\\Magda\\Volvo Course\\homework\\homework_3a";

            ProcessFiles(inputDirectory, outputDirectory);

            Console.WriteLine("Processing completed. Press any key to exit.");
            Console.ReadKey();
        }

        static void ProcessFiles(string inputDirectory, string outputDirectory)
        {
            var files = Directory.GetFiles(inputDirectory, "*.txt");

            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                var processingTask = new Task(() =>
                {
                    var analyzer = new TextAnalyzer(file);
                    analyzer.ProcessText();
                    analyzer.WriteResults(outputDirectory);

                    Console.WriteLine(
                        $"Task completed for {file.ToString().Split("\\").Last()}." +
                        $" Bytes {analyzer.result.HowManyBytes.ToString("#,##0")}." +
                        $" Words {analyzer.result.HowManyWords.ToString("#,##0")}." +
                        $" Sentences {analyzer.result.HowManySentences.ToString("#,##0")}.");
                });

                processingTask.Start();
                Console.WriteLine($"Task ongoing for {file.ToString().Split("\\").Last()}.");
                tasks.Add(processingTask);
            }
            int amountCompl = 0;
            while (amountCompl < tasks.Count)
            {
                int i = 0;
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                    {
                        i++;
                    }
                }
                if (i > amountCompl)
                {
                    amountCompl = i;
                    //Console.Clear();
                    Console.WriteLine($"** Status {amountCompl}/{tasks.Count} completed. **");
                    //Thread.Sleep(100);
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
    }

    class TextAnalyzer
    {
        private readonly string filePath;
        public Result result;
        const int TOP10 = 10;

        public TextAnalyzer(string filePath)
        {
            this.filePath = filePath;
        }

        public void ProcessText()
        {
            string content = File.ReadAllText(filePath);
            var sentences = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var words = Regex.Split(content, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToArray();

            result = new Result
            {
                LongestSentences = sentences.OrderByDescending(s => s.Length).Take(10).ToArray(),
                ShortestSentences = sentences.OrderBy(s => s.Length).Take(10).ToArray(),
                LongestWords = words.OrderByDescending(s => s.Length).Take(30).ToArray(),
                MostCommonLetters = content.GroupBy(c => c).OrderByDescending(g => g.Count()).Select(g => g.Key).Take(10).ToArray(),
                WordsByFrequency = words.GroupBy(w => w).OrderByDescending(g => g.Count()).ToDictionary(g => g.Key, g => g.Count()),
                HowManyBytes = content.Count(),
                HowManyWords = words.Count(),
                HowManySentences = sentences.Count()
            };
        }

        public void WriteResults(string outputDirectory)
        {
            string outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(filePath) + "_result.txt");

            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                int i, j;
                for (i = 0; i<TOP10; i++)
                    writer.WriteLine($"Longest sentence {i+1}: {result.LongestSentences[i]}\n");

                for(i = 0;i<TOP10; i++)
                    writer.WriteLine($"Shortest sentence {i + 1}: {result.ShortestSentences[i]}\n");

                i = 0;
                j = 0;
                string lastWord = "";
                while (j < TOP10)
                {
                    if (result.LongestWords[i] != lastWord)
                    {
                        writer.WriteLine($"Longest word {j + 1}: {result.LongestWords[i]}\n");
                        lastWord = result.LongestWords[i];
                        j++;
                    }
                    i++;
                }
                writer.WriteLine($"Most common letters:");
                for (i = 0; i < TOP10; i++)
                    writer.WriteLine($"'{result.MostCommonLetters[i]}'");
                writer.WriteLine($"");
                writer.WriteLine("Words sorted by frequency:\n");
                foreach (var word in result.WordsByFrequency)
                {
                    writer.WriteLine($"{word.Key}: {word.Value} times");
                }
            }
        }
    }

    class Result
    {
        public string[] LongestSentences { get; set; }
        public string[] ShortestSentences { get; set; }
        public string[] LongestWords { get; set; }
        public char[] MostCommonLetters { get; set; }
        public Dictionary<string, int> WordsByFrequency { get; set; }
        public int HowManyBytes { get; set; }
        public int HowManyWords { get; set; }
        public int HowManySentences { get; set; }
    }
}



