using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using CodePlex.WPF.CustomControls.Resources;

/// <summary>
/// Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is to be used:
/// 
/// xmlns:MyNamespace="clr-namespace:CodePlex.WPF.CustomControls.BindingNavigator;assembly=CodePlex.WPF.CustomControls.BindingNavigator"
/// 
/// You will also need to add a project reference from the project where the XAML file lives to this project and Rebuild to avoid compilation errors.
/// Go ahead and use your control in the XAML file.
/// 
/// <MyNamespace:BindingNavigator/>
/// </summary>
namespace CodePlex.WPF.CustomControls
{
	/// <summary>
	/// Specifies how the item controls are displayed on a BindingNavigator control.
	/// </summary>
	public enum NavigatorDisplayMode
	{
		FirstLastPreviousNext,
		PreviousNext
	}

	/// <summary>
	/// Represents the navigation and manipulation user interface (UI) for controls on a page that are bound to data.
	/// Based on DataPager class and template taken from Silverlight Framework.
    /// author: leos.sevcik@cmss.cz
	/// </summary>
	[TemplatePart(Name = BINDINGNAVIGATOR_elementFirstItemButton, Type = typeof(ButtonBase))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementPreviousItemButton, Type = typeof(ButtonBase))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementCurrentItemPrefixTextBlock, Type = typeof(TextBlock))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementCurrentItemSuffixTextBlock, Type = typeof(TextBlock))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementCurrentItemTextBox, Type = typeof(TextBox))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementNextItemButton, Type = typeof(ButtonBase))]
	[TemplatePart(Name = BINDINGNAVIGATOR_elementLastItemButton, Type = typeof(ButtonBase))]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateNormal, GroupName = BINDINGNAVIGATOR_groupCommon)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateDisabled, GroupName = BINDINGNAVIGATOR_groupCommon)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveEnabled, GroupName = BINDINGNAVIGATOR_groupMove)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveDisabled, GroupName = BINDINGNAVIGATOR_groupMove)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveFirstEnabled, GroupName = BINDINGNAVIGATOR_groupMoveFirst)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveFirstDisabled, GroupName = BINDINGNAVIGATOR_groupMoveFirst)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMovePreviousEnabled, GroupName = BINDINGNAVIGATOR_groupMovePrevious)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMovePreviousDisabled, GroupName = BINDINGNAVIGATOR_groupMovePrevious)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveNextEnabled, GroupName = BINDINGNAVIGATOR_groupMoveNext)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveNextDisabled, GroupName = BINDINGNAVIGATOR_groupMoveNext)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveLastEnabled, GroupName = BINDINGNAVIGATOR_groupMoveLast)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateMoveLastDisabled, GroupName = BINDINGNAVIGATOR_groupMoveLast)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_stateFirstLastPreviousNext, GroupName = BINDINGNAVIGATOR_groupDisplayMode)]
	[TemplateVisualState(Name = BINDINGNAVIGATOR_statePreviousNext, GroupName = BINDINGNAVIGATOR_groupDisplayMode)]
    public class BindingNavigator : Control
	{
		#region Constants

		// Automation Id constants
        private const string BINDINGNAVIGATOR_currentItemTextBoxAutomationId = "CurrentItem";
        private const string BINDINGNAVIGATOR_firstItemButtonAutomationId = "LargeDecrement";
        private const string BINDINGNAVIGATOR_lastItemButtonAutomationId = "LargeIncrement";
        private const string BINDINGNAVIGATOR_nextItemButtonAutomationId = "SmallIncrement";
        private const string BINDINGNAVIGATOR_previousItemButtonAutomationId = "SmallDecrement";

		// Parts constants
        private const string BINDINGNAVIGATOR_elementCurrentItemPrefixTextBlock = "CurrentItemPrefixTextBlock";
        private const string BINDINGNAVIGATOR_elementCurrentItemSuffixTextBlock = "CurrentItemSuffixTextBlock";
        private const string BINDINGNAVIGATOR_elementCurrentItemTextBox = "CurrentItemTextBox";
        private const string BINDINGNAVIGATOR_elementFirstItemButton = "FirstItemButton";
        private const string BINDINGNAVIGATOR_elementLastItemButton = "LastItemButton";
        private const string BINDINGNAVIGATOR_elementNextItemButton = "NextItemButton";
        private const string BINDINGNAVIGATOR_elementPreviousItemButton = "PreviousItemButton";

		// Common states constants
		private const string BINDINGNAVIGATOR_groupCommon = "CommonStates";
		private const string BINDINGNAVIGATOR_stateNormal = "Normal";
		private const string BINDINGNAVIGATOR_stateDisabled = "Disabled";

		// Move states constants
		private const string BINDINGNAVIGATOR_groupMove = "MoveStates";
		private const string BINDINGNAVIGATOR_stateMoveEnabled = "MoveEnabled";
		private const string BINDINGNAVIGATOR_stateMoveDisabled = "MoveDisabled";

		// MoveFirst states constants        
		private const string BINDINGNAVIGATOR_groupMoveFirst = "MoveFirstStates";
		private const string BINDINGNAVIGATOR_stateMoveFirstEnabled = "MoveFirstEnabled";
		private const string BINDINGNAVIGATOR_stateMoveFirstDisabled = "MoveFirstDisabled";

		// MovePrevious states constants        
		private const string BINDINGNAVIGATOR_groupMovePrevious = "MovePreviousStates";
		private const string BINDINGNAVIGATOR_stateMovePreviousEnabled = "MovePreviousEnabled";
		private const string BINDINGNAVIGATOR_stateMovePreviousDisabled = "MovePreviousDisabled";

		// MovePrevious states constants        
		private const string BINDINGNAVIGATOR_groupMoveNext = "MoveNextStates";
		private const string BINDINGNAVIGATOR_stateMoveNextEnabled = "MoveNextEnabled";
		private const string BINDINGNAVIGATOR_stateMoveNextDisabled = "MoveNextDisabled";

		// MovePrevious states constants        
		private const string BINDINGNAVIGATOR_groupMoveLast = "MoveLastStates";
		private const string BINDINGNAVIGATOR_stateMoveLastEnabled = "MoveLastEnabled";
		private const string BINDINGNAVIGATOR_stateMoveLastDisabled = "MoveLastDisabled";

		// DisplayModeStates states constants
		private const string BINDINGNAVIGATOR_groupDisplayMode = "DisplayModeStates";
		private const string BINDINGNAVIGATOR_stateFirstLastPreviousNext = "FirstLastPreviousNext";
		private const string BINDINGNAVIGATOR_statePreviousNext = "PreviousNext";

		// Default property value constants
		private const NavigatorDisplayMode BINDINGNAVIGATOR_defaultDisplayMode = NavigatorDisplayMode.FirstLastPreviousNext;
		private const int BINDINGNAVIGATOR_defaultItemIndex = -1;

		#endregion

        #region Private Fields

		/// <summary>
		/// The new index used to change the current index when a user enters something into the current item text box.
		/// </summary>
		private int _requestedItemIndex;

		/// <summary>
		/// Private accessor for the text block appearing before the current item text box.
		/// </summary>
		private TextBlock _currentItemPrefixTextBlock;

		/// <summary>
		/// Private accessor for the text block appearing after the current item text box.
		/// </summary>
		private TextBlock _currentItemSuffixTextBlock;

        /// <summary>
        /// Private accessor for the current item text box.
        /// </summary>
        private TextBox _currentItemTextBox;

        /// <summary>
        /// Private accessor for the first item ButtonBase.
        /// </summary>
        private ButtonBase _firstItemButtonBase;

        /// <summary>
        /// Private accessor for the previous item ButtonBase.
        /// </summary>
        private ButtonBase _previousItemButtonBase;

        /// <summary>
        /// Private accessor for the next item ButtonBase.
        /// </summary>
        private ButtonBase _nextItemButtonBase;

		/// <summary>
		/// Private accessor for the last item ButtonBase.
		/// </summary>
		private ButtonBase _lastItemButtonBase;

        #endregion

        #region Dependency Properties

        /// <summary>
		/// Identifies the ItemIndex dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemIndexProperty =
			DependencyProperty.Register(
				"ItemIndex",
				typeof(int),
				typeof(BindingNavigator),
				new PropertyMetadata(BINDINGNAVIGATOR_defaultItemIndex, OnItemIndexPropertyChanged));

		/// <summary>
		/// Identifies the ItemCount dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemCountProperty =
			DependencyProperty.Register(
				"ItemCount",
				typeof(int),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

		/// <summary>
		/// Identifies the DisplayMode dependency property.
		/// </summary>
		public static readonly DependencyProperty DisplayModeProperty =
			DependencyProperty.Register(
				"DisplayMode",
				typeof(NavigatorDisplayMode),
				typeof(BindingNavigator),
				new PropertyMetadata(BINDINGNAVIGATOR_defaultDisplayMode, OnDisplayModePropertyChanged));

		/// <summary>
		/// Identifies the BindingSource dependency property.
		/// </summary>
		public static readonly DependencyProperty BindingSourceProperty =
			DependencyProperty.Register(
				"BindingSource",
				typeof(IEnumerable),
				typeof(BindingNavigator),
				new PropertyMetadata(OnBindingSourcePropertyChanged));

		/// <summary>
		/// Identifies the CanChangeItem dependency property.
		/// </summary>
		public static readonly DependencyProperty CanChangeItemProperty =
			DependencyProperty.Register(
				"CanChangeItem",
				typeof(bool),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

		/// <summary>
		/// Identifies the CanMoveToFirstItem dependency property.
		/// </summary>
		public static readonly DependencyProperty CanMoveToFirstItemProperty =
			DependencyProperty.Register(
				"CanMoveToFirstItem",
				typeof(bool),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

		/// <summary>
		/// Identifies the CanMoveToPreviousItem dependency property.
		/// </summary>
		public static readonly DependencyProperty CanMoveToPreviousItemProperty =
			DependencyProperty.Register(
				"CanMoveToPreviousItem",
				typeof(bool),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

		/// <summary>
		/// Identifies the CanMoveToNextItem dependency property.
		/// </summary>
		public static readonly DependencyProperty CanMoveToNextItemProperty =
			DependencyProperty.Register(
				"CanMoveToNextItem",
				typeof(bool),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

		/// <summary>
		/// Identifies the CanMoveToLastItem dependency property.
		/// </summary>
		public static readonly DependencyProperty CanMoveToLastItemProperty =
			DependencyProperty.Register(
				"CanMoveToLastItem",
				typeof(bool),
				typeof(BindingNavigator),
				new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CollectionView dependency property.
        /// </summary>
        public static readonly DependencyProperty CollectionViewProperty =
            DependencyProperty.Register(
                "CollectionView",
                typeof(CollectionView),
                typeof(BindingNavigator),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingNavigator"/> class.
        /// </summary>
		public BindingNavigator()
        {
			this.DefaultStyleKey = typeof(BindingNavigator);

			// Listening to the IsEnabled changes so the BindingNavigator states can be updated accordingly.
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnBindingNavigatorIsEnabledChanged);
        }

        #endregion

        #region Events

		/// <summary>
        /// EventHandler for when ItemIndex is changing.
		/// </summary>
		public event EventHandler<CancelEventArgs> ItemIndexChanging;

		/// <summary>
        /// EventHandler for when ItemIndex has changed.
		/// </summary>
		public event EventHandler<EventArgs> ItemIndexChanged;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the TextBox holding the current ItemIndex value, if any.
        /// </summary>
        internal TextBox CurrentItemTextBox
        {
            get
            {
                return this._currentItemTextBox;
            }
        }

        #endregion

        #region Public Properties

		/// <summary>
		/// Gets or sets the current index in the <see cref="T:System.Collections.IEnumerable" />.
		/// </summary>
		[DefaultValueAttribute(-1)]
		public int ItemIndex
		{
			get
			{
				return (int)GetValue(ItemIndexProperty);
			}

			set
			{
				SetValue(ItemIndexProperty, value);
			}
		}

		/// <summary>
		/// Gets the current number of known items in the <see cref="T:System.Collections.IEnumerable" />.
		/// </summary>
		public int ItemCount
		{
			get
			{
				return (int)GetValue(ItemCountProperty);
			}

			private set
			{
				this.SetValueNoCallback(ItemCountProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates how the BindingNavigator user interface is displayed.
		/// </summary>
		/// <value>The display mode.</value>
		public NavigatorDisplayMode DisplayMode
		{
			get
			{
				return (NavigatorDisplayMode)GetValue(DisplayModeProperty);
			}

			set
			{
				SetValue(DisplayModeProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the binding source.
		/// </summary>
		/// <value>The binding source.</value>
		public IEnumerable BindingSource
		{
			get
			{
				return (IEnumerable)GetValue(BindingSourceProperty);
			}

			set
			{
				SetValue(BindingSourceProperty, value);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether or not the user is allowed to move to any item
		/// </summary>
		public bool CanChangeItem
		{
			get
			{
				return (bool)GetValue(CanChangeItemProperty);
			}

			private set
			{
				this.SetValueNoCallback(CanChangeItemProperty, value);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether or not the <see cref="T:CMSS.WPF.CustomControls.BindingNavigator" /> will 
		/// allow the user to attempt to move to the first item.
		/// </summary>
		public bool CanMoveToFirstItem
		{
			get
			{
				return (bool)GetValue(CanMoveToFirstItemProperty);
			}

			private set
			{
				this.SetValueNoCallback(CanMoveToFirstItemProperty, value);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether or not the <see cref="T:CMSS.WPF.CustomControls.BindingNavigator" /> 
		/// will allow the user to attempt to move to the previous item.
		/// </summary>
		public bool CanMoveToPreviousItem
		{
			get
			{
				return (bool)GetValue(CanMoveToPreviousItemProperty);
			}

			private set
			{
				this.SetValueNoCallback(CanMoveToPreviousItemProperty, value);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether or not the <see cref="T:CMSS.WPF.CustomControls.BindingNavigator" />
		/// will allow the user to attempt to move to the next item.
		/// </summary>
		public bool CanMoveToNextItem
		{
			get
			{
				return (bool)GetValue(CanMoveToNextItemProperty);
			}

			private set
			{
				this.SetValueNoCallback(CanMoveToNextItemProperty, value);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether or not the <see cref="T:CMSS.WPF.CustomControls.BindingNavigator" />
		/// will allow the user to attempt to move to the last item.
		/// </summary>
		public bool CanMoveToLastItem
		{
			get
			{
				return (bool)GetValue(CanMoveToLastItemProperty);
			}

			private set
			{
				this.SetValueNoCallback(CanMoveToLastItemProperty, value);
			}
		}

        /// <summary>
        /// Gets a collection view created from BindingSource.
        /// </summary>
        /// <value>The collection view.</value>
        public CollectionView CollectionView
        {
            get
            {
                return (CollectionView)GetValue(CollectionViewProperty);
            }

            private set
            {
                this.SetValueNoCallback(CollectionViewProperty, value);
            }
        }

        #endregion

        #region Private Static Methods

		/// <summary>
        /// ItemIndex property changed handler.
		/// </summary>
        /// <param name="d">BindingNavigator that changed its ItemIndex.</param>
		/// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
		private static void OnItemIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingNavigator bindingNavigator = d as BindingNavigator;

			if (!bindingNavigator.AreHandlersSuspended())
			{
				int newItemIndex = (int)e.NewValue;

				if (bindingNavigator.BindingSource == null && newItemIndex != -1)
				{
					bindingNavigator.SetValueNoCallback(e.Property, e.OldValue);
					throw new ArgumentOutOfRangeException("value", BindingNavigatorResources.ItemIndexMustBeNegativeOne);
				}

                if (bindingNavigator.BindingSource != null)
                {
				    if (newItemIndex < 0)
				    {
					    bindingNavigator.SetValueNoCallback(e.Property, e.OldValue);
					    throw new ArgumentOutOfRangeException(
						    "value",
						    string.Format(CultureInfo.InvariantCulture, BindingNavigatorResources.ValueMustBeGreaterThanOrEqualTo, "ItemIndex", 0));
				    }

                    if (newItemIndex >= bindingNavigator.ItemCount)
                    {
                        bindingNavigator.SetValueNoCallback(e.Property, e.OldValue);
                        throw new ArgumentOutOfRangeException(
                            "value",
                            string.Format(CultureInfo.InvariantCulture, BindingNavigatorResources.ValueMustBeGreaterThan, "ItemCount", "ItemIndex"));
                    }

                    bindingNavigator.ItemMoveHandler((int)e.OldValue, newItemIndex);
                }
			}
		}

		/// <summary>
		/// Called when a Read-Only dependency property is changed
		/// </summary>
		/// <param name="d">BindingNavigator that changed its read-only property.</param>
		/// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
		private static void OnReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingNavigator bindingNavigator = d as BindingNavigator;

			if (bindingNavigator != null && !bindingNavigator.AreHandlersSuspended())
			{
				bindingNavigator.SetValueNoCallback(e.Property, e.OldValue);
				throw new InvalidOperationException(
					string.Format(
						CultureInfo.InvariantCulture,
						BindingNavigatorResources.UnderlyingPropertyIsReadOnly,
						e.Property.ToString()));
			}
		}

		/// <summary>
		/// DisplayMode property changed handler.
		/// </summary>
		/// <param name="d">BindingNavigator that changed its DisplayMode.</param>
		/// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
		private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingNavigator bindingNavigator = d as BindingNavigator;

			if (!bindingNavigator.AreHandlersSuspended())
			{
				if (!Enum.IsDefined(typeof(NavigatorDisplayMode), e.NewValue))
				{
					bindingNavigator.SetValueNoCallback(e.Property, e.OldValue);
					throw new ArgumentException(
						string.Format(CultureInfo.InvariantCulture,
							BindingNavigatorResources.InvalidEnumArgumentException_InvalidEnumArgument,
							"value",
							e.NewValue.ToString(),
							typeof(NavigatorDisplayMode).Name));
				}

				bindingNavigator.UpdateControl();
			}
		}

		/// <summary>
		/// BindingSource property changed handler.
		/// </summary>
		/// <param name="d">BindingNavigator that changed its BindingSource.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnBindingSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingNavigator bindingNavigator = d as BindingNavigator;

			IEnumerable enumerable = e.NewValue as IEnumerable;
			if (enumerable != null)
			{
				IEnumerable<object> genericEnumerable = enumerable.Cast<object>();

                // setting CollectionViewSource for creating CollectionView
                CollectionViewSource collectionViewSource = new CollectionViewSource();
                collectionViewSource.Source = genericEnumerable;

                bindingNavigator.CollectionView = collectionViewSource.View as CollectionView;

                // set ItemCount and ItemIndex
				bindingNavigator.ItemCount = genericEnumerable.Count();
				bindingNavigator.ItemIndex = 0;
			}
			else
			{
                bindingNavigator.CollectionView = null;
				bindingNavigator.ItemCount = 0;
				bindingNavigator.ItemIndex = -1;
			}

			bindingNavigator.UpdateControl();
		}

        #endregion

        #region Private Methods

		/// <summary>
        /// Handles the notifications for the BindingNavigator.IsEnabled changes
		/// </summary>
        /// <param name="sender">BindingNavigator that changed its IsEnabled property</param>
		/// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
		private void OnBindingNavigatorIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateCommonState();
		}

        /// <summary>
        /// Handles the click of the first item ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnFirstItemButtonBaseClick(object sender, RoutedEventArgs e)
        {
			if (this.BindingSource != null)
			{
				this.ItemMoveHandler(ItemIndex, 0);
			}
		}

        /// <summary>
        /// Handles the click of the previous item ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnPreviousItemButtonBaseClick(object sender, RoutedEventArgs e)
        {
			if (this.BindingSource != null)
			{
				this.ItemMoveHandler(ItemIndex, ItemIndex - 1);
			}
		}

        /// <summary>
        /// Handles the click of the next item ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnNextItemButtonBaseClick(object sender, RoutedEventArgs e)
        {
			if (this.BindingSource != null)
			{
				this.ItemMoveHandler(ItemIndex, ItemIndex + 1);
			}
        }

		/// <summary>
		/// Handles the click of the last item ButtonBase.
		/// </summary>
		/// <param name="sender">The object firing this event.</param>
		/// <param name="e">The event args for this event.</param>
		private void OnLastItemButtonBaseClick(object sender, RoutedEventArgs e)
		{
			if (this.BindingSource != null)
			{
				this.ItemMoveHandler(ItemIndex, ItemCount - 1);
			}
		}

		/// <summary>
		/// Handles the KeyDown event on the current item text box.
		/// </summary>
		/// <param name="sender">The object firing this event.</param>
		/// <param name="e">The event args for this event.</param>
		private void OnCurrentItemTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				this.MoveCurrentItemToTextboxValue();
			}
		}

		/// <summary>
		/// Handles the loss of focus for the current item text box.
		/// </summary>
		/// <param name="sender">The object firing this event.</param>
		/// <param name="e">The event args for this event.</param>
		private void OnCurrentItemTextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			this.MoveCurrentItemToTextboxValue();
		}

		/// <summary>
        /// Attempts to put the integer value of the string in _currentItemTextBox into _requestedItemIndex.
		/// </summary>
		/// <returns>Whether or not the parsing of the string succeeded.</returns>
		private bool TryParseTextBoxItem()
		{
			bool successfullyParsed = int.TryParse(this._currentItemTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out this._requestedItemIndex);

			if (successfullyParsed)
			{
				// subtract one to make it zero-based
				this._requestedItemIndex--;
			}

			return successfullyParsed;
		}

		/// <summary>
		/// Attempts to move the current item index to the value in the current item textbox.
		/// </summary>
		private void MoveCurrentItemToTextboxValue()
		{
			if (this._currentItemTextBox.Text != (this.ItemIndex + 1).ToString())
			{
				if (this.BindingSource != null && this.TryParseTextBoxItem())
				{
					this.ItemMoveHandler(ItemIndex, _requestedItemIndex);
				}

				this._currentItemTextBox.Text = (this.ItemIndex + 1).ToString();
			}
		}

		/// <summary>
		/// Updates the state related to the IsEnabled property
		/// </summary>
		private void UpdateCommonState()
		{
			VisualStateManager.GoToState(this, this.IsEnabled ? BINDINGNAVIGATOR_stateNormal : BINDINGNAVIGATOR_stateDisabled, true);
		}

		/// <summary>
		/// Updates the visual display to show the current item mode we have selected.
		/// </summary>
		private void UpdateItemModeDisplay()
		{
			VisualStateManager.GoToState(this, Enum.GetName(typeof(NavigatorDisplayMode), this.DisplayMode), true);
		}

		/// <summary>
		/// Updates the captions of the text blocks surrounding the current item text box.
		/// </summary>
		private void UpdateCurrentItemPrefixAndSuffix()
		{
			string currentItemSuffix = (CultureInfo.CurrentCulture.Name == "cs-CZ") ? "z " : "of ";

			if (this._currentItemSuffixTextBlock != null)
			{
			    if (this.BindingSource == null)
			    {
			        currentItemSuffix += BindingNavigatorResources.CurrentItemSuffix_TotalItemCountUnknown;
			    }
			    else
			    {
			        currentItemSuffix += ItemCount.ToString();
			    }

			    this._currentItemSuffixTextBlock.Text = currentItemSuffix;
			}
		}

		/// <summary>
		/// Sets the UI enabled.
		/// </summary>
		private void SetCanChangeItem()
		{
			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMoveEnabled, true);

			if (this._currentItemTextBox != null)
			{
				this._currentItemTextBox.Text = (this.ItemIndex + 1).ToString();
			}
		}

		/// <summary>
		/// Sets the UI disabled.
		/// </summary>
		private void SetCannotChangeItem()
		{
			if (this._currentItemTextBox != null)
			{
				this._currentItemTextBox.Text = String.Empty;
			}

			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMoveDisabled, true);
			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMoveFirstDisabled, true);
			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMovePreviousDisabled, true);
			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMoveNextDisabled, true);
			VisualStateManager.GoToState(this, BINDINGNAVIGATOR_stateMoveLastDisabled, true);
		}

		/// <summary>
		/// Updates the states of whether user can move to the first and to the previous item.
		/// </summary>
		private void UpdateCanMoveFirstAndPrevious()
		{
			VisualStateManager.GoToState(this, this.CanMoveToFirstItem ? BINDINGNAVIGATOR_stateMoveFirstEnabled : BINDINGNAVIGATOR_stateMoveFirstDisabled, true);
			VisualStateManager.GoToState(this, this.CanMoveToPreviousItem ? BINDINGNAVIGATOR_stateMovePreviousEnabled : BINDINGNAVIGATOR_stateMovePreviousDisabled, true);
		}

		/// <summary>
		/// Updates the states of whether user can move to the next and to the last item.
		/// </summary>
		private void UpdateCanMoveNextAndLast()
		{
			VisualStateManager.GoToState(this, this.CanMoveToNextItem ? BINDINGNAVIGATOR_stateMoveNextEnabled : BINDINGNAVIGATOR_stateMoveNextDisabled, true);
			VisualStateManager.GoToState(this, this.CanMoveToLastItem ? BINDINGNAVIGATOR_stateMoveLastEnabled : BINDINGNAVIGATOR_stateMoveLastDisabled, true);
		}

        /// <summary>
        /// Updates the current item and the state of the control.
        /// </summary>
        private void UpdateControl()
        {
			this.UpdateItemModeDisplay();

			this.CanMoveToFirstItem = (this.ItemCount > 0 && this.ItemIndex > 0);
			this.CanMoveToPreviousItem = this.CanMoveToFirstItem;
			this.CanMoveToNextItem = (this.ItemCount > 0 && this.ItemIndex < this.ItemCount - 1);
			this.CanMoveToLastItem = this.CanMoveToNextItem;
			this.CanChangeItem = this.ItemCount > 0;

			this.UpdateCurrentItemPrefixAndSuffix();

			if (!this.CanChangeItem)
			{
				this.SetCannotChangeItem();
			}
			else
			{
				this.SetCanChangeItem();
				this.UpdateCanMoveFirstAndPrevious();
				this.UpdateCanMoveNextAndLast();
			}
        }

		/// <summary>
        /// Raises the ItemIndexChanged event.
		/// </summary>
		private void RaiseItemIndexChanged()
		{
			this.UpdateControl();

			EventHandler<EventArgs> handler = this.ItemIndexChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		/// <summary>
        /// Raises the ItemIndexChanging event.
		/// </summary>
		/// <param name="e">The event args to use for the event.</param>
		private void RaiseItemIndexChanging(CancelEventArgs e, int newItemIndex)
		{
			if (newItemIndex > -1 && newItemIndex < ItemCount)
			{
				EventHandler<CancelEventArgs> handler = this.ItemIndexChanging;
				if (handler != null)
				{
					handler(this, e);
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		/// <summary>
		/// This helper method will take care of setting the specified ItemIndex,
        /// while also firing the ItemIndexChanging and ItemIndexChanged events.
		/// </summary>
		/// <param name="oldItemIndex">The ItemIndex value before moving</param>
		/// <param name="newItemIndex">The requested ItemIndex after moving</param>
		private void ItemMoveHandler(int oldItemIndex, int newItemIndex)
		{
			CancelEventArgs cancelArgs = new CancelEventArgs(false);
			this.RaiseItemIndexChanging(cancelArgs, newItemIndex);

			if (cancelArgs.Cancel)
			{
				// Revert back to old value, since operation was canceled
				this.SetValueNoCallback(BindingNavigator.ItemIndexProperty, oldItemIndex);
			}
			else
			{
				// set requested value
                this.SetValueNoCallback(BindingNavigator.ItemIndexProperty, newItemIndex);
                // move to new position at CollectionView
                if (this.CollectionView != null)
                {
                    this.CollectionView.MoveCurrentToPosition(newItemIndex);
                }
                // raising event
				this.RaiseItemIndexChanged();
			}
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the control's template, retrieves the elements
        /// within it, and sets up events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // unsubscribe event handlers for previous template parts
            if (this._firstItemButtonBase != null)
            {
                this._firstItemButtonBase.Click -= new RoutedEventHandler(this.OnFirstItemButtonBaseClick);
            }

            if (this._previousItemButtonBase != null)
            {
                this._previousItemButtonBase.Click -= new RoutedEventHandler(this.OnPreviousItemButtonBaseClick);
            }

            if (this._nextItemButtonBase != null)
            {
                this._nextItemButtonBase.Click -= new RoutedEventHandler(this.OnNextItemButtonBaseClick);
            }

			if (this._lastItemButtonBase != null)
			{
				this._lastItemButtonBase.Click -= new RoutedEventHandler(this.OnLastItemButtonBaseClick);
			}

			if (this._currentItemTextBox != null)
			{
				this._currentItemTextBox.KeyDown -= new System.Windows.Input.KeyEventHandler(this.OnCurrentItemTextBoxKeyDown);
				this._currentItemTextBox.LostFocus -= new RoutedEventHandler(this.OnCurrentItemTextBoxLostFocus);
			}

            // get new template parts
			this._firstItemButtonBase = GetTemplateChild(BINDINGNAVIGATOR_elementFirstItemButton) as ButtonBase;
            this._previousItemButtonBase = GetTemplateChild(BINDINGNAVIGATOR_elementPreviousItemButton) as ButtonBase;
            this._nextItemButtonBase = GetTemplateChild(BINDINGNAVIGATOR_elementNextItemButton) as ButtonBase;
			this._lastItemButtonBase = GetTemplateChild(BINDINGNAVIGATOR_elementLastItemButton) as ButtonBase;

            if (this._firstItemButtonBase != null)
            {
                this._firstItemButtonBase.Click += new RoutedEventHandler(this.OnFirstItemButtonBaseClick);
                AutomationProperties.SetAutomationId(this._firstItemButtonBase, BINDINGNAVIGATOR_firstItemButtonAutomationId);
            }

            if (this._previousItemButtonBase != null)
            {
                this._previousItemButtonBase.Click += new RoutedEventHandler(this.OnPreviousItemButtonBaseClick);
                AutomationProperties.SetAutomationId(this._previousItemButtonBase, BINDINGNAVIGATOR_previousItemButtonAutomationId);
            }

            if (this._nextItemButtonBase != null)
            {
                this._nextItemButtonBase.Click += new RoutedEventHandler(this.OnNextItemButtonBaseClick);
                AutomationProperties.SetAutomationId(this._nextItemButtonBase, BINDINGNAVIGATOR_nextItemButtonAutomationId);
            }

			if (this._lastItemButtonBase != null)
			{
				this._lastItemButtonBase.Click += new RoutedEventHandler(this.OnLastItemButtonBaseClick);
				AutomationProperties.SetAutomationId(this._lastItemButtonBase, BINDINGNAVIGATOR_lastItemButtonAutomationId);
			}

			this._currentItemTextBox = GetTemplateChild(BINDINGNAVIGATOR_elementCurrentItemTextBox) as TextBox;
			this._currentItemPrefixTextBlock = GetTemplateChild(BINDINGNAVIGATOR_elementCurrentItemPrefixTextBlock) as TextBlock;
			this._currentItemSuffixTextBlock = GetTemplateChild(BINDINGNAVIGATOR_elementCurrentItemSuffixTextBlock) as TextBlock;

			if (this._currentItemTextBox != null)
			{
				this._currentItemTextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.OnCurrentItemTextBoxKeyDown);
				this._currentItemTextBox.LostFocus += new RoutedEventHandler(this.OnCurrentItemTextBoxLostFocus);
				AutomationProperties.SetAutomationId(this._currentItemTextBox, BINDINGNAVIGATOR_currentItemTextBoxAutomationId);
			}

            this.UpdateControl();
        }

        #endregion
    }
}
