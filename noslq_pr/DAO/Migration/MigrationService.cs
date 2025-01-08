using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public interface MigrationService
    {
        public void Migration(DataBaseType sourse, DataBaseType destination);
    }
}
