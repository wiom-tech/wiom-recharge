using System;
using System.Collections.Generic;
using System.Text;

namespace i2e1_core.Models
{
    [Serializable]
    public enum SmsStatus
    {
        DELIVRD = 0,
        AWATING = 1,
        NOT_DELIVERED = 2,
        DNDNUMB = 3,
        INV_NUMBER = 4,
        NO_CREDITS,
        SERIES_BLOCK,
        SERVER_ERR,
        SPAM,
        SNDRID_NOT_ALLOTED = 9,
        BLACKLIST,
        TEMPLATE_NOT_FOUND,
        INV_TEMPLATE_MATCH,
        SENDER_ID_NOT_FOUND,
        NOT_OPTIN,
        TIME_OUT_PROM,
        INVALID_SUB,
        ABSENT_SUB = 17,
        HANDSET_ERR,
        BARRED,
        NET_ERR,
        MEMEXEC,
        FAILED = 22,
        MOB_OFF,
        HANDSET_BUSY,
        SERIES_BLK,
        EXPIRED,
        REJECTED,
        OUTPUT_REJ,
        REJECTED_MULTIPART,
        NO_DLR_OPTR,
        AWAITED_DLR,
        BLACKLST,
        REJECTD,
        UNDELIV,
        UNKNOWN_ERR,
        SNDR_NT_REGD,
        NOT_PULLED = 99
    }
}
