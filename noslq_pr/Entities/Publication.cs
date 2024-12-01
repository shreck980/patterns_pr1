using noslq_pr.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public class Publication
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int Circulation { get; set; }
        public decimal Price { get; set; }
        public List<Author> Authors{ get; set; }    
        public Genre Genre { get; set; }
        public PrintQuality PrintQuality { get; set; }
        public int Quantity { get; set; }
      
        public Publication(PublicationBuilder pb) {
           
            Id = pb.Id;
            Title = pb.Title;
            PageCount = pb.PageCount;
            Circulation = pb.Circulation;
            Price = pb.Price;
            Authors = pb.Authors;
            Genre = pb.Genre;
            PrintQuality = pb.PrintQuality;
            Quantity = pb.Quantity;
        }

        public override string ToString()
        {
           
            string authorsList = Authors != null && Authors.Count > 0
                ? string.Join("\n\n", Authors.Select(a=>a.ToString()))
                : "No Authors";

            return $"Id: {Id} \nTitle: {Title}, \nPageCount: {PageCount}, \nCirculation: {Circulation}, Price: {Price}$, " +
                   $"Genre: {Genre}, \nPrintQuality: {PrintQuality}, Quantity: {Quantity}, \n---Authors---\n{authorsList}";
        }
    }

    public enum Genre
    {
        Fiction=1,
        NonFiction,
        Fantasy,
        ScienceFiction,
        Mystery,
        Biography,
        History,
        Children,
        Poetry,
        GraphicNovels,
        Other
    }
    public enum PrintQuality
    {
       
        Low=1,       
        Medium,     
        High,       
        Premium,     
        Other
    }

}
