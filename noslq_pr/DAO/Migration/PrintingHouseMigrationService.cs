using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public class PrintingHouseMigrationService : MigrationService
    {
        private readonly DAOFactory daoFactory;

        public PrintingHouseMigrationService(DAOFactory dAOFactory)
        {
            daoFactory = dAOFactory;


        }

        public void Migration(DataBaseType sourse, DataBaseType destination)
        {
            try
            {
                ConnectionService.EstablishConnection(sourse);
                IPrintingHouseDAO printingHouseDAO = daoFactory.GetPrintingHouseDAO();
                if (printingHouseDAO == null) throw new Exception("Author is null");
                List<PrintingHouse> printingHouses = printingHouseDAO.GetAllPrintingHouse();

                if (printingHouses.Count == 0) throw new Exception("No customers in mysql database found");

                ConnectionService.EstablishConnection(destination);
                printingHouseDAO = daoFactory.GetPrintingHouseDAO();
                if (printingHouseDAO == null) throw new Exception("PrintingHouse is null");
                printingHouseDAO.AddPrintingHouses(printingHouses);

                Console.WriteLine($"Successfully migrated PrintingHouse data from {sourse} to {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
