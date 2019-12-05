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
using UiPathCloudAPISharp;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.OData;

namespace UiPathCloudAPISharpStartJob
{
    class Program
    {
        static void Main(string[] args)
        {
            UiPathCloudAPI uiPath = new UiPathCloudAPI();
            if (Authentication(uiPath))
            {
                MenuLoop(uiPath);
            }
        }

        static bool Authentication(UiPathCloudAPI uiPathCloudAPI)
        {
            bool result = false;
            bool authenticated = false;

            Console.WriteLine("Authentication...");

            string tenantLogicalName = ConfigurationManager.AppSettings["TenantLogicalName"];
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string refreshToken = ConfigurationManager.AppSettings["UserKey"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(refreshToken))
            {
                int tryCount = 3;
                while (tryCount > 0 && !authenticated)
                {
                    Console.WriteLine("Attempt #{0}", 4 - tryCount);
                    Console.Write(" Enter login: ");
                    clientId = Console.ReadLine();
                    Console.Write(" Enter password: ");
                    refreshToken = Console.ReadLine();
                    authenticated = TryAuthorize(uiPathCloudAPI, tenantLogicalName, clientId, refreshToken);
                    tryCount--;
                }
            }
            else
            {
                authenticated = TryAuthorize(uiPathCloudAPI, tenantLogicalName, clientId, refreshToken);
            }
            if (authenticated)
            {
                try
                {
                    uiPathCloudAPI.GetMainData();
                    result = true;
                }
                catch (WebException)
                {
                    Console.WriteLine(uiPathCloudAPI.LastErrorMessage);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
            else
            {
                Console.ReadKey();
            }

            return result;
        }

        static bool TryAuthorize(UiPathCloudAPI uiPathCloudAPI, string tenantLogicalName, string clientId, string refreshToken)
        {
            bool result = false;

            try
            {
                uiPathCloudAPI.Authorization(tenantLogicalName, clientId, refreshToken);
                result = uiPathCloudAPI.Authorized;
            }
            catch (WebException)
            {
                Console.WriteLine(uiPathCloudAPI.LastErrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        static void MenuLoop(UiPathCloudAPI uiPath)
        {
            Dictionary<string, object> inputArguments = new Dictionary<string, object>
            {
                { "InputX1", 2 },
                { "InputX2", 3 },
                { "InOutZ", 4 }
            };
            var startedJob = uiPath.StartJob("Float Test Robot", "BackgroundProcess", "Demo Environment", inputArguments);
            var job = uiPath.WaitReadyJob(startedJob);
            var a1 = job.OutputArguments["OutputY"];
            Console.ReadKey();
            return;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome, {0}!", uiPath.TargetAccount.Name);
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
                        //PrintJobs(uiPath.GetJobs());
                        Console.ReadKey();
                        break;
                    case 2:
                        try
                        {
                            Console.Write("Enter robot name: ");
                            string robotName = Console.ReadLine();
                            Console.Write("Enter proccess name: ");
                            string proccessName = Console.ReadLine();
                            var newJobs = uiPath.StartJob(robotName, proccessName);
                            //PrintJobs(newJobs, "New Jobs:");
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

        static void PrintJobs(List<Job> jobs, string title = "Jobs:")
        {
            Console.WriteLine(title);
            ConsoleHelper.UpdateWidth();
            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintRow(nameof(Job.Id), nameof(Job.Key), nameof(Job.State), nameof(Job.StartTime), nameof(Job.EndTime));
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
