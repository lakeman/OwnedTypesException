using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OwnedTypes
{
    class Program
    {
        static Random _rand = new Random();
        static T Rand<T>(T[] values)
        {
            return values[_rand.Next(0,values.Length-1)];
        }
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<Database>()
                .ReplaceService<IComparer<ModificationCommand>, FixedComparer>()
                .Options;

            Console.WriteLine("Create database...");
            using (var context = new Database(options))
            {
                context.Database.Migrate();
            }

            var first = new string[] { "John", "Glen", "Robert", "David", "Tom"};
            var last = new string[] { "Smith", "Jones", "Clancy" };
            var title = new string[] { "The {0} of {1}", "How to {0} your {1}", "{1} {0}"};
            var verb = new string[] { "Binding","Shinning","Conquest", "Tale", "Work", "Train"};
            var noun = new string[] { "Isaac", "Cities", "Dragon" };
            var status = (Status[])Enum.GetValues(typeof(Status));
            var genre = (Genre[])Enum.GetValues(typeof(Genre));
            var section = (Section[])Enum.GetValues(typeof(Section));

            using (var context = new Database(options))
            {
                for (int i = 0; i<200; i++)
                {
                    var author = new Author { Name = $"{Rand(first)} {Rand(last)}", Status = Rand(status) };
                    for (int j = 0; j < 20; j++)
                    {
                        var book = new Book { Status = Rand(status), Genre = Rand(genre), Section = Rand(section), Title = string.Format(Rand(title), Rand(verb), Rand(noun)) };
                        book.Author = author;
                        author.Books.Add(book);
                        context.Book.Add(book);
                        Console.WriteLine($"{book.Title} by {book.Author.Name} [{book.Genre}, {book.Status}, {book.Section}]");
                    }
                    context.Author.Add(author);
                }
                Console.WriteLine("Insert...");
                context.SaveChanges();
            }

            using (var context = new Database(options))
            {
                foreach(var author in context.Author
                    .Include(a => a.Books).ThenInclude(b => b.Genre)
                    .Include(a => a.Books).ThenInclude(b => b.Section)
                    .Include(a => a.Books).ThenInclude(b => b.Status)
                    .Include(a => a.Books).ThenInclude(b => b.Audit))
                {
                    foreach(var book in author.Books)
                    {
                        switch (_rand.Next(0, 3))
                        {
                            case 0:
                                book.Title = string.Format(Rand(title), Rand(verb), Rand(noun));
                                break;
                            case 1:
                                book.Status.Value = Rand(status);
                                break;
                            case 2:
                                book.Genre.Value = Rand(genre);
                                break;
                            case 3:
                                book.Section.Value = Rand(section);
                                break;
                        }
                        Console.WriteLine($"{book.Title} by {book.Author.Name} [{book.Genre}, {book.Status}, {book.Section}]");
                    }
                    author.Name = $"{Rand(first)} {Rand(last)}";
                    author.Status.Value = Rand(status);
                }
                Console.WriteLine("Update...");
                context.SaveChanges();
            }

            Console.WriteLine("Drop...");
            using (var context = new Database(options))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
