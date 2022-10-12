using Domain;
using OutputTests.Client.Models;

namespace OutputTests.Client 
{
    public interface ITestAPIClient
    {
        Task<OperationResult<Course>> GetCourseAsync(int id);
        Task<OperationResult<Course>> PostCourseAsync(Course course);
        Task<OperationResult<Course>> PutCourseAsync(Course course);
        Task<OperationResult> DeleteCourseAsync(int id);
        Task<OperationResult<Student>> GetStudentAsync(int id);
        Task<OperationResult<Student>> PostStudentAsync(Student student);
        Task<OperationResult<Student>> PutStudentAsync(Student student);
        Task<OperationResult> DeleteStudentAsync(int id);
    }
}