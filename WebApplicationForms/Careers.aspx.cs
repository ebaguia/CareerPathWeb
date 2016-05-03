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
            // Populating career list
            //
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
                // Send all information to our javascript at once
                //
                // Data is being wrapped in a JSON object
                //
                ScriptManager.RegisterClientScriptBlock(this,
                    GetType(),
                    "coursepath_key",
                    "drawSomething(" + Utils.DrawCourseButtons(course,mDBConnection).ToString() + ");",
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
    }
}