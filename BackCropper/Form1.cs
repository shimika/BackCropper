using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace BackCropper {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
			this.MouseWheel += new MouseEventHandler(Mouse_Wheel);
		}
		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back))) {
				e.Handled = true;
			}
		}
		private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
			if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back))) {
				e.Handled = true;
			}
		}
		double size, RectResol;
		Point Resol;
		Size nowSize, RectSize, SaveSize;
		Image original, resized;
		Point startPoint;
		private int x = 0;
		private int y = 0;
		double Multiple;
		bool isLoaded = false;
		bool saving = false;
		string fname;

		public Bitmap ResizeImage(Image img, int resizedW, int resizedH) {
			Bitmap bmp = new Bitmap(resizedW, resizedH);
			Graphics graphic = Graphics.FromImage((Image)bmp);
			graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphic.DrawImage(img, 0, 0, resizedW, resizedH);
			graphic.Dispose();
			return bmp;
		}

		private void Mouse_Wheel(object sender, MouseEventArgs e) {
			if (!isLoaded) { return; }
			Size tResult;

			if ((e.Delta / 120) > 0) {
				tResult = new Size(RectSize.Width + 10, (RectSize.Width + 10) * SaveSize.Height / SaveSize.Width);
				if (tResult.Width > nowSize.Width || tResult.Height > nowSize.Height) {
					if (tResult.Width > nowSize.Width) {
						tResult = new Size(nowSize.Width, (nowSize.Width) * SaveSize.Height / SaveSize.Width);
					} else {
						tResult = new Size((nowSize.Height) * SaveSize.Width / SaveSize.Height, nowSize.Height);
					}
				}
				RectSize = tResult;
				this.Text = original.Width + " * " + original.Height + " : " + RectSize.Width + " * " + RectSize.Height;
			} else {
				tResult = new Size(RectSize.Width - 10, (RectSize.Width - 10) * SaveSize.Height / SaveSize.Width);
				if (tResult.Width <= 9 || tResult.Height <= 9) { return; }
				RectSize = tResult;
				this.Text = original.Width + " * " + original.Height + " : " + RectSize.Width + " * " + RectSize.Height;
			}
			pictureBox1.Invalidate();
		}

		private void makePicture() {
			label1.Text = this.Text = original.Width + " * " + original.Height;
			isLoaded = true; saving = false;
			this.Activate();
			this.Focus();
			try {
				int Width = original.Width;
				int Height = original.Height;
				double temp;
				double WperH = (double)Width / (double)Height;

				if (WperH > size) {
					temp = (double)Resol.X * (double)Height / (double)Width;
					resized = ResizeImage(original, Resol.X, (int)temp);
					nowSize = pictureBox1.Size = new Size(Resol.X, (int)temp);
				} else {
					temp = (double)Resol.Y * (double)Width / (double)Height;
					resized = ResizeImage(original, (int)temp, Resol.Y);
					nowSize = pictureBox1.Size = new Size((int)temp, Resol.Y);
				}
				pictureBox1.Image = resized;
				Multiple = (double)original.Width / (double)nowSize.Width;

				//MessageBox.Show(Resol[index].X + "\n" + Resol[index].Y + "\n" + size[index] + "\n" + temp);
				fullSelect();
				pictureBox1.Invalidate();
			} catch {
				// Resize Error...
			}
		}

		private bool isRealPicture() {
			try {
				StreamReader sr = new StreamReader(fname);
				original = Image.FromStream(sr.BaseStream);
				sr.Close();
			} catch {
				MessageBox.Show("Not Available Picture", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}


		string filename;
		string[] splitstr;

		private void button1_Click(object sender, EventArgs e) {
			if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }
			fname = openFileDialog1.FileName;
			splitstr = fname.Split('\\');
			splitstr = splitstr[splitstr.Length - 1].Split('.');
			filename = "";
			for (int i = 0; i < splitstr.Length - 1; i++) {
				filename += splitstr[i];
				if (i == splitstr.Length - 2) { break; }
				filename += ".";
			}
			saveFileDialog1.FileName = filename;
			if (isRealPicture()) { makePicture(); }
		}

		Label[] lbList;
		int[] cutWidth, cutHeight;
		string[] cutList;

		private void Form1_Load(object sender, EventArgs e) {
			this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - 1050, Screen.PrimaryScreen.WorkingArea.Top + 100);
			original = pictureBox1.Image;

			size = 1.6;
			Resol = new Point(800, 500);
			nowSize = new Size(800, 500);

			RectSize = new Size(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
			SaveSize = RectSize;
			RectResol = 0.56666666666666666666666666666667;
			saving = true;

			pictureBox1.Cursor = Cursors.Cross;

			cutList = new string[] { "캐릭터 사진 (100*120)", "애니메이션 커버 (375*148)", "모바일 배경 (272*480)", "프로필 사진 (320*320)", "아이폰 배경 (320*480)", "모바일 배경 (480*800)", "아이폰 배경 (640*960)", "모바일 배경 (720*1280)", "배경화면 (1280*720)", "배경화면 (1366*768)" };
			cutWidth = new int[] { 100, 375, 272, 320, 320, 480, 640, 720, 1280, 1366 };
			cutHeight = new int[] { 120, 148, 480, 320, 480, 800, 960, 1280, 720, 768 };
			lbList = new Label[cutList.Count()];
			for (int i = 0; i < cutList.Length; i++) {
				lbList[i] = new Label();
				lbList[i].Name = "lbList_" + i;
				lbList[i].Font = new Font("맑은 고딕", 9);
				lbList[i].Size = new Size(170, 23);
				lbList[i].Location = new Point(10, i * 30 + 25);
				lbList[i].Text = cutList[i];
				lbList[i].MouseEnter += new EventHandler(lbList_MouseEnter);
				lbList[i].MouseLeave += new EventHandler(lbList_MouseLeave);
				lbList[i].MouseClick += new MouseEventHandler(lbList_MouseClick);
				lbList[i].Cursor = Cursors.Hand;
				groupBox1.Controls.Add(lbList[i]);
			}
		}

		void lbList_MouseClick(object sender, MouseEventArgs e) {
			int idx = Convert.ToInt32(((Label)sender).Name.Split('_')[1]);
			textBox1.Text = cutWidth[idx].ToString();
			textBox2.Text = cutHeight[idx].ToString();
		}

		void lbList_MouseLeave(object sender, EventArgs e) {
			((Label)sender).ForeColor = Color.Black;
			((Label)sender).Font = new Font("맑은 고딕", 9, FontStyle.Regular);
		}

		void lbList_MouseEnter(object sender, EventArgs e) {
			((Label)sender).ForeColor = Color.OrangeRed;
			((Label)sender).Font = new Font("맑은 고딕", 9, FontStyle.Underline);
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e) {
			if (saving) { return; }
			Pen skyBluePen = new Pen(Brushes.DeepSkyBlue);

			int sX = x - RectSize.Width / 2, sY = y - RectSize.Height / 2;

			if (sX < 0) { sX = 0; }
			if (sX + RectSize.Width > nowSize.Width) { sX = nowSize.Width - RectSize.Width; }
			if (sY < 0) { sY = 0; }
			if (sY + RectSize.Height > nowSize.Height) { sY = nowSize.Height - RectSize.Height; }

			Brush brush = new SolidBrush(Color.FromArgb(160, 0, 162, 232));
			e.Graphics.FillRectangle(brush, sX, sY, RectSize.Width, RectSize.Height);
			//e.Graphics.DrawRectangle(skyBluePen, sX, sY, RectSize.Width - 1, RectSize.Height - 1);
			startPoint = new Point(sX, sY);
		}

		private ImageCodecInfo getEncoderInfo(string mimeType) {
			// Get image codecs for all image formats 
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			// Find the correct image codec 
			for (int i = 0; i < codecs.Length; i++) {
				if (codecs[i].MimeType == mimeType) { return codecs[i]; }
			}
			return null;
		}

		private void saveJpeg(string path, Bitmap img, long quality) {
			// Encoder parameter for image quality

			EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			// Jpeg image codec 
			ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");
			if (jpegCodec == null) { return; }
			EncoderParameters encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = qualityParam;
			img.Save(path, jpegCodec, encoderParams);
		}

		private void fullSelect() {
			double temp;
			if (RectResol > (double)nowSize.Width / (double)nowSize.Height) {
				temp = (double)nowSize.Width * (double)SaveSize.Height / (double)SaveSize.Width;
				RectSize = new Size(nowSize.Width, (int)temp);
			} else {
				temp = (double)nowSize.Height * (double)SaveSize.Width / (double)SaveSize.Height;
				RectSize = new Size((int)temp, nowSize.Height);
			}
			pictureBox1.Invalidate();
		}

		private void textBox_TextChanged(object sender, EventArgs e) {
			try {
				RectSize = SaveSize = new Size(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
				RectResol = (double)RectSize.Width / (double)RectSize.Height;
				fullSelect();
			} catch {
			}

			//MessageBox.Show("Text Changed");
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
			x = e.X; y = e.Y;
			pictureBox1.Invalidate();
		}

		Bitmap save;
		private void pictureBox1_Click(object sender, EventArgs e) {
			if (!isLoaded) {
				button1_Click(null, null);
				return;
			}
			double tStartX, tStartY, tCropWidth, tCropHeight;
			int StartX, StartY, CropWidth, CropHeight;

			tStartX = (double)startPoint.X * Multiple;
			tStartY = (double)startPoint.Y * Multiple;
			tCropWidth = (double)RectSize.Width * Multiple;
			tCropHeight = (double)RectSize.Height * Multiple;
			StartX = (int)tStartX;
			StartY = (int)tStartY;
			CropWidth = (int)tCropWidth;
			CropHeight = (int)tCropHeight;

			//MessageBox.Show(CropWidth.ToString() + "\n" + CropHeight.ToString());

			Rectangle rect = new Rectangle(StartX, StartY, CropWidth, CropHeight);
			Bitmap OriginalImage = new Bitmap(original, original.Width, original.Height);
			Bitmap img = new Bitmap(CropWidth, CropHeight);
			Graphics g = Graphics.FromImage(img);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);

			if (textBox1.Text == "375" && textBox2.Text == "148") {
				//g.FillRectangle(new SolidBrush(Color.FromArgb(120, 255, 255, 255)), 0, 0, CropWidth, CropHeight);
			}


			save = ResizeImage(img, SaveSize.Width, SaveSize.Height);
			Image PreView;

			double temp;
			double WperH = (double)SaveSize.Width / (double)SaveSize.Height;

			if (WperH > (double)nowSize.Width / (double)nowSize.Height) {
				temp = (double)nowSize.Width * (double)SaveSize.Height / (double)SaveSize.Width;
				PreView = ResizeImage(save, nowSize.Width, (int)temp);
			} else {
				temp = (double)nowSize.Height * (double)SaveSize.Width / (double)SaveSize.Height;
				PreView = ResizeImage(save, (int)temp, nowSize.Height);
			}

			pictureBox1.Image = PreView;
			saving = true;
			pictureBox1.Invalidate();

			Timer saveTimer = new Timer();
			saveTimer.Interval = 1500; saveTimer.Tick += saveTimer_Tick;
			saveTimer.Start();

			//pictureBox1.Width = PreView.Width;
			//pictureBox1.Height = PreView.Height;

			//MessageBox.Show(startPoint.X.ToString() + "\n" + startPoint.Y.ToString() + "\n" + Multiple.ToString());
		}

		void saveTimer_Tick(object sender, EventArgs e) {
			((Timer)sender).Stop();
			saveFileDialog1.FileName = filename;
			//MessageBox.Show(filename);
			if (saveFileDialog1.ShowDialog() != DialogResult.OK) {
				pictureBox1.Image = resized; saving = false;
				return;
			}
			//Image PreView = ResizeImage(img, RectSize.Width, RectSize.Height);

			saveJpeg(saveFileDialog1.FileName, save, 100);

			//save.Save(saveFileDialog1.FileName);
			pictureBox1.Image = resized;
			saving = false;
		}

		private void Form1_DragDrop(object sender, DragEventArgs e) {
			var directoryName = (string[])e.Data.GetData(DataFormats.FileDrop);
			fname = directoryName[0];
			splitstr = fname.Split('\\');
			splitstr = splitstr[splitstr.Length - 1].Split('.');
			filename = "";
			for (int i = 0; i < splitstr.Length - 1; i++) {
				filename += splitstr[i];
				if (i == splitstr.Length - 2) { break; }
				filename += ".";
			}
			saveFileDialog1.FileName = filename;
			if (isRealPicture()) { makePicture(); }
		}
		private void Form1_DragEnter(object sender, DragEventArgs e) { e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; }
		private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { label1.Text = e.Control.ToString(); if (e.Control && e.KeyCode == Keys.O) { button1_Click(null, null); } }
		private void textBox1_KeyDown(object sender, KeyEventArgs e) { if (e.Control && e.KeyCode == Keys.O) { button1_Click(null, null); } }
		private void textBox2_KeyDown(object sender, KeyEventArgs e) { if (e.Control && e.KeyCode == Keys.O) { button1_Click(null, null); } }
		private void button2_Click(object sender, EventArgs e) { Application.Exit(); }
	}
}
