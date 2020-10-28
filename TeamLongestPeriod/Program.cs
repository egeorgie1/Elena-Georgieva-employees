using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLongestPeriod
{
    public struct Record
    {
        public int EmpID { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public TimeSpan TestSpan => DateTo - DateFrom;

        public override string ToString() => $"{EmpID},  {ProjectID},  {DateFrom},  {DateTo},  {TestSpan}\n";
        

    }
    public class LongestPeriodCalculations
    {
        public Record[] Records { get; set; } 
        public LongestPeriodCalculations(string textfilePath)
        {
            string[] lines = System.IO.File.ReadAllLines(textfilePath);
            Records = new Record[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Record r = new Record();

                string[] data = line.Split(',');
                for (int j = 0; j < data.Length; j++)
                    data[j] = data[j].Trim();

                r.EmpID = int.Parse(data[0]);
                r.ProjectID = int.Parse(data[1]);
                r.DateFrom = DateTime.Parse(data[2]);

                if (data[3] == "NULL")
                    r.DateTo = DateTime.Now.Date;
                else
                    r.DateTo = DateTime.Parse(data[3]);

                Records[i] = r;
            }

        }

        //returns the pair of the EmpIDs of the both employees who have worked on common projects the longest time
        public KeyValuePair<int,int> TeamLongestPeriod()
        {
            List<int> employees = new List<int>();
            foreach (Record rec in Records)
                if (!employees.Contains(rec.EmpID))
                    employees.Add(rec.EmpID);
           
            TimeSpan maxPeriod = TimeSpan.Zero;
            KeyValuePair<int, int> maxPair = new KeyValuePair<int, int>();

            for (int emp1 = 0; emp1 < employees.Count; emp1++)
                for (int emp2 = emp1 + 1; emp2 < employees.Count; emp2++)
                {
                    TimeSpan currPairTeamPeriod = TotalTeamPeriod(employees[emp1], employees[emp2]);
                    if (currPairTeamPeriod > maxPeriod)
                    {
                        maxPeriod = currPairTeamPeriod;
                        maxPair = new KeyValuePair<int, int>(employees[emp1], employees[emp2]);

                    }
                }

            Console.WriteLine("Max Period: {0}", maxPeriod);
            return maxPair;
        }

      
        TimeSpan TotalTeamPeriod(int emp1, int emp2)
        {
            TimeSpan total = TimeSpan.Zero;
            List<int> projectsEmp1 = EmployeeProjects(emp1);
            List<int> projectsEmp2 = EmployeeProjects(emp2);

            var intersectionProjects = projectsEmp1.Intersect(projectsEmp2);

            foreach(int proj in intersectionProjects)
            {
                List<Record> recordsEmp1 = FindRecords(emp1, proj);
                List<Record> recordsEmp2 = FindRecords(emp2, proj);

                foreach(Record rec1 in recordsEmp1)
                    foreach(Record rec2 in recordsEmp2)
                    {
                        total += PeriodOverlap(rec1, rec2);
                    }
            }
            return total;
        }
        
        TimeSpan PeriodOverlap(Record rec1, Record rec2)
        {
            if(rec2.DateTo >= rec1.DateFrom && rec2.DateTo <= rec1.DateTo)
            {
                if (rec2.DateFrom < rec1.DateFrom)
                    return rec2.DateTo - rec1.DateFrom;
                else
                    return rec2.DateTo - rec2.DateFrom;
            }
            
            if(rec2.DateFrom >= rec1.DateFrom && rec2.DateFrom <= rec1.DateTo)
            {
                if (rec2.DateTo > rec1.DateTo)
                    return rec1.DateTo - rec2.DateFrom;
                else
                    return rec2.DateTo - rec2.DateFrom;
            }

            return TimeSpan.Zero;
        }
        List<int> EmployeeProjects(int employee)
        {
            List<int> empProjects = new List<int>();
            foreach (Record rec in Records)
                if (rec.EmpID == employee && !empProjects.Contains(rec.ProjectID))
                    empProjects.Add(rec.ProjectID);

            return empProjects;
        }

        List<Record> FindRecords(int employee, int project)
        {
            List<Record> records = new List<Record>();

            foreach (Record rec in Records)
                if (rec.EmpID == employee && rec.ProjectID == project)
                    records.Add(rec);

            return records;
        }
    }
    class Program
    {
        static int Main(string[] args)
        {
            // args[0] will be the filename.
            // Test if input argument was supplied.

            if (args.Length == 0)
            {
                Console.WriteLine("Please enter the absolute name (path and filename) of the input file.");
                
                return 1;
            }

            LongestPeriodCalculations l = new LongestPeriodCalculations(args[0]);

            Console.WriteLine("EmpID,  ProjectID,  DateFrom,  DateTo,  Span");
            foreach(Record rec in l.Records)
            {
                Console.WriteLine(rec);
            }

            KeyValuePair<int, int> teamLongestPair = l.TeamLongestPeriod();
            Console.WriteLine("Employee 1: {0}", teamLongestPair.Key);
            Console.WriteLine("Employee 2: {0}", teamLongestPair.Value);

            return 0;
        }
    }
}
