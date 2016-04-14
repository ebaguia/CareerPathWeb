using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace WebApplicationForms.Controller
{
    public class DatabaseConnection
    {
        private String dbConnectioName = null;
        private SQLiteConnection dbConnection = null;
        private SQLiteCommand sqlCommand = null;
        private SQLiteDataReader dataReader = null;

        public DatabaseConnection()
        {
        }

        public DatabaseConnection(string _connectionString)
        {
            dbConnectioName = _connectionString;
        }

        private void openDatabase(List<Career> careers = null)
        {
            try
            {
                dbConnectioName = "Data Source=" + System.Environment.CurrentDirectory + "\\" + CommonInternals.DB_NAME + ";Version=3;New=False;Compress=False;Read Only=True";
                //dbConnectioName = "Data Source=C:\\inetpub\\wwwroot\\CareerPathWebDBService" + "\\" + CommonInternals.DB_NAME +";Version=3;New=False;Compress=False;Read Only=True";
                dbConnection = new SQLiteConnection(dbConnectioName);
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void closeDatabase()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Course createCourse(SQLiteDataReader dataReader)
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
                throw ex;
            }

            return course;
        }

        public void executeQuery(string queryStatement)
        {
            try
            {
                openDatabase();
                sqlCommand = dbConnection.CreateCommand();
                sqlCommand.CommandText = queryStatement;
                sqlCommand.ExecuteNonQuery();
                closeDatabase();
            }
            catch (Exception ex)
            {
                // TODO: Logger
            }
        }

        public List<Career> readCareers()
        {
            string readCareerTable = "SELECT * FROM Career";
            List<Career> careers = new List<Career>();

            try
            {
                openDatabase(careers);
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
                            finalCourse = createCourse(dataReaderTemp2);

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
                                        Course preReq = createCourse(dataReaderTemp5);

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
                closeDatabase();
            }
            catch (Exception ex)
            {
                careers.Add(new Career("Exception",
                    ex.Data.ToString(),
                    ex.Data.ToString(),
                    null,
                    null));
            }

            return careers;
        }

        public List<String> readCareerJobs(Career career)
        {
            List<String> jobs = new List<string>();
            String readJobsTable = "SELECT NAME FROM Job WHERE CAREERID ='" + career.id + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readJobsTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    String job = dataReader["NAME"].ToString();
                    jobs.Add(job);
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return jobs;
        }

        public List<Course> readCareerFinalCourses(Career career)
        {
            List<Course> courses = new List<Course>();
            String readJobsTable = "SELECT COURSEID FROM FinalCourse WHERE CAREERID ='" + career.id + "'";

            try
            {
                openDatabase();
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
                        finalCourse = createCourse(dataReaderTemp);
                    }
                    if (finalCourse != null)
                    {
                        courses.Add(finalCourse);
                    }
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return courses;
        }

        public List<Programme> readProgrammes()
        {
            List<Programme> programmes = new List<Programme>();
            string readProgrammeStatement = "SELECT * FROM Programme";

            try
            {
                openDatabase();
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
                            Course cseCourse = createCourse(dataReaderCourse);

                            courses.Add(cseCourse);
                        }
                    }

                    Programme programme = new Programme(dataReader["ID"].ToString(),
                                                        dataReader["NAME"].ToString(),
                                                        dataReader["DESC"].ToString(),
                                                        courses);
                    programmes.Add(programme);
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return programmes;
        }

        public String readProgrammeName(String programmeId)
        {
            String progName = null;
            String readProgrammeStatement = "SELECT NAME FROM Programme WHERE ID = '" + programmeId + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readProgrammeStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();
                progName = dataReader.Read() ? dataReader.GetString(0) : "";
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return progName;
        }

        public String readProgrammeDescription(String programmeId)
        {
            String progName = null;
            String readProgrammeStatement = "SELECT DESC FROM Programme WHERE ID = '" + programmeId + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readProgrammeStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();
                progName = dataReader.Read() ? dataReader.GetString(0) : "";
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return progName;
        }

        public void readCourses(List<Course> courses)
        {
            String readCourseTable = "SELECT * FROM Course";

            courses.Clear();

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readCourseTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    Course course = createCourse(dataReader);
                    courses.Add(course);
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }
        }

        public Course getCourse(string courseId)
        {
            Course course = null;
            String readCourseTable = "SELECT * FROM Course WHERE ID  = '" + courseId + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readCourseTable, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    course = createCourse(dataReader);

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
                                Course preReq = createCourse(dataReaderTemp5);

                                course.preReqCourses.Add(preReq);
                            }
                        }
                    }
                }

                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return course;
        }

        public List<Course> generateAllRelatedPreRequisites(Course course)
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
                openDatabase();
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
                        preReq = createCourse(dataReaderTemp);

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
                                    Course preReqTemp = createCourse(dataReaderTemp2);

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
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            preRequisiteCourses.Add(course);
            preRequisiteCourses.Sort((x, y) => y.year.CompareTo(x.year));
            return preRequisiteCourses;
        }

        public List<Course> getPrerequisiteCourses(String courseId)
        {
            List<Course> preRequisiteCourses = new List<Course>();
            String readPreReqs = "SELECT * FROM Course WHERE ID = '" + courseId + "'";
            Course targetCourse = null;

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readPreReqs, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    targetCourse = createCourse(dataReader);
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
                            Course preReq = createCourse(dataReaderTemp);

                            preRequisiteCourses.Add(preReq);
                        }
                    }
                }
                closeDatabase();
            }
            catch (Exception ex)
            {
                // TODO: Logger
            }

            preRequisiteCourses.Sort((x, y) => y.year.CompareTo(x.year));
            return preRequisiteCourses;
        }

        public List<Course> readCoursesFromProgramme(Programme programme)
        {
            List<Course> courses = new List<Course>();
            String readCSECoursesStatement = "SELECT COURSEID FROM CourseProgrammePart WHERE PROGRAMMEID = '" + programme.id + "'";

            try
            {
                openDatabase();
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
                        Course cseCourse = createCourse(dataReaderTemp);

                        courses.Add(cseCourse);
                    }
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return courses;
        }

        public List<Course> readCoursesUsingProgrammePart(string programmeId, string part)
        {
            List<Course> courses = new List<Course>();
            String readCoursesStatement = "SELECT COURSEID FROM CourseProgrammePart WHERE PROGRAMMEID = '" + programmeId + "' AND PART = '" + part + "'";

            try
            {
                openDatabase();
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
                        Course cseCourse = createCourse(dataReaderTemp);

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
                                    Course preReq = createCourse(dataReaderTemp5);

                                    cseCourse.preReqCourses.Add(preReq);
                                }
                            }
                        }

                        courses.Add(cseCourse);
                    }
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return courses;
        }

        public List<CourseProgrammePart> readCourseProgrammePart(Programme programme, string part)
        {
            List<CourseProgrammePart> courseProgrammePartList = new List<CourseProgrammePart>();
            String readCSECoursesStatement = "SELECT * FROM CourseProgrammePart WHERE PROGRAMMEID = '" + programme.id + "' WHERE part = '" +  part + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readCSECoursesStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    CourseProgrammePart courseProgrammePart = new CourseProgrammePart(dataReader["COURSEID"].ToString(), dataReader["PART"].ToString(), dataReader["PROGRAMMEID"].ToString());
                    courseProgrammePartList.Add(courseProgrammePart);
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return courseProgrammePartList;
        }

        public List<string> readProgrammeParts(Programme programme)
        {
            List<string> programmeParts = new List<string>();
            String readCSECoursesStatement = "SELECT part AS part FROM CourseProgrammePart GROUP BY part";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(readCSECoursesStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    programmeParts.Add(dataReader["PART"].ToString());
                }
                closeDatabase();
            }
            catch (Exception e)
            {
                // TODO: Logger
            }

            return programmeParts;
        }

        public List<Course> getRestrictionCourses(String courseId)
        {
            List<Course> restrictionList = new List<Course>();
            String readPreReqs = "SELECT RESTRCOURSEID FROM Restrictions WHERE COURSEID = '" + courseId + "'";

            try
            {
                openDatabase();
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
                        Course restrCourse = createCourse(dataReaderTemp);

                        restrictionList.Add(restrCourse);
                    }
                }
                closeDatabase();
            }
            catch (Exception ex)
            {
                // TODO: Logger
            }

            return restrictionList;
        }

        public bool isCourseCompulsory(Course targetCourse)
        {
            bool isCompulsory = false;

            String queryStatement = "SELECT * FROM CompulsoryCourse WHERE COURSEID = '" + targetCourse.id + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(queryStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();
                isCompulsory = dataReader.Read() ? true : false;
                closeDatabase();
            }
            catch (Exception ex)
            {
                // TODO: Logger
            }

            return isCompulsory;
        }

        public bool isCourseElective(Course targetCourse)
        {
            bool isElective = false;

            String queryStatement = "SELECT * FROM ElectiveCourse WHERE COURSEID = '" + targetCourse.id + "'";

            try
            {
                openDatabase();
                sqlCommand = new SQLiteCommand(queryStatement, dbConnection);
                dataReader = sqlCommand.ExecuteReader();
                isElective = dataReader.Read() ? true : false;
                closeDatabase();
            }
            catch (Exception ex)
            {
                // TODO: Logger
            }

            return isElective;
        }
    }
}
