# AzDOMSProject

MS Project add-in to integrate with **Azure DevOps**: link plans to team projects, publish tasks as work items, import work items and team members, and sync updates.

**First release:** [v0.1-alpha](https://github.com/ashamrai/AzDOMSProject/releases/tag/v0.1-alpha)

---

## Documentation

| For … | Read |
|-------|------|
| **Using the add-in** | [User Guide](docs/USER_GUIDE.md) — ribbon, PAT, linking, publish, import, update, settings, troubleshooting |
| **Installing / building** | [Installation](docs/INSTALL.md) — prerequisites, build, Inno Setup, manual install |
| **Developing / contributing** | [Development](docs/DEVELOPMENT.md) · [Contributing](CONTRIBUTING.md) |
| **What changed** | [Changelog](CHANGELOG.md) |

All docs are under **[docs/](docs/README.md)**.

---

## Prerequisites

- **Microsoft Project** (desktop), e.g. 2016 or later
- **.NET Framework 4.8**
- **Visual Studio 2019/2022** with:
  - .NET desktop development
  - Office/SharePoint development (VSTO) workload

## Build

1. Open `AzDOAddIn\AzDOAddIn.sln` in Visual Studio.
2. Restore NuGet packages (Solution → Restore NuGet Packages).
3. Build the solution (e.g. Release | Any CPU).
4. Output: `AzDOAddIn\AzDOAddIn\bin\Release\` (add-in DLL and dependencies).  
   To create an installer, use the Inno Setup script (see below).

## Install the add-in

- **Option A – Inno Setup (recommended)**  
  1. Build the solution in Release.  
  2. Install [Inno Setup](https://jrsoftware.org/isinfo.php), then compile `installer\AzDOAddIn.iss`.  
  3. Run the generated `AzDOAddInSetup.exe` and follow the steps.

- **Option B – Manual**  
  1. Copy the contents of `AzDOAddIn\AzDOAddIn\bin\Release\` (and the Core DLL) to a folder (e.g. `%ProgramFiles%\AzDOAddIn\`).  
  2. Register the add-in per [Microsoft’s VSTO deployment docs](https://docs.microsoft.com/en-us/visualstudio/vsto/deploying-an-office-solution-by-using-windows-installer).

After installation, start MS Project and use the **Azure DevOps Work Items** tab. For detailed steps and troubleshooting, see the [User Guide](docs/USER_GUIDE.md).

![Add-in tab](images/addin_tab.png)

## Generate Personal Access Token

The add-in uses only **Personal Access Token (PAT)** authentication. PATs are stored in **Windows Credential Manager** (not in the project file). See: [Use personal access tokens](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page).

Add the following permissions to your PAT:

![PAT permissions](images/pat_permissions.png)

## Associate Project Plan with Team Project
To synchronize work items with plan tasks, you should associate your MS Project plan and Azure DevOps team project.

1. Press **Link to Team Project** button

![Link to team project](images/link_team_project.png)

2. Add Azure DevOps organization URL and PAT, then press **Get Team Project**:

![Org URL](images/add_org_url.png)

3. Select a team project to sync and press **OK**:

![Team projects](images/add_team_project.png)

4. Select work item types to sync and press **OK**:

![Work item types](images/work_items_list.png)

5. Save changes

## Add Azure DevOps columns
To view Work Item ID, State, Type, and other work item fields:

1. Press **Add columns** button.

![Add columns](images/add_columns.png)

2. Move the new columns to the position you want.

![Azure DevOps columns](images/devops_columns.png)

## Import Azure DevOps team members
Each MS Project resource in a project plan must be equal to the Display Name of the corresponding team member. The solution may import necessary team members to avoid errors.
1. Press **Import Team Members** button

![Import team members](images/import_users.png)

2. Select a team and press **OK**.

![Select team](images/team_to_import.png)

## Plan Work Items
1.	Create a task list in your plan with the necessary hierarchy and dates. Add Work Item Type value to each task in the plan.

![Plan with work item types](images/plan_workitems.png)

2. Set the project baseline to save planned dates.

![Set baseline](images/set_baseline.png)

3. Press **Publish Work Items**.

![Publish](images/publish.png)

4. New work items are created in the linked Azure DevOps project.

![Work items](images/work_items.png)

5. Each published task gets a Work Item ID and Type in the plan.

![Published plan](images/pulished_tasks.png)

## Import Work Items

To import existing work items to a plan:
1. Press **Get Work Items** button

![Get work items](images/import_work_items.png)

2. Enter work item ID(s), click **Search**, select items, then **OK**.

![Search and select](images/get_work_item.png)

3. The plan shows the imported work items:

![Imported in plan](images/imported_work_item_in_plan.png)

4. For a task with a work item ID, use **Import Childs** to add its child work items.

![Import childs](images/import_childs.png)

## Get work item updates

1. When work items are updated in Azure DevOps (state, completed work, etc.):

![Updated work item](images/updated_work_item.png)

2. Click **Update Plan** to refresh the plan from Azure DevOps.

![Update plan](images/update_plan.png)

3. The plan shows the updated data:

![Updated plan](images/updated_plan.png)

---

For full help, see the [User Guide](docs/USER_GUIDE.md).
