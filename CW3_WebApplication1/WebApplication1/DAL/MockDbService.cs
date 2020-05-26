using System;
using System.Collections.Generic;
using WebApplication1.Models;
using System.Data.SqlClient;
using WebApplication1.DTOs.Requests;

namespace WebApplication1.DAL
{
    public class MockDbService : IDbService
    {
        private static List<Student> _students;
        private const string CONN_STR = "Data Source=db-mssql;Initial Catalog=s16446;Integrated Security=True";
        static MockDbService() 
        {
            _students = new List<Student>();
           
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "select IndexNumber, FirstName, LastName, BirthDate, ss.Name as Studies from dbo.Student s " +
                        " left join dbo.Enrollment e on s.IdEnrollment = e.IdEnrollment " +
                        " left join dbo.Studies ss on e.IdStudy = ss.IdStudy";

                client.Open();

                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        Studies = dr["Studies"].ToString()
                    };
                    _students.Add(st);
                }
            }
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public IEnumerable<Student> GetStudent(string id) 
        {
            List <Student> n = new List<Student>();
            string index_no = id;

            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "select IndexNumber, FirstName, LastName, BirthDate from dbo.Student WHERE IndexNumber = @index_no";
                com.Parameters.AddWithValue("index_no", index_no);
                client.Open();
                
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString()
                    };
                    n.Add(st);
                }
            }
            return n;
        }

        public void AddStudent(Student student)
        {
            _students.Add(student);
        }

        public void DeleteStudent(Student student)
        {
            if (_students.Contains(student))
                _students.Remove(student);
        }

        public Student FindStudent(string index)
        {
            for(int i = 0; i < _students.Count; i++) {
                if (_students[i].IndexNumber.Equals(index)){
                    return _students[i];
                }
            }
            return null;
        }


        public IEnumerable<Enrollment> GetEnrollments(string id, int semester) {
            List<Enrollment> wpisy = null;
            
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "" +
                "SELECT " +
                  "  st.IndexNumber " +
	              ", st.FirstName " +
	              ", st.LastName " +
                  ", e.Semester " +
                  ", e.StartDate " +
	              ", s.[Name] " +
                "FROM dbo.Student st " +
                "LEFT JOIN dbo.Enrollment e ON st.IdEnrollment = e.IdEnrollment " +
                "LEFT JOIN dbo.Studies s ON e.IdStudy = s.IdStudy " +
                "WHERE IndexNumber = '" + id + "' AND e.Semester = '" + semester + "'";
  
                client.Open();
                wpisy = new List<Enrollment>();

                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var e = new Enrollment();
                    e.Semester = dr["Semester"].ToString();
                    e.StartDate = DateTime.Parse(dr["StartDate"].ToString()).ToShortDateString();
                    e.StudiesName = dr["Name"].ToString();
                    wpisy.Add(e);
                }
            }
            return wpisy;
        }

        public bool CheckLogin(string id, string passwordNew)
        {
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "" +
                "SELECT TOP 1 st.Password, st.Salt " +
                "FROM dbo.Student st " +
                "WHERE IndexNumber = @index_no";
                com.Parameters.AddWithValue("index_no", id);

                client.Open();

                var dr = com.ExecuteReader();
                
                while (dr.Read())
                {
                    string HashFromDb = dr["Password"].ToString();
                    string SaltFromDb = dr["Salt"].ToString();
                    return Password.Validate(passwordNew, SaltFromDb, HashFromDb);
                }
             }
             return false;
        }

        public void SaveRefreshToken(string id, string refreshToken)
        {
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "" +
                "UPDATE dbo.Student " +
                "SET RefreshToken = @token" +
                " WHERE IndexNumber = @index_no";
                com.Parameters.AddWithValue("index_no", id);
                com.Parameters.AddWithValue("token", refreshToken);

                client.Open();
                var dr = com.ExecuteNonQuery();
            }
        }

         public bool PopulateWithData()
        {
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
            // ---------------------------------------------
            // ONLY FOR TESTING PURPOSE
             string passwordPlainText = "password";
            // ---------------------------------------------
                string salt = Password.CreateSalt();
                string passwordHash = Password.CreatePasswordHash(passwordPlainText, salt);
                com.Connection = client;
                com.CommandText = "" +
                "UPDATE dbo.Student " +
                "SET Password = @hash " +
                ", Salt = @salt ;";
                com.Parameters.AddWithValue("hash", passwordHash);
                com.Parameters.AddWithValue("salt", salt);

                client.Open();
                var dr = com.ExecuteNonQuery();
                if (dr > 0) return true;
                else return false;
            }
        }

        public string FindRefreshToken(string refToken)
        {
            using (var client = new SqlConnection(CONN_STR))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "" +
                "SELECT TOP 1 IndexNumber FROM dbo.Student " +
                "WHERE refreshToken = @refToken";
                com.Parameters.AddWithValue("refToken", refToken);

                client.Open();
                var dr = com.ExecuteReader();
                
                if (dr.HasRows) {
                    dr.Read();
                    return dr["IndexNumber"].ToString();
                    
                }
                else 
                    return null;
            }

        }
    }
}
