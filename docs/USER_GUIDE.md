# AzDO MS Project Add-in — User Guide

This guide explains how to use the **Azure DevOps Work Items** add-in for Microsoft Project to sync your project plans with Azure DevOps.

---

## Table of contents

1. [Getting started](#getting-started)
2. [Ribbon reference](#ribbon-reference)
3. [Personal Access Token (PAT) and security](#personal-access-token-pat-and-security)
4. [Linking a plan to a team project](#linking-a-plan-to-a-team-project)
5. [Adding Azure DevOps columns](#adding-azure-devops-columns)
6. [Importing team members](#importing-team-members)
7. [Planning and publishing work items](#planning-and-publishing-work-items)
8. [Importing work items into a plan](#importing-work-items-into-a-plan)
9. [Updating the plan from Azure DevOps](#updating-the-plan-from-azure-devops)
10. [Settings](#settings)
11. [Troubleshooting](#troubleshooting)

---

## Getting started

- Install the add-in (see [Installation](INSTALL.md)).
- Start **Microsoft Project** and open or create a project plan.
- On the ribbon, open the **Azure DevOps Work Items** tab.

![Add-in tab](../images/addin_tab.png)

Before you can sync work items, you must:

1. Create a **Personal Access Token (PAT)** in Azure DevOps with the right permissions.
2. **Link** the current plan to an Azure DevOps organization and team project.
3. Optionally **add columns** so you can see Work Item ID, State, Type, and other fields in the plan.

---

## Ribbon reference

| Button | Description |
|--------|-------------|
| **Link to Team Project** | Associate this plan with an Azure DevOps org and team project; choose work item types to sync. |
| **Add columns** | Add custom columns for Work Item ID, Type, State, Area, Iteration so you can view and edit them. |
| **Update Plan** | Pull latest state and completed work from linked work items into the plan. |
| **Get Work Items** | Search for work items by ID and add selected ones to the plan. |
| **Import Childs** | For the selected task (with a work item ID), import its child work items from Azure DevOps. |
| **Publish Work Items** | Create or update work items in Azure DevOps from the current plan tasks. |
| **Import Team Members** | Add team members from a selected team as resources in the plan. |
| **Settings** | Configure planning options (e.g. use sprint start date) and operational options (save plan, work item tag). |

---

## Personal Access Token (PAT) and security

The add-in uses **only** a Personal Access Token to authenticate with Azure DevOps. No other sign-in method is supported.

- **Where PATs are stored:** PATs are stored in **Windows Credential Manager** under a target like `AzDOAddIn:<organization URL>`. They are not stored in the project file.
- **Creating a PAT:** See [Use personal access tokens](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page).

**Recommended PAT permissions:**

- **Work Items:** Read, write, & manage
- **Project and team:** Read
- **Identity:** Read (optional; useful for user resolution)

![PAT permissions](../images/pat_permissions.png)

Use the minimum scope needed (e.g. only the specific project) and set an expiration date. You can update the PAT later in the **Link to Team Project** dialog.

---

## Linking a plan to a team project

You must link the plan to an Azure DevOps organization and team project before publishing or importing work items.

1. Click **Link to Team Project** on the Azure DevOps Work Items tab.
2. Enter your **Azure DevOps organization URL** (e.g. `https://dev.azure.com/yourorg`) and your **PAT**, then click **Get Team Project**.
3. Select the **team project** to sync with and click **OK**.
4. In the next dialog, select the **work item types** you want to use in the plan (e.g. Task, User Story, Bug), then click **OK**.
5. Save the plan if prompted.

![Link to team project](../images/link_team_project.png)

![Org URL and PAT](../images/add_org_url.png)

![Team projects](../images/add_team_project.png)

![Work item types](../images/work_items_list.png)

After linking, the add-in remembers the org URL and can load the PAT from Credential Manager when you open this plan again.

---

## Adding Azure DevOps columns

To see and edit Work Item ID, State, Type, Area, and Iteration in the plan:

1. Click **Add columns**.
2. The add-in adds the Azure DevOps columns to the current table; you can **move** them to the position you want.

![Add columns](../images/add_columns.png)

![Azure DevOps columns](../images/devops_columns.png)

---

## Importing team members

Resource names in the plan should match the **Display Name** of users in Azure DevOps when you assign work. To add team members as resources:

1. Click **Import Team Members**.
2. Choose the **team** whose members you want to import.
3. Click **OK**. New members are added as resources; existing names are skipped.

![Import team members](../images/import_users.png)

![Select team](../images/team_to_import.png)

---

## Planning and publishing work items

**Workflow:**

1. Build your **task list** in the plan (hierarchy and dates). For each task, set the **Work Item Type** (e.g. Task, User Story) in the added column.
2. Set the **project baseline** so planned dates and work are stored.
3. Click **Publish Work Items**. The add-in creates new work items in the linked team project and updates existing ones; tasks get **Work Item ID** and **Type** filled in.
4. Save the plan if you use the “Save plan after sync” option in Settings.

![Plan with work item types](../images/plan_workitems.png)

![Set baseline](../images/set_baseline.png)

![Publish](../images/publish.png)

![Work items in Azure DevOps](../images/work_items.png)

![Published plan](../images/pulished_tasks.png)

**Notes:**

- Tasks without a Work Item Type are skipped for publish.
- Parent/child hierarchy in the plan is reflected in work item parent/child links when supported.
- Area and Iteration from the plan are sent to the work item if you use those columns.

---

## Importing work items into a plan

To bring existing Azure DevOps work items into the plan:

1. Click **Get Work Items**.
2. Enter one or more **work item IDs** (comma-separated) and click **Search**.
3. Select the work items you want in the list, then click **OK**. They are added as tasks and linked by Work Item ID.

![Get work items](../images/import_work_items.png)

![Search and select](../images/get_work_item.png)

![Imported in plan](../images/imported_work_item_in_plan.png)

To add **child work items** of an existing task:

1. Select the task that has a Work Item ID.
2. Click **Import Childs**. Child work items are added under that task.

![Import childs](../images/import_childs.png)

---

## Updating the plan from Azure DevOps

When work items are updated in Azure DevOps (e.g. state or completed work), refresh the plan:

1. Click **Update Plan**.
2. The add-in fetches the latest data for all tasks that have a Work Item ID and updates State, completed work, and other mapped fields in the plan.

![Updated work item in Azure DevOps](../images/updated_work_item.png)

![Update plan](../images/update_plan.png)

![Updated plan](../images/updated_plan.png)

---

## Settings

Click **Settings** to open the options dialog.

- **Planning**
  - **Use sprint start date:** When importing or mapping work items, use the iteration’s start (and optionally end) date for scheduling.
- **Operational**
  - **Save plan after sync:** Automatically save the plan after publish, import, or update.
  - **Work item tag:** Optional tag to apply to newly created work items when publishing.

---

## Troubleshooting

| Issue | What to try |
|-------|-------------|
| **401 Unauthorized** | PAT may be expired or revoked. Create a new PAT and re-enter it in **Link to Team Project** (or update PAT in Credential Manager for that org URL). |
| **404 Not Found** | Check that the organization URL and team project name are correct and that the PAT has access to that project. |
| **Add-in tab missing** | Ensure the add-in is installed and registered; restart Project. See [INSTALL.md](INSTALL.md). |
| **“Task without name”** | Every task you publish must have a name. Fill in the task name before publishing. |
| **Parent/child errors** | When publishing hierarchy, ensure parent tasks are published before or with their children; all tasks in the chain that map to work items need a valid Work Item Type. |
| **PAT not found** | If you switched machines or cleared Credential Manager, re-enter the PAT in the Link to Team Project dialog so it is stored again. |

For build and installation issues, see [INSTALL.md](INSTALL.md) and [DEVELOPMENT.md](DEVELOPMENT.md).
