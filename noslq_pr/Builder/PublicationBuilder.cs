using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public class PublicationBuilder
    {
        public long Id { get; private set; }
        public string Title { get; private set; }
        public int PageCount { get; private set; }
        public int Circulation { get; private set; }
        public decimal Price { get; private set; }
        public List<Author> Authors { get; private set; }
        public Genre Genre { get; private set; }
        public PrintQuality PrintQuality { get; private set; }
        public int Quantity { get; private set; }

        public PublicationBuilder()
        {
            Id = 0;
            PageCount = 0;
            Circulation = 0;
            Price = decimal.Zero;
            Genre = Genre.Other;
            PrintQuality = PrintQuality.Other;
            Authors = new List<Author>();
            Quantity = 0;
        }

        public PublicationBuilder SetId(long id)
        {
            Id = id;
            return this;
        }
        public PublicationBuilder SetTitle(string Title)
        {
            this.Title = Title;
            return this;
        }

        public PublicationBuilder SetPageCount(int PageCount)
        {
            this.PageCount = PageCount;
            return this;
        }

        public PublicationBuilder SetCirculation(int Circulation)
        {
            this.Circulation = Circulation;
            return this;
        }

        public PublicationBuilder SetPrice(decimal Price)
        {
            this.Price = Price;
            return this;
        }


        public PublicationBuilder SetPrintQuality(PrintQuality PrintQuality)
        {
            this.PrintQuality = PrintQuality;
            return this;
        }

        public PublicationBuilder SetGenre(Genre Genre)
        {
            this.Genre = Genre;
            return this;
        }

        public PublicationBuilder SetQuantity(int Quantity)
        {
            this.Quantity = Quantity;
            return this;
        }

        public PublicationBuilder AddAuthor(Author author)
        {
            Authors.Add(author);
            return this;
        }
        public PublicationBuilder AddAuthors(List<Author> authors)
        {
            Authors = authors;
            return this;
        }

        public Publication Build()
        {
            return new Publication(this);
        }

        internal void Reset()
        {
            Id = 0;
            PageCount = 0;
            Circulation = 0;
            Price = decimal.Zero;
            Genre = Genre.Other;
            PrintQuality = PrintQuality.Other;
            Authors = new List<Author>();
            Quantity = 0;
            Title = String.Empty;
        }
    }
}
