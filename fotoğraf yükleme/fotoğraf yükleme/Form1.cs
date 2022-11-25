using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;


namespace fotoğraf_yükleme
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-DHVOIQK\\SQLEXPRESS;Initial Catalog=images;Integrated Security=True; User Id=sa;Password=19830126;");
        OpenFileDialog file = new OpenFileDialog();
        DataTable dataTable= new DataTable();
        string imagepath;
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlDataAdapter adtr= new SqlDataAdapter("select * from images",baglanti);
            adtr.Fill(dataTable);
            dataGridView1.DataSource=dataTable;
            baglanti.Close();

            dataGridView1.RowTemplate.Height = 70;
        }

        private void btnsec_Click(object sender, EventArgs e)
        {
            file.Title = "resim seç";
            //file.Filter = "Jpeg Dosyaları(*.jpeg)|*.jpeg| Png Dosyaları(*.png)|*.png| Gif Dosyaları(*.gif)|*.gif| Tif Dosyaları(*.tif)|*.tif|";
            if (file.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(file.FileName);
                imagepath = file.FileName;
            }
        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream(imagepath, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                byte[] resim = binaryReader.ReadBytes((int)fileStream.Length);
                binaryReader.Close();
                fileStream.Close();
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into images(images) values(@images)", baglanti);
                komut.Parameters.Add("@images", SqlDbType.Image, resim.Length).Value = resim;
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("işlem başarılı");
            }
            catch (Exception)
            {
                MessageBox.Show("lütfen fotoğraf seçiniz");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from images where id=" +
                "'" + int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString()) + "'",baglanti);
            SqlDataReader dr=komut.ExecuteReader();
            if (dr.Read())
            {
                if (dr["images"]!=null)
                {
                    byte[] resim = new byte[0];
                    resim = (byte[])dr["images"];
                    MemoryStream memoryStream=new MemoryStream(resim);
                    pictureBox1.Image = Image.FromStream(memoryStream);
                    dr.Close(); 
                    komut.Dispose();
                    baglanti.Close() ;  
                }
            }
            baglanti.Close();   
        }
    }
}
