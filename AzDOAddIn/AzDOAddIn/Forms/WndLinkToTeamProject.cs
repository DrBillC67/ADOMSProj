using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AzDOAddIn.Core;

namespace AzDOAddIn.Forms
{
    public partial class WndLinkToTeamProject : Form
    {
        public string Url => cmdAzDoUrl.Text;
        public string TeamProject => cmdBox_TeamProjects.Text;
        public string PAT { get => txtBox_PAT.Text; set => txtBox_PAT.Text = value; }

        public WndLinkToTeamProject()
        {
            InitializeComponent();

            var azdoUrls = PatHelper.GetStoredUrls();

            if (azdoUrls.Count > 0)
                foreach (string url in azdoUrls) cmdAzDoUrl.Items.Add(url);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private async void btn_GetTeamProjects_Click(object sender, EventArgs e)
        {
            try
            {
                var projects = await AzDoRestClient.GetTeamProjectsAsync(Url, PAT).ConfigureAwait(true);

                cmdBox_TeamProjects.Items.Clear();

                foreach (var project in projects.TeamProjects)
                    cmdBox_TeamProjects.Items.Add(project.name);

                if (cmdBox_TeamProjects.Items.Count > 0)
                {
                    cmdAzDoUrl.Enabled = false;
                    txtBox_PAT.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdAzDoUrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Url != "")
                PAT = PatHelper.GetPat(Url);
        }

        private async void btn_UpdatePAT_Click(object sender, EventArgs e)
        {
            try
            {
                if (Url.Length == 0) return;

                var projects = await AzDoRestClient.GetTeamProjectsAsync(Url, PAT).ConfigureAwait(true);

                if (projects.count > 0)
                {
                    DialogResult = DialogResult.Yes;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
