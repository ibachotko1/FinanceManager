using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FinanceManager.Models;

namespace FinanceManager.Views
{
    public partial class CategoryPieChart : UserControl
    {
        public CategoryPieChart()
        {
            InitializeComponent();
            // Перерисовка, если контролу поменяли размер
            SizeChanged += (_, __) => DrawChart();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable<CategorySummary>),
                typeof(CategoryPieChart),
                new PropertyMetadata(null, OnItemsSourceChanged)
            );

        public IEnumerable<CategorySummary> ItemsSource
        {
            get => (IEnumerable<CategorySummary>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CategoryPieChart)d;

            // Подписка на CollectionChanged, чтобы диаграмма обновлялась сама
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= control.OnCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += control.OnCollectionChanged;
            }

            control.DrawChart();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DrawChart();
        }

        private void DrawChart()
        {
            // Рисуем "с нуля" — зато не надо думать про диффы сегментов
            ChartCanvas.Children.Clear();

            var data = ItemsSource?.Where(c => c.Amount > 0).ToList();
            if (data == null || data.Count == 0)
                return;

            double total = (double)data.Sum(c => c.Amount);
            if (total <= 0)
                return;

            double size = Math.Min(ActualWidth, ActualHeight);
            if (size <= 0)
                size = 200;

            double radius = size / 2 - 4;
            Point center = new Point(size / 2, size / 2);

            ChartCanvas.Width = size;
            ChartCanvas.Height = size;

            double startAngle = 0;
            int colorIndex = 0;
            Color[] colors =
            {
                (Color)ColorConverter.ConvertFromString("#42A5F5"),
                (Color)ColorConverter.ConvertFromString("#66BB6A"),
                (Color)ColorConverter.ConvertFromString("#FFA726"),
                (Color)ColorConverter.ConvertFromString("#EF5350"),
                (Color)ColorConverter.ConvertFromString("#AB47BC"),
                (Color)ColorConverter.ConvertFromString("#26C6DA")
            };

            foreach (var item in data)
            {
                double sweepAngle = (double)item.Amount / total * 360.0;
                if (sweepAngle <= 0)
                    continue;

                // Каждый элемент — отдельный сектор (PathGeometry)
                var path = new Path
                {
                    Fill = new SolidColorBrush(colors[colorIndex % colors.Length]),
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    ToolTip = $"{item.Category}: {item.Amount:N0} руб."
                };

                double startRad = startAngle * Math.PI / 180.0;
                double endRad = (startAngle + sweepAngle) * Math.PI / 180.0;

                Point startPoint = new Point(
                    center.X + radius * Math.Cos(startRad),
                    center.Y + radius * Math.Sin(startRad)
                );

                Point endPoint = new Point(
                    center.X + radius * Math.Cos(endRad),
                    center.Y + radius * Math.Sin(endRad)
                );

                bool isLargeArc = sweepAngle > 180;

                var figure = new PathFigure { StartPoint = center };
                figure.Segments.Add(new LineSegment(startPoint, true));
                figure.Segments.Add(
                    new ArcSegment(
                        endPoint,
                        new Size(radius, radius),
                        0,
                        isLargeArc,
                        SweepDirection.Clockwise,
                        true
                    )
                );
                figure.Segments.Add(new LineSegment(center, true));

                var geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                path.Data = geometry;

                ChartCanvas.Children.Add(path);

                startAngle += sweepAngle;
                colorIndex++;
            }
        }
    }
}
