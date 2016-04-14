using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationForms.Controller
{
    public class Programme
    {
        public Programme()
        {
        }

        public Programme(String id,
                         String name,
                         String description,
                         List<Course> courses)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.courses = courses;
        }

        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public List<Course> courses { get; set; }
    }
}
