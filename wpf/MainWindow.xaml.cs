using System;
using System.Collections.Generic;
using System.Data;
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

namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string supplyText = supplyTextBox.Text;
            string demandText = demandTextBox.Text;
            string costsText = costsTextBox.Text;

            int[] supplyValues = supplyText.Split(',').Select(int.Parse).ToArray();
            int[] demandValues = demandText.Split(',').Select(int.Parse).ToArray();
            int[,] costs = ParseCosts(costsText);

            int[,] plan = SolveNorthWestCorner(supplyValues, demandValues, costs);
            DisplayResult(plan, costs);
        }

        private int[,] ParseCosts(string costsText)
        {
            string[] rows = costsText.Split(',');
            int rowCount = rows.Length / 3; // Assuming 4 values per row
            int[,] costs = new int[rowCount, 3];

            int index = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    costs[i, j] = int.Parse(rows[index]);
                    index++;
                }
            }

            return costs;
        }

        private int[,] SolveNorthWestCorner(int[] supply, int[] demand, int[,] costs)
        {
            int m = supply.Length;
            int n = demand.Length;
            int[,] plan = new int[m, n];

            int i = 0;
            int j = 0;

            while (i < m && j < n)
            {
                int quantity = Math.Min(supply[i], demand[j]);

                plan[i, j] = quantity;
                supply[i] -= quantity;
                demand[j] -= quantity;

                if (supply[i] == 0)
                {
                    i++;
                }
                if (demand[j] == 0)
                {
                    j++;
                }
            }

            return plan;
        }

        private void DisplayResult(int[,] plan, int[,] costs)
        {
            DataTable dt = new DataTable();

            // Добавляем столбцы в DataTable
            for (int j = 0; j < plan.GetLength(1); j++)
            {
                dt.Columns.Add(new DataColumn($"Column{j + 1}", typeof(int)));
            }

            // Добавляем строки с данными в DataTable
            for (int i = 0; i < plan.GetLength(0); i++)
            {
                DataRow newRow = dt.NewRow();
                for (int j = 0; j < plan.GetLength(1); j++)
                {
                    newRow[j] = plan[i, j];
                }
                dt.Rows.Add(newRow);
            }

            // Привязываем DataTable к DataGrid
            resultGrid.ItemsSource = dt.DefaultView;

            // Вычисляем и выводим общие затраты
            int totalCost = CalculateTotalCost(plan, costs);
            totalCostTextBlock.Text = totalCost.ToString();
        }

        private int CalculateTotalCost(int[,] plan, int[,] costs)
        {
            int totalCost = 0;

            for (int i = 0; i < plan.GetLength(0); i++)
            {
                for (int j = 0; j < plan.GetLength(1); j++)
                {
                    totalCost += plan[i, j] * costs[i, j];
                }
            }

            return totalCost;
        }
    }
}
