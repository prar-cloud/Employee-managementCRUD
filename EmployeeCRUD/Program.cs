using System;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable modern Windows Forms visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the modern redesigned main form
            Application.Run(new ModernMainForm());
        }
    }
}
