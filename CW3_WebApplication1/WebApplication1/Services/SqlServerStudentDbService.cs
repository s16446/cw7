using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DTOs.Requests;
using WebApplication1.DTOs.Responses;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string CONN_STR = "Data Source=db-mssql;Initial Catalog=s16446;Integrated Security=True";
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            var response = new EnrollStudentResponse();
            response.setStatus(400, "Unknown Error"); // domyślnie - błąd
            _ = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IndexNumber = request.IndexNumber,
                BirthDate = request.BirthDate,
                Studies = request.Studies
            };

            using (var connection = new SqlConnection(CONN_STR))
            { 
                connection.Open();
                var command = connection.CreateCommand();
                var transaction = connection.BeginTransaction();
                
                command.Connection = connection;
                command.Transaction = transaction;
            try
            {
                command.CommandText = "select IdStudy, Name from dbo.Studies where name = @studies;";
                command.Parameters.AddWithValue("studies", request.Studies);
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    response.setStatus(400, "ERROR: Nie istnieją studia przekazane przez klienta");
                }
                else
                {
                    int idStudy = int.Parse(reader["IdStudy"].ToString());
                    string studiesName = reader["Name"].ToString();
                    reader.Close();

                    command.CommandText = "SELECT TOP 1 IdEnrollment, StartDate " +
                    " FROM dbo.Enrollment e " +
                    " INNER JOIN dbo.Studies s ON e.IdStudy = s.IdStudy and Semester = 1 " +
                    " WHERE s.name = @studies " +
                    " ORDER BY StartDate DESC;";

                    DateTime enrollmentDate = DateTime.Now.Date;
                    reader = command.ExecuteReader();

                    int nextEnrollment;
                    if (!reader.Read())
                    {
                        reader.Close();
                        command.CommandText = "SELECT ISNULL(MAX(IdEnrollment), 0) as id FROM dbo.Enrollment";
                        reader = command.ExecuteReader();
                        if (!reader.Read())
                            nextEnrollment = 1;
                        else
                            nextEnrollment = int.Parse(reader["id"].ToString()) + 1;
                        reader.Close();

                        command.CommandText = "INSERT INTO dbo.Enrollment(IdEnrollment, Semester, IdStudy, StartDate) " +
                            "VALUES( " + nextEnrollment + ", 1, " + idStudy + ", '" + enrollmentDate.ToString("yyyy-MM-dd") + "');";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        nextEnrollment = int.Parse(reader["IdEnrollment"].ToString());
                        enrollmentDate = DateTime.ParseExact(reader["StartDate"].ToString().Substring(0,10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    ;
                    reader.Close();

                    command.CommandText = "SELECT IndexNumber FROM dbo.Student s WHERE IndexNumber = @IndexNumber;";
                    command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    reader = command.ExecuteReader();
                    if (reader.Read()) 
                    {
                        reader.Close();
                        transaction.Rollback();
                        response.setStatus(400, "ERROR: Numer indeksu studenta nie jest unikalny"); // nr indeksu studenta nie jest unikalny
                    }
                    else 
                    {
                        reader.Close();
                        DateTime BirthDateNew;
                        if (DateTime.TryParseExact(request.BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out BirthDateNew))
                        {

                            command.CommandText = "INSERT INTO dbo.Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment)" +
                                "VALUES(@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment);";
                            command.Parameters.AddWithValue("FirstName", request.FirstName);
                            command.Parameters.AddWithValue("LastName", request.LastName);
                            command.Parameters.AddWithValue("BirthDate", BirthDateNew);
                            command.Parameters.AddWithValue("IdEnrollment", nextEnrollment);
                            command.ExecuteNonQuery();
                            reader.Close();
                            transaction.Commit();
                        
                            response.LastName = request.LastName;
                            response.Semester = 1;
                            response.Studies = studiesName;
                            response.StartDate = enrollmentDate.ToString("dd.MM.yyyy");
                            response.setStatus(201, "Student został poprawnie zapisany na semestr"); // student został poprawnie zapisany na semestr                           
                        }
                        else 
                        {
                            reader.Close();
                            transaction.Rollback();
                            response.setStatus(400, "ERROR: Błędna data urodzenia"); // nr indeksu studenta nie jest unikalny
                        }
                    }
                };
            }
            catch (SqlException e)
            {
               Console.WriteLine("An exception of type " + e.GetType());
               transaction.Rollback();
            }
            return response;
        }
    }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            var response = new PromoteStudentsResponse();
            response.setStatus(400, "Unknown Error"); // domyślnie - błąd

            using (var connection = new SqlConnection(CONN_STR))
            { 
                connection.Open();
                var command = connection.CreateCommand();
                var transaction = connection.BeginTransaction();
                
                command.Connection = connection;
                command.Transaction = transaction;

                command.CommandText = "select e.IdEnrollment from dbo.Enrollment e inner join dbo.Studies s on e.IdStudy = s.IdStudy where s.Name = @studyName and e.Semester = @semesterNumber";
                command.Parameters.AddWithValue("studyName", request.Studies);
                command.Parameters.AddWithValue("semesterNumber", request.Semester);
                
                var reader = command.ExecuteReader();
                if (!reader.Read()) {
                    reader.Close();
                    response.setStatus(404, "ERROR: nie znaleziono semestru i/lub studiów");
                    transaction.Rollback();
                }
                else
                { 
                    reader.Close();
                    

                    using (SqlConnection conn = new SqlConnection(CONN_STR)) 
                    {
                        conn.Open();
                        SqlCommand cmd  = new SqlCommand("Promote", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@studies", request.Studies));
                        cmd.Parameters.Add(new SqlParameter("@semester", request.Semester));
                        cmd.ExecuteReader();
                        conn.Close();
                    }
                response.Semester = request.Semester + 1;
                response.StartDate = DateTime.Now.ToString("dd.MM.yyyy");
                response.StudiesName = request.Studies;

                transaction.Commit();     
                response.setStatus(201, "Studenci zostali promowani na następny semestr");
                }
            }
            return response;
        }


    }
}
