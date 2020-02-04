using System;
using System.Windows;
using KeyGenerate;
using KeyCommon;
using MahApps.Metro.Controls;

namespace KeyGenerateUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtProdID.Text.ToString(), out var ID))
            {
                txtLicense.Text = GenerateKey(ID);
            }
            else
            {
                txtLicense.Text = "ERROR: Product ID must be an integer.";
            }
        }

        private string GenerateKey(int ID)
        {
            var keyGenerator = new KeyGenerator();

            var keyBytes = new[] {

                new KeyByteSet(1, 254, 122, 96),
                new KeyByteSet(2, 54, 124, 222),
                new KeyByteSet(3, 119, 142, 132),
                new KeyByteSet(4, 128, 122, 10),
                new KeyByteSet(5, 165, 15, 132),
                new KeyByteSet(6, 128, 175, 213),
                new KeyByteSet(7, 7, 244, 132 ),
                new KeyByteSet(8, 128, 122, 251)
            };

            var key = keyGenerator.MakeKey(ID, keyBytes);
            return key;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtLicense.Text = "";
            txtProdID.Text = "";
        }
    }
}