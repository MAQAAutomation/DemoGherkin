using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.UIAutomation.Framework.Application;
using Demo.UIAutomation.Framework.Helpers;
using Demo.UIAutomation.Framework.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using RazorEngine.Compilation.ImpromptuInterface;
using static System.Reflection.MethodBase;
using static Demo.UIAutomation.Framework.Helpers.SqWait;
using IWebElement = OpenQA.Selenium.IWebElement;

namespace Demo.UIAutomation.Framework.Extensions.SqWebElements
{
    internal static class GenericSqWebElementsActions
    {
        private static readonly ITestLogger Log = TestLog.GetTestLogger();

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getWebElement">The get the element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <param name="scrollToElement">if set to <c>false</c> [scroll to element].</param>
        /// <param name="log">If true it will log the method and its caller as info level</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T Click<T>(this T getWebElement, int maxWaitMs = 5000, bool scrollToElement = false, bool log = false, string elementName = null) where T : ICloneable, ISerializable
        {
            string logMessage = null;
            if (log)
            {
                logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
                Log.Info(logMessage);
            }

            var del = getWebElement as Delegate;
            WaitForElementToBeClickable(getWebElement, maxWaitMs, scrollToElement);
            WaitForCondition(() =>
            {
                if (scrollToElement)
                {
                    ScrollTo(getWebElement, elementName: elementName);
                }

                ((IWebElement)del.DynamicInvoke()).Click();
                if (log)
                {
                    Log.Debug($"{logMessage} was successful ");
                }

                return true;
            }, maxWaitMs);

            return getWebElement;
        }

        /// <summary>
        /// Waits for element to be displayed and enabled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getWebElement">The get web element.</param>
        /// <param name="maxMsWait">The maximum ms wait.</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="log">if set to <c>true</c> [log].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        /// The element if it is clickable otherwise and exception
        /// </returns>
        internal static T WaitForElementToBeClickable<T>(this T getWebElement, int maxMsWait = 10000, bool scrollToElement = false, bool log = false, string elementName = null) where T : ICloneable, ISerializable
        {
            var del = getWebElement as Delegate;

            if (log)
            {
                var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
                Log.Info(logMessage);
            }

            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            WaitForCondition(() =>
            {
                if (scrollToElement)
                {
                    ScrollTo(getWebElement, elementName: elementName);
                }
                return WebElement().Displayed && WebElement().Enabled;
            }, maxMsWait);

            return getWebElement;
        }

        /// <summary>
        /// Method will move to until a specified element is into the view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The sq web element.</param>
        /// <param name="log">if set to <c>false</c> [log].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        /// The element
        /// </returns>
        internal static T ScrollTo<T>(this T sqWebElement, bool log = false, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            if (log)
            {
                Log.Info(logMessage);
            }

            IWebDriver driver = sqWebElement.GetWebDriver();
            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            if (driver.GetType() == typeof(ChromeDriver) ||
                (driver.GetType() == typeof(RemoteWebDriver) && RunSettings.Browser.Contains("chrome")))
            {
                var scrollIfNeeded = "var rect = arguments[0].getBoundingClientRect();"
                                     + "if (rect.bottom > window.innerHeight)"
                                     + "{"
                                     + "arguments[0].scrollIntoView();"
                                     + "}"
                                     + "if (rect.top < 0)"
                                     + "{"
                                     + "arguments[0].scrollIntoView(false);"
                                     + "}"
                                     + "arguments[0].scrollIntoView({block: 'center'});";

                ExecuteJavascript(scrollIfNeeded, WebElement());
            }

            string scrollElementIntoMiddle = "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
                                             + "var elementTop = arguments[0].getBoundingClientRect().top;"
                                             + "window.scrollBy(0, elementTop-(viewPortHeight/2));";
            ExecuteJavascript(scrollElementIntoMiddle, WebElement());

            return sqWebElement;
        }



        /// <summary>
        /// Clicks the visible element from list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The get the element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <exception cref="FrameworkException">There are no elements visible</exception>
        internal static void ClickVisibleElement<T>(this T elements, int maxWaitMs = 10000, bool scrollToElement = false, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            SqWebElement visibleElement = elements.GetVisibleElement(maxWaitMs);
            if (visibleElement == null)
            {
                throw new FrameworkException("There are no elements visible");
            }
            visibleElement.Click(maxWaitMs, scrollToElement, elementName);
        }

        /// <summary>
        /// Gets the visible element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="maxMsWait">The maximum wait.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static SqWebElement GetVisibleElement<T>(this T elements, int maxMsWait = 100, string elementName = null)
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            var del = elements as Delegate;
            IList<IWebElement> WebElements() => (IList<IWebElement>)del.DynamicInvoke();

            foreach (var item in WebElements())
            {
                IWebElement WebElement() => item;
                if (TryWaitForCondition(() =>
                {
                    ScrollTo((SqWebElement)WebElement, elementName: elementName);
                    return WebElement().Displayed;
                }, maxMsWait))
                {
                    return () => item;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to click element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static bool TryClick<T>(this T sqWebElement, int maxWaitMs = 10000, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            try
            {
                sqWebElement.Click(maxWaitMs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether this instance is displayed.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="maxWaitMs"></param>
        /// <returns>
        ///   <c>true</c> if the specified element is displayed; otherwise, <c>false</c>.
        /// </returns>
        internal static bool Displayed<T>(this T element, int maxWaitMs = 4000)
        {
            var del = element as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();
            return TryWaitForCondition(() => WebElement().Displayed, maxWaitMs);
        }

        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is enabled; otherwise, <c>false</c>.
        /// </returns>
        internal static bool Enabled<T>(this T element, int maxWaitMs = 4000) where T : ICloneable, ISerializable
        {
            var del = element as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();
            return TryWaitForCondition(() => WebElement().Enabled, maxWaitMs);
        }

        /// <summary>
        /// Gets the innerText of this element
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="maxWaitMs"></param>
        /// <returns></returns>
        internal static string Text<T>(this T element, int maxWaitMs = 4000) where T : ICloneable, ISerializable
        {
            var del = element as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();
            WaitForCondition(() => WebElement().Text != null, maxWaitMs);

            return WebElement().Text;
        }

        /// <summary>
        /// Get the web driver associated with a web element. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getWebElement"></param>
        /// <returns></returns>
        internal static IWebDriver GetWebDriver<T>(this T getWebElement) where T : ICloneable, ISerializable
        {
            var del = getWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            return ((IWrapsDriver)WebElement()).WrappedDriver;
        }

        /// <summary>
        /// Determines whether the specified get the element is selected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///   <c>true</c> if the specified get the element is selected; otherwise, <c>false</c>.
        /// </returns>
        internal static bool Selected<T>(this T sqWebElement, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            Delegate del = sqWebElement as Delegate;
            IWebElement webElement = (IWebElement)del.DynamicInvoke();

            string[] selected = { "true", "selected" };
            var attribute = webElement.GetAttribute("selected");
            bool result = selected.Contains(attribute, StringComparer.InvariantCultureIgnoreCase);
            if (!result)
            {
                attribute = webElement.GetAttribute("class");
                result = attribute.Contains("selected");
                Log.Debug("This is component is from an Angular 11 version");
            }
            Log.Debug($"Is {logMessage} result: {result}");

            return result;
        }

        /// <summary>
        /// Inputs the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="text">The text.</param>
        /// <param name="tabOut">if set to <c>true</c> [tab out].</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T Input<T>(this T sqWebElement, string text, bool tabOut = true, bool scrollToElement = false, string elementName = null, int maxWaitForCondition = 4000) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            if (logMessage.ToLower().Contains("password"))
            {
                Log.Info(logMessage + $" value: *******");
            }
            else
            {
                Log.Info(logMessage + $" value: {text}");
            }

            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            WaitForCondition(() =>
            {
                if (scrollToElement) ScrollTo(sqWebElement, elementName: elementName);
                WebElement().Click();
                WebElement().Clear();
                WebElement().SendKeys(text);
                if (tabOut)
                {
                    WebElement().SendKeys(Keys.Tab);
                }
                return true;
            }, maxWaitForCondition);

            return sqWebElement;
        }

        /// <summary>
        /// Inputs the value on search field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="text">The text.</param>
        /// <param name="tabOut">if set to <c>true</c> [tab out].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T InputOnSearchField<T>(this T sqWebElement, string text, bool tabOut = false, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage + $" value: {text}");

            IWebDriver driver = sqWebElement.GetWebDriver();
            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            WaitForCondition(() =>
            {
                var actions = new Actions(driver);
                actions.MoveToElement(WebElement());
                actions.Click();
                actions.SendKeys(text);
                if (tabOut) actions.SendKeys(Keys.Tab);
                actions.Build().Perform();

                return true;
            });

            return sqWebElement;
        }

        /// <summary>
        /// Send keys.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="text">The text.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T SendKeys<T>(this T sqWebElement, string text, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage + $" value: {text}");

            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            WaitForCondition(() =>
            {
                WebElement().SendKeys(text);
                return true;
            });

            return sqWebElement;
        }


        /// <summary>
        /// Selects the option by text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="optionText">The string.</param>
        /// <param name="matchType">Type of the match.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T SelectOptionByText<T>(this T sqWebElement, string optionText, TextMatchType matchType = TextMatchType.Exact, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage + $" where matches {matchType} Text: {optionText}");

            var del = sqWebElement as Delegate;
            IWebElement SqWebElement() => (IWebElement)del.DynamicInvoke();

            Func<IWebElement, bool> condition = null;
            switch (matchType)
            {
                case TextMatchType.Contains:
                    condition = o => o.Text.Contains(optionText);
                    break;
                case TextMatchType.StartsWith:
                    condition = o => o.Text.StartsWith(optionText);
                    break;
                case TextMatchType.EndsWith:
                    condition = o => o.Text.EndsWith(optionText);
                    break;
                default:
                    condition = o => o.Text == optionText;
                    break;
            }

            WaitForCondition(() =>
            {
                var se = new SelectElement(SqWebElement());
                foreach (IWebElement option in se.Options)
                {
                    if (condition(option))
                    {
                        option.Click();
                        break;
                    }
                }

                SqWebElement actual = sqWebElement.GetSelectedOption();
                return condition(actual());
            });

            return sqWebElement;
        }

        /// <summary>
        /// Selects the option by value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="value">The value.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static T SelectOptionByValue<T>(this T sqWebElement, string value, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage + $" with {nameof(value)} {value}");

            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            SelectElement se;
            WaitForCondition(() =>
            {
                se = new SelectElement(WebElement());
                se.SelectByValue(value);
                string actual = sqWebElement.GetSelectedOption()().GetAttribute("value");
                return value.Equals(actual);
            });

            return sqWebElement;
        }

        /// <summary>
        /// Select the option inside a select element based on the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getTheElement">The get the element.</param>
        /// <param name="index">The index.</param>
        /// <param name="elementName">Name of the element.</param>
        internal static void SelectOptionByIndex<T>(this T getTheElement, int index, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage + $" with {nameof(index)} {index}");

            var del = getTheElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();

            WaitForCondition(() =>
            {
                SelectElement se = new SelectElement(WebElement());
                se.Options[index].Click();
                return se.SelectedOption.Equals(se.Options[index]);
            }, 10000);
        }

        /// <summary>
        /// Gets the selected option.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqWebElement">The get the element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static SqWebElement GetSelectedOption<T>(this T sqWebElement, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();
            SelectElement se;
            IWebElement option = null;
            WaitForCondition(() =>
            {
                se = new SelectElement(WebElement());
                option = se.SelectedOption;
                return true;
            });

            return () => option;
        }

        /// <summary>
        /// Gets the dropdown options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="drpSelectElement">The DRP select element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static SqListWebElement GetDropdownOptions<T>(this T drpSelectElement, string elementName = null) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            var del = drpSelectElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.DynamicInvoke();
            var selectList = new SelectElement(WebElement());

            return () => selectList.Options.ToList();
        }

        /// <summary>
        /// Gets the random dropdown option.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="drpSelectElement">The DRP select element.</param>
        /// <param name="ignoreFirstOption">if set to <c>true</c> [ignore first option].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static SqWebElement GetRandomDropdownOption<T>(this T drpSelectElement, bool ignoreFirstOption = true) where T : ICloneable, ISerializable
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name);
            Log.Info(logMessage);

            SqListWebElement dropdownOptions = drpSelectElement.GetDropdownOptions();
            int startIndex = ignoreFirstOption ? 1 : 0;
            if (dropdownOptions().Any())
            {
                int randomIndex = BaseUtils.RandomizeInt(startIndex, dropdownOptions().Count - 1);
                return () => dropdownOptions().ElementAt(randomIndex);
            }

            Log.Debug("Not any option has been found into the dropdown");
            return null;
        }

        /// <summary>
        /// Selects the random dropdown option.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="drpSelectElement">The DRP select element.</param>
        /// <param name="drpName">Name of the DRP.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static SqWebElement SelectRandomDropdownOption<T>(this T drpSelectElement, string drpName, string elementName = null)
        {
            var logMessage = CreateLogMethodMessage(GetCurrentMethod()?.Name, elementName);
            Log.Info(logMessage);

            var del = drpSelectElement as Delegate;
            SqWebElement sqWebElement = () => (IWebElement)del.DynamicInvoke();

            SqWebElement randomOption = sqWebElement.GetRandomDropdownOption();
            sqWebElement.SelectOptionByValue(randomOption().GetValue());

            return randomOption;
        }

        /// <summary>
        /// Hover a sequel web element
        /// </summary>
        /// <param name="sqWebElement"></param>
        internal static T Hover<T>(this T sqWebElement) where T : ICloneable, ISerializable
        {
            var driver = sqWebElement.GetWebDriver();

            var del = sqWebElement as Delegate;
            IWebElement WebElement() => (IWebElement)del.FastDynamicInvoke();

            Actions actions = new Actions(driver);
            actions.MoveToElement(WebElement());
            actions.Build().Perform();

            return sqWebElement;
        }

        /// <summary>
        /// Execute a javascript over the driver involving one WebElement        
        /// </summary>
        /// <param name="js"></param>
        /// <param name="param"></param>
        internal static object ExecuteJavascript(string js, IWebElement param)
        {
            IWebDriver driver = ((IWrapsDriver)param).WrappedDriver;
            return ((IJavaScriptExecutor)driver).ExecuteScript(js, param);
        }



        /// <summary>
        /// Creates the log method message.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        private static string CreateLogMethodMessage(string methodName, string elementName = null)
        {
            string callerName = elementName ?? CallerName.GetCallerName(methodName);
            return $"{methodName} on {callerName}";
        }
    }
}
