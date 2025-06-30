using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Ornek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static string connectionString = "Server=MK2OTO015;Database=OgrenciKayitDB;Integrated Security=True;";

        private void kaydet_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string kayit = "INSERT INTO OgrenciBilgiTable(oAdi,oSoyadi,eMail,oNumara) VALUES(@oAdi,@oSoyadi,@eMail,@oNumara)";
                    SqlCommand komut = new SqlCommand(kayit, connection);
                    komut.Parameters.AddWithValue("@oAdi", textBox1.Text);
                    komut.Parameters.AddWithValue("@oSoyadi", textBox2.Text);
                    komut.Parameters.AddWithValue("@eMail", textBox3.Text);
                    komut.Parameters.AddWithValue("@oNumara", textBox4.Text);
                    int result = komut.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Kayıt başarılı bir şekilde eklendi.");
                    }
                    else
                    {
                        MessageBox.Show("Kayıt eklenemedi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void duzenle_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView1.SelectedItem != null)
            {
                DataRowView selectedRow = dataGridView1.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    string yeniAdi = textBox1.Text;
                    string yeniSoyadi = textBox2.Text;
                    string yeniEmail = textBox3.Text;

                    // oNumara'yı dönüştürmeye çalışıyoruz
                    if (int.TryParse(textBox4.Text, out int oNumara))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            try
                            {
                                connection.Open();

                                // SQL UPDATE sorgusu
                                string updateSorgu = "UPDATE OgrenciBilgiTable SET oAdi = @oAdi, oSoyadi = @oSoyadi, eMail = @eMail WHERE oNumara = @oNumara";
                                SqlCommand komut = new SqlCommand(updateSorgu, connection);
                                komut.Parameters.AddWithValue("@oAdi", yeniAdi);
                                komut.Parameters.AddWithValue("@oSoyadi", yeniSoyadi);
                                komut.Parameters.AddWithValue("@eMail", yeniEmail);
                                komut.Parameters.AddWithValue("@oNumara", oNumara);

                                int result = komut.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    MessageBox.Show("Kayıt başarılı bir şekilde güncellendi.");

                                    // Güncellenen verileri DataGrid'de de güncelle
                                    selectedRow["oAdi"] = yeniAdi;
                                    selectedRow["oSoyadi"] = yeniSoyadi;
                                    selectedRow["eMail"] = yeniEmail;

                                    selectedRow.EndEdit();  // Değişiklikleri kabul et
                                }
                                else
                                {
                                    MessageBox.Show("Kayıt güncellenemedi.");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Güncelleme hatası: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lütfen geçerli bir numara girin.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için bir satır seçin.");
            }
        }

        private void sil_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView1.SelectedItem != null)
            {
                DataRowView selectedRow = dataGridView1.SelectedItem as DataRowView;

                if (selectedRow != null && int.TryParse(selectedRow["oNumara"].ToString(), out int oNumara))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            // SQL DELETE sorgusu
                            string silSorgu = "DELETE FROM OgrenciBilgiTable WHERE oNumara = @oNumara";
                            SqlCommand komut = new SqlCommand(silSorgu, connection);
                            komut.Parameters.AddWithValue("@oNumara", oNumara);

                            int result = komut.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Kayıt başarılı bir şekilde silindi.");

                                // DataGrid'den satırı sil
                                selectedRow.Delete();
                            }
                            else
                            {
                                MessageBox.Show("Kayıt silinemedi.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Silme hatası: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Geçerli bir kayıt seçin.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satır seçin.");
            }
        }

        private void kayitListel_Click(object sender, RoutedEventArgs e)
        {
            //veri çekme
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL sorgusu
                    string sorgu = "SELECT ID, oAdi, oSoyadi, eMail, oNumara FROM OgrenciBilgiTable";
                    SqlDataAdapter da = new SqlDataAdapter(sorgu, connection);

                    // DataTable nesnesi oluşturma ve veriyi doldurma
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // DataGridView'in veri kaynağı olarak DataTable'ı ayarlama
                    dataGridView1.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri çekme hatası: " + ex.Message);
                }
            }

        }
        private void dataGridView1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGridView1.SelectedItem != null)
            {
                // Seçilen satırı DataRowView olarak alıyoruz
                DataRowView selectedRow = dataGridView1.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    // Seçilen satırın verilerini TextBox'lara aktarıyoruz
                    textBox1.Text = selectedRow["oAdi"].ToString();
                    textBox2.Text = selectedRow["oSoyadi"].ToString();
                    textBox3.Text = selectedRow["eMail"].ToString();
                    textBox4.Text = selectedRow["oNumara"].ToString();
                }
            }
        }
    }
}