using MNDSoap.Entities;
using MNDSoap.MNBServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace MNDSoap
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> currencies = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();
            cbxCurrency.DataSource = currencies;
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetCurrenciesRequestBody request = new GetCurrenciesRequestBody();
            var response = mnbService.GetCurrencies(request);
            string result = response.GetCurrenciesResult;
            XmlDocument vxml = new XmlDocument();
            vxml.LoadXml(result);

            foreach (XmlElement item in vxml.DocumentElement.FirstChild.ChildNodes)
            {
                currencies.Add(item.InnerText);
            }





            RefreshData();
        }

        private void RefreshData()
        {
            if (cbxCurrency.SelectedItem == null)
            {
                return;
            }
            Rates.Clear();
            string xmlstring = Consume(); //a webszolg. hívás eredményét betöltjük az xmlstringbe és ezt adjuk át a LoadXmlnek
            LoadXml(xmlstring);
            dataGridView1.DataSource = Rates;
            Charting();
        }

        private void Charting()
        {
            chartRateData.DataSource = Rates;
            Series series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;
            var chartAera = chartRateData.ChartAreas[0];
            chartAera.AxisX.MajorGrid.Enabled = false;
            chartAera.AxisY.MajorGrid.Enabled = false;
            chartAera.AxisY.IsStartedFromZero = false;
            var legend = chartRateData.Legends[0];
            legend.Enabled = false;
        }

        private void LoadXml(string input)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(input);
            foreach (XmlElement item in xml.DocumentElement)
            {
                RateData r = new RateData();
                r.Date = DateTime.Parse(item.GetAttribute("date")); //a date nevet az xml fileból tudjuk, pontosan kell írni
                XmlElement child = (XmlElement)item.FirstChild; //FirstChild = ChildNodes(0)
                if (child == null)
                {
                    continue;
                }
                r.Currency = child.GetAttribute("curr");
                r.Value = decimal.Parse(child.InnerText);
                int unit = int.Parse(child.GetAttribute("unit"));
                if (unit!=0)
                {
                    r.Value = r.Value / unit;
                }
                Rates.Add(r);
            }
        }

        string Consume()
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody();
            request.currencyNames = cbxCurrency.SelectedItem.ToString();//"EUR";
            request.startDate = dateTimePickerStart.Value.ToString("yyyy-MM-dd");//"2020-01-01";
            request.endDate = dateTimePickerEnd.Value.ToString("yyyy-MM-dd");//"2020-06-30";
            var response = mnbService.GetExchangeRates(request);
            string result = response.GetExchangeRatesResult;
            return result;
        }

        private void filterChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
