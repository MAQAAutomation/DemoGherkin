using System.Collections.Generic;
using Demo.CommonFramework.ExceptionHandler;
using Demo.WebFW.Framework.Steps;
using NUnit.Framework;
using WebFW.Helpers.PageHelper;
using WebFW.PageReferences;

namespace Demo.WebFW.Framework.Helpers
{
	public static class TestHelper
	{
		/// <summary>
		/// Determines whether the specified actual is at.
		/// </summary>
		/// <param name="actual">The actual.</param>
		public static void IsAt(Page actual)
		{
			Assert.IsTrue(actual.IsDoneLoading());
		}

		/// <summary>
		/// Determines whether the specified page is at.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="stepPages">The step pages.</param>
		/// <exception cref="FrameworkException">The page name does not correspond to any of the available! Please try to set it again</exception>
		public static void IsAt(string page, StepPages stepPages)
		{
			switch (page.Trim().ToLower())
			{
				case PageNameRefs.LOGIN_HEROKUAPP:
					IsAt(stepPages.LoginPageInstance);
					break;
				case PageNameRefs.LOGINOUT_HEROKUAPP:
					IsAt(stepPages.SecureAreaPageInstance);
					break;
				case PageNameRefs.HOME_HEROKUAPP:
					IsAt(stepPages.MainPageInstance);
					break;
				default:
					throw new FrameworkException("The page name does not correspond to any of the available! Please try to set it again");
			}
		}

		/// <summary>Authentications the specified page.</summary>
		/// <param name="page">The page.</param>
		/// <param name="pageName">Name of the page.</param>
		/// <param name="user">The user.</param>
		/// <param name="passwd">The passwd.</param>
		public static void Authentication(StepPages page, string pageName, string user, string passwd)
		{
			switch (pageName.ToLower())
			{
				case PageNameRefs.HEROKUAPP:
					page.LoginPageInstance.IsDoneLoading();
					page.LoginPageInstance.SetUser(user);
					page.LoginPageInstance.SetPasswd(passwd);
					page.LoginPageInstance.PressButton(PageNameRefs.LOGIN_HEROKUAPP_BUTTON);
					break;
				default:
					throw new FrameworkException("The login is not supporting for app: " + pageName);
			}
		}

		/// <summary>
		/// Fills the page in.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="pageName">Name of the page.</param>
		/// <param name="parameters">The parameters.</param>
		/// <exception cref="FrameworkException"></exception>
		public static void FillPageIn(StepPages page, string pageName, Dictionary<string, string> parameters)
		{
			switch (pageName.ToLower())
			{
				default:
					throw new FrameworkException(pageName + " page is not supported");
			}
		}



		/// <summary>
		/// Reviews the page in.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="pageName">Name of the page.</param>
		/// <param name="parameters">The parameters.</param>
		public static bool ReviewPageIn(StepPages page, string pageName, Dictionary<string, string> parameters, out string error)
		{
			switch (pageName.ToLower())
			{
				case PageNameRefs.LOGINOUT_HEROKUAPP:
					return LogPageHelper.ReviewLogoutPage(page, parameters, out error);
				case PageNameRefs.LOGIN_HEROKUAPP:
					return LogPageHelper.ReviewLoginPage(page, parameters, out error);
				default:
					throw new FrameworkException(pageName + " page is not supported");
			}
		}


		/// <summary>
		/// Presses the button.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="button">The button.</param>
		/// <param name="pageName">Name of the page.</param>
		/// <exception cref="FrameworkException"></exception>
		public static void PressButton(StepPages page, string button, string pageName)
		{
			switch (pageName.ToLower())
			{
				case PageNameRefs.LOGIN_HEROKUAPP:
					page.LoginPageInstance.PressButton(button);
					break;
				case PageNameRefs.LOGINOUT_HEROKUAPP:
					page.SecureAreaPageInstance.PressButton(button);
					break;
				case PageNameRefs.HOME_HEROKUAPP:
					page.MainPageInstance.LinkToElement(button);
					break;
				default:
					throw new FrameworkException(pageName + " page is not supported");
			}
		}
	}
}
