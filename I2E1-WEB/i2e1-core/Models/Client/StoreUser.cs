using i2e1_basics.Utilities;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models.Client
{

    [Serializable]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum Feature
    {
        NO_FEATURE = 0,
        BLOCKED_PHONE_NUMBER_LIST = 1,
        LANDING_PAGE = 2,
        PHONE_NUMBER_WHITELISTING = 3,
        VIP_PHONE_NUMBER_LIST = 4,
        BANDWIDTH_CONTROL = 5, // Apply to enable upload and download bandwidth settings in basic configuration
        DATA_USAGE_REPORTS = 6,
        SMS_FEATURE = 7,
        PROMOTION = 8,
        TOGGLING_FEATURE = 9,
        DATA_USAGE_CONTROL_MONTH = 10, // Apply to enable data usage control per month in basic configuration
        BANDWIDTH_AFTER_EXHAUSTED = 11, // in kbps // Apply to enable upload and download bandwidth after data exhausted settings in basic configuration
        VIP_MAC_LIST = 12,
        SMS_PRIME_FEATURE = 13,
        DATA_USAGE_TAB = 14,
        ADVANCE_CONFIG = 15,
        TEMPLATE_CONFIG = 16,
        USER_GROUP = 17,
        BLOCK_WEBSITE = 18,
        SHOW_PHONE_NUMBER = 19,
        ADVANCE_ANALYTICS = 20,
        THIRTY_MIN_CHECK = 21,



        PACKAGES = 22,
        IMPERSONATE = 23,
        MAX_DATA_USAGE_PER_DAY = 24,
        SESSION_TIMEOUT = 25, // Apply to enable session timeout in basic configuration
        NUMBER_OF_SESSIONS_PER_DAY = 26, // APPLICABLE ONLY IF SESSION TIME IS LESS THAN 24 HOURS
        DNS_REPORTS = 27,
        HEADER_IMAGE = 28,
        MAC_BLACKLIST = 29, // Apply to enable mac blacklisting
        SETTINGS_NAVIGATOR = 30,
        STORE_OPERATIONS_NAVIGATOR = 31,
        CHOOSE_AUTHENTICATION_MODE = 32, // Apply to enable authentication mode in advance configuration
        FACEBOOK_SHARE = 33, // Apply to enable facebook share page in advance configuration
        AD_HOC_PACKAGES = 34,
        WIFI_METRICS = 35,
        MAP_METRICS = 36,
        SHINE_PLUS = 37, // Apply to enable shine plus in advance configuration
        SET_AUTHENTICATION_GROUP = 38,  // Apply to enable authentication group in advance configuration
        REPORTS = 39,
        HIDE_QUESTIONS = 40, // Apply to enable hide questions in basic configuration
        OPERATING_HOURS = 41, // Apply to enable operating hours in basic configuration
        NUMBER_OF_DEVICE_PER_USER = 42, // Apply to enable number of devices in basic configuration
        HEALTH_ALERTS = 43, // Apply to enable configure alerts in basic configuration
        ADVANCE_SETTINGS_OPTIONS = 44,
        SPLASH_PAGE = 45,
        DEVICE_ACTIVE = 46,  // Apply to enable device active in advance configuration
        GLOBAL_OTP = 47,  // Apply to enable global otp in advance configuration
        WELCOME_MSG = 48,
        DETAILED_REPORTS = 49,
        USER_MANAGEMENT = 50, // userManagement
        SHOPPER_DENSITY = 51,
        ADMIN_OPERATIONS = 52,
        DATA_USAGE_PER_SESSION = 53, // Apply to enable data usage control per session in basic configuration
        CUSTOM_REPORT_SUBSCRIPTION = 54,
        BANDWIDTH_REPORT = 55,

        WOFR = 56,

        DEVICE_CONFIG = 57,
        EVENT_LOGGER = 58,
        OPERATIONS_PORTAL = 59,
        MEDIA_MANAGER = 60,

        ADMIN_PORTAL_ACCESS = 61,


        SETTINGS_PORTAL = 62,
        COUPON_RESELLER_PORTAL = 63,
        WIOM_DASHBOARD = 64,
        HOME_ROUTER = 65,
        WIOM_SALES = 66,
        WIOM_LEADS = 67,
        WIOM_TICKETS = 68,

        LINQ_DASHBOARD = 200, // Can Access Dashboard
        LINQ_CATEGORIES = 201, // Can View Categories
        LINQ_SUBCATEGORIES = 202, // Can View Subcategories
        LINQ_TAGS = 203,// Can View Tags
        LINQ_LISTINGS = 204, // Can View Listings
        LINQ_USERS = 205, // Can View Linq Users
        LINQ_CATEGORIES_EDIT = 211, // Can Edit Categories
        LINQ_SUBCATEGORIES_EDIT = 212, // Can Edit SubCategories
        LINQ_TAGS_EDIT = 213, // Can Edit Tags
        LINQ_USERS_EDIT = 223, // Can Edit Linq Users
        LINQ_LISTINGS_EDIT = 224, // Can Edit Linqs
        LINQ_LISTINGS_DELETE = 225, // Can Delete Linqs
        LINQ_LISTINGS_OWNERSHIP = 226, // Can Transfer Ownership
        LINQ_LISTINGS_MODERATION = 227 // Can Edit Moderators
    }
}