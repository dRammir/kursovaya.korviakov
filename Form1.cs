using System;
using System.Data;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private DataAccess _da;
    private DataSet _projectsDs;
    private BindingSource _projectsBs;

    public Form1()
    {
        InitializeComponent();
        _da = new DataAccess();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        LoadProjects();
    }

    private void LoadProjects()
    {
        _projectsDs = _da.GetAllProjectsDataSet();
        _projectsBs = new BindingSource { DataSource = _projectsDs.Tables["projects"] };
        dgvProjects.DataSource = _projectsBs;
    }

    private void btnSaveProjects_Click(object sender, EventArgs e)
    {
        try
        {
            _da.UpdateProjectsFromDataSet(_projectsDs);
            MessageBox.Show("Projects saved.");
            LoadProjects();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving: " + ex.Message);
        }
    }

    private void btnLoadTasks_Click(object sender, EventArgs e)
    {
        if (dgvProjects.CurrentRow == null) return;
        var row = ((DataRowView)dgvProjects.CurrentRow.DataBoundItem).Row;
        int projectId = Convert.ToInt32(row["id"]);
        var dsTasks = _da.GetTasksByProjectDataSet(projectId);
        dgvTasks.DataSource = dsTasks.Tables["tasks"];
    }

    private void btnRunSqlReport_Click(object sender, EventArgs e)
    {
        var dt = _da.GetProjectsByDeadline(dtpFrom.Value.Date, dtpTo.Value.Date);
        dgvReportSql.DataSource = dt;
    }

    private void btnRunSp_Click(object sender, EventArgs e)
    {
        if (dgvProjects.CurrentRow == null) return;
        var row = ((DataRowView)dgvProjects.CurrentRow.DataBoundItem).Row;
        int projectId = Convert.ToInt32(row["id"]);
        var dt = _da.GetProjectSummary(projectId);
        dgvReportSp.DataSource = dt;
    }
}
