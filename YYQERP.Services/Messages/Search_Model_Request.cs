namespace YYQERP.Services.Messages
{
    public class Search_Model_Request : PageRequest
    {
        public string ModelName { get; set; }

        public string ModelCode { get; set; }
    }
}
