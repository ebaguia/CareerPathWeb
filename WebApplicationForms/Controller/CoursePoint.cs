using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForms.Controller
{
    public class CoursePoint
    {
        public int mYAxis { get; set; }
        public Course mCourse { get; set; }

        public CoursePoint(int yAxis, Course course)
        {
            mYAxis = yAxis;
            mCourse = course;
        }
    }
}