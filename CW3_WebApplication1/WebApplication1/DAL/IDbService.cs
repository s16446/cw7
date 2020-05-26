using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents();
        
        void AddStudent(Student student);

        void DeleteStudent(Student student);

        Student FindStudent(string index);

        IEnumerable<Student> GetStudent(string id);

        IEnumerable<Enrollment> GetEnrollments(string id, int semester);
        bool CheckLogin(string id, string hash);
        bool PopulateWithData();
        string FindRefreshToken(string refToken);
        void SaveRefreshToken(string login, string refreshToken);
    }
}
