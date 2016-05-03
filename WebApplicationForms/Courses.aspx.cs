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
using WebApplicationForms.Models;
using WebApplicationForms.Controller;

namespace WebApplicationForms
{
    public partial class Courses : System.Web.UI.Page
    {
        private static DatabaseConnection mDBConnection = new DatabaseConnection();
        private static List<Programme> mProgrammeList = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
            else
            {
                TreeViewCourses_SelectedNodeChanged(sender, e);
            }
        }

        private void BindData()
        {
            // Populating course list
            //
            mProgrammeList = mDBConnection.readProgrammes();
            PopulateTreeViewControl();
        }

        private void PopulateTreeViewControl()
        {
            TreeNode programmeNode = null;

            foreach (Programme programme in mProgrammeList)
            {
                programmeNode = new TreeNode(programme.id);
                programmeNode.ToolTip = programme.name;

                // List all courses each programme
                //
                programmeNode.ChildNodes.Clear();
                List<Course> programmeCourses = mDBConnection.readCoursesFromProgramme(programme);
                foreach (Course course in programmeCourses)
                {
                    TreeNode courseNode = new TreeNode(/*"- " +*/ course.id, course.id);
                    programmeNode.ChildNodes.Add(courseNode);
                }

                programmeNode.Collapse();

                treeViewCourses.Nodes.Add(programmeNode);
            }
        }

        protected void TreeViewCourses_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode courseNode = treeViewCourses.SelectedNode;

            // Retrieving the path of the selected course
            //
            string courseId = courseNode.Text.Replace("-", string.Empty);
            Course course = mDBConnection.getCourse(courseId.Trim());
            if (course != null)
            {
                // Send all information to our javascript at once
                //
                // Data is being wrapped in a JSON object
                //
                ScriptManager.RegisterClientScriptBlock(this,
                    GetType(),
                    "coursepath_key",
                    "drawSomething(" + Utils.DrawCourseButtons(course, mDBConnection).ToString() + ");",
                    true);
            }
            else
            {
                // When career item is selected instead of the course within it
                //
                foreach (var programme in mProgrammeList)
                {
                    if (programme.id == courseNode.Text)
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
    }
}