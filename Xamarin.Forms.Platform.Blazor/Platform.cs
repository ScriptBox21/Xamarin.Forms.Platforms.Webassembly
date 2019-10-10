﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Blazor
{
	public class Platform : BindableObject, INavigation
	{
		readonly FormsApplicationPage _page;

        public static void Init()
        {
            Forms.Init();
        }

		Page Page { get; set; }

		internal static readonly BindableProperty RendererTypeProperty = BindableProperty.CreateAttached(
			"RendererType",
			typeof(Type),
			typeof(Platform),
			default(Type));

		public static Type GetRendererType(VisualElement self)
		{
			return (Type)self.GetValue(RendererTypeProperty);
		}

		public static void SetRendererType(
			VisualElement self,
			Type rendererType)
		{
			self.SetValue(RendererTypeProperty, rendererType);
			self.IsPlatformEnabled = rendererType != null;
		}

		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached(
			"Renderer", 
			typeof(IVisualElementRenderer), 
			typeof(Platform), 
			default(IVisualElementRenderer));

		internal Platform(FormsApplicationPage page)
		{
			_page = page;

			var busyCount = 0;
			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
			});

			MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, OnPageAlert);
			MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, OnPageActionSheet);
		}

		async void OnPageAlert(Page sender, AlertArguments options)
		{

		}

		async void OnPageActionSheet(Page sender, ActionSheetArguments options)
		{

		}

		public static SizeRequest GetNativeSize(
			VisualElement view, 
			double widthConstraint, 
			double heightConstraint)
		{
			return new SizeRequest(new Size(widthConstraint, heightConstraint));
		}

		public static Type GetOrCreateRendererType(VisualElement element)
		{
			if (GetRendererType(element) == null)
				SetRendererType(element, CreateRendererType(element));
			return GetRendererType(element);
		}

		public static Type CreateRendererType(VisualElement element)
		{
			Type result = Registrar.Registered.GetHandlerType(element.GetType()) ??
				typeof(DefaultViewRenderer);
			return result;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement self)
		{
			return (IVisualElementRenderer)self.GetValue(RendererProperty);
		}

		public static void SetRenderer(
			VisualElement self, 
			IVisualElementRenderer renderer)
		{
			self.SetValue(RendererProperty, renderer);
			self.IsPlatformEnabled = renderer != null;
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;

			Page = newRoot;
			//_page.StartupPage = Page;
			Application.Current.NavigationProxy.Inner = this;
		}

		public IReadOnlyList<Page> NavigationStack
		{
			get { throw new InvalidOperationException(
				"NavigationStack is not supported globally on Blazor, please use a NavigationPage."); }
		}

		public IReadOnlyList<Page> ModalStack
		{
			get
			{
				return null;
				//return _page.Children.Cast<Page>().ToList();
			}
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on Blazor, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on Blazor, please use a NavigationPage.");
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException("PopToRootAsync is not supported globally on Blazor, please use a NavigationPage.");
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on Blazor, please use a NavigationPage.");
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException("InsertPageBefore is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page page)
		{
			throw new InvalidOperationException("PushModalAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopModalAsync()
		{
			throw new InvalidOperationException("PopModalAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page page, bool animated)
		{
			throw new InvalidOperationException("PushModalAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopModalAsync(bool animated)
		{
			throw new InvalidOperationException("PopModalAsync is not supported globally on Windows, please use a NavigationPage.");
		}
	}
}
