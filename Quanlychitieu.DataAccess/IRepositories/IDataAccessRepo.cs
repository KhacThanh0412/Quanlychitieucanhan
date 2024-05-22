namespace Quanlychitieu.DataAccess.IRepositories;

public interface IDataAccessRepo
{
    IMongoDatabase GetDb();
    void DeleteDB();
}
