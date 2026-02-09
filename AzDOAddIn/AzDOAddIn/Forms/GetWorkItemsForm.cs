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
    public partial class GetWorkItemsForm : Form
    {
        public string TeamProject { get; set; }
        public string OrgUrl { get; set; }

        public List<int> WiIds
        {
            get
            {
                var wiIds = new List<int>();

                if (lstBoxResult.SelectedItems.Count > 0)
                {
                    foreach (var item in lstBoxResult.SelectedItems)
                    {
                        int wiId = ParseIdFromResult(item.ToString());
                        if (wiId > 0) wiIds.Add(wiId);
                    }
                }

                return wiIds;
            }
        }

        private static int ParseIdFromResult(string stringToParse)
        {
            int wiId = 0;
            var parseArray = stringToParse.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parseArray.Length > 2)
                if (!int.TryParse(parseArray[0].Trim(), out wiId))
                    return 0;

            return wiId;
        }

        public GetWorkItemsForm()
        {
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var workItems = await AzDoRestClient.GetWorkItemsAsync(ProjectOperations.ActiveOrgUrl, ProjectOperations.ActiveTeamProject, txtBoxIds.Text, ProjectOperations.ActivePAT).ConfigureAwait(true);

                lstBoxResult.Items.Clear();
                if (workItems?.count > 0 && workItems.WorkItems != null)
                {
                    foreach (var workItem in workItems.WorkItems)
                        lstBoxResult.Items.Add(string.Format("{0, 7} : {1, 10} : {2} ",
                            workItem.fields["System.Id"], workItem.fields["System.WorkItemType"], workItem.fields["System.Title"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lstBoxResult.SelectedItems.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Mark work items to import", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
