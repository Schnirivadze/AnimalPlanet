#pragma warning disable CA1416 // Validate platform compatibility
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Xml;

namespace AnimalPlanet
{
	internal class cardgen
	{
		static Font font;
		static Font midfont;
		static Font bigfont;
		static Bitmap card;
		static string icopath;
		static string imgpath;
		static Dictionary<string,string> Icons = new Dictionary<string,string>();
		static void Main(string[] args)
		{

			XmlDocument xmlDoc = new XmlDocument();
			// Load the XML file
			xmlDoc.Load("./src/cdata.xml");
			imgpath = xmlDoc.SelectSingleNode("/root/media-info/img-path").InnerText;
			icopath = xmlDoc.SelectSingleNode("/root/media-info/ico-path").InnerText;
			string fontPath = xmlDoc.SelectSingleNode("/root/media-info/font-path").InnerText;


			Console.OutputEncoding = Encoding.UTF8;
			var foo = new PrivateFontCollection();
			foo.AddFontFile(fontPath);
			font = new Font((FontFamily)foo.Families[0], 14f);
			midfont = new Font((FontFamily)foo.Families[0], 17f);
			bigfont = new Font((FontFamily)foo.Families[0], 33f);


			XmlNodeList cardNodes = xmlDoc.SelectNodes("/root/cards/card");
			int node = 0;
			foreach (XmlNode cardNode in cardNodes)
			{
				Console.SetCursorPosition(0, 0);
				string name = cardNode.SelectSingleNode("name").InnerText;
				int speed = int.Parse(cardNode.SelectSingleNode("speed").InnerText);
				int strength = int.Parse(cardNode.SelectSingleNode("strength").InnerText);
				string[] zones = cardNode.SelectSingleNode("zones").InnerText.Split(' ');
				string[] kingdoms = cardNode.SelectSingleNode("kingdoms").InnerText.Split(' ');
				bool rare = (int.Parse(cardNode.SelectSingleNode("rare").InnerText)==1)? true : false;


				MakeCard(name, speed, strength, zones, kingdoms, rare);
				Console.Write(node + "/" + cardNodes.Count);
			}
			Console.SetCursorPosition(0, 0);
			Console.WriteLine("Done");
		}
		static void MakeCard(string name,int speed,int strength, string[] zones, string[] kingdoms, bool rare) 
		{
			card = new Bitmap(510, 612);
			Graphics g = Graphics.FromImage(card);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

			g.FillRectangle(Brushes.White, 0, 0, 510, 612);
			g.FillRectangle(Brushes.Black, 0, 495, 510, 21);
			//---------------------------------------------------------------------------name
			var strwidth = g.MeasureString(name, font).Width;
			g.DrawString(name, font, Brushes.White, (512 - strwidth) / 2, 492);
			//---------------------------------------------------------------------------stength
			g.DrawString("СИЛА", midfont, Brushes.Black, 28, 533);
			g.DrawString(strength.ToString(), midfont, Brushes.Black, 110, 533);
			//---------------------------------------------------------------------------speed
			g.DrawString("СКОР", midfont, Brushes.Black, 28, 567);
			g.DrawString(speed.ToString(), midfont, Brushes.Black, 110, 567);
			//---------------------------------------------------------------------------image
			try { g.DrawImage(Image.FromFile(imgpath + name + ".png"),0,0,510,495);}
			catch (Exception ex) { Console.WriteLine("Cant find " + name + ".png"); }
			//---------------------------------------------------------------------------rare
			if (rare) g.DrawString("R", bigfont, Brushes.Black, 23, 23);
			//---------------------------------------------------------------------------orgy
			int height = 20;
			
			foreach (string kingdom in kingdoms)
			{
				g.DrawImage(Image.FromFile(icopath + kingdom + ".png"), 460, height, 25, 25);
				g.DrawString(kingdom.ToUpper(), midfont, Brushes.Black, 510 - g.MeasureString(kingdom.ToUpper(), midfont).Width - 60, height + 2);
				height += 30;
			}
			foreach (string zone in zones)
			{
				g.DrawImage(Image.FromFile(icopath + zone + ".png"), 460, height, 25, 25);
				g.DrawString(zone.ToUpper(), midfont, Brushes.Black, 510 - g.MeasureString(zone.ToUpper(), midfont).Width - 60, height + 2);
				height += 30;
			}
			//---------------------------------------------------------------------------save
			card.Save("./cards/"+name+".png",ImageFormat.Png);
			g.Flush();
		}
	}
}
