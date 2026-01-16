using System;
using System.Drawing;
using Tesseract;
using System.Diagnostics;

using System.IO;
using warframe_relice_price.Utils;

namespace warframe_relice_price.OCRVision
{
    static class ImageToText
    {

		public static string ConvertImageToText(Bitmap img)
		{
			Pix pix = PixConverter.ToPix(img);

			TesseractObject.tessEngine.SetVariable(
				"tessedit_char_whitelist",
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 ");

			using var page = TesseractObject.tessEngine.Process(pix);
			string text = page.GetText().Trim();

			return text;
		}

		public static int ScoreText (string text)
        {
            if (string.IsNullOrWhiteSpace(text))    
            {
                return 0;
            }
            
            int score = 0;
            score += text.Count(char.IsLetter);
            score -= text.Count(c => !char.IsLetter(c) && c != ' ');

            return score;
        }

		public static string multiPassOCR(Bitmap original)
		{
			var results = new List<(string Text, int Score)>();

			// 1️⃣ Raw OCR
			string raw = ImageToText.ConvertImageToText(original);
			results.Add((raw, ScoreText(raw)));

			// 2️⃣ Grayscale OCR
			string gray = ImageToText.ConvertImageToText(ScreenCaptureRow.toGrayScale(original));
			results.Add((gray, ScoreText(gray)));

			// 3️⃣ Threshold OCR
			string grayThresh = ImageToText.ConvertImageToText(
				ScreenCaptureRow.Threshold(ScreenCaptureRow.toGrayScale(original), 160)
			);
			results.Add((grayThresh, ScoreText(grayThresh)));

			// 4️⃣ Threshold OCR again (seems duplicated in your code; might be unnecessary)
			string grayThresh2 = ImageToText.ConvertImageToText(
				ScreenCaptureRow.Threshold(ScreenCaptureRow.toGrayScale(original), 160)
			);
			results.Add((grayThresh2, ScoreText(grayThresh2)));

			var bestResult = results.OrderByDescending(r => r.Score).First();

			return bestResult.Text;
		}

		public static string singleBoxOCR(int boxIndex, int numRewards)
		{
			var box = ScreenCaptureRow.GetRewardBoxPx(boxIndex, numRewards);
			var screenBox = ScreenCaptureRow.ToScreenRect(box);

			using var bmp = ScreenCaptureRow.captureRegion(screenBox);
			string item = multiPassOCR(bmp);

			saveDebugImage(bmp, $"reward_box_{boxIndex}");

			return item;
		}


		public static void saveDebugImage(Bitmap img, string tag)
        {
            string debugDir = Path.Combine(AppContext.BaseDirectory, "debug_images");
            if (!Directory.Exists(debugDir))
            {
                Directory.CreateDirectory(debugDir);
            }
            string filePath = Path.Combine(
                debugDir,
                $"{DateTime.Now:HHmmss_fff}_{tag}.png");

            img.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }


    }
}
