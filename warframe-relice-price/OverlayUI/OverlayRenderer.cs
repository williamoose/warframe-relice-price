using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Rewards.Matching;
using Rewards.Processing;
using Rewards.Services;
using warframe_relice_price.OCRVision;
using warframe_relice_price.Utils;
using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.OverlayUI
{
	class OverlayRenderer
	{
		private readonly Canvas _overlayCanvas;
		private readonly WarframeMarketClient _marketClient;

		public OverlayRenderer(Canvas overlayCanvas)
		{
			_overlayCanvas = overlayCanvas;
			_marketClient = new WarframeMarketClient();
		}

        // Replace this later with actual rendering logic
        public void DrawRelicPrices(List<int?> prices)
        {
            _overlayCanvas.Children.Clear();
            double width = _overlayCanvas.ActualWidth;
            double height = _overlayCanvas.ActualHeight;

            int slots = prices.Count;

            if (width <= 0 || height <= 0) return;

            double rowY = height * OverlayConstants.RewardRowYPercent;
            double priceOffsetY = rowY - (height * OverlayConstants.PriceOffsetYPercent);

            double totalRowWidth = width * OverlayConstants.TotalRowWidthPercent; 
            double startX = (width - totalRowWidth) / 2;
            double slotWidth = totalRowWidth / slots;

			// string ocrText = ImageToText.multiPassOCR(bmp);
			// var items = RewardCounter.Count(ocrText);

			for (int i = 0; i < slots; i++)
			{
				double slotX = startX + i * slotWidth;

                string displayText = "—";

                int? price = prices[i];
				displayText = price is null ? "No listings" : $"{price}p";

                var priceText = new TextBlock
				{
					Text = displayText,
					FontWeight = FontWeights.Bold,
					Foreground = System.Windows.Media.Brushes.Gold,
					FontSize = height * 0.018,
					TextAlignment = TextAlignment.Center
				};

				priceText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

				Canvas.SetLeft(priceText, slotX + (slotWidth - priceText.DesiredSize.Width) / 2);
				Canvas.SetTop(priceText, priceOffsetY);

				_overlayCanvas.Children.Add(priceText);

				// --- Remove after 15 seconds ---
				_ = RemoveAfterDelayAsync(priceText, 1500);
			}
		}

		// Helper to remove a TextBlock after a delay
		private async Task RemoveAfterDelayAsync(UIElement element, int milliseconds)
		{
			await Task.Delay(milliseconds);

			// Remove safely on UI thread
			_overlayCanvas.Dispatcher.Invoke(() =>
			{
				_overlayCanvas.Children.Remove(element);
			});
		}

        private Rect PxToDip(System.Drawing.Rectangle px)
        {
            return new Rect(
                px.X / WarframeWindowInfo.DpiX,
                px.Y / WarframeWindowInfo.DpiY,
                px.Width / WarframeWindowInfo.DpiX,
                px.Height / WarframeWindowInfo.DpiY
            );
        }


        // Debug / boundary visualization
        public void DrawTestBoundary()
        {
            var pxRect = ScreenCaptureRow.GetDetectionBoxPx();
            var dipRect = PxToDip(pxRect);

            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = dipRect.Width,
                Height = dipRect.Height,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(rect, dipRect.X);
            Canvas.SetTop(rect, dipRect.Y);

            _overlayCanvas.Children.Add(rect);
        }

        
        public void DrawDebugRewardBoxes()
        {
            for (int i = 1; i <= 4; i++)
            {
                System.Drawing.Rectangle rectangle = ScreenCaptureRow.GetRewardBoxPx(i, 4);
                int left = rectangle.Left;
                DrawDebugRectPx(rectangle, System.Windows.Media.Colors.LimeGreen);
            }
        }

        

        public void DrawDebugRectPx(System.Drawing.Rectangle pxRect, System.Windows.Media.Color color)
        {
            var dip = PxToDip(pxRect);

            var r = new System.Windows.Shapes.Rectangle
            {
                Width = dip.Width,
                Height = dip.Height,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(r, dip.X);
            Canvas.SetTop(r, dip.Y);

            _overlayCanvas.Children.Add(r);
        }

        public void DrawItemsName(string text)
        {
            var itemNameText = new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Light,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 16
            };
            itemNameText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = itemNameText.DesiredSize.Width;

        }

        public void DrawAll()
        {
            _overlayCanvas.Children.Clear();
            DrawTestBoundary();
            DrawDebugRectPx(ScreenCaptureRow.GetDetectionBoxPx(), System.Windows.Media.Colors.Red);
        }
    }



}
