using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System.Configuration;
using Microsoft.TeamFoundation.Server;

namespace TfsSdk_ConnectToTfsProgrammatically
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly string MyUri = ConfigurationManager.AppSettings["TfsUri"];
        private static TfsTeamProjectCollection _tfs;
        private static ProjectInfo _selectedTeamProject;

        private void InteractiveConnectToTfsClick(object sender, EventArgs e)
        {
            lblNotes.Text = string.Empty;

            try
            {
                var notes = new StringBuilder();
                var tfsPp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);

                tfsPp.ShowDialog();

                _tfs = tfsPp.SelectedTeamProjectCollection;

                if (tfsPp.SelectedProjects.Any())
                {
                    _selectedTeamProject = tfsPp.SelectedProjects[0];
                }

                notes.AppendFormat("{0} Team Project : {1} - {2}{0}", Environment.NewLine, _selectedTeamProject.Name, _selectedTeamProject.Uri);
                


                lblNotes.Text = notes.ToString();
            }
            catch (Exception ex)
            {
                lblError.Text = " Message : " + ex.Message +
                                (ex.InnerException != null ? " Inner Exception : " + ex.InnerException : string.Empty);
            }
        }

        private void UnInteractiveConnectToTfsClick(object sender, EventArgs e)
        {
            lblNotes.Text = string.Empty;

            try
            {
                var notes = new StringBuilder();

                var configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(MyUri));

                var catalogNode = configurationServer.CatalogNode;

                var tpcNodes = catalogNode.QueryChildren(
                    new Guid[] { CatalogResourceTypes.ProjectCollection },
                    false, CatalogQueryOptions.None);
                foreach (var tpcNode in tpcNodes)
                {
                    var tpcId = new Guid(tpcNode.Resource.Properties["InstanceId"]);
                    TfsTeamProjectCollection tpc = configurationServer.GetTeamProjectCollection(tpcId);

                    notes.AppendFormat("{0} Team Project Collection : {1}{0}", Environment.NewLine, tpc.Name);
                    var tpNodes = tpcNode.QueryChildren(
                        new Guid[] { CatalogResourceTypes.TeamProject },
                        false, CatalogQueryOptions.None);

                    foreach (var p in tpNodes)
                    {
                        notes.AppendFormat("{0} Team Project : {1} - {2}{0}", Environment.NewLine,
                                           p.Resource.DisplayName, p.Resource.Description);
                    }
                }
                lblNotes.Text = notes.ToString();
            }
            catch (Exception ex)
            {
                lblError.Text = " Message : " + ex.Message +
                                (ex.InnerException != null ? " Inner Exception : " + ex.InnerException : string.Empty);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
