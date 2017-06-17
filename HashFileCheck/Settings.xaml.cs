using HashCheck.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HashFileCheck
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (HashParams.EncMethod item in Enum.GetValues(typeof(HashParams.EncMethod)))
                this.Lv2.Items.Add(new HashParams { Name = item, IsChecked = false });
        }

        private void Lv2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (HashParams item in e.AddedItems)
                item.IsChecked = !item.IsChecked;

            this.Lv2.Items.Refresh();

        }

        // button Reset
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // restore default setting
            foreach (HashParams i in this.Lv2.Items)
                i.IsChecked = (i.Name == HashParams.EncMethod.CRC || i.Name == HashParams.EncMethod.MD5 || i.Name == HashParams.EncMethod.SHA1) ?
                    true :
                    false;

            this.Lv2.Items.Refresh();
        }

        // button Cancel
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // button Select All
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (HashParams item in this.Lv2.Items)
                item.IsChecked = true;

            this.Lv2.Items.Refresh();
        }

        // button Select None
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (HashParams item in this.Lv2.Items)
                item.IsChecked = false;

            this.Lv2.Items.Refresh();
        }

        // button OK
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            List<HashParams> lstNew = new List<HashParams>();

            foreach (HashParams item in this.Lv2.Items)
                if (item.IsChecked)
                    lstNew.Add(item);

            Params.lstPrms = lstNew;
            this.DialogResult = true;
            this.Close();
        }
    }
}
