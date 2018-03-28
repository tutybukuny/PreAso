using System;
using System.IO;
using System.Windows.Forms;
using PreAsso.Bussiness;

namespace PreAsso
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbbThres.SelectedIndex = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open file",
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|(*.xls)|*.xls|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                txtBrowse.Text = dialog.FileName;
        }

        private void btnPreAsso_Click(object sender, EventArgs e)
        {
            var excelFile = txtBrowse.Text;
            if (excelFile.Length == 0)
            {
                MessageBox.Show("Please choose an excel file");
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Title = "Save arff Files",
                    CheckPathExists = true,
                    DefaultExt = "arff",
                    Filter = "Text files (*.arff)|*.arff|All files (*.*)|*.*",
                    RestoreDirectory = true,
                    AddExtension = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    double thres = cbbThres.SelectedIndex == 0 ? 50 : 70;
                    var building = new DataBuilding();
                    building.PrepareForAr(Path.GetDirectoryName(excelFile), excelFile,
                        dialog.FileName, thres);

                    MessageBox.Show("Done");
                }
            }
        }
    }
}