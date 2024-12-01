using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface IPrintingHouseDAO : ISubject 
    {
        void AddPrintingHouse(PrintingHouse printingHouse);
        void UpdatePrintingHouse(PrintingHouse printingHouse);
        PrintingHouse GetPrintingHouse(int id);
        PrintingHouse GetPrintingHouseByName(string name);
        List<PrintingHouse> GetPrintingHouseByCountry(string country);
        List<PrintingHouse> GetAllPrintingHouse();
    }
}
