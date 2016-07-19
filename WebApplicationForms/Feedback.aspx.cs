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
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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

        private void Clear()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtComments.Text = string.Empty;
        }

        protected void SubmitComments(object sender, EventArgs e)
        {
            try
            {
                EmailFeedback();
                lblMsg.ForeColor = System.Drawing.Color.Green;
                Clear();
                lblMsg.Text = "You comments are greatly appreciated. Thank you.";
            }
            catch(Exception ex)
            {
                lblMsg.Text = ex.InnerException.Message;
            }
        }
    }
}