namespace Quanlychitieu.DataAccess.IRepositories;

public interface IDataAccessRepo
{
    LiteDatabaseAsync GetDb();
    void DeleteDB();
}
