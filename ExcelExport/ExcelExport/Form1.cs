using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System;
using System.Drawing;

namespace ExcelExport
{
    public partial class Form1 : Form
    {
        private int million = (int)Math.Pow(10, 6);

        RealEstateEntities context = new RealEstateEntities();
        List<Flat> lakasok;
        Excel.Application xlApp;
        Excel.Workbook xlWb;
        Excel.Worksheet xlSheet;
        string[] headers; //letrehozzuk osztalyszinten, hogy a FormatTable() fvben is lathato legyen
        public Form1()
        {
            InitializeComponent();
            LoadData();
            dataGridView1.DataSource = lakasok;
            CreateExcel();
            
        }

        public void LoadData()
        {
            lakasok = context.Flats.ToList();
        }

        public void CreateExcel()
        {
            try
            {
                xlApp = new Excel.Application();
                xlWb = xlApp.Workbooks.Add(Missing.Value); //üres dokumentum létrehozása 
                xlSheet = xlWb.ActiveSheet;

                CreateTable();
                FormatTable();

                xlApp.Visible = true;
                xlApp.UserControl = true;

                
            }
            catch (Exception ex)
            {
                string hiba = string.Format("Error: {0}\nLine: {1}", ex.Message, ex.Source);
                MessageBox.Show(hiba, "error");
                xlWb.Close(false, Type.Missing,Type.Missing);
                xlApp.Quit();
                xlWb = null;
                xlApp = null; //kidobja a memóriából is 
            }
            
        }

        private void CreateTable()
        {
            headers = new string[]
            {
                 "Kód",
                 "Eladó",
                 "Oldal",
                 "Kerület",
                 "Lift",
                 "Szobák száma",
                 "Alapterület (m2)",
                 "Ár (mFt)",
                 "Négyzetméter ár (Ft/m2)"
            };
            //xlSheet.Cells[1, 1] = "Hello";
            for (int i = 0; i < headers.Length; i++)
            {
                xlSheet.Cells[1, i + 1] = headers[i];
            }

            //az object típusú változóba bármit belerakhatok
            object[,] values = new object[lakasok.Count, headers.Length];

            int counter = 0;
            int floorcolumn = 6;
            foreach (var lakas in lakasok)
            {
                values[counter, 0] = lakas.Code;
                values[counter, 1] = lakas.Vendor;
                values[counter, 2] = lakas.Side;
                values[counter, 3] = lakas.District;
                if (lakas.Elevator == true) values[counter, 4] = "van";
                else values[counter, 4] = "nincs";
                values[counter, 5] = lakas.NumberOfRooms;
                values[counter, 6] = lakas.FloorArea;
                values[counter, 7] = lakas.Price;
                values[counter, 8] = string.Format("={0}/{1}*{2}","H"+(counter+2).ToString(),GetCell(counter+2,floorcolumn+1),million.ToString()); //"H2/G2*1000000"
                counter++;
            }

           var range = xlSheet.get_Range(
                GetCell(2,1),
                GetCell(1+values.GetLength(0), values.GetLength(1)) //logikailag nem a sorok szamat akarom beirni, hanem a values objektum meretet
                );
            //values.GetLength(0) = lakasok.count,mert a values elso dimenziojat adja meg

            range.Value2 = values;
           
        }
        private string GetCell(int x, int y) //szovegkent adja vissza az excel koordinatakat
        {
            string ExcelCoordinate = "";
            int dividend = y;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                ExcelCoordinate = Convert.ToChar(65 + modulo).ToString() + ExcelCoordinate;
                dividend = (int)((dividend - modulo) / 26);
            }
            ExcelCoordinate += x.ToString();

            return ExcelCoordinate;
        }

        private void FormatTable()
        {
            Excel.Range headerRange = xlSheet.get_Range(GetCell(1, 1), GetCell(1, headers.Length));
            headerRange.Font.Bold = true;
            headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.EntireColumn.AutoFit();
            headerRange.RowHeight = 40;
            headerRange.Interior.Color = Color.LightBlue;
            headerRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);

            int lastrowid = xlSheet.UsedRange.Rows.Count;
            Excel.Range completetable = xlSheet.get_Range(GetCell(1, 1), GetCell(1, headers.Length));
            completetable.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
        }
    }
}
