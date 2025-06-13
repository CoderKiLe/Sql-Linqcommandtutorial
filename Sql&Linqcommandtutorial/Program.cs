using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace DatabaseViewer
{
    // data model 
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Creating the database database file will be created if it doesnt exits
            string connectionString = "Data Source=mydatabase.db";
            var connection = new SqliteConnection(connectionString);

            // create
            CreateDatabase(connection);

            // Insert
            InsertData(connection, "Lekey", $"lekeydema{RandomNumberGenerator()}@gmail.com");

            // View Data
            PrintData(connection);

            // update dataf
            UpdateUser(connection, 3, "Luke", "Luke@gmail.com");
            PrintData(connection);

            // delete data
            DeleteUser(connection, 3);
            PrintData(connection);
        }

        static void CreateDatabase(SqliteConnection connection)
        {
            Console.WriteLine("hello world! Database is being created now...");

            using (connection)
            {
                connection.Open();

                // now creating a new table (if it doesnt exist)
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT UNIQUE NOT NULL
                )";
                createTableCmd.ExecuteNonQuery();
                Console.WriteLine("TABLE \"Users\" created successfully!");

            }
            // automatically disposed when it is used (using(){} keyword)

        }

        // lets perform some crud operation below now since we have created the table of the database
        // INSERT
        static void InsertData(SqliteConnection connection, string name, string email)
        {
            Console.WriteLine("Inserting New User");
            try
            {
                using (connection)
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = @"INSERT INTO Users (Name, Email) VALUES ($name, $email)";
                    command.Parameters.AddWithValue("$name", name);
                    command.Parameters.AddWithValue("$email", email);
                    command.ExecuteNonQuery();

                    Console.WriteLine("User added successfully");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception raised: ${e}");
            }


        }

        // READ data
        static void PrintData(SqliteConnection connection)
        {
            using (connection)
            {
                connection.Open();

                var users = GetAllUser(connection);
                var activeUsers =
                    from user in users
                    select $"ID: {user.Id}; Name: {user.Name}; Email: {user.Email}";

                foreach (var item in activeUsers)
                {
                    Console.WriteLine(item);
                }
            }
        }


        // Update data
        static void UpdateUser(SqliteConnection connection, int id, string newName, string newEmail)
        {
            using (connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                UPDATE Users 
                SET Name = $name, Email = $email
                WHERE Id = $id";

                command.Parameters.AddWithValue("$name", newName);
                command.Parameters.AddWithValue("$email", newEmail);
                command.Parameters.AddWithValue("$id", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine("User updated successfully!");
                else
                    Console.WriteLine("User not found.");
            }

        }


        // delection operation
        static void DeleteUser(SqliteConnection connection, int id)
        {

            using (connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Users WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine("User deleted successfully!");
                else
                    Console.WriteLine("User not found.");
            }

        }

        // Other Helper Functions
        // to return the IEnumerable for the whatever
        // using the Yield concepts is cool yk.
        static IEnumerable<User> GetAllUser(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users";
            // lets use the LINQ query here the lessons that you have learned earlier

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return new User
                {
                    Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                };
            }
        }

        static int RandomNumberGenerator()
        {
            Random random = new Random();
            int randomInt = random.Next(1, 999);
            return randomInt;
            
        }


    }
}