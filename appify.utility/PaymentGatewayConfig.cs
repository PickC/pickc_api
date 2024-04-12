using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.utility
{
    public static class PaymentGatewayConfig
    {

        public static readonly string PAYMENTGATEWAY_TESTKEY = "2PBP7IABZ2";
        public static readonly string PAYMENTGATEWAY_TESTSALT = "DAH88E3UWQ";

        /*
         Modification History 
         Modified by    :   sharma
         Modified On    :   10-Feb-2024
         Reference      :   new production credentials received from Easebuzz payment gateway partner.
         Description    :   the below KEY & SALT are depricated on 09-Feb-2024.
         
         */
        //public static readonly string PAYMENTGATEWAY_KEY = "2PBP7IABZ2";
        //public static readonly string PAYMENTGATEWAY_SALT = "DAH88E3UWQ";


        /*
         Modification History 
         Modified by    :   sharma
         Modified On    :   10-Feb-2024
         Reference      :   new production credentials received from Easebuzz payment gateway partner.
         Description    :   The below are the new EASEBUZZ - WIRE GATEWAY KEY & SALT VALUES.
         
         */
        public static readonly string PAYMENTGATEWAY_KEY = "D1D57B6931";
        public static readonly string PAYMENTGATEWAY_SALT = "9B30AFD3AA";
        
            

        public static readonly string PAYMENTGATEWAY_ENV = "test";
        public static readonly string PAYMENTGATEWAY_SEAMLESS = "false";
        public static readonly string PAYMENTGATEWAY_SUCCESSURL = "http://appi-fy.ai/paymentsuccess.html";
        public static readonly string PAYMENTGATEWAY_ERRORURL = "http://appi-fy.ai/paymenterror.html";
    }

}
