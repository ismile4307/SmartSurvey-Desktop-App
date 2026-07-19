using System.Collections.Generic;
using System.Windows;

namespace DBI_Scripting.Forms.Download
{
    public partial class FrmSyncResults : Window
    {
        public FrmSyncResults(int successCount, List<string> failedIds, List<string> serverMessages)
        {
            InitializeComponent();

            string summary = successCount + " record(s) synced successfully.";
            if (failedIds.Count > 0)
                summary += "    |    Failed (" + failedIds.Count + "): " + string.Join(", ", failedIds);

            lblSummary.Content = summary;
            txtResults.Text = string.Join("\r\n", serverMessages);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
