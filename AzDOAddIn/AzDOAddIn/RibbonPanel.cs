using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AzDOAddIn.Forms;
using Microsoft.Office.Tools.Ribbon;

namespace AzDOAddIn
{
    public partial class RibbonPanel
    {
        private void RibbonPanel_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private async void btn_LinkToTeamProject_Click(object sender, RibbonControlEventArgs e)
        {
            await ProjectOperations.LinkToTeamProjectAsync().ConfigureAwait(true);
        }

        private void btn_AddColumns_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.UpdateView();
        }

        private async void btn_UpdatePlan_Click(object sender, RibbonControlEventArgs e)
        {
            await ProjectOperations.UpdateProjectPlanAsync().ConfigureAwait(true);
        }

        private async void btnGetWorkItems_Click(object sender, RibbonControlEventArgs e)
        {
            var getWorkItemsForm = new Forms.GetWorkItemsForm();
            if (getWorkItemsForm.ShowDialog() == DialogResult.OK)
            {
                await ProjectOperations.AddWorkItemsToPlanAsync(getWorkItemsForm.WiIds).ConfigureAwait(true);
            }
        }

        private async void btnImportChilds_Click(object sender, RibbonControlEventArgs e)
        {
            await ProjectOperations.ImportChildsAsync().ConfigureAwait(true);
        }

        private async void btn_PublishWorkItems_Click(object sender, RibbonControlEventArgs e)
        {
            await ProjectOperations.PublishProjectPlanAsync().ConfigureAwait(true);
        }

        private async void btn_ImportTeamMembers_Click(object sender, RibbonControlEventArgs e)
        {
            await ProjectOperations.ImportTeamMembersAsync().ConfigureAwait(true);
        }

        private void btn_Settings_Click(object sender, RibbonControlEventArgs e)
        {
            var settingsForm = new Forms.SettingsForm();

            settingsForm.UpdateSettingsValues(ProjectOperations.GetPlanSettings(), ProjectOperations.GetOperationalSettings());

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                ProjectOperations.SavePlanningSettings(settingsForm.useSprintStartDate);
                ProjectOperations.SaveOperationalSettings(settingsForm.savePlan, settingsForm.workItemTag);
            }
        }
    }
}
