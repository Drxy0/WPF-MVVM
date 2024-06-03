using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NetworkService.Helpers
{
	public class KeyPressBehavior : Behavior<UIElement>
	{
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(KeyPressBehavior));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (Command != null && Command.CanExecute(e))
			{
				Command.Execute(e);
			}
		}
	}
}
