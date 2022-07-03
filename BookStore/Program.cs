using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace BookStore
{
    public class Book
    {
        private readonly string m_parentName;
        private readonly List<string> m_state_displayName;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Book() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Book(string parentName, List<string> state_displayName)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            m_parentName = parentName;
            m_state_displayName = state_displayName;
        }

        public string ParentName { get; set; }
        public List<string>? State_DisplayName { get; set; }
    }

    public class BooksStore
    {
        public static List<Book> GetBooks(string[,] arr)
        {
            List<Book> books = new() { };
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i, 0] != null)
                {
                     books.Add(new Book() { ParentName = arr[i, 0], State_DisplayName = new List<string> { arr[i, 1], arr[i, 2] } });
                }
            };
            
            return books;
        }

        private static void LoadURLdata()
        {
            //using var httpClient = new HttpClient();
            var url = "https://api.actionnetwork.com/web/v1/books";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new(response.GetResponseStream());

            string data = reader.ReadToEnd();
            string[,]? arr = new string[20, 3];

            int nIndex = 0;
            int nIndexStart = 0;
            int nIndexEnd = 0;
            int nIndexLoop = 0;
            int i = 0;

            while (nIndexLoop < data.Length)
            {
                try
                {
                    if ((nIndex = data.IndexOf("display_name", nIndexLoop)) > 0)
                    {
                        nIndexStart = nIndex + 15;
                        nIndexEnd = data.IndexOf(@",""", nIndexStart) - 1;
                        string displayName = data.Substring(nIndexStart, nIndexEnd - nIndexStart);

                        if (displayName != null)
                        {
                            nIndex = data.IndexOf("states", nIndexStart);
                            nIndexStart = nIndex + 10;
                            nIndexEnd = nIndexStart + 2; // data.IndexOf(@",""", nIndexStart);
                            string state = data.Substring(nIndexStart, nIndexEnd - nIndexStart);

                            if (state.IndexOf(",", 0) < 0)
                            {
                                nIndex = data.IndexOf("parent_name", nIndexLoop);
                                nIndexStart = nIndex + 14;
                                nIndexEnd = data.IndexOf(@",""", nIndexStart) - 1;
                                string parentName = data.Substring(nIndexStart, nIndexEnd - nIndexStart);

                                if (parentName != "ul")
                                {
                                    if (state == "CO")
                                    {
                                        arr[i, 0] = parentName;
                                        arr[i, 1] = displayName;
                                        arr[i, 2] = state;

                                        i++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                nIndexLoop = nIndexEnd;
            }
            MSave(arr);
        }

        private static void MSave(string[,] arr)
        {
            //int i = 0;
            string fileName = AppDomain.CurrentDomain.BaseDirectory + @"result.txt";
            FileStream StreamSave = new(fileName, FileMode.OpenOrCreate);

            try
            {
                using StreamWriter writer = new(StreamSave);
                List<Book> books = GetBooks(arr);

                var bookQuery =
                    from book in books
                    group book by book.ParentName; // into g
                    //orderby g.Key
                    //select g;

                foreach (var bookGroup in bookQuery)
                {
                    foreach (var book in bookGroup)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        writer.WriteLine("{0}\n  {1},  {2}\n", book.ParentName, book.State_DisplayName[0], book.State_DisplayName[1]);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        public static void Main()
        {
            LoadURLdata();
        }
    }
}

