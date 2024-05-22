using Android.Renderscripts;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using static Java.Util.Jar.Attributes;

namespace Quanlychitieu.DataAccess
{
    public class DataAccessRepo : IDataAccessRepo
    {
        private MongoClient mongo;
        private IMongoDatabase db;
        private readonly string connectionString = "mongodb://localhost:27017";
        private readonly string databaseName = "testdb";

        public IMongoDatabase GetDb()
        {
            if (db == null)
            {
                mongo = new MongoClient(connectionString);
                db = mongo.GetDatabase(databaseName);
                Debug.WriteLine($"Database '{databaseName}' đã được tạo thành công.");
            }
            else
            {
                Debug.WriteLine($"Sử dụng kết nối hiện có đến database '{databaseName}'.");
            }
            return db;
        }

        public void DeleteDB()
        {
            try
            {
                var client = new MongoClient(connectionString);
                client.DropDatabase(databaseName);
                Debug.WriteLine("Database đã bị xóa");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
