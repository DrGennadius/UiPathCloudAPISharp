using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using UiPathCloudAPISharp;
using UiPathCloudAPISharp.Managers;
using UiPathCloudAPISharp.Models;

namespace UiPathCloudAPISharpStartJob
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Authentication...");

            string tenantLogicalName = ConfigurationManager.AppSettings["TenantLogicalName"];
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string refreshToken = ConfigurationManager.AppSettings["UserKey"];

            UiPathCloudAPI uiPath = new UiPathCloudAPI(tenantLogicalName, clientId, refreshToken, BehaviorMode.AutoInitiation);
            MenuLoop(uiPath);
        }

        static void MenuLoop(UiPathCloudAPI uiPath)
        {
            uiPath.RobotManager.UseSession = false;
            var robots = uiPath.RobotManager.GetCollection();
            while (true)
            {
                Console.Clear();
                //Console.WriteLine("Welcome, {0}!", uiPath.TargetAccount.Name);
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Robots.");
                Console.WriteLine("2. Processes.");
                Console.WriteLine("3. Jobs.");
                Console.WriteLine("4. Assets.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        RobotsMenuLoop(uiPath);
                        break;
                    case 2:
                        ProccessesMenuLoop(uiPath);
                        break;
                    case 3:
                        JobsMenuLoop(uiPath);
                        break;
                    case 4:
                        AssetsMenuLoop(uiPath);
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void RobotsMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Robots menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintRobots(uiPath.GetRobots());
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void ProccessesMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Proccesses menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintProcesses(uiPath.GetProcesses());
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void JobsMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Jobs menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.WriteLine("2. Start new job.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintJobs(uiPath.GetJobs());
                        Console.ReadKey();
                        break;
                    case 2:
                        try
                        {
                            Console.Write("Enter robot name: ");
                            string robotName = Console.ReadLine();
                            Console.Write("Enter proccess name: ");
                            string proccessName = Console.ReadLine();
                            var newJob = uiPath.StartJob(robotName, proccessName);
                            PrintJobs(new List<JobWithArguments> { (JobWithArguments)newJob }, "New Job:");
                        }
                        catch (WebException)
                        {
                            Console.WriteLine(uiPath.LastErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void AssetsMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Assets menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Get assets.");
                Console.WriteLine("2. Get asset by Robot name.");
                Console.WriteLine("3. Create asset.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        try
                        {
                            PrintConcreteAssets(uiPath.GetConcreteAssets());
                        }
                        catch (WebException)
                        {
                            Console.WriteLine(uiPath.LastErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                        break;
                    case 2:
                        try
                        {
                            Console.Write("Enter robot name: ");
                            string robotName = Console.ReadLine();
                            Console.Write("Enter asset name: ");
                            string assetName = Console.ReadLine();
                            Asset asset = uiPath.GetRobotAsset(robotName, assetName);
                            var sinpledAsset = GetSimpledAsset(asset);
                            Console.WriteLine("{0} = {1}", sinpledAsset.Key, sinpledAsset.Value);
                        }
                        catch (WebException)
                        {
                            Console.WriteLine(uiPath.LastErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                        break;
                    case 3:
                        try
                        {

                        }
                        catch (WebException)
                        {
                            Console.WriteLine(uiPath.LastErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void PrintRobots(List<Robot> robots, string title = "Robots:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow(nameof(Robot.Id), nameof(Robot.Name), nameof(Robot.Description));
            ConsoleHelper.PrintLine();
            foreach (var item in robots)
            {
                ConsoleHelper.PrintRow(item.Id.ToString(), item.Name, item.Description);
            }
            ConsoleHelper.PrintLine();
        }

        static void PrintProcesses(List<Process> proccess, string title = "Processes:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow(nameof(Process.Id), nameof(Process.Name), nameof(Process.Key), nameof(Process.Description));
            ConsoleHelper.PrintLine();
            foreach (var item in proccess)
            {
                ConsoleHelper.PrintRow(item.Id.ToString(), item.Name, item.Key, item.Description);
            }
            ConsoleHelper.PrintLine();
        }

        static void PrintJobs(List<JobWithArguments> jobs, string title = "Jobs:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow(nameof(JobWithArguments.Id), nameof(JobWithArguments.Key), nameof(JobWithArguments.State), nameof(JobWithArguments.StartTime), nameof(JobWithArguments.EndTime));
            ConsoleHelper.PrintLine();
            foreach (var item in jobs)
            {
                ConsoleHelper.PrintRow(item.Id.ToString(), item.Key, item.State.ToString(), item.StartTime.ToString(), item.EndTime.ToString());
            }
            ConsoleHelper.PrintLine();
        }

        static void PrintConcreteAssets(List<ConcreteAsset> assets, string title = "Assets:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow("Name", "Value");
            ConsoleHelper.PrintLine();
            foreach (var item in assets)
            {
                ConsoleHelper.PrintRow(item.Name, item.ForceStringValue());
            }
            ConsoleHelper.PrintLine();
        }

        static void PrintAssets(List<Asset> assets, string title = "Assets:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow("Name", "Value");
            ConsoleHelper.PrintLine();
            foreach (var item in assets)
            {
                var simpledItem = GetSimpledAsset(item);
                ConsoleHelper.PrintRow(simpledItem.Key, simpledItem.Value);
            }
            ConsoleHelper.PrintLine();
        }

        static KeyValuePair<string, string> GetSimpledAsset(Asset asset)
        {
            return new KeyValuePair<string, string>(
                asset.Name,
                asset.ValueType == AssetValueType.Text ? asset.StringValue :
                asset.ValueType == AssetValueType.Integer ? asset.IntValue.ToString() :
                asset.ValueType == AssetValueType.Bool ? asset.BoolValue.ToString() :
                asset.CredentialUsername
                );
        }
    }
}
