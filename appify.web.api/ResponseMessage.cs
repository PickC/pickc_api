/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
namespace appify.web.api
{
    public class ResponseMessage
    {
        public Int16 statusCode { get; set; }
        public string name { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }

    public static class StatusName
    {

        public const string ok = "SUCCESS_OK";
        public const string add = "SUCCESS_ADD";
        public const string remove = "SUCCESS_REMOVE";
        public const string invalid = "INVALID TRANSACTION";
    }


    public static class StatusCodes
    {

        public const Int16 OK = 200;
        public const Int16 ERROR = 400;
        public const Int16 SERVER_ERROR = 501;
    }


    
}
