using OpenQA.Selenium;
using System.Collections.Generic;

namespace Demo.UIAutomation.Framework.Extensions.SqWebElements
{
    public delegate IWebElement SqWebElement();

    public delegate IList<IWebElement> SqListWebElement();

    public delegate IWebElement SqButtonWebElement();

    public delegate IWebElement SqDropdownWebElement();

    public delegate IWebElement SqInputFieldWebElement();

    public delegate IWebElement SqLabelWebElement();
}
