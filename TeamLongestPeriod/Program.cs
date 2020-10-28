﻿using System;
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
    public static class LongestPeriodCalculations
    {
        public static Record[] ExtractData(string textfilePath)
        {
            string[] lines = System.IO.File.ReadAllLines(textfilePath);
            Record[] records = new Record[lines.Length];

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

                records[i] = r;
            }

            return records;
        }

        //returns the pair of the EmpIDs of the both employees who have worked on common projects the longest time
        public static KeyValuePair<int,int> TeamLongestPeriod(Record[] records)
        {
            List<int> employees = new List<int>();
            foreach (Record rec in records)
                if (!employees.Contains(rec.EmpID))
                    employees.Add(rec.EmpID);
           
            TimeSpan maxPeriod = TimeSpan.Zero;
            KeyValuePair<int, int> maxPair = new KeyValuePair<int, int>();

            for (int emp1 = 0; emp1 < employees.Count; emp1++)
                for (int emp2 = emp1 + 1; emp2 < employees.Count; emp2++)
                {
                    TimeSpan currPairTeamPeriod = TotalTeamPeriod(employees[emp1], employees[emp2], records);
                    if (currPairTeamPeriod > maxPeriod)
                    {
                        maxPeriod = currPairTeamPeriod;
                        maxPair = new KeyValuePair<int, int>(employees[emp1], employees[emp2]);

                    }
                }

            Console.WriteLine("Max Period: {0}", maxPeriod);
            return maxPair;
        }

      
         static TimeSpan TotalTeamPeriod(int emp1, int emp2, Record[] recs)
        {
            TimeSpan total = TimeSpan.Zero;
            List<int> projectsEmp1 = EmployeeProjects(emp1, recs);
            List<int> projectsEmp2 = EmployeeProjects(emp2, recs);

            var intersectionProjects = projectsEmp1.Intersect(projectsEmp2);

            foreach(int proj in intersectionProjects)
            {
                List<Record> recordsEmp1 = FindRecords(emp1, proj, recs);
                List<Record> recordsEmp2 = FindRecords(emp2, proj, recs);

                foreach(Record rec1 in recordsEmp1)
                    foreach(Record rec2 in recordsEmp2)
                    {
                        total += PeriodOverlap(rec1, rec2);
                    }
            }
            return total;
        }
        
        static TimeSpan PeriodOverlap(Record rec1, Record rec2)
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
         static List<int> EmployeeProjects(int employee, Record[] recs)
        {
            List<int> empProjects = new List<int>();
            foreach (Record rec in recs)
                if (rec.EmpID == employee && !empProjects.Contains(rec.ProjectID))
                    empProjects.Add(rec.ProjectID);

            return empProjects;
        }

         static List<Record> FindRecords(int employee, int project, Record[] recs)
        {
            List<Record> records = new List<Record>();

            foreach (Record rec in recs)
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
            Record[] recs = LongestPeriodCalculations.ExtractData(args[0]);

            Console.WriteLine("EmpID,  ProjectID,  DateFrom,  DateTo,  Span");
            foreach(Record rec in recs)
            {
                Console.WriteLine(rec);
            }

            KeyValuePair<int, int> teamLongestPair = LongestPeriodCalculations.TeamLongestPeriod(recs);
            Console.WriteLine("Employee 1: {0}", teamLongestPair.Key);
            Console.WriteLine("Employee 2: {0}", teamLongestPair.Value);

            return 0;
        }
    }
}
