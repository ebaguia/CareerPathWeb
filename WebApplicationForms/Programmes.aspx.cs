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
            // Populating programme list
            //
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
                    
                    //if (programmePart != lastItem)
                    //{
                        TreeNode partNode = new TreeNode(programmePart, programmePart);
                        programmeNode.ChildNodes.Add(partNode);

                        // List all courses under each part of the programme
                        //
                        List<Course> programmeCourses = mDBConnection.readCoursesUsingProgrammePart(programme.id, programmePart);
                        foreach(Course course in programmeCourses)
                        {
                            // Format: "- <COURSE ID>"
                            //
                            TreeNode courseNode = new TreeNode(course.id, course.name);
                            partNode.ChildNodes.Add(courseNode);
                        }

                        partNode.Collapse();
                    //}
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
    }
}