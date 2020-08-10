using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Drawing.Imaging;

namespace DatagridView
{
    public partial class Form1 : Form
    {
        Control control;
        const int POSTURE_HEIGHT = 16;
        const int POSTURE_WIDTH = 8;
        int[,] force_value = new int[POSTURE_HEIGHT, POSTURE_WIDTH];

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        // Timer가 돌아갈 때마다 실행
        private void timer1_Tick(object sender, EventArgs e)
        {
            readData();
        }

        // DB값을 읽어서 Button에 값 넣어주기
        public void readData()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = "Server=192.168.0.3;Database=sde;Uid=mwf;Pwd=12345678;charset=euckr";
            conn.Open();
            MySqlCommand Com = new MySqlCommand("SELECT * FROM sde.a_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            MySqlDataReader rSet;
            rSet = Com.ExecuteReader();
            int cnt = 0;

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 4;

            Com = new MySqlCommand("SELECT * FROM sde.b_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 28;

            Com = new MySqlCommand("SELECT * FROM sde.c_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 4;

            Com = new MySqlCommand("SELECT * FROM sde.d_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 28;

            Com = new MySqlCommand("SELECT * FROM sde.e_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 4;

            Com = new MySqlCommand("SELECT * FROM sde.f_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 28;

            Com = new MySqlCommand("SELECT * FROM sde.g_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            cnt += 4;

            Com = new MySqlCommand("SELECT * FROM sde.h_section ORDER BY Checked_Time DESC LIMIT 1;", conn);
            rSet = Com.ExecuteReader();

            dataControl(rSet, cnt);

            rSet.Close();
            conn.Close();


            for (int i = 0; i < POSTURE_HEIGHT; i++)
            {
                for (int j = 0; j < POSTURE_WIDTH; j++)
                {
                    control = Controls["button" + (i * POSTURE_WIDTH + j + 1)];
                    force_value[i, j] = Int16.Parse(control.Text);
                }
            }

            poseDiscriminator();
        }

        public void dataControl(MySqlDataReader rSet, int cnt)
        {
            while (rSet.Read())
            {
                for (int i = 0; i < POSTURE_HEIGHT; i++)
                {
                    control = Controls["button" + (cnt + (i + 1) + (i / 4) * 4)];
                    control.Text = rSet.GetString(i + 2);
                    try
                    {
                        if (Int16.Parse(rSet.GetString(i + 2)) > int.Parse(highest.Text))
                        {
                            control.BackColor = Color.Red;
                        }
                        else if (Int16.Parse(rSet.GetString(i + 2)) > int.Parse(high.Text))
                        {
                            control.BackColor = Color.FromArgb(255, 255, 128, 128);
                        }
                        else if (Int16.Parse(rSet.GetString(i + 2)) > int.Parse(middle.Text))
                        {
                            control.BackColor = Color.FromArgb(255, 255, 128, 0);
                        }
                        else if (Int16.Parse(rSet.GetString(i + 2)) > int.Parse(low.Text))
                        {
                            control.BackColor = Color.Yellow;
                        }
                        else if (Int16.Parse(rSet.GetString(i + 2)) > int.Parse(lowest.Text))
                        {
                            control.BackColor = Color.FromArgb(255, 255, 255, 192);
                        }
                        else
                        {
                            control.BackColor = Color.Transparent;
                        }
                    }
                    catch (Exception ex)
                    {
                        chk_Text();
                    }
                }
            }
        }

        public void poseDiscriminator()
        {
            int[,] shoulder = new int[2, 2];
            int[,] pelvis = new int[2, 3];
            int sum_s = 0, sum_p = 0;
            int max = -1;
            int chk_s_i = 0, chk_p_i = 0, chk_s_j = 0, chk_p_j = 0;
            int dif = 0;
            int under_s = 0;
            int left = 0, right = 0;
            int dif_cnt = 0, dif_sum = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < POSTURE_WIDTH - 1; j++)
                {
                    sum_s = force_value[i, j] + force_value[i + 1, j] + force_value[i, j + 1] + force_value[i + 1, j + 1];
                    if (sum_s > max)
                    {
                        max = sum_s;
                        chk_s_i = i;
                        chk_s_j = j;
                    }
                }
            } // 어깨를 찾음 2X2

            for (int k = 0; k < 4; k++)
            {
                if (chk_s_j == 0)
                {
                    k++;
                }
                under_s += force_value[chk_s_i + 2, chk_s_j + k - 1];
                if (k < 2)
                {
                    left += force_value[chk_s_i + 2, chk_s_j + k - 1];
                }
                else
                {
                    right += force_value[chk_s_i + 2, chk_s_j + k - 1];
                }
                if (chk_s_j + k - 1 == POSTURE_WIDTH - 1)
                {
                    break;
                }
            } // 어깨 아래 1X4 찾음 (정자세와 오른쪽 왼쪽 비교를 위한)

            max = -1;

            for (int i = chk_s_i + 2; i < chk_s_i + 6; i++)
            {
                for (int j = 0; j < POSTURE_WIDTH - 2; j++)
                {
                    sum_p = force_value[i, j] + force_value[i + 1, j] + force_value[i, j + 1] + force_value[i + 1, j + 1] + force_value[i, j + 2] + force_value[i + 1, j + 2];
                    if (sum_p > max)
                    {
                        max = sum_p;
                        chk_p_i = i;
                        chk_p_j = j;
                    }
                }
            } // 골반을 찾음 2X3

            dif_cnt = chk_p_i - chk_s_i - 2;
            for (int i = 0; i < dif_cnt; i++)
            {
                dif_sum += (force_value[chk_s_i + 2 + i, chk_s_j] + force_value[chk_s_i + 2 + i, chk_s_j + 1]);
            }

            dif = chk_s_j - chk_p_j;
            if (dif == 2)
            {
                if (max > 2100)
                {
                    imgPosture.Image = Resource1.SOLDIER_LEFT;
                    textPosture.Text = "정자세_왼쪽";
                }
                else
                {
                    imgPosture.Image = Resource1.LEFT;
                    textPosture.Text = "왼쪽";
                }
            } // 왼쪽 판별
            else if (dif == -1)
            {
                if (max > 2100)
                {
                    imgPosture.Image = Resource1.SOLDIER_RIGHT;
                    textPosture.Text = "정자세_오른쪽";
                }
                else
                {
                    imgPosture.Image = Resource1.RIGHT;
                    textPosture.Text = "오른쪽";
                }
            } // 오른쪽 판별
            else
            {
                if (max > 1800)
                {
                    if (dif_cnt == 0)
                    {
                        imgPosture.Image = Resource1.FREEALLER;
                        textPosture.Text = "엎드린 자세";
                    }
                    else if (dif_sum / dif_cnt > 300)
                    {
                        imgPosture.Image = Resource1.FREEALLER;
                        textPosture.Text = "엎드린 자세";
                    }
                    else
                    {
                        imgPosture.Image = Resource1.SOLDIER;
                        textPosture.Text = "정자세";
                    }
                }
                else
                {
                    if (dif_cnt == 0)
                    {
                        imgPosture.Image = Resource1.FREEALLER;
                        textPosture.Text = "엎드린 자세";
                    }
                    else if (dif_sum / dif_cnt > 300)
                    {
                        imgPosture.Image = Resource1.FREEALLER;
                        textPosture.Text = "엎드린 자세";
                    }
                    else if (left < right)
                    {
                        imgPosture.Image = Resource1.RIGHT;
                        textPosture.Text = "오른쪽";
                    }
                    else
                    {
                        imgPosture.Image = Resource1.LEFT;
                        textPosture.Text = "왼쪽";
                    }
                }

            } // 정자세 판별
        }

        private void chk_Text()
        {
            if (lowest.Text.Equals(""))
            {
                lowest.Text = "0";
            }
            else if (low.Text.Equals(""))
            {
                low.Text = "0";
            }
            else if (middle.Text.Equals(""))
            {
                middle.Text = "0";
            }
            else if (high.Text.Equals(""))
            {
                high.Text = "0";
            }
            else
            {
                highest.Text = "0";
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            lowest.Text = "0";
            low.Text = "0";
            middle.Text = "0";
            high.Text = "0";
            highest.Text = "0";
        }

        private void lowest_Click(object sender, EventArgs e)
        {
            lowest.Text = "0";
        }

        private void low_Click(object sender, EventArgs e)
        {
            low.Text = "0";
        }

        private void middle_Click(object sender, EventArgs e)
        {
            middle.Text = "0";
        }

        private void high_Click(object sender, EventArgs e)
        {
            high.Text = "0";
        }

        private void highest_Click(object sender, EventArgs e)
        {
            highest.Text = "0";
        }

        private void setpath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            path.Text = dialog.SelectedPath;
        }

        private void screenshot_Click(object sender, EventArgs e)
        {
            screenShot();
        }

        public void screenShot()
        {
            if (path.Text.Equals(""))
            {
                MessageBox.Show("Set the path, first!");
                button102.Focus();
            }
            else
            {
                String fileName = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
                Rectangle form = this.Bounds;
                using (Bitmap bitmap = new Bitmap(380, 750))
                {
                    using (Graphics graphic = Graphics.FromImage(bitmap))
                    {
                        Point temp = new Point();
                        temp.X = this.Location.X + 10;
                        temp.Y = this.Location.Y + 35;
                        graphic.CopyFromScreen(temp, Point.Empty, form.Size);
                    }
                    bitmap.Save(path.Text + "\\" + fileName + ".jpg", ImageFormat.Jpeg);
                }
                MessageBox.Show("Image had been successfully saved!");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                screenShot();
            }

            else if (e.Control && e.KeyCode == Keys.R)
            {
                lowest.Text = "0";
                low.Text = "0";
                middle.Text = "0";
                high.Text = "0";
                highest.Text = "0";
            }
        }
    }
}