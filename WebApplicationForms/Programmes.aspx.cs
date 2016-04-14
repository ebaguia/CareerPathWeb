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
    public partial class Programmes : System.Web.UI.Page
    {
        private static DatabaseConnection mDBConnection = new DatabaseConnection();
        private static List<Programme> mECEProgrammes = null;
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
                TreeViewProgrammes_SelectedNodeChanged(sender, e);
            }
        }

        private void BindData()
        {
            mECEProgrammes = mDBConnection.readProgrammes();
            PopulateTreeViewControl(mECEProgrammes);
        }

        private void PopulateTreeViewControl(List<Programme> programmeList)
        {
            TreeNode programmeNode = null;

            // Populating career list
            //
            foreach (Programme programme in programmeList)
            {
                programmeNode = new TreeNode(programme.name);
                programmeNode.ChildNodes.Clear();

                // List all parts (e.g. I, II, III) under each programme
                //
                List<string> programmeParts = mDBConnection.readProgrammeParts(programme);
                string lastItem = programmeParts[programmeParts.Count - 1];
                foreach(string programmePart in programmeParts)
                {
                    if (programmePart != lastItem)
                    {
                        TreeNode partNode = new TreeNode(programmePart, programmePart);
                        programmeNode.ChildNodes.Add(partNode);

                        // List all courses under each part of the programme
                        //
                        List<Course> programmeCourses = mDBConnection.readCoursesUsingProgrammePart(programme.id, programmePart);
                        foreach(Course course in programmeCourses)
                        {
                            // Format: "- <COURSE ID>"
                            //
                            TreeNode courseNode = new TreeNode(/*"- " +*/ course.id, course.name);
                            partNode.ChildNodes.Add(courseNode);
                        }

                        partNode.Collapse();
                    }
                }

                programmeNode.Collapse();

                treeViewProgrammes.Nodes.Add(programmeNode);
            }
        }

        protected void TreeViewProgrammes_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode courseNode = treeViewProgrammes.SelectedNode;

            // Retrieving the path of the selected course
            //
            string courseId = courseNode.Text.Replace("-", string.Empty);
            Course course = mDBConnection.getCourse(courseId.Trim());
            if (course != null)
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
                    foreach (Course eachCoureReq in courseReq)
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
                // When programme item is selected instead of the course within it
                //
                foreach (var programme in mECEProgrammes)
                {
                    if (programme.name == courseNode.Text)
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
            foreach (string id in mExistingCourses)
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