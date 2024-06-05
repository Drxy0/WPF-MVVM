using System.Windows;
using System.Windows.Controls;

namespace NetworkService.Helpers
{
	public static class CaretIndexBehavior
	{
		public static readonly DependencyProperty CaretIndexProperty =
			DependencyProperty.RegisterAttached(
				"CaretIndex",
				typeof(int),
				typeof(CaretIndexBehavior),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCaretIndexChanged));

		public static int GetCaretIndex(DependencyObject obj)
		{
			return (int)obj.GetValue(CaretIndexProperty);
		}

		public static void SetCaretIndex(DependencyObject obj, int value)
		{
			obj.SetValue(CaretIndexProperty, value);
		}

		private static void OnCaretIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBox textBox)
			{
				textBox.CaretIndex = (int)e.NewValue;
			}
		}
	}
}
