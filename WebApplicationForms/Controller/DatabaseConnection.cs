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
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using WebApplicationForms.Helper;

namespace WebApplicationForms.Controller
{
    /// <summary>
    /// Database API
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
    public class DatabaseConnection
    {
        private String dbConnectioName = null;          // database name with path
        private SQLiteConnection dbConnection = null;   // database connection object
        private SQLiteCommand sqlCommand = null;        // SQL command object
        private SQLiteDataReader dataReader = null;     // SQL Data Reader object

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="void"></param>
        public DatabaseConnection()
        {
        }

        /// <summary>
        /// Constructor which accepts a database name with path
        /// </summary>
        /// <param name="connectionString">The location of the database</param>
        public DatabaseConnection(string connectionString)
        {
            dbConnectioName = connectionString;
        }

        /// <summary>
        /// Creates a database connection
        /// </summary>
        /// <param name="void"></param>
        private void OpenDatabase()
        {
            try
            {
                dbConnectioName = "Data Source=" + System.Environment.CurrentDirectory + "\\" + CommonInternals.DB_NAME + ";Version=3;New=False;Compress=False;Read Only=True";

                // Deployment location: C:\windows\system32\inetsrv
                // Development location: C:\Program Files (x86)\IIS Express
                //
                // Do we need to move it to a common place??? If yes, below is the suggested location.
                //
                // dbConnectioName = "Data Source=C:\\inetpub\\wwwroot\\CareerPath\\DB" + "\\" + CommonInternals.DB_NAME +";Version=3;New=False;Compress=False;Read Only=True";
                //

                dbConnection = new SQLiteConnection(dbConnectioName);
                dbConnection.Open();
                CareerPathLogger.Info("DatabaseConnection::DatabaseConnection():dbConnectioName = " + dbConnectioName);
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::openDatabase() " + ex.Message);
            }
        }

        /// <summary>
        /// Closes a database connection
        /// </summary>
        /// <param name="void"></param>
        private void CloseDatabase()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::closeDatabase() " + ex.Message);
            }
        }

        /// <summary>
        /// A helper function wich initializes a Course object
        /// </summary>
        /// <param name="dataReader">The resulting object which contains the course information from an SQL query</param>
        private Course CreateCourse(SQLiteDataReader dataReader)
        {
            Course course = null;

            try
            {
                course = new Course(dataReader["ID"].ToString(),
                        (int)dataReader["YR"],
                        (int)dataReader["SEM"],
                        dataReader["NAME"].ToString(),
                        dataReader["DESC"].ToString(),
                        (int)dataReader["POINTS"],
                        dataReader["ACADEMICORG"].ToString(),
                        dataReader["ACADEMICGROUP"].ToString(),
                        dataReader["COURSECOMP"].ToString(),
                        dataReader["GRADINGBASIS"].ToString(),
                        dataReader["TYPOFFERED"].ToString(),
                        dataReader["REMARKS"].ToString(),
                        dataReader["CAREERID"].ToString(),
                        (int)dataReader["COMPULSORY"]);
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::createCourse() " + ex.Message);
            }

            return course;
        }

        /// <summary>
        /// Retrieves all careers from the database
        /// </summary>
        /// <param name="void"></param>
        public List<Career> ReadCareers()
        {
            string readCareerTable = "SELECT * FROM Career";
            List<Career> careers = new List<Career>();

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readCareerTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();
                
                while (dataReader.Read())
                {
                    // Retrieve all jobs under this career
                    //
                    string readJobsStatement = "SELECT NAME FROM Job WHERE CAREERID = '" + dataReader["ID"].ToString() + "'";
                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readJobsStatement, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();
                    List<string> jobs = new List<string>();
                    while (dataReaderTemp.Read())
                    {
                        jobs.Add(dataReaderTemp["NAME"].ToString());
                    }

                    // Retrieve all final courses under this career
                    //
                    string readFinalCourseStatement = "SELECT COURSEID FROM FinalCourse WHERE CAREERID ='" + dataReader["ID"].ToString() + "'";
                    SQLiteCommand sqlCommandTemp1 = new SQLiteCommand(readFinalCourseStatement, dbConnection);
                    SQLiteDataReader dataReaderTemp1 = sqlCommandTemp1.ExecuteReader();
                    List<Course> finalCourses = new List<Course>();
                    while (dataReaderTemp1.Read())
                    {
                        string readCourseStatement = "SELECT * FROM Course WHERE ID ='" + dataReaderTemp1["COURSEID"].ToString() + "'";
                        SQLiteCommand sqlCommandTemp2 = new SQLiteCommand(readCourseStatement, dbConnection);
                        SQLiteDataReader dataReaderTemp2 = sqlCommandTemp2.ExecuteReader();

                        Course finalCourse = null;
                        while (dataReaderTemp2.Read())
                        {
                            finalCourse = CreateCourse(dataReaderTemp2);

                            // Retrieve all pre req courses
                            //
                            if (finalCourse != null)
                            {
                                String readPreReqInfo = "SELECT * FROM PreRequisite WHERE FOLLOWID = '" + finalCourse.id + "'";
                                SQLiteCommand sqlCommandTemp4 = new SQLiteCommand(readPreReqInfo, dbConnection);
                                SQLiteDataReader dataReaderTemp4 = sqlCommandTemp4.ExecuteReader();

                                while (dataReaderTemp4.Read())
                                {
                                    String preReqCourseId = dataReaderTemp4["COURSEID"].ToString();
                                    String readPreReqCourse = "SELECT * FROM Course WHERE ID = '" + preReqCourseId + "'";
                                    SQLiteCommand sqlCommandTemp5 = new SQLiteCommand(readPreReqCourse, dbConnection);
                                    SQLiteDataReader dataReaderTemp5 = sqlCommandTemp5.ExecuteReader();

                                    while (dataReaderTemp5.Read())
                                    {
                                        Course preReq = CreateCourse(dataReaderTemp5);

                                        finalCourse.preReqCourses.Add(preReq);
                                    }
                                }
                            }
                        }
                        if (finalCourse != null)
                        {
                            finalCourses.Add(finalCourse);
                        }
                    }

                    Career career = new Career(dataReader["ID"].ToString(),
                                               dataReader["NAME"].ToString(),
                                               dataReader["DESC"].ToString(),
                                               jobs,
                                               finalCourses);
                    careers.Add(career);
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readCareers() " + ex.Message);
            }

            return careers;
        }

        public List<Course> ReadCareerFinalCourses(Career career)
        {
            List<Course> courses = new List<Course>();
            String readJobsTable = "SELECT COURSEID FROM FinalCourse WHERE CAREERID ='" + career.id + "'";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readJobsTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String finalCourseId = dataReader["COURSEID"].ToString();
                    String readFinalCourse = "SELECT * FROM Course WHERE ID = '" + finalCourseId + "'";

                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readFinalCourse, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();
                    Course finalCourse = null;
                    while (dataReaderTemp.Read())
                    {
                        finalCourse = CreateCourse(dataReaderTemp);
                    }
                    if (finalCourse != null)
                    {
                        courses.Add(finalCourse);
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readCareerFinalCourses() " + ex.Message);
            }

            return courses;
        }

        public List<Programme> ReadProgrammes()
        {
            List<Programme> programmes = new List<Programme>();
            string readProgrammeStatement = "SELECT * FROM Programme";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readProgrammeStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    List<Course> courses = new List<Course>();
                    string programmeId = dataReader["ID"].ToString();
                    string readCourseFromProgramme = "SELECT COURSEID FROM CourseProgrammePart WHERE PROGRAMMEID = '" + programmeId + "'";

                    SQLiteCommand sqlCommandCourseProg = new SQLiteCommand(readCourseFromProgramme, dbConnection);
                    SQLiteDataReader dataReaderCourseProg = sqlCommandCourseProg.ExecuteReader();
                    while (dataReaderCourseProg.Read())
                    {
                        String courseId = dataReaderCourseProg["COURSEID"].ToString();
                        String readCourse = "SELECT * FROM Course WHERE ID = '" + courseId + "'";

                        SQLiteCommand sqlCommandCourse = new SQLiteCommand(readCourse, dbConnection);
                        SQLiteDataReader dataReaderCourse = sqlCommandCourse.ExecuteReader();
                        while (dataReaderCourse.Read())
                        {
                            Course cseCourse = CreateCourse(dataReaderCourse);

                            courses.Add(cseCourse);
                        }
                    }

                    Programme programme = new Programme(dataReader["ID"].ToString(),
                                                        dataReader["NAME"].ToString(),
                                                        dataReader["DESC"].ToString(),
                                                        courses);
                    programmes.Add(programme);
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readProgrammes() " + ex.Message);
            }

            return programmes;
        }

        public Course GetCourse(string courseId)
        {
            Course course = null;
            String readCourseTable = "SELECT * FROM Course WHERE ID  = '" + courseId + "'";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readCourseTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    course = CreateCourse(dataReader);

                    // Retrieve all pre req courses
                    //
                    if (course != null)
                    {
                        String readPreReqInfo = "SELECT * FROM PreRequisite WHERE FOLLOWID = '" + course.id + "'";
                        SQLiteCommand sqlCommandTemp4 = new SQLiteCommand(readPreReqInfo, dbConnection);
                        SQLiteDataReader dataReaderTemp4 = sqlCommandTemp4.ExecuteReader();

                        while (dataReaderTemp4.Read())
                        {
                            String preReqCourseId = dataReaderTemp4["COURSEID"].ToString();
                            String readPreReqCourse = "SELECT * FROM Course WHERE ID = '" + preReqCourseId + "'";
                            SQLiteCommand sqlCommandTemp5 = new SQLiteCommand(readPreReqCourse, dbConnection);
                            SQLiteDataReader dataReaderTemp5 = sqlCommandTemp5.ExecuteReader();

                            while (dataReaderTemp5.Read())
                            {
                                Course preReq = CreateCourse(dataReaderTemp5);

                                course.preReqCourses.Add(preReq);
                            }
                        }
                    }
                }

                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::getCourse() " + ex.Message);
            }

            return course;
        }

        public List<Course> GenerateAllRelatedPreRequisites(Course course)
        {
            List<Course> preRequisiteCourses = new List<Course>();
            String queryStatement = @"WITH RECURSIVE
pre_req_courses(n) AS (
VALUES('" + course.id + @"')
UNION
SELECT COURSEID FROM PreRequisite, pre_req_courses
    WHERE PreRequisite.FOLLOWID=pre_req_courses.n
)
SELECT COURSEID FROM PreRequisite
WHERE PreRequisite.FOLLOWID IN pre_req_courses";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(queryStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String preReqCourseId = dataReader["COURSEID"].ToString();
                    String readPreReqCourse = "SELECT * FROM Course WHERE ID = '" + preReqCourseId + "'";

                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readPreReqCourse, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();
                    Course preReq = null;
                    while (dataReaderTemp.Read())
                    {
                        preReq = CreateCourse(dataReaderTemp);

                        // Retrieve all pre req courses
                        //
                        if (preReq != null)
                        {
                            String readPreReqInfo = "SELECT * FROM PreRequisite WHERE FOLLOWID = '" + preReq.id + "'";
                            SQLiteCommand sqlCommandTemp1 = new SQLiteCommand(readPreReqInfo, dbConnection);
                            SQLiteDataReader dataReaderTemp1 = sqlCommandTemp1.ExecuteReader();

                            while (dataReaderTemp1.Read())
                            {
                                String preReqId = dataReaderTemp1["COURSEID"].ToString();
                                String readPreReq = "SELECT * FROM Course WHERE ID = '" + preReqId + "'";
                                SQLiteCommand sqlCommandTemp2 = new SQLiteCommand(readPreReq, dbConnection);
                                SQLiteDataReader dataReaderTemp2 = sqlCommandTemp2.ExecuteReader();

                                while (dataReaderTemp2.Read())
                                {
                                    Course preReqTemp = CreateCourse(dataReaderTemp2);

                                    preReq.preReqCourses.Add(preReqTemp);
                                }
                            }
                        }
                    }
                    if (preReq != null)
                    {
                        preRequisiteCourses.Add(preReq);
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::generateAllRelatedPreRequisites() " + ex.Message);
            }

            preRequisiteCourses.Add(course);
            preRequisiteCourses.Sort((x, y) => y.year.CompareTo(x.year));
            return preRequisiteCourses;
        }

        public List<Course> GetPrerequisiteCourses(String courseId)
        {
            List<Course> preRequisiteCourses = new List<Course>();
            String readPreReqs = "SELECT * FROM Course WHERE ID = '" + courseId + "'";
            Course targetCourse = null;

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readPreReqs, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    targetCourse = CreateCourse(dataReader);
                }

                if (targetCourse != null)
                {
                    String readPreReqInfo = "SELECT * FROM PreRequisite WHERE FOLLOWID = '" + targetCourse.id + "'";

                    sqlCommand = new SQLiteCommand(readPreReqInfo, dbConnection);
                    dataReader = sqlCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        String preReqCourseId = dataReader["COURSEID"].ToString();
                        String readPreReqCourse = "SELECT * FROM Course WHERE ID = '" + preReqCourseId + "'";

                        SQLiteCommand sqlCommandTemp = new SQLiteCommand(readPreReqCourse, dbConnection);
                        SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();

                        while (dataReaderTemp.Read())
                        {
                            Course preReq = CreateCourse(dataReaderTemp);

                            preRequisiteCourses.Add(preReq);
                        }
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::getPrerequisiteCourses() " + ex.Message);
            }

            preRequisiteCourses.Sort((x, y) => y.year.CompareTo(x.year));
            return preRequisiteCourses;
        }

        public List<Course> ReadCoursesFromProgramme(Programme programme)
        {
            List<Course> courses = new List<Course>();
            String readCSECoursesStatement = "SELECT DISTINCT COURSEID FROM CourseProgrammePart WHERE PROGRAMMEID = '" + programme.id + "' ORDER BY COURSEID";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readCSECoursesStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String cseCourseId = dataReader["COURSEID"].ToString();
                    String readCSECourse = "SELECT * FROM Course WHERE ID = '" + cseCourseId + "'";

                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readCSECourse, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();

                    while (dataReaderTemp.Read())
                    {
                        Course cseCourse = CreateCourse(dataReaderTemp);

                        courses.Add(cseCourse);
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readCoursesFromProgramme() " + ex.Message);
            }

            return courses;
        }

        public List<Course> ReadCoursesUsingProgrammePart(Programme programme, string part)
        {
            List<Course> courses = new List<Course>();
            String readCoursesStatement = "SELECT DISTINCT COURSEID FROM CourseProgrammePart WHERE PROGRAMMEPARTID = '" + programme.id + "' AND PART = '" + part + "' ORDER BY COURSEID";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readCoursesStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String cseCourseId = dataReader["COURSEID"].ToString();
                    String readCSECourse = "SELECT * FROM Course WHERE ID = '" + cseCourseId + "'";

                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readCSECourse, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();

                    while (dataReaderTemp.Read())
                    {
                        Course cseCourse = CreateCourse(dataReaderTemp);

                        // Retrieve all pre req courses
                        //
                        if (cseCourse != null)
                        {
                            String readPreReqInfo = "SELECT * FROM PreRequisite WHERE FOLLOWID = '" + cseCourse.id + "'";
                            SQLiteCommand sqlCommandTemp4 = new SQLiteCommand(readPreReqInfo, dbConnection);
                            SQLiteDataReader dataReaderTemp4 = sqlCommandTemp4.ExecuteReader();

                            while (dataReaderTemp4.Read())
                            {
                                String preReqCourseId = dataReaderTemp4["COURSEID"].ToString();
                                String readPreReqCourse = "SELECT * FROM Course WHERE ID = '" + preReqCourseId + "'";
                                SQLiteCommand sqlCommandTemp5 = new SQLiteCommand(readPreReqCourse, dbConnection);
                                SQLiteDataReader dataReaderTemp5 = sqlCommandTemp5.ExecuteReader();

                                while (dataReaderTemp5.Read())
                                {
                                    Course preReq = CreateCourse(dataReaderTemp5);

                                    cseCourse.preReqCourses.Add(preReq);
                                }
                            }
                        }

                        courses.Add(cseCourse);
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readCoursesUsingProgrammePart() " + ex.Message);
            }

            return courses;
        }

        public List<string> ReadProgrammeParts(Programme programme)
        {
            List<string> programmeParts = new List<string>();
            String readCSECoursesStatement = "SELECT part AS part FROM CourseProgrammePart GROUP BY part";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readCSECoursesStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    programmeParts.Add(dataReader["PART"].ToString());
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::readProgrammeParts() " + ex.Message);
            }

            return programmeParts;
        }

        public List<Course> GetRestrictionCourses(String courseId)
        {
            List<Course> restrictionList = new List<Course>();
            String readPreReqs = "SELECT RESTRCOURSEID FROM Restrictions WHERE COURSEID = '" + courseId + "'";

            try
            {
                OpenDatabase();
                sqlCommand = new SQLiteCommand(readPreReqs, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String restrCourseId = dataReader["RESTRCOURSEID"].ToString();
                    String readCourse = "SELECT * FROM Course WHERE ID = '" + restrCourseId + "'";

                    SQLiteCommand sqlCommandTemp = new SQLiteCommand(readCourse, dbConnection);
                    SQLiteDataReader dataReaderTemp = sqlCommandTemp.ExecuteReader();

                    while (dataReaderTemp.Read())
                    {
                        Course restrCourse = CreateCourse(dataReaderTemp);

                        restrictionList.Add(restrCourse);
                    }
                }
                CloseDatabase();
            }
            catch (Exception ex)
            {
                CareerPathLogger.Error("DatabaseConnection::getRestrictionCourses() " + ex.Message);
            }

            return restrictionList;
        }
    }
}
