using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UiPathOrchestrator;

namespace UiPathOrchestratorStartJob
{
    class Program
    {
        static void Main(string[] args)
        {
            string login = ConfigurationManager.AppSettings["login"];
            string password = ConfigurationManager.AppSettings["password"];
            UiPathCloudAPI uiPath = new UiPathCloudAPI(login, password);
            uiPath.Initiation();
            var robots = uiPath.GetRobots();
            PrintRobots(robots);
            var processes = uiPath.GetProcesses();
            PrintProcesses(processes);
            var jobs = uiPath.GetJobs();
            PrintJobs(jobs);
            var newJobs = uiPath.StartJob(robots.FirstOrDefault().Id);
            Console.Write("New ");
            PrintJobs(newJobs);
            Console.ReadKey();
        }

        static void PrintRobots(List<Robot> robots)
        {
            Console.WriteLine("Robots:");
            foreach (var item in robots)
            {
                Console.WriteLine(string.Format("\t{0} {1}", item.Id, item.Name));
            }
        }

        static void PrintProcesses(List<Process> proccess)
        {
            Console.WriteLine("Processes:");
            foreach (var item in proccess)
            {
                Console.WriteLine(string.Format("\t{0} {1}", item.Id, item.Name));
            }
        }

        static void PrintJobs(List<Job> jobs)
        {
            Console.WriteLine("Jobs:");
            foreach (var item in jobs)
            {
                Console.WriteLine(string.Format("\t{0} {1}", item.Id, item.Key));
            }
        }
    }
}
