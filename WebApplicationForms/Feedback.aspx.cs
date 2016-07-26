/*
 *
 * Version:
 *     $Id$
 *
 * Revisions:
 *     $Log$
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationForms
{

    /// <summary>
    /// Class representing the Feedback tab
    /// 
    /// <list type="bullet">
    /// 
    /// <item>
    /// <term>Author</term>
    /// <description>Emmanuel Baguia</description>
    /// </item>
    /// 
    /// </list>
    /// 
    /// </summary>
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Configures the mail to be delivered
        /// </summary>
        /// <param name="void"></param>
        private void EmailFeedback()
        {
            try
            {
                // Create the mail message
                //
                MailMessage mail = new MailMessage();

                // Set the addresses
                //
                mail.From = new MailAddress("e.baguia@auckland.ac.nz");
                mail.To.Add("careerpathuoa@gmail.com");

                // Set the content
                //
                mail.Subject = "Feedback Form";

                // Body to be displayed
                //
                mail.Body = "<h2>" + "From: " + " " + txtName.Text + "</h2>" + "<br><br>" +
                        "Email : " + txtEmail.Text.Trim() + "<br>" +
                        "Comments: " + txtComments.Text.Trim() + "<br>" +
                        "Thank You";

                mail.IsBodyHtml = true;

                // SMTP settings
                //
                var smtp = new SmtpClient
                {
                    Host = "mailhost.auckland.ac.nz",
                    Port = 25,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("e.baguia@auckland.ac.nz", "Myjoan06")
                };

                smtp.Send(mail);
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Removes all texts
        /// </summary>
        /// <param name="void"></param>
        private void Clear()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtComments.Text = string.Empty;
        }

        /// <summary>
        /// Handles the "Send" button in the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SubmitComments(object sender, EventArgs e)
        {
            try
            {
                EmailFeedback();
                lblMsg.ForeColor = System.Drawing.Color.Green;
                Clear();
                lblMsg.Text = "Your comments are greatly appreciated. Thank you.";
            }
            catch(Exception ex)
            {
                lblMsg.Text = ex.InnerException.Message;
            }
        }
    }
}