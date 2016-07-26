using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebApplicationForms.Controller
{
    public static class Utils
    {
        /**
         * Checks the existence of a course from a list.
         * @param courseId - The course ID to be checked.
         * @param existingCourses - The list of existing courses
         * 
         * @return Returns true if the course is in the list, otherwise false.
         */
        public static bool IsCourseExist(string courseId, List<string> existingCourses)
        {
            bool isExist = false;
            foreach (string id in existingCourses)
            {
                if (id == courseId)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        /**
         * Checks if the course button Y-axis from the canvas is already in used.
         * 
         * @param Y - The Y-axis to be checked.
         * @param listOfY - The list of used Y-axis from the canvas.
         * 
         * @return Returns true if the Y-axis is found, otherwise false.
         */ 
        public static bool IsCoursePointYExist(double Y, List<double> listOfY)
        {
            bool isYExist = false;
            foreach (double tempY in listOfY)
            {
                if (tempY == Y)
                {
                    isYExist = true;
                    break;
                }
            }

            return isYExist;
        }

        /**
         * Deletes all Y-axis from a given list.
         * 
         * @param collectionOflistOfY - The list of Y-axis to be cleared.
         */
        public static void ClearListOfY(List<double>[] collectionOflistOfY)
        {
            foreach (List<double> listOfY in collectionOflistOfY)
            {
                if (listOfY != null)
                {
                    listOfY.Clear();
                }
            }
        }

        /**
         * Generates a HTML value of a list of pre-requisite courses.
         * 
         * @param preReqList - The list of pre-requisite courses.
         * 
         * @return String representation of the HTML value.
         */ 
        public static string GetPreReqDisplayString(List<Course> preReqList)
        {
            // Let us add the pre-req courses
            //
            string preReqString = "";
            foreach (Course preReqCourse in preReqList)
            {
                preReqString += preReqCourse.id + "<br>";
            }
            if (preReqString.Length > 1)
            {
                preReqString = preReqString.Remove(preReqString.Length - 1);
            }

            return preReqString;
        }

        /**
         * Generates a HTML value of a list of restriction courses.
         * 
         * @param preReqList - The list of restriction courses.
         * 
         * @return String representation of the HTML value.
         */ 
        public static string GetRestrictionDisplayString(DatabaseConnection dbConnection, Course course)
        {
            List<Course> restrList = dbConnection.GetRestrictionCourses(course.id);
            string restrString = "";
            foreach (Course restrCourse in restrList)
            {
                restrString += restrCourse.id + "<br>";
            }
            if (restrString.Length > 1)
            {
                restrString = restrString.Remove(restrString.Length - 1);
            }

            return restrString;
        }

        /**
         * Returns the next available y-axis from the canvas where the course button
         * can be drawn.
         * 
         * @param course - Course to be drawn
         * @param coursePoints - List of existing courses drawn in the canvas
         * @param ycoor - The pre-defined value of y-axis
         * 
         * @return The next available y-axis from the canvas.
         * */
        public static int GetYAxis(Course course, List<CoursePoint> coursePoints, int ycoor)
        {
            CoursePoint coursePoint = null;

            // Get the y-axis of the existing course
            //
            foreach (CoursePoint cPoint in coursePoints)
            {
                if (course.id == cPoint.mCourse.id)
                {
                    coursePoint = cPoint;
                    break;
                }
                ycoor = cPoint.mYAxis;
            }

            if (coursePoint != null)
            {
                // If the course exists, hold on to its y-axis
                //
                ycoor = coursePoint.mYAxis;
            }
            else
            {
                // If the course does not exist, take the next available
                //
                ycoor += CommonInternals.BUTTONSTART_Y;
                coursePoints.Add(new CoursePoint(ycoor, course));
            }

            return ycoor;
        }

        public static string ResolveCourseRemarksString(string remarks)
        {
            string remarksString = "";
            List<string> result = remarks.Split(';').ToList();

            if (result.Count != 0)
            {
                // Let us add the pre-req courses
                //
                foreach(string remark in result)
                {
                    remarksString += "* " + remark.Trim('"') + "<br/>";
                }
            }
            return remarksString;
        }

        /**
         * Handles the drawing of career path when a course is selected from the selection.
         * 
         * @param topCourse - The selected course.
         * @param dbConnection - The database connection.
         * 
         * @return The JSON string object.
         */
        public static StringBuilder DrawCourseButtons(Course topCourse,
                                                      DatabaseConnection dbConnection)
        {
            // JSON Object to wrap all information
            //
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));
            List<string> existingCourses = new List<string>();
            List<CoursePoint>[] listOfCoursePoints = new List<CoursePoint>[CommonInternals.TOTAL_YEARS];

            if (topCourse != null)
            {
                List<Course> relatedPreReq = dbConnection.GenerateAllRelatedPreRequisites(topCourse);

                // Init --->
                //
                for (int i = 0; i < listOfCoursePoints.Length; i++)
                {
                    if (null == listOfCoursePoints[i])
                    {
                        listOfCoursePoints[i] = new List<CoursePoint>();
                    }
                }

                jw.Formatting = Formatting.Indented;
                jw.WriteStartObject();
                jw.WritePropertyName("Courses");
                jw.WriteStartArray();
                // Init <---
                //

                foreach (Course preReq in relatedPreReq)
                {
                    int xcoor = 0;
                    int ycoor1 = CommonInternals.BUTTONSTART_Y;
                    int ycoor2 = CommonInternals.BUTTONSTART_Y2;
                    int ycoor3 = CommonInternals.BUTTONSTART_Y3;
                    int ycoor4 = CommonInternals.BUTTONSTART_Y4;

                    // Top course
                    //
                    List<Course> courseReq = dbConnection.GetPrerequisiteCourses(preReq.id);
                    if (topCourse.id == preReq.id)
                    {
                        jw.WriteStartObject();
                        jw.WritePropertyName("year");
                        jw.WriteValue(preReq.year);
                        jw.WritePropertyName("sem");
                        jw.WriteValue(preReq.sem);
                        jw.WritePropertyName("id");
                        jw.WriteValue(preReq.id);
                        jw.WritePropertyName("name");
                        jw.WriteValue(preReq.name);
                        jw.WritePropertyName("description");
                        jw.WriteValue(preReq.description);
                        jw.WritePropertyName("academicOrg");
                        jw.WriteValue(preReq.academicOrg);
                        jw.WritePropertyName("academicGroup");
                        jw.WriteValue(preReq.academicGroup);
                        jw.WritePropertyName("courseComp");
                        jw.WriteValue(preReq.courseComp);
                        jw.WritePropertyName("gradingBasis");
                        jw.WriteValue(preReq.gradingBasis);
                        jw.WritePropertyName("typeOffered");
                        jw.WriteValue(preReq.typeOffered);
                        jw.WritePropertyName("remarks");
                        jw.WriteValue(Utils.ResolveCourseRemarksString(preReq.remarks));
                        jw.WritePropertyName("preReqString");
                        jw.WriteValue(Utils.GetPreReqDisplayString(courseReq));
                        jw.WritePropertyName("restrString");
                        jw.WriteValue(Utils.GetRestrictionDisplayString(dbConnection, preReq));
                        jw.WritePropertyName("points");
                        jw.WriteValue(preReq.points);
                        jw.WritePropertyName("xcoor");
                        jw.WriteValue(xcoor);
                        jw.WritePropertyName("ycoor");
                        if (preReq.year == 1)
                        {
                            jw.WriteValue(GetYAxis(preReq, 
                                                    listOfCoursePoints[(int)CommonInternals.CourseYear.FIRST_YEAR], 
                                                    ycoor1));
                        }
                        else if (preReq.year == 2)
                        {
                            jw.WriteValue(GetYAxis(preReq, 
                                                    listOfCoursePoints[(int)CommonInternals.CourseYear.SECOND_YEAR], 
                                                    ycoor2));
                        }
                        else if (preReq.year == 3)
                        {
                            jw.WriteValue(GetYAxis(preReq, 
                                                    listOfCoursePoints[(int)CommonInternals.CourseYear.THIRD_YEAR], 
                                                    ycoor3));
                        }
                        else if (preReq.year == 4)
                        {
                            jw.WriteValue(GetYAxis(preReq, 
                                                    listOfCoursePoints[(int)CommonInternals.CourseYear.FOURTH_YEAR], 
                                                    ycoor4));
                        }

                        jw.WritePropertyName("top");
                        jw.WriteValue("[this]");
                        jw.WritePropertyName("compulsory");
                        jw.WriteValue(preReq.compulsory);
                        jw.WriteEndObject();
                    }

                    // Pre-requisites
                    //
                    ycoor1 = CommonInternals.BUTTONSTART_Y;
                    ycoor2 = CommonInternals.BUTTONSTART_Y2;
                    ycoor3 = CommonInternals.BUTTONSTART_Y3;
                    ycoor4 = CommonInternals.BUTTONSTART_Y4;
                    foreach (Course eachCoureReq in courseReq)
                    {
                        jw.WriteStartObject();
                        jw.WritePropertyName("year");
                        jw.WriteValue(eachCoureReq.year);
                        jw.WritePropertyName("sem");
                        jw.WriteValue(eachCoureReq.sem);
                        jw.WritePropertyName("id");
                        jw.WriteValue(eachCoureReq.id);
                        jw.WritePropertyName("name");
                        jw.WriteValue(eachCoureReq.name);
                        jw.WritePropertyName("description");
                        jw.WriteValue(eachCoureReq.description);
                        jw.WritePropertyName("academicOrg");
                        jw.WriteValue(eachCoureReq.academicOrg);
                        jw.WritePropertyName("academicGroup");
                        jw.WriteValue(eachCoureReq.academicGroup);
                        jw.WritePropertyName("courseComp");
                        jw.WriteValue(eachCoureReq.courseComp);
                        jw.WritePropertyName("gradingBasis");
                        jw.WriteValue(eachCoureReq.gradingBasis);
                        jw.WritePropertyName("typeOffered");
                        jw.WriteValue(eachCoureReq.typeOffered);
                        jw.WritePropertyName("remarks");
                        jw.WriteValue(Utils.ResolveCourseRemarksString(eachCoureReq.remarks));
                        jw.WritePropertyName("preReqString");
                        jw.WriteValue(Utils.GetPreReqDisplayString(dbConnection.GetPrerequisiteCourses(eachCoureReq.id)));
                        jw.WritePropertyName("restrString");
                        jw.WriteValue(Utils.GetRestrictionDisplayString(dbConnection, eachCoureReq));
                        jw.WritePropertyName("points");
                        jw.WriteValue(eachCoureReq.points);
                        jw.WritePropertyName("xcoor");
                        jw.WriteValue(xcoor);
                        jw.WritePropertyName("ycoor");
                        if (eachCoureReq.year == 1)
                        {
                            jw.WriteValue(GetYAxis(eachCoureReq, 
                                                   listOfCoursePoints[(int)CommonInternals.CourseYear.FIRST_YEAR], 
                                                   ycoor1));
                        }
                        else if (eachCoureReq.year == 2)
                        {
                            jw.WriteValue(GetYAxis(eachCoureReq, 
                                                   listOfCoursePoints[(int)CommonInternals.CourseYear.SECOND_YEAR], 
                                                   ycoor2));
                        }
                        else if (eachCoureReq.year == 3)
                        {
                            jw.WriteValue(GetYAxis(eachCoureReq, 
                                                   listOfCoursePoints[(int)CommonInternals.CourseYear.THIRD_YEAR], 
                                                   ycoor3));
                        }
                        else if (eachCoureReq.year == 4)
                        {
                            jw.WriteValue(GetYAxis(eachCoureReq, 
                                                   listOfCoursePoints[(int)CommonInternals.CourseYear.FOURTH_YEAR], 
                                                   ycoor4));
                        }
                        jw.WritePropertyName("top");
                        jw.WriteValue(preReq.id);
                        jw.WritePropertyName("compulsory");
                        jw.WriteValue(preReq.compulsory);
                        jw.WriteEndObject();
                    }
                }

                jw.WriteEndArray();
                jw.WriteEndObject();
            }

            return sb;
        }
    }
}