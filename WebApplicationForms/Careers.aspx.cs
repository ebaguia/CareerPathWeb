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
using WebApplicationForms.Controller;

namespace WebApplicationForms
{
    public partial class Careers : System.Web.UI.Page
    {
        private static DatabaseConnection mDBConnection = new DatabaseConnection();
        private static List<Career> mECECareers = null;
        private static int mButtonStartY = 90;
        private static List<string> mExistingCourses = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
            else
            {
                // This is called when the treeview item is re-selected
                //
                TreeViewCareers_SelectedNodeChanged(sender, e);
            }
        }

        private void BindData()
        {
            mECECareers = mDBConnection.readCareers();
            PopulateTreeViewControl(mECECareers);
        }

        private void PopulateTreeViewControl(List<Career> careerList)
        {
            TreeNode parentNode = null;

            // Populating career list
            //
            foreach (Career career in careerList)
            {
                parentNode = new TreeNode(career.name);

                var jobString = "";

                foreach (string job in career.jobs)
                {
                    jobString += job + ", ";
                }

                // Adding tooltip; this will show the different jobs in this career
                //
                parentNode.ToolTip = jobString.Remove(jobString.Length-2);

                List<Course> finalCoursesToTake = mDBConnection.readCareerFinalCourses(career);

                parentNode.ChildNodes.Clear();
                foreach (Course course in finalCoursesToTake)
                {
                    // Format: "- <COURSE ID>"
                    //
                    TreeNode childNode = new TreeNode(course.id, course.name);
                    parentNode.ChildNodes.Add(childNode);
                }

                parentNode.Collapse();
                treeViewCareers.Nodes.Add(parentNode);
            }
        }

        protected void TreeViewCareers_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode courseNode = treeViewCareers.SelectedNode;

            // Retrieving the path of the selected course
            //
            string courseId = courseNode.Text.Replace("-", string.Empty);
            Course course = mDBConnection.getCourse(courseId.Trim());
            if(course != null)
            {
                List<Course> relatedPreReq = mDBConnection.generateAllRelatedPreRequisites(course);

                // JSON Object to wrap all information
                //
                StringBuilder sb = new StringBuilder();
                JsonWriter jw = new JsonTextWriter(new StringWriter(sb));

                jw.Formatting = Formatting.Indented;
                jw.WriteStartObject();
                jw.WritePropertyName("Courses");
                jw.WriteStartArray();

                int xcoor = 0;
                int ycoor1 = 0;
                int ycoor2 = 0;
                int ycoor3 = 0;
                int ycoor4 = 0;

                // Getting all pre-requisites of each path
                //
                mExistingCourses.Clear();
                foreach (Course preReq in relatedPreReq)
                {
                    // Top course
                    //
                    List<Course> courseReq = mDBConnection.getPrerequisiteCourses(preReq.id);
                    if (course.id == preReq.id)
                    {
                        mExistingCourses.Add(preReq.id);

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
                        jw.WritePropertyName("points");
                        jw.WriteValue(preReq.points);
                        jw.WritePropertyName("xcoor");
                        jw.WriteValue(xcoor);
                        jw.WritePropertyName("ycoor");
                        if (preReq.year == 1)
                        {
                            ycoor1 += mButtonStartY;
                            jw.WriteValue(ycoor1);
                        }
                        else if (preReq.year == 2)
                        {
                            ycoor2 += mButtonStartY;
                            jw.WriteValue(ycoor2);
                        }
                        else if (preReq.year == 3)
                        {
                            ycoor3 += mButtonStartY;
                            jw.WriteValue(ycoor3);
                        }
                        else if (preReq.year == 4)
                        {
                            ycoor4 += mButtonStartY;
                            jw.WriteValue(ycoor4);
                        }

                        jw.WritePropertyName("top");
                        jw.WriteValue("[this]");
                        jw.WritePropertyName("compulsory");
                        jw.WriteValue(preReq.compulsory);
                        jw.WriteEndObject();
                    }

                    // Pre-requisites
                    //
                    foreach(Course eachCoureReq in courseReq)
                    {
                        mExistingCourses.Add(eachCoureReq.id);

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
                        jw.WritePropertyName("points");
                        jw.WriteValue(eachCoureReq.points);
                        jw.WritePropertyName("xcoor");
                        jw.WriteValue(xcoor);
                        jw.WritePropertyName("ycoor");
                        if (eachCoureReq.year == 1)
                        {
                            ycoor1 += mButtonStartY;
                            jw.WriteValue(ycoor1);
                        }
                        else if (eachCoureReq.year == 2)
                        {
                            ycoor2 += mButtonStartY;
                            jw.WriteValue(ycoor2);
                        }
                        else if (eachCoureReq.year == 3)
                        {
                            ycoor3 += mButtonStartY;
                            jw.WriteValue(ycoor3);
                        }
                        else if (eachCoureReq.year == 4)
                        {
                            ycoor4 += mButtonStartY;
                            jw.WriteValue(ycoor4);
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

                // Send all information to our javascript at once
                //
                // Data is being wrapped in a JSON object
                //
                ScriptManager.RegisterClientScriptBlock(this, 
                    GetType(), 
                    "coursepath_key",
                    "drawSomething(" + sb.ToString() + ");", 
                    true);
            }
            else
            {
                // When career item is selected instead of the course within it
                //
                foreach (var career in mECECareers)
                {
                    if (career.name == courseNode.Text)
                    {
                        if (!(bool)courseNode.Expanded)
                        {
                            courseNode.Expand();
                        }
                        /*else
                        {
                            courseNode.Collapse();
                        }*/
                        break;
                    }
                }
            }
        }

        private bool IsCourseExist(string courseId)
        {
            bool isExist = false;
            foreach(string id in mExistingCourses)
            {
                if (id == courseId)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }
    }
}