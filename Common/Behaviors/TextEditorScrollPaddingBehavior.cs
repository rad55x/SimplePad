using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace SimplePad.Behaviors
{
    /// <summary>
    /// This behavior automatically adjusts the padding of a TextEditor control depending on whether its scrollbars are visible.
    /// </summary>
    public class TextEditorScrollPaddingBehavior : Behavior<TextEditor>
    {
        private readonly List<ScrollBar> _scrollBars = [];

        /// <summary>
        /// Override the OnAttached method to subscribe to the Loaded and Unloaded events of the associated TextEditor control.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded += OnLoaded;
                AssociatedObject.Unloaded += OnUnloaded;

                if (AssociatedObject.IsLoaded)
                {
                    Initialize();
                }
            }
        }

        /// <summary>
        /// Override the OnDetaching method to unsubscribe from events and clean up resources.
        /// </summary>
        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= OnLoaded;
                AssociatedObject.Unloaded -= OnUnloaded;
            }

            Cleanup();
            base.OnDetaching();
        }

        /// <summary>
        /// Handles the Loaded event of the TextEditor control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            Initialize();
        }

        /// <summary>
        /// Handles the Unloaded event of the TextEditor control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object? sender, RoutedEventArgs e)
        {
            Cleanup();
        }

        /// <summary>
        /// Initializes scrollbars tracking and updates the initial padding.
        /// </summary>
        private void Initialize()
        {
            Cleanup();

            if (AssociatedObject == null) return;

            // Find all scrollbars and subscribe to their property changes
            _scrollBars.AddRange(AssociatedObject.GetVisualDescendants().OfType<ScrollBar>());

            foreach (var sb in _scrollBars)
            {
                sb.PropertyChanged += OnScrollBarPropertyChanged;
            }
            //

            UpdatePadding();
        }

        /// <summary>
        /// Cleans up scrollbars subscriptions and clears the list.
        /// </summary>
        private void Cleanup()
        {
            foreach (var sb in _scrollBars)
            {
                sb.PropertyChanged -= OnScrollBarPropertyChanged;
            }

            _scrollBars.Clear();
        }

        /// <summary>
        /// Handles property changes on scrollbars to update padding when visibility changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScrollBarPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == Visual.IsVisibleProperty)
            {
                UpdatePadding();
            }
        }

        /// <summary>
        /// Updates the padding of the TextEditor based on scrollbars visibility.
        /// </summary>
        private void UpdatePadding()
        {
            if (AssociatedObject == null) return;

            // Check visibility of vertical and horizontal scrollbars
            bool isVerticalVisible = _scrollBars.Any(s => s.Orientation == Orientation.Vertical && s.IsVisible);
            bool isHorizontalVisible = _scrollBars.Any(s => s.Orientation == Orientation.Horizontal && s.IsVisible);
            //

            // Apply new padding values
            var newPadding = new Thickness(5, 5, isVerticalVisible ? 20 : 5, isHorizontalVisible ? 20 : 5);

            if (AssociatedObject.Padding != newPadding)
            {
                AssociatedObject.Padding = newPadding;
            }
        }
        //
    }
}