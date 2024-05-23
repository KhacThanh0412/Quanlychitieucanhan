using Android.Renderscripts;
using LiteDB;
using System.Diagnostics;
using static Java.Util.Jar.Attributes;
using System;
namespace Quanlychitieu.DataAccess
{
    public class DataAccessRepo : IDataAccessRepo
    {
        LiteDatabase db;

        public LiteDatabase GetDb() //this function returns the path where the db file is saved
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "quanlychitieu.db");
            Console.WriteLine("Database path on this system: " + path);
            db = new LiteDatabase(path);
            return db;
        }
        public void DeleteDB()
        {
            string path;

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "quanlychitieu.db");
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Debug.WriteLine("File deleted");
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
