namespace Demo.UIAutomation.Framework.Browsers
{
    public interface INavigable
    {
        /// <summary>
        /// Pages that we can navigate using directly an URL will implement this interface. Tha aim of this is avoid overuse the UI when
        /// is not necessary for our test. 
        /// </summary>
        /// <param name="parameter">Optional parameter to be normally included into the target URL</param>
        void NavigateTo(string parameter = null);
    }
}
