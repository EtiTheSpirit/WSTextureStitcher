using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace WSTextureStitcher {
	public class TextureStitcher {

		private static DirectoryInfo PromptForDirectory(string prompt, bool mustExist) {
			DirectoryInfo? target;
			do {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(prompt);
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Do not put quotes around this path. It can optionally end in a slash, doesn't matter.");
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write("> ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				string? path = Console.ReadLine();
				if (path == null) {
					Environment.Exit(1);
				}
				try {
					target = new DirectoryInfo(path);
					if (!target.Exists) {
						if (mustExist) {
							target = null;
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Invalid path! It doesn't exist!");
						} else {
							target.Create();
						}
					}
				} catch {
					target = null;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid path! I wasn't able to turn this into a valid directory.");
				}
			} while (target == null);
			return target;
		}

		private static void InsertImageIntoAtlas(Bitmap atlas, Bitmap src, int x, int y) {
			for (int srcY = 0; srcY < src.Height; srcY++) {
				for (int srcX = 0; srcX < src.Width; srcX++) {
					Color pixel = src.GetPixel(srcX, srcY);
					atlas.SetPixel(srcX + x, srcY + y, pixel);
				}
			}
		}

		private static bool PromptYN(string prompt) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(prompt);
			Console.Write(" (Y/N): ");
			while (true) {
				ConsoleKeyInfo key = Console.ReadKey();
				if (key.Key == ConsoleKey.Y) {
					return true;
				} else if (key.Key == ConsoleKey.N) {
					return false;
				} else {
					Console.Beep();
				}
			}
		}

		private static void InsertIfPresent(Bitmap atlas, Bitmap? src, int x, int y, string name) {
			if (src != null) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Added {name} to atlas.");
				InsertImageIntoAtlas(atlas, src, x, y);
			} else {
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Failed to add {name} to atlas (the file was not in the input folder).");
			}
		}

		public static void Main(string[] args) {
			DirectoryInfo inputDir = PromptForDirectory("Input the path to the folder containing your textures.", true);
			DirectoryInfo outputDir = PromptForDirectory("Input the path to the folder where you want the merged texture to go. You can use the same path.", false);
			// bool isNormal = PromptYN("Is this texture a normal map, and would you like to automatically convert them?");

			using Bitmap outImg = new Bitmap(1024, 1024);

			FileInfo[] files = inputDir.GetFiles("*.png");
			Bitmap? boots = null;
			Bitmap? buckle = null;
			Bitmap? head = null;
			Bitmap? face = null;
			Bitmap? gloves = null;
			Bitmap? misc = null;
			Bitmap? pants = null;
			Bitmap? shoulders = null;
			Bitmap? sleeves = null;
			Bitmap? chest = null;
			foreach (FileInfo file in files) {
				string name = file.Name.ToLower()[..(file.Name.Length-file.Extension.Length)];
				Bitmap bmp = new Bitmap(Image.FromFile(file.FullName));
				if (name.EndsWith("boots")) {
					boots = bmp;
				} else if (name.EndsWith("buckle")) {
					buckle = bmp;
				} else if (name.EndsWith("chest")) {
					chest = bmp;
				} else if (name.EndsWith("face")) {
					face = bmp;
				} else if (name.EndsWith("gloves")) {
					gloves = bmp;
				} else if (name.EndsWith("head")) {
					head = bmp;
				} else if (name.EndsWith("misc")) {
					misc = bmp;
				} else if (name.EndsWith("pants")) {
					pants = bmp;
				} else if (name.EndsWith("shoulders")) {
					shoulders = bmp;
				} else if (name.EndsWith("sleeves")) {
					sleeves = bmp;
				} else {
					bmp.Dispose();
				}
			}

			InsertIfPresent(outImg, shoulders, 0, 0, "shoulders");
			InsertIfPresent(outImg, pants, 0, 384, "pants");
			InsertIfPresent(outImg, boots, 0, 704, "boots");

			InsertIfPresent(outImg, misc, 320, 0, "misc");
			InsertIfPresent(outImg, head, 512, 0, "head");
			InsertIfPresent(outImg, chest, 320, 192, "chest");

			InsertIfPresent(outImg, face, 320, 768, "face");
			InsertIfPresent(outImg, buckle, 576, 768, "buckle");
			InsertIfPresent(outImg, gloves, 768, 576, "gloves");
			InsertIfPresent(outImg, sleeves, 768, 192, "sleeves");

			outImg.Save(Path.Combine(outputDir.FullName, "Atlas.png"), ImageFormat.Png);
		}
	}
}