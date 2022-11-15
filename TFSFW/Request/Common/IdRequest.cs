namespace Demo.TFSAutomationFramework.Request.Common
{
    public class IdRequest
    {
        public int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdRequest"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public IdRequest(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdRequest"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public IdRequest(string id)
        {
            Id = int.Parse(id);
        }
    }
}
