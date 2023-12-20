using Cafe_management_system_backend.MVC.Models;

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

        protected ConnectionRepositoryDB()
        {
            db = new CafeEntities();
        }
    }
}