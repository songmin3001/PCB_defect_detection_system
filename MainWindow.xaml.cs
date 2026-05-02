using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

public partial class MainWindow : Window
{
    private readonly ApiService _apiService = new ApiService();
    private string _selectedImagePath;
    private double _imageScaleX, _imageScaleY, _imageOffsetX, _imageOffsetY;

    public MainWindow() => InitializeComponent();

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp"
        };
        if (dialog.ShowDialog() != true) return;

        _selectedImagePath = dialog.FileName;
        var bitmap = new BitmapImage(new Uri(_selectedImagePath));
        PreviewImage.Source = bitmap;
        OverlayCanvas.Children.Clear();
        DetectionList.ItemsSource = null;
        StatusText.Text = "이미지 로드 완료";
        CountText.Text = "";
        StatusBorder.Background = new SolidColorBrush(Color.FromRgb(42, 42, 62));
        InspectButton.IsEnabled = true;
    }

    private async void InspectButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedImagePath)) return;

        InspectButton.IsEnabled = false;
        LoadingText.Visibility = Visibility.Visible;
        OverlayCanvas.Children.Clear();

        try
        {
            var result = await _apiService.PredictAsync(_selectedImagePath);
            DisplayResult(result);
        }
        catch (Exception ex)
        {
            StatusText.Text = "오류 발생";
            CountText.Text = ex.Message;
            StatusBorder.Background = new SolidColorBrush(Color.FromRgb(80, 30, 30));
        }
        finally
        {
            InspectButton.IsEnabled = true;
            LoadingText.Visibility = Visibility.Collapsed;
        }
    }

    private void DisplayResult(PredictResponse result)
    {
        // 상태 표시
        if (result.DefectFound)
        {
            StatusText.Text = "불량 검출됨";
            CountText.Text = $"총 {result.Count}개 불량 발견";
            StatusBorder.Background = new SolidColorBrush(Color.FromRgb(80, 30, 30));
        }
        else
        {
            StatusText.Text = "정상";
            CountText.Text = "불량 없음";
            StatusBorder.Background = new SolidColorBrush(Color.FromRgb(30, 70, 45));
        }

        DetectionList.ItemsSource = result.Detections;

        // 바운딩박스 그리기
        if (PreviewImage.Source is BitmapSource bmp)
        {
            // 실제 이미지가 화면에 렌더링된 크기/위치 계산
            double imgW = bmp.PixelWidth;
            double imgH = bmp.PixelHeight;
            double canvasW = OverlayCanvas.ActualWidth;
            double canvasH = OverlayCanvas.ActualHeight;

            double scale = Math.Min(canvasW / imgW, canvasH / imgH);
            double renderedW = imgW * scale;
            double renderedH = imgH * scale;
            _imageOffsetX = (canvasW - renderedW) / 2;
            _imageOffsetY = (canvasH - renderedH) / 2;
            _imageScaleX = scale;
            _imageScaleY = scale;

            foreach (var det in result.Detections)
            {
                double x = det.BBox.X1 * _imageScaleX + _imageOffsetX;
                double y = det.BBox.Y1 * _imageScaleY + _imageOffsetY;
                double w = (det.BBox.X2 - det.BBox.X1) * _imageScaleX;
                double h = (det.BBox.Y2 - det.BBox.Y1) * _imageScaleY;

                var rect = new Rectangle
                {
                    Width = w, Height = h,
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 80, 80)),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(40, 255, 80, 80))
                };
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                OverlayCanvas.Children.Add(rect);

                // 라벨
                var label = new TextBlock
                {
                    Text = $"{det.ClassName} {det.Confidence:P0}",
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.FromRgb(255, 80, 80)),
                    FontSize = 11, Padding = new Thickness(3, 1, 3, 1)
                };
                Canvas.SetLeft(label, x);
                Canvas.SetTop(label, y - 20);
                OverlayCanvas.Children.Add(label);
            }
        }
    }
}