﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Xamarin.Forms.Platform.Webassembly.Interop;

[assembly: Xamarin.Forms.Platform.Webassembly.ExportRenderer(
	typeof(Button),
	typeof(Xamarin.Forms.Platform.Webassembly.Renderers.ButtonRenderer))]

namespace Xamarin.Forms.Platform.Webassembly.Renderers
{
	public class ButtonRenderer : ViewRenderer<Button>
	{
		bool _needsTextMeasure = true;
		Size _textSize;

		static HashSet<string> _renderProperties = new HashSet<string>
		{
			nameof(Button.BackgroundColor),
			nameof(Button.BorderColor),
			nameof(Button.BorderWidth),
			nameof(Button.CornerRadius),
			nameof(Button.FontAttributes),
			nameof(Button.FontFamily),
			nameof(Button.FontSize),
			nameof(Button.IsEnabled),
			nameof(Button.Text),
			nameof(Button.TextColor),
		};

		protected override void OnElementPropertyChanged(
			object sender,
			PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(Button.Text):
				case nameof(Button.FontFamily):
				case nameof(Button.FontSize):
					this.Element.NativeSizeChanged();
					_needsTextMeasure = true;
					break;
			}
			base.OnElementPropertyChanged(sender, e);
		}

		protected override bool AffectsRender(string propertyName)
		{
			return base.AffectsRender(propertyName) ||
				_renderProperties.Contains(propertyName);
		}

		protected override void SetBasicStyles()
		{
			base.SetBasicStyles();

			this.Styles["line-height"] = "1";
			this.Styles["cursor"] = "pointer";
			this.Styles["font"] = this.Element.FontFamily;
			this.Styles["font-family"] = this.ActualFontFamily;

			double fs = this.Element.FontSize;
			if (!double.IsNaN(fs))
				this.Styles["font-size"] = fs.ToString() + "px";

			this.Styles["color"] = this.Element.TextColor.ToHTMLColor();
			this.Styles["background-color"] = Element.BackgroundColor.ToHTMLColor();

			this.Styles["border"] = $"{this.ActualBorderWidth}px";

			this.Styles["border-style"] = "solid";
			this.Styles["border-color"] = $"{Element.BorderColor.ToHTMLColor()}";
			this.Styles["border-radius"] = $"{Element.CornerRadius}px";
		}

		protected override void AddAdditionalAttributes(RenderTreeBuilder builder)
		{
            builder.AddAttribute(
                this.RenderCounter++,
                "onmousedown",
                EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
                {
                    this.Element.SendPressed();
                }));
			builder.AddAttribute(
				this.RenderCounter++,
				"onmouseup",
				EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
				{
					this.Element.SendReleased();
				}));
			builder.AddAttribute(
				this.RenderCounter++, 
				"onclick",
				EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
				{
					this.Element.SendClicked();
				}));
		}

		protected override void RenderContent(RenderTreeBuilder builder)
		{
			builder.OpenElement(RenderCounter++, "div");
			builder.AddAttribute(RenderCounter++, "style", 
				$"position: absolute; " +
				$"top: {Element.Padding.Top}px; " +
				$"left: {Element.Padding.Left}px; ");
			builder.AddContent(this.RenderCounter++, this.Element.Text);
			builder.CloseElement();
		}

		private double ActualBorderWidth => 
			this.Element.BorderWidth == -1 ? 0 : this.Element.BorderWidth;

		private string ActualFontFamily => 
			string.IsNullOrEmpty(this.Element.FontFamily) ? "Times New Roman" : this.Element.FontFamily;

		protected override Size MeasureOverride(Size availableSize)
		{
			var sz = XFUtilities.MeasureText(
				this.Element.Text,
				this.ActualFontFamily,
				this.Element.FontSize,
				(double.IsInfinity(availableSize.Width) || availableSize.Width == -1)
					? 0
					: availableSize.Width);
			sz.Width += this.Element.Padding.HorizontalThickness;
			sz.Height += this.Element.Padding.VerticalThickness;
			return sz;
		}
	}
}
