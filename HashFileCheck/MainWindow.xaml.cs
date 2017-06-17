using HashCheck.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HashFileCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SOURCE-FILE
        private string pathToFile = null;

        // type of icon depends of comparing hash of two files
        private enum IconType { Question, Good, Bad };

        public MainWindow()
        {
            InitializeComponent();

            this.BindWithFile();
        }

        // binding program with file which hash will be computed
        private void BindWithFile()
        {
            try
            {
                // program is running with params (from context menu of file)
                this.pathToFile = Environment.GetCommandLineArgs()[1];
            }
            catch (IndexOutOfRangeException)
            {
                // program is running without binding file (directly)
                OpenFileDialog ofd = new OpenFileDialog();

                if (ofd.ShowDialog() == true)
                    this.pathToFile = ofd.FileName;
                else
                    this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LoadIcon(IconType.Question);

                this.SyncWork();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // load icon
        private void LoadIcon(IconType it)
        {
            this.Img1.Source = new BitmapImage(new Uri(string.Format("Icon\\{0}.png", it), UriKind.Relative));
        }

        void Calculate(object obj)
        {
            try
            {
                var vLvItem = this.CalcHash(((ParamsToThread)obj).Path, ((ParamsToThread)obj).Method);
                this.Dispatcher.Invoke(new Action(() => this.Lv1.Items.Add(vLvItem)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SyncWork()
        {
            foreach (var item in Params.lstPrms)
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.Calculate), new ParamsToThread { Path = this.pathToFile, Method = item.Name });

        }

        // settings
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Settings set = new Settings();
                if (set.ShowDialog() == true)
                {
                    this.Lv1.Items.Clear();
                    this.SyncWork();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // choosing method to calculate hash
        private HashParams CalcHash(string path, HashParams.EncMethod type = HashParams.EncMethod.CRC)
        {
            HashAlgorithm ha;

            switch (type)
            {
                case HashParams.EncMethod.CRC:
                    ha = new Crc32();
                    break;

                case HashParams.EncMethod.MD5:
                    ha = MD5.Create();
                    break;

                case HashParams.EncMethod.SHA1:
                    ha = SHA1.Create();
                    break;

                case HashParams.EncMethod.SHA256:
                    ha = SHA256.Create();
                    break;

                case HashParams.EncMethod.SHA512:
                    ha = SHA512.Create();
                    break;

                default:
                    return null;
            }

            // compute the hash of the file
            return new HashParams { Name = type, Value = this.ComputeHash(path, ha) };
        }

        // main method of calculating hash and return hex-string
        private string ComputeHash(string pathToFile, HashAlgorithm ha)
        {
            string hash = string.Empty;

            try
            {
                using (FileStream fs = File.OpenRead(pathToFile))
                    foreach (byte b in ha.ComputeHash(fs))
                        hash += b.ToString("x2").ToUpper();

                // return result
                return hash;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        // button Compare
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();

                if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    var selectItem = this.Lv1.SelectedItem as HashParams;

                    // calc and output hash of choosen file in the textbox
                    this.Tb1.Text = this.CalcHash(ofd.FileName, selectItem?.Name ?? HashParams.EncMethod.CRC).Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // changing ico depends of result of comparing
        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string selectedHash = (this.Lv1.SelectedItem as HashParams)?.Value ?? ((HashParams)this.Lv1.Items[0]).Value;

            if (selectedHash == this.Tb1.Text)
                this.LoadIcon(IconType.Good);
            else
                this.LoadIcon(IconType.Bad);
        }

        // context menu 
        private void Lv1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            double dSizeOfFrame = 23d;
            double dSizeOfMargin = 7.6d;

            // cursor position of click in ListView
            Point p = Mouse.GetPosition(this.Lv1);

            double dHeightOfClick = dSizeOfFrame + (this.Lv1.FontSize + dSizeOfMargin) * this.Lv1.Items.Count;

            if (p.Y > dHeightOfClick || p.Y < dSizeOfFrame)
            {
                this.Lv1.SelectedIndex = -1;
                this.FormingContextMenu(false);
            }
            else
                this.FormingContextMenu(true);
        }

        // context menu in ListView
        private void FormingContextMenu(bool isSelectedElement)
        {
            ContextMenu cm = new ContextMenu();

            // copy to clipboard
            MenuItem miCopy = new MenuItem();
            miCopy.Click += MiCopy_Click;

            // save to file
            MenuItem miSave = new MenuItem();
            miSave.Click += MiSave_Click;

            // settings
            MenuItem miSettings = new MenuItem();
            miSettings.Header = "Settings";
            miSettings.Click += MiSettings_Click; ;


            if (isSelectedElement)
            {
                // for selected item
                miCopy.Header = "Copy to clipboard";
                miSave.Header = "Save to file...";
            }
            else
            {
                // for all
                miCopy.Header = "Copy all to clipboard";
                miSave.Header = "Save all to file...";
            }

            cm.Items.Add(miCopy);
            cm.Items.Add(miSave);
            cm.Items.Add(miSettings);

            this.Lv1.ContextMenu = cm;
        }

        // settings
        private void MiSettings_Click(object sender, RoutedEventArgs e)
        {
            this.TextBlock_MouseLeftButtonUp(sender, null);
        }

        // saving hashes to file
        private void MiSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.FileName = string.Format($"Hash of ({System.IO.Path.GetFileName(this.pathToFile)}).txt");
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sfd.OpenFile()))
                {
                    if (this.Lv1.SelectedIndex == -1)
                        // saving hashes of all methods
                        foreach (HashParams item in this.Lv1.Items)
                            sw.WriteLine($"{item.Name}: {item.Value}");
                    else
                        // saving hash of selected method
                        sw.WriteLine($"{((HashParams)this.Lv1.SelectedItem).Name}: {((HashParams)this.Lv1.SelectedItem).Value}");
                }
            }
        }

        // copying hash to clipboard
        private void MiCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.Lv1.SelectedIndex == -1)
            {
                // saving hashes of all encrypt methods
                StringBuilder sb = new StringBuilder();
                foreach (HashParams item in this.Lv1.Items)
                    sb.AppendLine(string.Format($"{item.Name}:{item.Value}"));

                // copy all hashes
                Clipboard.SetText(sb.ToString());
            }
            else
                Clipboard.SetText(((HashParams)this.Lv1.SelectedItem).Value);
        }
    }
}
