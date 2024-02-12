using System.Text.RegularExpressions;

namespace homework_3
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputDirectory = "C:\\Magda\\Volvo Course\\homework\\100 books";
            string outputDirectory = "C:\\Magda\\Volvo Course\\homework\\homework_3";

            ProcessFiles(inputDirectory, outputDirectory);

            Console.WriteLine("Processing completed. Press any key to exit.");
            Console.ReadKey();
        }

        static void ProcessFiles(string inputDirectory, string outputDirectory)
        {
            var files = Directory.GetFiles(inputDirectory, "*.txt");

            List<Task> tasks = new List<Task>();
            foreach(var file in files)
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
                if(i> amountCompl)
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
                LongestSentence = sentences.OrderByDescending(s => s.Length).FirstOrDefault(),
                ShortestSentence = sentences.OrderBy(s => s.Split(' ').Length).FirstOrDefault(),
                LongestWord = words.OrderByDescending(w => w.Length).FirstOrDefault(),
                MostCommonLetter = content.GroupBy(c => c).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault().ToString(),
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
                writer.WriteLine($"Longest sentence: {result.LongestSentence}\n");
                writer.WriteLine($"Shortest sentence: {result.ShortestSentence}\n");
                writer.WriteLine($"Longest word: {result.LongestWord}\n");
                writer.WriteLine($"Most common letter: '{result.MostCommonLetter}'\n");

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
        public string LongestSentence { get; set; }
        public string ShortestSentence { get; set; }
        public string LongestWord { get; set; }
        public string MostCommonLetter { get; set; }
        public Dictionary<string, int> WordsByFrequency { get; set; }
        public int HowManyBytes { get; set; }
        public int HowManyWords { get; set; }
        public int HowManySentences { get; set; }
    }
}



