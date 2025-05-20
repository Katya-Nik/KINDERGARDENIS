using System.Drawing;
using System.Windows.Forms;

public class BaseForm : Form
{
    protected void ConfigureDataGridView(DataGridView dataGridView)
    {
        dataGridView.BackgroundColor = Color.FromArgb(238, 245, 245);
        dataGridView.DefaultCellStyle.Font = new Font("Verdana", 12, FontStyle.Regular);
        dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(39, 39, 39);
        dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 14, FontStyle.Bold);
        dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(39, 39, 39);
        dataGridView.EnableHeadersVisualStyles = false;
        dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 245, 245);
        dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView.RowHeadersVisible = false;
        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
        dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);
    }
}