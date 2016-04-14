using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Web.UI.WebControls;
using System.Drawing;

namespace WebApplicationForms.Controller
{
    public class Course
    {
        public Course()
        {

        }

        public Course(String id,
                    int year,
                    int sem,
                    String name,
                    String description,
                    int points,
                    String academicOrg,
                    String academicGroup,
                    String courseComp,
                    String gradingBasis,
                    String typeOffered,
                    String remarks = "",
                    string careerId = "",
                    int compulsory = 0)
        {
            this.id = id;
            this.year = year;
            this.sem = sem;
            this.name = name;
            this.description = description;
            this.points = points;
            this.academicOrg = academicOrg;
            this.academicGroup = academicGroup;
            this.courseComp = courseComp;
            this.gradingBasis = gradingBasis;
            this.typeOffered = typeOffered;
            this.remarks = remarks;
            this.careerId = careerId;
            this.compulsory = compulsory;

            initialise();
        }

        private void initialise()
        {
            preReqCourses = new List<Course>();
        }

        // Course items
        //
        public String id { get; set; }
        public int year { get; set; }
        public int sem { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public int points { get; set; }
        public String academicOrg { get; set; }
        public String academicGroup { get; set; }
        public String courseComp { get; set; }
        public String gradingBasis { get; set; }
        public String typeOffered { get; set; }
        public String remarks { get; set; }
        public string careerId { get; set; }
        public int compulsory { get; set; }
        public List<Course> preReqCourses { get; set; }
    }

    public class CourseInfoDataItem
    {
        public String item { get; set; }
        public String description { get; set; }
    }
}