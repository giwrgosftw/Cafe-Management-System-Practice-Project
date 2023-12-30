using Cafe_management_system_backend.MVC.Models;
using NLog;
using System;

/* 
 * The RepositoryConnectionDB class serves as a base (Parent) class designed for inheritance.
 * Specifically, we are assuming that the derived (child) classes are the rest Repository classes. 
 * It eliminates the need to repeatedly initialize the 'CafeEntities db' variable in each Repository class. 
 * By inheriting from this class, child Repository classes share a common database context, promoting
 * code reuse and centralizing the database connection logic.
 */
namespace Cafe_management_system_backend.MVC.Repositories
{
    public class ConnectionRepositoryDB
    {
        protected readonly CafeEntities db;
        protected static Logger logger = LogManager.GetLogger("NLogger");

        protected ConnectionRepositoryDB()
        {
            db = new CafeEntities();
        }

        protected void GetInnerException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                logger.Error($"InnerException: {ex.InnerException?.Message}");

                // Check for InnerException's InnerException
                if (ex.InnerException.InnerException != null)
                {
                    logger.Error($"InnerException.InnerException: {ex.InnerException.InnerException?.Message}");
                }
            }
        }

    }
}