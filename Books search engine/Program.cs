using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace BookSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the word to search for from the user
            Console.Write("Enter a word to search for: ");
            string word = Console.ReadLine();

            // Loop through each file in the Books folder
            string folderPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Books");
            string[] filePaths = Directory.GetFiles(folderPath);
            List<Thread> threads = new List<Thread>();
            foreach (string filePath in filePaths)
            {
                if (System.IO.Path.GetExtension(filePath).ToLower() == ".pdf")
                {
                    // Create a new thread to search for the word in the PDF file
                    Thread thread = new Thread(() => SearchPdfFile(filePath, word));
                    thread.Start();
                    threads.Add(thread);
                }
            }

            // Wait for all threads to complete
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Wait for the user to press a key before exiting
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void SearchPdfFile(string filePath, string word)
        {
            // Open the PDF file and search for the word
            PdfReader reader = new PdfReader(filePath);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                string contents = PdfTextExtractor.GetTextFromPage(reader, page);
                string[] lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                for (int lineNum = 0; lineNum < lines.Length; lineNum++)
                {
                    MatchCollection matches = Regex.Matches(lines[lineNum], "\\b" + word + "\\b");
                    if (matches.Count > 0)
                    {
                        Console.WriteLine("{0}, Page {1}, Line {2}:", System.IO.Path.GetFileName(filePath), page, lineNum + 1);
                        foreach (Match match in matches)
                        {
                            Console.WriteLine("  Position: {0}", match.Index);
                        }
                    }
                }
            }
        }
    }
}
