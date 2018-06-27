using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Module4Lesson1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectionString = "";
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = connectionString;

            DataSet ds = new DataSet();

           // SqlDataAdapter da = new SqlDataAdapter("select * from newEquipment", con);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select top 3 * from AccessTab;";
                // + "select * from AccessTab;";
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            //download data from cmd using con
            da.Fill(ds);

            ds.Tables[0].ColumnChanged += ch_ChangeColumn;


            foreach (DataTable item in ds.Tables)
            {
                Label l = new Label() { Content = item.TableName, FontWeight=FontWeights.Bold };
                spDataInfo.Children.Add(l);

                //foreach (DataColumn column in item.Columns)
                //{
                //    string coumnInfo = string.Format("\t{0} - dataType: {1}",
                //        column.ColumnName,
                //        column.DataType);

                //    Label lcolumn = new Label() { Content = coumnInfo,
                //        FontStyle = FontStyles.Italic };
                //    spDataInfo.Children.Add(lcolumn);
                //}

                int y = 0;
                foreach (DataRow row in item.Rows)
                {
                    if (y == 0)
                    {
                        lableStatusBar.Content += "\nBefore:" + row.RowState + "\n";
                        row["strTabName"] = "newValue";
                        lableStatusBar.Content += "\nAfter:" + row.RowState;
                        y++;
                     }


                    
                    Label lrow = new Label()
                    {        
                        FontStyle = FontStyles.Italic
                    };

                        foreach (object cel  in row.ItemArray)
                        {
                                lrow.Content += " \t" + cel.ToString();  
                        }

                             spDataInfo.Children.Add(lrow);
                }
                ds.AcceptChanges();

                if(ds.HasChanges())
                {
                    Label lchange = new Label()
                    {
                        FontStyle = FontStyles.Italic,
                        Foreground = new SolidColorBrush(Colors.Red),
                        Content = "До изменения"
                    };
                    foreach (DataTable tableChange in ds.GetChanges(DataRowState.Modified).Tables)
                    {
                        foreach (DataRow row in tableChange.Rows)
                        {
                            lchange.Content += "\n--------------------------";
                            foreach (var itemRow in row.ItemArray)
                            {
                                lchange.Content += string.Format("\t Before: {0}; After: {1}",
                                    row["strTabName", DataRowVersion.Original],
                                    row["strTabName", DataRowVersion.Current]);
                            }
                            lchange.Content += "\n--------------------------";
                        }
                    }
                    spDataInfo.Children.Add(lchange);
                }
            }

        }

        private void ch_ChangeColumn(object sender, DataColumnChangeEventArgs e)
        {
            lableStatusBar.Content += e.Column + " - " + e.ProposedValue + " - " + e.Row[e.Column, DataRowVersion.Original];
        }
    }
}
