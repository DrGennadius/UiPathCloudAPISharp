using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UiPathCloudAPISharp;
using UiPathCloudAPISharp.Models;

namespace UiPathWinFormExample.net40
{
    public partial class Form1 : Form
    {
        private UiPathCloudAPI _uiPathOrchestrator;

        public Form1()
        {
            InitializeComponent();
            string tenantLogicalName = ConfigurationManager.AppSettings["TenantLogicalName"];
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string refreshToken = ConfigurationManager.AppSettings["UserKey"];
            _uiPathOrchestrator = new UiPathCloudAPI(tenantLogicalName, clientId, refreshToken, BehaviorMode.AutoInitiation);
            _uiPathOrchestrator.JobManager.WaitReadyJobCompleted += JobManager_WaitReadyJobCompleted;

            var bindingSourceRobots = new BindingSource();
            bindingSourceRobots.DataSource = _uiPathOrchestrator.RobotManager.GetCollection().ToList();

            cbRobot.DataSource = bindingSourceRobots.DataSource;
            cbRobot.DisplayMember = "Name";
            cbRobot.ValueMember = "Name";

            var bindingSourceProcesses = new BindingSource();
            bindingSourceProcesses.DataSource = _uiPathOrchestrator.ProcessManager.GetCollection();

            cbProcess.DataSource = bindingSourceProcesses.DataSource;
            cbProcess.DisplayMember = "Name";
            cbProcess.ValueMember = "Name";
        }

        private void JobManager_WaitReadyJobCompleted(object sender, UiPathCloudAPISharp.Managers.WaitReadyJobCompletedEventArgs e)
        {
            tbOutput.Text += string.Format("\n{0} done!", e.ReadyJob.Key);
        }

        private void bStartJob_Click(object sender, EventArgs e)
        {
            Robot robot = (Robot)cbRobot.SelectedItem;
            Process process = (Process)cbProcess.SelectedItem;
            tbOutput.Text += "\nStarting job...";
            var newJob =_uiPathOrchestrator.JobManager.StartJob(robot, process);
            tbOutput.Text += "\nPending job...";
            _uiPathOrchestrator.JobManager.RunWaitReadyJobAsync(newJob);
        }
    }
}
