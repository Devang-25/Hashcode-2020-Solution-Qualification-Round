using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hashcode_2020
{

    public class Book {
        public int ID { get; set; }
        public int Score { get; set; }

        public override string ToString() {
            return ID.ToString();
        }
    }
    public class Library
    {
        public int ID { get; set; }
        public Book[] Books { get; set; }
        public int DaysForSignup { get; set; }
        public int BooksPerDay { get; set; }

        public ulong Score {
            get {
                return Books.GetLibraryScore();
            }
        }
        public Book[] GetBooks(int daysLeft) {
            var realDays = daysLeft - DaysForSignup;
            if (realDays <= 0) {
                return new Book[0];
            }
            var numberOfBooksCanProcess = (int)Math.Floor((double)(realDays / BooksPerDay));
            numberOfBooksCanProcess += realDays % BooksPerDay;
            return Books
                .OrderByDescending(i => i.Score)
                .Take(numberOfBooksCanProcess)
                .ToArray();
        }
        public ulong getLibraryScore(int daysLeft) {
            var realDays = daysLeft - DaysForSignup;
            if (realDays <= 0) {
                return (ulong)0;
            }
            var numberOfBooksCanProcess = (int)Math.Floor((double)(realDays / BooksPerDay));
            numberOfBooksCanProcess += realDays % BooksPerDay;
            return Books
                .OrderByDescending(i => i.Score)
                .Take(numberOfBooksCanProcess)
                .GetLibraryScore();
        }
    }
    public static class Algorithms
    {
        public static Book[] GetBooks(this string bookList, int bookQuantity) {
            var books = new Book[bookQuantity];
            for (var j = bookQuantity - 1; j >= 0; j--) {
                books[j] = new Book { ID = j, Score = bookList[j] };
            }
            return books;
        }

        public static ulong GetLibraryScore(this IEnumerable<Book> books) {
            return (ulong)(books.Count() + books.Sum(i => i.Score));
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            var path = "./input/";
            DirectoryInfo d = new DirectoryInfo(path);
            foreach (var file in d.GetFiles("*.txt"))
            {
                Console.WriteLine(file.Name);
                var input = System.IO.File.ReadAllLines(path + file.Name);
                var (B, L, daysLeft) = input
                    .Select(i => i.Split(" "))
                    .Select(i => (int.Parse(i[0]), int.Parse(i[1]), int.Parse(i[2])))
                    .First();
                var books = input.Skip(1).Select(i => i.GetBooks(B)).First();
                input = input.Skip(2).ToArray();
                var libraries = new Library[L];
                for (var i = 0; i < L; i++) {
                    var libraryStrings = input.Skip(i * 2).Take(2).ToArray();
                    var libraryMetaData = libraryStrings[0].Split(" ");
                    libraries[i] = new Library
                    {
                        ID = i,
                        DaysForSignup = int.Parse(libraryMetaData[1]),
                        BooksPerDay = int.Parse(libraryMetaData[2]),
                        Books = libraryStrings[1].Split(" ").Select(j => books[int.Parse(j)]).ToArray()
                    };
                }
                var selectedLibraries = new List<Library>();
                var output = new List<String>();
                var retries = 1000;
                do
                {
                    libraries = libraries
                        .OrderByDescending(i => i.getLibraryScore(daysLeft))
                        .Where(i => i.getLibraryScore(daysLeft) > 0)
                        .ToArray();
                    var library = libraries.FirstOrDefault();
                    libraries = libraries.Where(i => i.ID != library.ID).ToArray();
                    if (library == null) {
                        break;
                    }
                    output.Add($"{library.ID} {library.GetBooks(daysLeft).Count()}");
                    output.Add(string.Join(" ", library.GetBooks(daysLeft).Select(i => i.ToString())));
                    selectedLibraries.Add(library);
                    retries--;
                    daysLeft -= library.DaysForSignup;
                } while (daysLeft > 0 && retries > 0);
                output = new List<string>() { (output.Count() / 2).ToString() }.Concat(output).ToList();
                File.WriteAllLines("./output/" + file.Name, output);
            }
        }
    }
}