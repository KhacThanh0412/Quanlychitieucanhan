using LiteDB;

namespace Quanlychitieu.DataAccess.IRepositories;

public interface IDataAccessRepo
{
    LiteDatabase GetDb();
    void DeleteDB();
}
