using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationForms.Controller
{
    public class CourseProgrammePart
    {
        public CourseProgrammePart()
        {
        }

        public CourseProgrammePart(String courseId, String part, String programmeId)
        {
            this.courseId = courseId;
            this.part = part;
            this.programmeId = programmeId;

            Initialise();
        }

        private void Initialise()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            courses = dbConnection.ReadCoursesUsingProgrammePart(programmeId, part);
        }

        public String courseId { get; set; }
        public String part { get; set; }
        public String programmeId { get; set; }

        public List<Course> courses { get; set; }
    }
}
