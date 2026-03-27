using i2e1_basics.Utilities;
using i2e1_basics.Utilities;
using System.Collections.Generic;

namespace i2e1_core.Utilities
{
    public class Constants : i2e1_basics.Utilities.BasicConstants
    {
		public static BigDataHelper CUSTOMER_LOG = BigDataHelper.CreateTopic("customer_logs");

		public const int ROAMING_GROUP = 1;

        public const int SETTING_ID_ROAM_10_20_100 = 1;
        public const int SETTING_ID_HOME_300 = 2;
        public const int SETTING_ID_HOME_399 = 3;
        public const int SETTING_ID_ROAM_HOME_10_20_100_300 = 4;
        public const int SETTING_ID_ROAM_HOME_10_20_100_399 = 5;

        public static List<int> HMR_SETTING_IDS = new List<int>() {2,3,6,8,9,10 };
        
        public const long ONE_KB = 1024;
        public const long ONE_MB = 1024 * 1024;
        public const long ONE_GB = 1024 * 1024 * 1024;
        public const int SECONDS_IN_MONTH = 30 * 24 * 3600;
        public const int SECONDS_IN_HOUR = 3600;
        public const int SECONDS_IN_DAY = 24 * 3600;
        public const int MINUTES_IN_DAY = 24 * 60;
        public const int LENGTH_OF_MAC = 17;
        public const int HOME_ROUTER_DEVICE_LIMIT = 10;
        public const string FREE_INFINITY_SSID = "∞ व्योम नेट";
        public const string CASH_DONE = "CASH_DONE";
        public const string CASH_REJECTED = "CASH_REJECTED";

        public static int WIOM_MEMBER_CHARGES = I2e1ConfigurationManager.MEMBERSHIP_FEE;
        public static int CASH_HANDLING_CHARGES = I2e1ConfigurationManager.CASH_HANDLING_FEE;
        public static int SUBSCRIPTION_DISCOUNT = 25;
        public const int PM_WANI_DISCOUNT = 151;

        public const string PAYMENT_ACCESS_CODE = "PAYMENT";
        public const string DOMAIN_FOR_NET_CHECK = "pmwifi.in";
        public const string DOMAIN_FOR_REMOTE = "remote.i2e1.in";

        public const string PARTNER_CONFIGURATION_ID = "partner_configuration_id";

        public static readonly bool IS_PRODUCTION;

        public const string IDS_PG_USER = "IDS.PG.USER";
        public const string ADMIN_USER = "adminUser";

        public const string EXTERNAL_USER = "externalUser";

        //FRESH_ADMIN is newly registered merchant who has not yet added any devices to his account
        //FRESH_ADMIN will be converted to ADMIN_USER after adding atleast one device.
        public const string FRESH_ADMIN = "freshAdmin";

        public const string TOKEN = "token";
        public const string SWAP_TOKEN = "SWAP_APP";
        public const string SWAP_API_TOKEN = "31ff85e5-af5d-43e1-a3dd-2886fee36311";
        public const string AES_KEY = "y/B?E(H+MbPeShVmYq3t6w9z$C&F)J@NcRfTjWnZr4u7x!A%D*G-KaPdSgVkXp2s5v8y/B?E(H+MbQeThWmZq3t6w9z$C&F)J@NcRfUjXn2r5u8x!A%D*G-KaPdSgVkY";
        public const string DAY_MONTH = "dd-MMM";
        public const string MONTH_YEAR = "MMM-yyyy";
        public const string DAY_MONTH_YEAR = "dd-MMM-yyyy";
        public const int MAX_WARNING_TOLERANCE = 30; // in minutes
        public const int MAX_ACTIVE_TOLERANCE = 10; // in minutes

        public const string WANI_COOKIE_NAME = "waniMobile";


        public const string LINQ_ADMIN = "linqAdmin";
        public const string ADMIN_IN_POWER = "adminInPower";
        public const string INVALID_LOGIN = "invalidLogin";

        public const string GOOGLE_TOKEN = "google_token";

        public const string USER_QUESTIONS = "USER_QUESTIONS";

        public const string PACKAGE_ID = "packageId";
        public const string PACKAGE_GUID = "packageGuid";
        public const string PAYMENT_OBJECT = "paymentObject";

        public const string VISIT_RANGE = "Visit range";
        public const string DATE_RANGE = "Date range";
        public const string TIME_RANGE = "Time range";
        public const string POWER_RANGE = "Power range";
        public const string REPEATION_RANGE = "Repeat in days";

        public const string SECOND_STATE_QUESTIONS = "SECOND_STATE_QUESTIONS";
        public const int NO_QUESTION_LEFT = 0;
        public const int EMAIL_QUESTION = 8;
        public const int GENDER_QUESTION = 14;
        public const string PLUS = "Plus";
        public const string PRIME = "Prime";
        public const string ONE = "One";
        public const string PARTNER_CODE = "PARTNER_CODE";
        public const string PARTNER_ID = "PARTNER_ID";
        public const string AFTER_FIRST_PAGE = "AfterFirstPage";
        public const string ACCUWEATHER_API_KEY = "yCdOVrYOGEpZsrFGee4YjXDGXMi0T2Qo";
        public const string ANDROID = "ANDROID";
        public const string IOS = "IOS";

        public const string WIOM_LOGIN_TOKEN = "WIOM_LOGIN_TOKEN";
        public const string JWT_OBJECT = "JWT_OBJECT";
        public const string JWT_TOKEN = "JWT_TOKEN";
        public const string NOTIFICATIONSTATUS = "NotificationStatus";
        public const long NORMAL_PLAN_DAYS_COUNT = 28;
        public const long SECONDS_COUNT_DAY = 86400;
        public const int ADDITIONAL_DAYS_FOR_ROUTER_PICKUP = 15;
        public const int ADDITIONAL_DAYS_FOR_ROUTER_PICKUP_WIOM_MEMBER = 45;

        public const long WIOM_DIY_PARTNER_ACCOUNT = 274877950645;

        public static List<string> SMS_EMAIL_LIST = new List<string>() { "somya.shree@wiom.in", "ashutosh.mishra@i2e1.com", "deepak.srivastava@wiom.in", "ravi.sahu@i2e1.com", "komal.saroya@wiom.in", "kanishk.tyagi@wiom.in" };
        public static List<string> failureEmailList = new List<string>() { "vinayak.rastogi@wiom.in", "ashutosh.mishra@i2e1.com", "ashish.agrawal@i2e1.com", "ravi.sahu@i2e1.com", "adarsh.kumar@wiom.in" };
        public static List<string> rateLimiterEmailList = new List<string>() { "ashutosh.mishra@i2e1.com", "ravi.sahu@i2e1.com", "adarsh.kumar@wiom.in" };
        public static List<string> H8_FAILURE_LIST = new List<string>() { "ashutosh.mishra@wiom.in", "shariq.khan@wiom.in", "shubham.verma@wiom.in" };
        public static List<string> refundEmailList = new List<string>() { "vinayak.rastogi@wiom.in", "ashutosh.mishra@i2e1.com", "ashish.agrawal@i2e1.com", "ravi.sahu@i2e1.com", "adarsh.kumar@wiom.in", "prateek.kimothi@wiom.in" };
        public static string[] CUSTOMER_APP_UNAUTHORIZED_LOGIN_POSSIBLE_CASES = { "Growth App User Logging Into Customer", "Partner App User Logging Into Customer", "Warning: Unauthorized Login"};
        public const string SESSION_REDIS_HOST = "13.233.134.118";
        public const string SESSION_REDIS_PASSWORD = "CZrQQqsrJImh";
        public const string SESSION_STARTED = "session_started";
        public const string SESSION_ENDED = "session_ended";

        public const int USER_DOES_NOT_EXISTS_ISP_RESPONSE_CODE = 4721038;
        static Constants()
        {
            var environmentVariable = System.Environment.GetEnvironmentVariable("DEPLOYED_ON");
            IS_PRODUCTION = environmentVariable != null && (environmentVariable.ToLower() == "production" || environmentVariable.ToLower() == "prod" || environmentVariable.ToLower() == "stage") ? true : false;
        }

        

    }

    public class RadiusConstants
    {
        public const string SESSION_TIMEOUT = "Session-Timeout";
        public const string IDLE_TIMEOUT = "Idle-Timeout";
        public const string MAX_UPLOAD_BANDWIDTH = "ChilliSpot-Bandwidth-Max-Up";
        public const string MAX_DOWNLOAD_BANDWIDTH = "ChilliSpot-Bandwidth-Max-Down";
        public const string MAX_DATA_USAGE = "ChilliSpot-Max-Total-Octets";
    }

    public class ClientId
    {
        public const int REDBUS = 41;

        public const int MY_GOV = 79;
    }

    public class InteractionConstants
    {
        public const int WEB_ORIGIN = 4;

        public const int DISCOVER_ORIGIN = 5;
    }

    public enum MACAUTH
    {
        NEW_TO_I2E1,
        NEW_TO_CLIENT,
        WHITELISTED,
        KNOWN_TO_NAS,
        ACCEPT,
        USER_LOGGED_OUT,
        SESSION_EXPIRED,
        DATA_EXHAUSTED,
        DEVICE_OFF,
        NOT_LOGGED_IN
    }

    public enum BROWSER
    {
        CHROME
    }
    public enum FACEBOOK
    {
        PAGE_SHOWN
    }
    public enum USER_LOGIN
    {
        SHOWING_SPLASH_PAGE,
        OTP_SUBMITTED,
        OTP_GENERATED,
        USSD_GENERATED,
        INVALID_OTP,
        PASSPORT_LOGIN_TRIGGERED,
        CONTACT_RECEPTION,
        INT_OTP_GENERATED,
        OTP_REGENERATED,
        LOGIN_SUCCESSFUL,
        AUTO_LOGIN_SUCCESSFUL,
        AUTO_LOGIN_TRIGGERED,
        SESSION_EXPIRED,
        DATA_EXHAUSTED,
        BLOCKED,
        NOT_ALLOWED,
        SHOWING_FIRST_STATE,
        PAYMENT_INITIATED,
        PAYMENT_SUCCESSFUL,
        PAYMENT_FAILED,
        ACCESS_CODE_SUBMITTED,
        LINQ_PROMO_MANUAL,
        LINQ_PROMO_PROCEED,
        ERROR_NASID_ZERO,
        STATE_NULL_MOBILE,
        STATE_LOGOFF,
        STATE_RADIUS_SUCCESS,
        STATE_SESSION_EXHAUSTED,
        STATE_SESSION_EXHAUSTED_PHONE,
        STATE_DATA_EXHAUSTED,
        STATE_WELCOMEBACK_TARGET,
        STATE_WELCOMEBACK_NOTARGET,
        STATE_STATELESS_GAP,
        PLAN_REQUESTED,
        PLAN_APPROVED,
        PLAN_REDEEMED
    }

    public enum AUTH
    {
        ACCEPT,
        SESSION_EXPIRED,
        DATA_EXHAUSTED,
        DEVICE_OFF,
        NOT_LOGGED_IN
    }

    public enum NOTIFICATION_SENT
    {
        USER_REDIRECTED
    }

    public enum ROUTER_UPGRADE
    {
        UPGRADE_SUCCESSFUL
    }

    public class PaymentSource
    {
        public const string HOME_ROUTER = "wg";
        public const string HOME_ROUTER_MANDATE = "wgMand";
        public const string HOME_ROUTER_MANDATE_EXECUTE = "wgSubs";
        public const string PDO_PLAN = "w";
        public const string HOME_ROUTER_IMPLICITLY_MEMBER = "wgIMem";
        public const string HOME_ROUTER_MEMBER = "wgMem";
        public const string BOOKING_HOME_ROUTER = "wgBook";
        public const string ONBOARDING_SUBSCRIPTION = "wgOnbSubs";
        public const string WALLET_RECHARGE = "wiomWall";
        public const string CUSTOMER_WALLET_RECHARGE = "custWall";
        public const string WALLET_HOME_ROUTER_RECHARGE = "wallet";
    }
    
    public class WiomServiceValidity
    {
        public const int SUBSCRIPTION = 12;
    }
    public class CustomHeader
    {
        public const string APP_NAME = "app-name";
        public const string APP_VERSION = "app-version";
    }
    public static class SubscriptionNotificationStatus
    {
        public const string NOTIFICATION_FAILED = "NOTIFICATION_FAILED";
        public const string NOTIFICATION_SUCCEEDED = "NOTIFICATION_SUCCEEDED";
    }

}
