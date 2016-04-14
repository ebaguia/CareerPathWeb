using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationForms.Controller
{
    public partial class Career
    {
        public Career()
        {
        }

        public Career(String id, String name, String description, List<string> jobs, List<Course> finalCourses = null)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.jobs = jobs;
            this.finalCourses = finalCourses;
        }

        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public List<string> jobs { get; set; }
        public List<Course> finalCourses { get; set; }
    }

    public class CareerInfoDataItem
    {
        public String finalCourse { get; set; }
    }
}
